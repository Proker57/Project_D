using UnityEngine;

namespace BOYAREngine.Ui
{
    public class UiSettings : MonoBehaviour
    {
        /// <summary>
        /// 0: Hp, 1: Special, 2: SpecialCountdown
        /// </summary>
        /// <param name="type">Type of UI</param>
        public delegate void ToggleShipUiDelegate(int type);
        public static ToggleShipUiDelegate ToggleShipUi;

        [SerializeField] private GameObject _hpBarButton;
        [SerializeField] private GameObject _specialBarButton;
        [SerializeField] private GameObject _specialCountdownButton;

        public void ToggleUiSettings()
        {
            _hpBarButton.SetActive(!_hpBarButton.activeSelf);
            _specialBarButton.SetActive(!_specialBarButton.activeSelf);
            _specialCountdownButton.SetActive(!_specialCountdownButton.activeSelf);
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
    }
}

