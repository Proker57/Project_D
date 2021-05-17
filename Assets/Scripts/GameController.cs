using System.Collections;
using System.Collections.Generic;
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

            [Header("InGame UI")]
            public Text AlliesCountText;
            public Text EnemiesCountText;
            public Text TimeText;
            [Space]
            public Text AllyAttackers;
            public Text AllyTanks;
            public Text AllyMedics;
            //
            public Text EnemyAttackers;
            public Text EnemyTanks;
            public Text EnemyMedics;

            [Space]
            public CameraTargetGroup CameraTargetGroup;
            public List<GameObject> AllyShips;
            public List<GameObject> EnemyShips;
        }

        [System.Serializable]
        public struct AvailableShips
        {
            public Transform SpawnPositionAlly;
            public Transform SpawnPositionEnemy;

            [Header("Ships")]
            public GameObject Attacker;
            public GameObject Tank;
            public GameObject Medic;

            [HideInInspector] public GameObject AllyParent;
            [HideInInspector] public GameObject EnemyParent;
        }

        [Header("Points")]
        public int Points;
        public int PointsEarned;

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

        public NoneGameSettings Setup;
        public AvailableShips Ships;
        private GoogleAds _googleAds;
        private int _secondsInBattle;
        private int _minutesInBattle;

        private void Awake()
        {
            Instance = this;
            // Initialize the Google Mobile Ads SDK.
            _googleAds = new GoogleAds();
            MobileAds.Initialize(initStatus => { });
        }

        private void Start()
        {
            _googleAds.RequestBanner();

            UiUpdatePoints();
        }

        private void LateUpdate()
        {
            if (_isInBattle && (Setup.AllyShips.Count == 0 || Setup.EnemyShips.Count == 0))
            {
                EndBattle(true);
            }
        }

        public void StartBattle()
        {
            _isInBattle = true;

            PointsEarned = 0;

            _secondsInBattle = 0;
            _minutesInBattle = 0;
            Setup.TimeText.text = _minutesInBattle.ToString("00") + ":" + _secondsInBattle.ToString("00");
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

            //SpawnAllyShips();
            //SpawnEnemyShips();

            MiniTask.Run(0f, SpawnShips);

            _googleAds.BannerView.Destroy();

            UiUpdateShipBoards();
        }

        private void SpawnShips()
        {
            SpawnAllyShips();
            SpawnEnemyShips();
        }

        private void SpawnAllyShips()
        {
            
            // Attacker
            for (var i = 0; i < AllyAttackers; i++)
            {
                var startPos = new Vector2(Random.Range(10f, 15f), Random.Range(-9f, 9f));
                //var ship = Instantiate(Ships.Attacker, Ships.SpawnPositionAlly.position, Quaternion.identity, Ships.AllyParent.transform);
                var ship = Instantiate(Ships.Attacker, startPos, Quaternion.identity, Ships.AllyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = true;
            }
            // Tank
            for (var i = 0; i < AllyTanks; i++)
            {
                var startPos = new Vector2(Random.Range(15f, 18f), Random.Range(-9f, 9f));
                //var ship = Instantiate(Ships.Tank, Ships.SpawnPositionAlly.position, Quaternion.identity, Ships.AllyParent.transform);
                var ship = Instantiate(Ships.Tank, startPos, Quaternion.identity, Ships.AllyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = true;
            }
            // Medic
            for (var i = 0; i < AllyMedics; i++)
            {
                var startPos = new Vector2(Random.Range(18f, 19f), Random.Range(-9f, 9f));
                //var ship = Instantiate(Ships.Medic, Ships.SpawnPositionAlly.position, Quaternion.identity, Ships.AllyParent.transform);
                var ship = Instantiate(Ships.Medic, startPos, Quaternion.identity, Ships.AllyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = true;
            }
        }

        private void SpawnEnemyShips()
        {
            // Attacker
            for (var i = 0; i < EnemyAttackers; i++)
            {
                var startPos = new Vector2(Random.Range(-10f, -15f), Random.Range(-9f, 9f));
                //var ship = Instantiate(Ships.Attacker, Ships.SpawnPositionEnemy.position, Quaternion.identity, Ships.EnemyParent.transform);
                var ship = Instantiate(Ships.Attacker, startPos, Quaternion.identity, Ships.EnemyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = false;
            }
            // Tank
            for (var i = 0; i < EnemyTanks; i++)
            {
                var startPos = new Vector2(Random.Range(-15f, -18f), Random.Range(-9f, 9f));
                //var ship = Instantiate(Ships.Tank, Ships.SpawnPositionEnemy.position, Quaternion.identity, Ships.EnemyParent.transform);
                var ship = Instantiate(Ships.Tank, startPos, Quaternion.identity, Ships.EnemyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = false;
            }
            // Medic
            for (var i = 0; i < EnemyMedics; i++)
            {
                var startPos = new Vector2(Random.Range(-18f, -19f), Random.Range(-9f, 9f));
                //var ship = Instantiate(Ships.Medic, Ships.SpawnPositionEnemy.position, Quaternion.identity, Ships.EnemyParent.transform);
                var ship = Instantiate(Ships.Medic, startPos, Quaternion.identity, Ships.EnemyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = false;
            }
        }

        public void EndBattle(bool isFullBattle)
        {
            Destroy(Ships.AllyParent);
            Destroy(Ships.EnemyParent);

            if (isFullBattle)
            {
                Points += PointsEarned;
            }

            Setup.MainMenuStateParent.SetActive(true);
            Setup.GameStateParent.SetActive(false);

            StopCoroutine(TimeInBattle());

            UiUpdatePoints();

            _googleAds.RequestInterstitial();

            while (!_googleAds.Interstitial.IsLoaded())
            {
                Debug.Log("Wainting");
            }

            _googleAds.Interstitial.Show();

            _isInBattle = false;
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
                Setup.TimeText.text = _minutesInBattle.ToString("00") + ":" + _secondsInBattle.ToString("00");
            }
        }

        public void UiUpdatePoints()
        {
            Setup.PointsText.text = Points.ToString();
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
    }
}
