using System.Collections;
using System.Collections.Generic;
using BOYAREngine.Units;
using BOYAREngine.Utils;
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
            public CameraTargetGroup CameraTargetGroup;
            public List<GameObject> AllyShips;
            public List<GameObject> EnemyShips;
        }
        public NoneGameSettings Setup;

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
        public AvailableShips Ships;

        [Header("Points")]
        public int Points;

        [Header("Battle settings")]
        public int AllyAttackers;
        public int AllyTanks;
        public int AllyMedics;
        public int EnemyAttackers;
        public int EnemyTanks;
        public int EnemyMedics;

        private int _secondsInBattle;
        private int _minutesInBattle;

        private void Awake()
        {
            Instance = this;
        }

        private void LateUpdate()
        {
            if (_isInBattle && (Setup.AllyShips.Count == 0 || Setup.EnemyShips.Count == 0))
            {
                EndBattle();
            }
        }

        public void StartBattle()
        {
            _isInBattle = true;

            _secondsInBattle = 0;
            _minutesInBattle = 0;
            Setup.TimeText.text = _minutesInBattle.ToString("00") + ":" + _secondsInBattle.ToString("00");
            StartCoroutine(TimeInBattle());

            Setup.MainMenuStateParent.SetActive(false);
            Setup.GameStateParent.SetActive(true);

            Ships.AllyParent = new GameObject("== ALLY ==");
            Ships.AllyParent.transform.parent = Setup.GameStateParent.transform;
            Ships.EnemyParent = new GameObject("== ENEMY ==");
            Ships.EnemyParent.transform.parent = Setup.GameStateParent.transform;

            SpawnAllyShips();
            SpawnEnemyShips();
        }

        private void SpawnAllyShips()
        {
            // Attacker
            for (var i = 0; i < AllyAttackers; i++)
            {
                var ship = Instantiate(Ships.Attacker, Ships.SpawnPositionAlly.position, Quaternion.identity, Ships.AllyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = true;
            }
            // Tank
            for (var i = 0; i < AllyTanks; i++)
            {
                var ship = Instantiate(Ships.Tank, Ships.SpawnPositionAlly.position, Quaternion.identity, Ships.AllyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = true;
            }
            // Medic
            for (var i = 0; i < AllyMedics; i++)
            {
                var ship = Instantiate(Ships.Medic, Ships.SpawnPositionAlly.position, Quaternion.identity, Ships.AllyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = true;
            }
        }

        private void SpawnEnemyShips()
        {
            // Attacker
            for (var i = 0; i < EnemyAttackers; i++)
            {
                var ship = Instantiate(Ships.Attacker, Ships.SpawnPositionEnemy.position, Quaternion.identity, Ships.EnemyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = false;
            }
            // Tank
            for (var i = 0; i < EnemyTanks; i++)
            {
                var ship = Instantiate(Ships.Tank, Ships.SpawnPositionEnemy.position, Quaternion.identity, Ships.EnemyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = false;
            }
            // Medic
            for (var i = 0; i < EnemyMedics; i++)
            {
                var ship = Instantiate(Ships.Medic, Ships.SpawnPositionEnemy.position, Quaternion.identity, Ships.EnemyParent.transform);
                ship.GetComponentInChildren<UnitBase>().IsAlly = false;
            }
        }

        public void EndBattle()
        {
            Destroy(Ships.AllyParent);
            Destroy(Ships.EnemyParent);

            Setup.MainMenuStateParent.SetActive(true);
            Setup.GameStateParent.SetActive(false);

            StopCoroutine(TimeInBattle());

            Setup.PointsText.text = "Points: " + Points;
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
    }
}
