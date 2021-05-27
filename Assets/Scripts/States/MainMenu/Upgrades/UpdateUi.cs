using BOYAREngine.MainMenu;
using BOYAREngine.Units;
using UnityEngine;
using UnityEngine.UI;

namespace BOYAREngine.Upgrades
{
    public class UpdateUi : MonoBehaviour
    {
        [SerializeField] private Text _maxHealthCost;
        [SerializeField] private Text _maxHealthCurrent;
        [Space]
        [SerializeField] private Text _damageCost;
        [SerializeField] private Text _damageCurrent;
        [Space]
        [SerializeField] private Text _reloadTimeCost;
        [SerializeField] private Text _reloadTimeCurrent;
        [Space]
        [SerializeField] private MainMenu.Upgrades _upgrades;

        public void UiUpdate()
        {
            var type = gameObject.name;
            switch (type)
            {
                case "Attacker":
                    _maxHealthCost.text = _upgrades.AttackerUpgradeCost.ToString();
                    _maxHealthCurrent.text = UnitStats.AttackerHealthMax.ToString();
                    _damageCost.text = _upgrades.AttackerUpgradeCost.ToString();
                    _damageCurrent.text = UnitStats.AttackerDamage.ToString();
                    _reloadTimeCost.text = _upgrades.AttackerUpgradeCost.ToString();
                    _reloadTimeCurrent.text = UnitStats.AttackerReloadTime.ToString("0.00");
                    break;
                case "Tank":
                    _maxHealthCost.text = _upgrades.TankUpgradeCost.ToString();
                    _maxHealthCurrent.text = UnitStats.TankHealthMax.ToString();
                    _damageCost.text = _upgrades.TankUpgradeCost.ToString();
                    _damageCurrent.text = UnitStats.TankDamage.ToString();
                    _reloadTimeCost.text = _upgrades.TankUpgradeCost.ToString();
                    _reloadTimeCurrent.text = UnitStats.TankReloadTime.ToString("0.00");
                    break;
                case "Medic":
                    _maxHealthCost.text = _upgrades.MedicUpgradeCost.ToString();
                    _maxHealthCurrent.text = UnitStats.MedicHealthMax.ToString();
                    _damageCost.text = _upgrades.MedicUpgradeCost.ToString();
                    _damageCurrent.text = UnitStats.MedicDamage.ToString();
                    _reloadTimeCost.text = _upgrades.MedicUpgradeCost.ToString();
                    _reloadTimeCurrent.text = UnitStats.MedicReloadTime.ToString("0.00");
                    break;
            }
        }

        private void OnEnable()
        {
            MainMenuEvents.UpgradeUpdateUi += UiUpdate;
            UiUpdate();
        }
    }
}

