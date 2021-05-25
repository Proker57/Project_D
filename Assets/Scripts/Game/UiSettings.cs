using BOYAREngine.Controller;
using BOYAREngine.Units;
using UnityEngine;

namespace BOYAREngine.Ui
{
    public class UiSettings : MonoBehaviour
    {
        /// <summary>
        /// 0: Hp, 1: Special, 2: SpecialCountdown 3: NameTag
        /// </summary>
        /// <param name="type">Type of UI</param>
        public delegate void ToggleShipUiDelegate(int type);
        public static ToggleShipUiDelegate ToggleShipUi;

        [SerializeField] private GameObject _hpBarButton;
        [SerializeField] private GameObject _specialBarButton;
        [SerializeField] private GameObject _specialCountdownButton;
        [SerializeField] private GameObject _nameTag;
        [SerializeField] private GameObject _escapeBattle;

        private bool _isNameTagToggled;

        public void ToggleUiSettings()
        {
            _hpBarButton.SetActive(!_hpBarButton.activeSelf);
            _specialBarButton.SetActive(!_specialBarButton.activeSelf);
            _specialCountdownButton.SetActive(!_specialCountdownButton.activeSelf);
            _nameTag.SetActive(!_nameTag.activeSelf);
            _escapeBattle.SetActive(!_escapeBattle.activeSelf);
        }

        public void ToggleBoardSpecificNameTagAllyOn(string shipType)
        {
            foreach (var ship in GameController.Instance.Setup.AllyShips)
            {
                var unit = ship.GetComponent<UnitBase>();
                if (unit.Type.Equals(shipType))
                {
                    unit.NameTagGo.SetActive(true);
                }
                else
                {
                    unit.NameTagGo.SetActive(false);
                }
            }

            foreach (var ship in GameController.Instance.Setup.EnemyShips)
            {
                var unit = ship.GetComponent<UnitBase>();
                unit.NameTagGo.SetActive(false);
            }
        }

        public void ToggleBoardSpecificNameTagAllyOff(string shipType)
        {
            foreach (var ship in GameController.Instance.Setup.AllyShips)
            {
                var unit = ship.GetComponent<UnitBase>();
                unit.NameTagGo.SetActive(_isNameTagToggled);
            }

            foreach (var ship in GameController.Instance.Setup.EnemyShips)
            {
                var unit = ship.GetComponent<UnitBase>();
                unit.NameTagGo.SetActive(_isNameTagToggled);
            }
        }

        public void ToggleBoardSpecificNameTagEnemyOn(string shipType)
        {
            foreach (var ship in GameController.Instance.Setup.EnemyShips)
            {
                var unit = ship.GetComponent<UnitBase>();
                if (unit.Type.Equals(shipType))
                {
                    unit.NameTagGo.SetActive(true);
                }
                else
                {
                    unit.NameTagGo.SetActive(false);
                }
            }

            foreach (var ship in GameController.Instance.Setup.AllyShips)
            {
                var unit = ship.GetComponent<UnitBase>();
                unit.NameTagGo.SetActive(false);
            }
        }

        public void ToggleBoardSpecificNameTagEnemyOff(string shipType)
        {
            foreach (var ship in GameController.Instance.Setup.EnemyShips)
            {
                var unit = ship.GetComponent<UnitBase>();
                unit.NameTagGo.SetActive(_isNameTagToggled);
            }

            foreach (var ship in GameController.Instance.Setup.AllyShips)
            {
                var unit = ship.GetComponent<UnitBase>();
                unit.NameTagGo.SetActive(_isNameTagToggled);
            }
        }

        public void EscapeBattle()
        {
            Debug.Log("Ad UiSettings");
            GameController.Instance.EndBattle(false);
        }

        public void ToggleHpBar()
        {
            ToggleShipUi?.Invoke(0);
        }

        public void ToggleSpecialBar()
        {
            ToggleShipUi?.Invoke(1);
        }

        public void ToggleSpecialCountdownBar()
        {
            ToggleShipUi?.Invoke(2);
        }

        public void ToggleNameTag()
        {
            _isNameTagToggled = !_isNameTagToggled;
            ToggleShipUi?.Invoke(3);
        }
    }
}

