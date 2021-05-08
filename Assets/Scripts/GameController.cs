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

        public delegate void AddShipEvent(UnitBase ship);
        public AddShipEvent AddShip;
        public delegate void RemoveShipEvent(UnitBase ship);
        public AddShipEvent RemoveShip;

        [Header("Managing visibility of objects between states")]
        public GameObject MainMenuObject;
        public GameObject GameObject;
        [Header("InGame UI")]
        public Text AlliesCountText;
        public Text EnemiesCountText;

        [Space]
        [SerializeField] private CameraTargetGroup _cameraTargetGroup;
        public List<GameObject> AllyShips;
        public List<GameObject> EnemyShips;

        private void Awake()
        {
            Instance = this;
        }

        public void AddShips(UnitBase ship)
        {
            if (ship.IsAlly)
            {
                AllyShips.Add(ship.gameObject);
                AlliesCountText.text = "Allies: " + AllyShips.Count;
            }
            else
            {
                EnemyShips.Add(ship.gameObject);
                EnemiesCountText.text = "Enemies: " + EnemyShips.Count;
            }

            _cameraTargetGroup.Targets.Add(ship.gameObject);
            
        }

        public void RemoveShips(UnitBase ship)
        {
            if (ship.IsAlly)
            {
                AllyShips.Remove(ship.gameObject);
                AlliesCountText.text = "Allies: " + AllyShips.Count;
            }
            else
            {
                EnemyShips.Remove(ship.gameObject);
                EnemiesCountText.text = "Enemies: " + EnemyShips.Count;
            }

            _cameraTargetGroup.Targets.Remove(ship.gameObject);
        }

        private void OnEnable()
        {
            AddShip += AddShips;
            RemoveShip += RemoveShips;
        }

        private void OnDisable()
        {
            AddShip -= AddShips;
            RemoveShip -= RemoveShips;
        }
    }
}
