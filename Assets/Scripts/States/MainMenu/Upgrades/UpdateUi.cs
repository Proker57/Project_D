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

        [Header("Tank")]
        [SerializeField] private Text _tankShieldCost;
        [SerializeField] private Text _tankShieldCurrent;
        [Header("Medic")]
        [SerializeField] private Text _medicHealPowerCost;
        [SerializeField] private Text _medicHealPowerCurrent;

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
                    _reloadTimeCurrent.text = $"{UnitStats.AttackerReloadTime:0.00}s";
                    break;
                case "Tank":
                    _maxHealthCost.text = _upgrades.TankUpgradeCost.ToString();
                    _maxHealthCurrent.text = UnitStats.TankHealthMax.ToString();
                    _damageCost.text = _upgrades.TankUpgradeCost.ToString();
                    _damageCurrent.text = UnitStats.TankDamage.ToString();
                    _reloadTimeCost.text = _upgrades.TankUpgradeCost.ToString();
                    _reloadTimeCurrent.text = $"{UnitStats.TankReloadTime:0.00}s";
                    _tankShieldCost.text = _upgrades.TankUpgradeCost.ToString();
                    _tankShieldCurrent.text = UnitStats.TankShieldCapacity.ToString();
                    break;
                case "Medic":
                    _maxHealthCost.text = _upgrades.MedicUpgradeCost.ToString();
                    _maxHealthCurrent.text = UnitStats.MedicHealthMax.ToString();
                    _damageCost.text = _upgrades.MedicUpgradeCost.ToString();
                    _damageCurrent.text = UnitStats.MedicDamage.ToString();
                    _reloadTimeCost.text = _upgrades.MedicUpgradeCost.ToString();
                    _reloadTimeCurrent.text = $"{UnitStats.MedicReloadTime:0.00}s";
                    _medicHealPowerCost.text = _upgrades.MedicUpgradeCost.ToString();
                    _medicHealPowerCurrent.text = UnitStats.MedicHealPower.ToString();
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

