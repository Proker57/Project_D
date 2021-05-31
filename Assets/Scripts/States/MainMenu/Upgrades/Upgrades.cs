using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine.MainMenu
{
    public class Upgrades : MonoBehaviour
    {
        [SerializeField] private GameObject _upgradesGo;

        public int AttackerUpgradeCost = 200;
        public int TankUpgradeCost = 200;
        public int MedicUpgradeCost = 200;

        private GameController _gc;

        private void Start()
        {
            _gc = GameController.Instance;
        }

        public void ToggleUpgradesTab()
        {
            MainMenuEvents.CloseTabs?.Invoke();
            _upgradesGo.SetActive(!_upgradesGo.activeSelf);

            if (_upgradesGo.activeSelf)
            {
                UiUpgradesUpdate();
            }
        }

        private void UiUpgradesUpdate()
        {

        }

        private void OnClose()
        {
            _upgradesGo.SetActive(false);
        }

        private void OnEnable()
        {
            MainMenuEvents.CloseTabs += OnClose;
        }

        private void OnDisable()
        {
            MainMenuEvents.CloseTabs -= OnClose;
        }
    }
}

