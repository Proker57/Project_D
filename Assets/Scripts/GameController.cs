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

        [Header("Managing visibility of objects between states")]
        public GameObject MainMenu;
        public GameObject Game;
        [Header("InGame UI")]
        public Text AlliesCountText;
        public Text EnemiesCountText;

        [Space]
        public CameraTargetGroup CameraTargetGroup;
        public List<GameObject> AllyShips;
        public List<GameObject> EnemyShips;

        [Header("Points")]
        public int Points;

        private void Awake()
        {
            Instance = this;
        }

        public void EndBattle()
        {
            MainMenu.SetActive(true);
            Game.SetActive(false);
        }
    }
}
