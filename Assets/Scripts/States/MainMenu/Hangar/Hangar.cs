using BOYAREngine.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace BOYAREngine.MainMenu
{
    public class Hangar : MonoBehaviour
    {
        [SerializeField] private GameObject _hangarGo;

        [Header("Attacker")]
        [SerializeField] private Text _attackerCountText;
        private int _attackerCost = 1000;
        [SerializeField] private Text _attackerCostText;
        [Header("Tank")]
        [SerializeField] private Text _tankCountText;
        private int _tankCost = 1000;
        [SerializeField] private Text _tankCostText;
        [Header("Medic")]
        [SerializeField] private Text _medicCountText;
        private int _medicCost = 1000;
        [SerializeField] private Text _medicCostText;

        private GameController _gc;

        private void Start()
        {
            _gc = GameController.Instance;

            _attackerCostText.text = _attackerCost.ToString();
            _tankCostText.text = _tankCost.ToString();

            UiHangarUpdate();
        }

        public void ToggleHangarTab()
        {
            MainMenuEvents.CloseTabs?.Invoke();
            _hangarGo.SetActive(true);

            if (_hangarGo.activeSelf)
            {
                UiHangarUpdate();
            }
        }

        // Attacker
        public void BuyAttacker()
        {
            if (_gc.Points >= _attackerCost)
            {
                _gc.Points -= _attackerCost;
                _gc.UiUpdatePoints();
                _gc.AllyAttackers++;

                UiHangarUpdate();
            }
        }

        // Tank
        public void BuyTank()
        {
            if (_gc.Points >= _tankCost)
            {
                _gc.Points -= _tankCost;
                _gc.UiUpdatePoints();
                _gc.AllyTanks++;

                UiHangarUpdate();
            }
        }

        // Medic
        public void BuyMedic()
        {
            if (_gc.Points >= _medicCost)
            {
                _gc.Points -= _medicCost;
                _gc.UiUpdatePoints();
                _gc.AllyMedics++;

                UiHangarUpdate();
            }
        }

        private void UiHangarUpdate()
        {
            _attackerCost = _gc.AllyAttackers / 2 * 1000;
            _attackerCountText.text = _gc.AllyAttackers.ToString();
            _attackerCostText.text = _attackerCost.ToString();

            _tankCost = _gc.AllyTanks / 2 * 2000;
            _tankCountText.text = _gc.AllyTanks.ToString();
            _tankCostText.text = _tankCost.ToString();

            _medicCost = _gc.AllyMedics / 2 * 3000;
            _medicCountText.text = _gc.AllyMedics.ToString();
            _medicCostText.text = _medicCost.ToString();
        }

        private void OnClose()
        {
            _hangarGo.SetActive(false);
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

