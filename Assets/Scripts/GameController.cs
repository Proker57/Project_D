using System.Collections;
using System.Collections.Generic;
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

            [Header("Ally ships")]
            public GameObject AttackerAlly;

            [Header("Enemy ships")]
            public GameObject AttackerEnemy;

            [HideInInspector] public GameObject AllyParent;
            [HideInInspector] public GameObject EnemyParent;
        }
        public AvailableShips Ships;

        [Header("Points")]
        public int Points;

        [Header("Battle settings")]
        public int AllyShipsCount;
        public int EnemyShipsCount;

        private int _secondsInBattle;
        private int _minutesInBattle;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
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
            StartCoroutine(TimeInBattle());

            Setup.MainMenuStateParent.SetActive(false);
            Setup.GameStateParent.SetActive(true);

            Ships.AllyParent = Instantiate(new GameObject("== ALLY =="), Setup.GameStateParent.transform);
            Ships.EnemyParent = Instantiate(new GameObject("== ENEMY =="), Setup.GameStateParent.transform);

            for (var i = 0; i < AllyShipsCount; i++)
            {
                Instantiate(Ships.AttackerAlly, Ships.SpawnPositionAlly.position, Quaternion.identity, Ships.AllyParent.transform);
            }

            for (var i = 0; i < EnemyShipsCount; i++)
            {
                Instantiate(Ships.AttackerEnemy, Ships.SpawnPositionEnemy.position, Quaternion.identity, Ships.EnemyParent.transform);
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
