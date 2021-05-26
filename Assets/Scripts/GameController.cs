using System.Collections;
using System.Collections.Generic;
using System.Threading;
using BOYAREngine.Ads;
using BOYAREngine.Units;
using BOYAREngine.Utils;
using GoogleMobileAds.Api;
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

        private void Awake()
        {
            Instance = this;

            _googleAds = new GoogleAds();
            MobileAds.Initialize(initStatus => { });
        }

        private void Start()
        {
            _googleAds.RequestBanner();

#if !UNITY_EDITOR
            
            Load();
#endif
            Counts = new[]
            {
                AllyAttackers, EnemyAttackers,
                AllyTanks, EnemyTanks,
                AllyMedics, EnemyMedics
            };

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

            Setup.MainMenuStateParent.SetActive(false);
            Setup.GameStateParent.SetActive(true);

            Ships.AllyParent = new GameObject("== ALLY ==");
            Ships.AllyParent.transform.parent = Setup.GameStateParent.transform;
            Ships.EnemyParent = new GameObject("== ENEMY ==");
            Ships.EnemyParent.transform.parent = Setup.GameStateParent.transform;

            Setup.Stars.Play();

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
                Destroy(Ships.AllyParent);
                Destroy(Ships.EnemyParent);

                if (isFullBattle)
                {
                    Points += PointsEarned;
                    UiUpdatePoints();

                    Level++;
                    UiUpdateLevelText();
                }

                Setup.MainMenuStateParent.SetActive(true);
                Setup.GameStateParent.SetActive(false);

                Setup.Stars.Clear();

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
                if (_secondsInBattle == 60)
                {
                    _minutesInBattle++;
                    _secondsInBattle = 0;
                }
                yield return new WaitForSeconds(1f);
                _secondsInBattle++;
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
            Debug.Log("App is Paused");
        }

        private void OnApplicationQuit()
        {
            if (_isInBattle) return;
            Save();
            Debug.Log("App is Quit");
        }

        public void Save()
        {
            var data = new SaveData
            {
                Points = Points,
                Level = Level,

                AllyAttackers = AllyAttackers,
                AllyTanks = AllyTanks,
                AllyMedics = AllyMedics,

                EnemyAttackers = EnemyAttackers,
                EnemyTanks = EnemyTanks,
                EnemyMedics = EnemyMedics
            };

            SaveLoad.Save(data);
        }

        private void Load()
        {
            if (SaveLoad.Load() == null) return;
            var data = SaveLoad.Load();
            Points = data.Points;
            Level = data.Level;

            AllyAttackers = data.AllyAttackers;
            AllyTanks = data.AllyTanks;
            AllyMedics = data.AllyMedics;

            EnemyAttackers = data.EnemyAttackers;
            EnemyTanks = data.EnemyTanks;
            EnemyMedics = data.EnemyMedics;
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public int Points;
        public int Level;

        public int AllyAttackers;
        public int AllyTanks;
        public int AllyMedics;

        public int EnemyAttackers;
        public int EnemyTanks;
        public int EnemyMedics;
    }
}
