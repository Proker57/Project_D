using UnityEngine;

namespace BOYAREngine.Upgrades
{
    public class UiLogic : MonoBehaviour
    {
        [SerializeField] private GameObject _helpText;

        [SerializeField] private GameObject _attackerGo;
        [SerializeField] private GameObject _tankGo;
        [SerializeField] private GameObject _medicGo;

        public void OnAttackerClick()
        {
            _helpText.SetActive(false);

            _tankGo.SetActive(false);
            _medicGo.SetActive(false);

            _attackerGo.SetActive(true);
        }

        public void OnTankClick()
        {
            _helpText.SetActive(false);

            _attackerGo.SetActive(false);
            _medicGo.SetActive(false);

            _tankGo.SetActive(true);
        }

        public void OnMedicClick()
        {
            _helpText.SetActive(false);

            _tankGo.SetActive(false);
            _attackerGo.SetActive(false);

            _medicGo.SetActive(true);
        }
    }
}

