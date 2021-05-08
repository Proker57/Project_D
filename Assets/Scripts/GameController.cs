using System.Collections.Generic;
using BOYAREngine.Units;
using BOYAREngine.Utils;
using UnityEngine;

namespace BOYAREngine.Controller

{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        public delegate void AddShipEvent(UnitBase ship);
        public AddShipEvent AddShip;

        [Header("Managing visibility of objects between states")]
        public GameObject MainMenuObject;
        public GameObject GameObject;

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
            }
            else
            {
                EnemyShips.Add(ship.gameObject);
            }

            _cameraTargetGroup.FindAllTargets();
        }

        private void OnEnable()
        {
            AddShip += AddShips;
        }

        private void OnDisable()
        {
            AddShip -= AddShips;
        }
    }
}
