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
        private int _attackerCost = 100;
        [SerializeField] private Text _attackerCostText;
        [Header("Tank")]
        [SerializeField] private Text _tankCountText;
        private int _tankCost = 100;
        [SerializeField] private Text _tankCostText;
        [Header("Medic")]
        [SerializeField] private Text _medicCountText;
        private int _medicCost = 100;
        [SerializeField] private Text _medicCostText;

        private GameController _gc;

        private void Start()
        {
            _gc = GameController.Instance;

            _attackerCostText.text = _attackerCost.ToString();
            _tankCostText.text = _tankCost.ToString();
        }

        // Attacker
        public void ToggleHangarTab()
        {
            _hangarGo.SetActive(!_hangarGo.activeSelf);

            if (_hangarGo.activeSelf)
            {
                UiUpdate();
            }
        }

        public void BuyAttacker()
        {
            if (_gc.Points >= _attackerCost)
            {
                _gc.Points -= _attackerCost;
                _gc.UiUpdatePoints();
                _gc.AllyAttackers++;

                UiUpdate();
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

                UiUpdate();
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

                UiUpdate();
            }
        }


        private void UiUpdate()
        {
            _attackerCost = _gc.AllyAttackers * 10;
            _attackerCountText.text = _gc.AllyAttackers.ToString();
            _attackerCostText.text = _attackerCost.ToString();

            _tankCost = (int) (_gc.AllyTanks * 10 * 2f);
            _tankCountText.text = _gc.AllyTanks.ToString();
            _tankCostText.text = _tankCost.ToString();

            _medicCost = (int)(_gc.AllyMedics * 10 * 2.5f);
            _medicCountText.text = _gc.AllyMedics.ToString();
            _medicCostText.text = _medicCost.ToString();
        }
    }
}

