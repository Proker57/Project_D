using System.Collections;
using System.Collections.Generic;
using BOYAREngine.Ads;
using BOYAREngine.MainMenu;
using BOYAREngine.Units;
using BOYAREngine.Utils;
using GoogleMobileAds.Api;
using GooglePlayGames;
using UnityEngine;
using UnityEngine.UI;

namespace BOYAREngine.Controller
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        private bool _isInBattle;

        [System.Serializable]
        public struct NoneGameSettings
        {
            public Achievements Achievements;

            [Header("Managing visibility of objects between states")]
            public GameObject MainMenuStateParent;
            public GameObject GameStateParent;

            [Header("Menu UI")]
            public Text PointsText;
            public Text LevelText;

            [Header("InGame UI")]
            public Text AlliesCountText;
            public Text EnemiesCountText;
            public Text TimeText;
            [Space]
            public Text AllyAttackers;
            public Text AllyTanks;
            public Text AllyMedics;

            public Text EnemyAttackers;
            public Text EnemyTanks;
            public Text EnemyMedics;

            [Header("InGame Background Particle System")]
            public ParticleSystem Stars;

            [Header("Sounds")]
            public AudioSource BattleMusic;

            [Space]
            public CameraTargetGroup CameraTargetGroup;
            public List<GameObject> AllyShips;
            public List<GameObject> EnemyShips;


        }

        [System.Serializable]
        public struct AvailableShips
        {
            [Header("Ships")]
            public GameObject[] Prefabs;

            [HideInInspector] public GameObject AllyParent;
            [HideInInspector] public GameObject EnemyParent;
        }

        [Header("Progress")]
        [SerializeField] private MainMenu.Upgrades _upgrades;
        public int Points;
        public int PointsEarned;
        public int Level;

        [Header("Battle settings [Amount of ships]")]
        public int AllyAttackers;
        public int AllyAttackersCurrent;
        public int AllyTanks;
        public int AllyTanksCurrent;
        public int AllyMedics;
        public int AllyMedicsCurrent;
        public int EnemyAttackers;
        public int EnemyAttackersCurrent;
        public int EnemyTanks;
        public int EnemyTanksCurrent;
        public int EnemyMedics;
        public int EnemyMedicsCurrent;

        public int[] Counts;

        public NoneGameSettings Setup;
        public AvailableShips Ships;

        private GoogleAds _googleAds;
        private int _secondsInBattle;
        private int _minutesInBattle;

        private bool _isGameLoaded;
        private bool _isFirstBattle = true;

        private void Awake()
        {
            Instance = this;

            _googleAds = new GoogleAds();
            MobileAds.Initialize(initStatus => { });
        }

        private void Start()
        {
            _googleAds.RequestBanner();

            Load();

            UiUpdatePoints();
            UiUpdateLevelText();

            _isGameLoaded = true;

            Debug.Log(Application.systemLanguage);
        }

        public void StartBattle()
        {
            Save();
            _isInBattle = true;

            PointsEarned = 0;

            _secondsInBattle = 0;
            _minutesInBattle = 0;
            Setup.TimeText.text = $"{_minutesInBattle:00}:{_secondsInBattle:00}";
            StartCoroutine(TimeInBattle());

            AllyAttackersCurrent = 0;
            AllyTanksCurrent = 0;
            AllyMedicsCurrent = 0;

            EnemyAttackersCurrent = 0;
            EnemyTanksCurrent = 0;
            EnemyMedicsCurrent = 0;


            EnemyAttackers = Level + 2;
            EnemyTanks = Level + 1;
            EnemyMedics = Level + 1;
            Counts = new[]
            {
                AllyAttackers, EnemyAttackers,
                AllyTanks, EnemyTanks,
                AllyMedics, EnemyMedics
            };

            UnitStats.EnemyAttackerHealthMax = 100 + Level * 2;
            UnitStats.EnemyAttackerDamage = 10 + Level * 2;

            UnitStats.EnemyTankHealthMax = 150 + Level * 10;
            UnitStats.EnemyTankDamage = 5 + Level * 3;

            UnitStats.EnemyAttackerHealthMax = 100 + Level * 10;
            UnitStats.EnemyAttackerDamage = 10 + Level * 3;

            Setup.MainMenuStateParent.SetActive(false);
            Setup.GameStateParent.SetActive(true);

            Ships.AllyParent = new GameObject("== ALLY ==");
            Ships.AllyParent.transform.parent = Setup.GameStateParent.transform;
            Ships.EnemyParent = new GameObject("== ENEMY ==");
            Ships.EnemyParent.transform.parent = Setup.GameStateParent.transform;

            Setup.Stars.Play();
            Setup.BattleMusic.Play();

            MiniTask.Run(0f, SpawnShips);

            _googleAds.BannerView.Destroy();
        }

        private void SpawnShips()
        {
            var range = 10 / Ships.Prefabs.Length;
            for (var i = 0; i < Ships.Prefabs.Length; i++)
            {
                var index = i * 2;
                // Ally
                for (var j = 0; j < Counts[index]; j++)
                {
                    var startPos = new Vector2(Random.Range(10f + i * range, 10f + i * range + range), Random.Range(-9f, 9f));
                    var ship = Instantiate(Ships.Prefabs[i], startPos, Quaternion.identity, Ships.AllyParent.transform);
                    ship.GetComponentInChildren<UnitBase>().IsAlly = true;
                }
                // Enemy
                for (var j = 0; j < Counts[index + 1]; j++)
                {
                    var startPos = new Vector2(Random.Range(-10f - i * range, -10f - i * range + range), Random.Range(-9f, 9f));
                    var ship = Instantiate(Ships.Prefabs[i], startPos, Quaternion.identity, Ships.EnemyParent.transform);
                    ship.GetComponentInChildren<UnitBase>().IsAlly = false;
                }
            }

            UiUpdateShipBoards();
        }

        public void EndBattle(bool isFullBattle)
        {
            StartCoroutine(End(isFullBattle));
        }

        private IEnumerator End(bool isFullBattle)
        {
            yield return new WaitForEndOfFrame();

            if (_isInBattle)
            {
                _isInBattle = false;

                if (isFullBattle)
                {
                    Points += PointsEarned;
                    UiUpdatePoints();

                    if (Setup.AllyShips.Count > 0)
                    {
                        if (PlayGamesPlatform.Instance.IsAuthenticated())
                        {
                            Social.ReportScore(Points, "CgkIoLGG7ZMFEAIQBQ", (bool success) => { });
                        }

                        if (_isFirstBattle)
                        {
                            PlayGamesPlatform.Instance.UnlockAchievement(Setup.Achievements.Beginner, (bool success) => { });
                        }

                        Level++;
                    }
                    UiUpdateLevelText();
                }

                Destroy(Ships.AllyParent);
                Destroy(Ships.EnemyParent);

                Setup.MainMenuStateParent.SetActive(true);
                Setup.GameStateParent.SetActive(false);

                Setup.Stars.Clear();
                Setup.BattleMusic.Stop();

                _isFirstBattle = false;

                _googleAds.RequestInterstitial();

                StartCoroutine(WaitForAd());

                Save();
            }

            yield return null;
        }

        private IEnumerator WaitForAd()
        {
            if (!_googleAds.Interstitial.IsLoaded())
            {
                yield return new WaitForSecondsRealtime(1f);
            }
            _googleAds.Interstitial.Show();
        }

        private IEnumerator TimeInBattle()
        {
            while (_isInBattle)
            {
                if (_secondsInBattle == 59)
                {
                    _minutesInBattle++;
                    _secondsInBattle = 0;
                }
                yield return new WaitForSeconds(1f);
                _secondsInBattle++;
                Points += 5;
                Setup.TimeText.text = $"{_minutesInBattle:00}:{_secondsInBattle:00}";
            }

            yield return null;
        }

        public void UiUpdatePoints()
        {
            Setup.PointsText.text = Points.ToString();
        }

        public void UiUpdateLevelText()
        {
            Setup.LevelText.text = Level.ToString();
        }

        public void UiUpdateShipBoards()
        {
            Setup.AllyAttackers.text = AllyAttackersCurrent.ToString();
            Setup.AllyTanks.text = AllyTanksCurrent.ToString();
            Setup.AllyMedics.text = AllyMedicsCurrent.ToString();

            Setup.EnemyAttackers.text = EnemyAttackersCurrent.ToString();
            Setup.EnemyTanks.text = EnemyTanksCurrent.ToString();
            Setup.EnemyMedics.text = EnemyMedicsCurrent.ToString();
        }

        private void OnApplicationPause(bool pause)
        {
            if (_isInBattle || !_isGameLoaded) return;
            Save();
        }

        private void OnApplicationQuit()
        {
            if (_isInBattle) return;
            Save();
        }

        public void Save()
        {
            var data = new SaveData
            {
                Points = Points,
                Level = Level,
                IsFirstBattle = _isFirstBattle,
                AttackerUpgradeCost = _upgrades.AttackerUpgradeCost,
                TankUpgradeCost = _upgrades.TankUpgradeCost,
                MedicUpgradeCost = _upgrades.MedicUpgradeCost,
                // Ally ships
                AllyAttackers = AllyAttackers,
                AllyTanks = AllyTanks,
                AllyMedics = AllyMedics,
                // Enemy ships
                EnemyAttackers = EnemyAttackers,
                EnemyTanks = EnemyTanks,
                EnemyMedics = EnemyMedics,
                // Ally stats
                AttackerHealthMax = UnitStats.AttackerHealthMax,
                AttackerDamage = UnitStats.AttackerDamage,
                AttackerReloadTime = UnitStats.AttackerReloadTime,

                TankHealthMax = UnitStats.TankHealthMax,
                TankDamage = UnitStats.TankDamage,
                TankReloadTime = UnitStats.TankReloadTime,
                TankShieldCapacity = UnitStats.TankShieldCapacity,

                MedicHealthMax = UnitStats.MedicHealthMax,
                MedicDamage = UnitStats.MedicDamage,
                MedicReloadTime = UnitStats.MedicReloadTime,
                MedicHealPower = UnitStats.MedicHealPower,

                // Enemy stats
                EnemyAttackerHealthMax = UnitStats.EnemyAttackerHealthMax,
                EnemyAttackerDamage = UnitStats.EnemyAttackerDamage,
                EnemyAttackerReloadTime = UnitStats.EnemyAttackerReloadTime,

                EnemyTankHealthMax = UnitStats.EnemyTankHealthMax,
                EnemyTankDamage = UnitStats.EnemyTankDamage,
                EnemyTankReloadTime = UnitStats.EnemyTankReloadTime,
                EnemyTankShieldCapacity = UnitStats.EnemyTankShieldCapacity,

                EnemyMedicHealthMax = UnitStats.EnemyMedicHealthMax,
                EnemyMedicDamage = UnitStats.EnemyMedicDamage,
                EnemyMedicReloadTime = UnitStats.EnemyMedicReloadTime,
                EnemyMedicHealPower = UnitStats.EnemyMedicHealPower
            };

            SaveLoad.Save(data);
        }

        private void Load()
        {
            if (SaveLoad.Load() == null) return;
            var data = SaveLoad.Load();
            Points = data.Points;
            Level = data.Level;
            _isFirstBattle = data.IsFirstBattle;
            _upgrades.AttackerUpgradeCost = data.AttackerUpgradeCost;
            _upgrades.TankUpgradeCost = data.TankUpgradeCost;
            _upgrades.MedicUpgradeCost = data.MedicUpgradeCost;
            // Ally ships
            AllyAttackers = data.AllyAttackers;
            AllyTanks = data.AllyTanks;
            AllyMedics = data.AllyMedics;
            // Enemy ships
            EnemyAttackers = data.EnemyAttackers;
            EnemyTanks = data.EnemyTanks;
            EnemyMedics = data.EnemyMedics;
            // Ally stats
            UnitStats.AttackerHealthMax = data.AttackerHealthMax;
            UnitStats.AttackerDamage = data.AttackerDamage;
            UnitStats.AttackerReloadTime = data.AttackerReloadTime;

            UnitStats.TankHealthMax = data.TankHealthMax;
            UnitStats.TankDamage = data.TankDamage;
            UnitStats.TankReloadTime = data.TankReloadTime;
            UnitStats.TankShieldCapacity = data.TankShieldCapacity;

            UnitStats.MedicHealthMax = data.MedicHealthMax;
            UnitStats.MedicDamage = data.MedicDamage;
            UnitStats.MedicReloadTime = data.MedicReloadTime;
            UnitStats.MedicHealPower = data.MedicHealPower;

            // Enemy stats
            UnitStats.EnemyAttackerHealthMax = data.EnemyAttackerHealthMax;
            UnitStats.EnemyAttackerDamage = data.EnemyAttackerDamage;
            UnitStats.EnemyAttackerReloadTime = data.EnemyAttackerReloadTime;

            UnitStats.EnemyTankHealthMax = data.EnemyTankHealthMax;
            UnitStats.EnemyTankDamage = data.EnemyTankDamage;
            UnitStats.EnemyTankReloadTime = data.EnemyTankReloadTime;
            UnitStats.EnemyTankShieldCapacity = data.EnemyTankShieldCapacity;

            UnitStats.EnemyMedicHealthMax = data.EnemyMedicHealthMax;
            UnitStats.EnemyMedicDamage = data.EnemyMedicDamage;
            UnitStats.EnemyMedicReloadTime = data.EnemyMedicReloadTime;
            UnitStats.EnemyMedicHealPower = data.EnemyMedicHealPower;
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public int Points;
        public int Level;
        public bool IsFirstBattle;
        public int AttackerUpgradeCost;
        public int TankUpgradeCost;
        public int MedicUpgradeCost;

        public int AllyAttackers;
        public int AllyTanks;
        public int AllyMedics;

        public int EnemyAttackers;
        public int EnemyTanks;
        public int EnemyMedics;


        // Ally ***********************************************
        // Attacker
        public int AttackerHealthMax;
        public int AttackerDamage;
        public float AttackerReloadTime;
        // Tank
        public int TankHealthMax;
        public int TankDamage;
        public float TankReloadTime;
        public int TankShieldCapacity;
        // Medic
        public int MedicHealthMax;
        public int MedicDamage;
        public float MedicReloadTime;
        public int MedicHealPower;

        // Enemy *********************************************
        // Attacker
        public int EnemyAttackerHealthMax;
        public int EnemyAttackerDamage;
        public float EnemyAttackerReloadTime;
        // Tank
        public int EnemyTankHealthMax;
        public int EnemyTankDamage;
        public float EnemyTankReloadTime;
        public int EnemyTankShieldCapacity;
        // Medic
        public int EnemyMedicHealthMax;
        public int EnemyMedicDamage;
        public float EnemyMedicReloadTime;
        public int EnemyMedicHealPower;
    }
}
