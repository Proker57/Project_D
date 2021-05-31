using BOYAREngine.Controller;
using BOYAREngine.MainMenu;
using BOYAREngine.Units;
using UnityEngine;

namespace BOYAREngine.Upgrades
{
    public class ButtonEvents : MonoBehaviour
    {
        [SerializeField] private MainMenu.Upgrades _upgrades;
        [Space]
        public int AddMaxHealthValue = 5;
        public int AddDamage = 3;
        public float AddReloadTime = -0.05f;
        // Tank
        public int AddShieldCapacity = 5;
        // Medic
        public int AddHealPower = 5;

        private GameController _gc;

        private void Start()
        {
            _gc = GameController.Instance;
        }

        // Attacker
        public void AttackerAddMaxHealth()
        {
            if (_gc.Points >= _upgrades.AttackerUpgradeCost)
            {
                UnitStats.AttackerHealthMax += AddMaxHealthValue;
                _gc.Points -= _upgrades.AttackerUpgradeCost;
                _upgrades.AttackerUpgradeCost += 100;

                _gc.UiUpdatePoints();
                MainMenuEvents.UpgradeUpdateUi?.Invoke();
            }
        }

        public void AttackerAddDamage()
        {
            if (_gc.Points >= _upgrades.AttackerUpgradeCost)
            {
                UnitStats.AttackerDamage += AddDamage;
                _gc.Points -= _upgrades.AttackerUpgradeCost;
                _upgrades.AttackerUpgradeCost += 100;

                _gc.UiUpdatePoints();
                MainMenuEvents.UpgradeUpdateUi?.Invoke();
            }
        }

        public void AttackerAddReloadTime()
        {
            if (_gc.Points >= _upgrades.AttackerUpgradeCost)
            {
                UnitStats.AttackerReloadTime += AddReloadTime;
                _gc.Points -= _upgrades.AttackerUpgradeCost;
                _upgrades.AttackerUpgradeCost += 100;

                _gc.UiUpdatePoints();
                MainMenuEvents.UpgradeUpdateUi?.Invoke();
            }
        }

        // Tank
        public void TankAddMaxHealth()
        {
            if (_gc.Points >= _upgrades.TankUpgradeCost)
            {
                UnitStats.TankHealthMax += AddMaxHealthValue;
                _gc.Points -= _upgrades.TankUpgradeCost;
                _upgrades.TankUpgradeCost += 100;

                _gc.UiUpdatePoints();
                MainMenuEvents.UpgradeUpdateUi?.Invoke();
            }
        }

        public void TankAddDamage()
        {
            if (_gc.Points >= _upgrades.TankUpgradeCost)
            {
                UnitStats.TankDamage += AddDamage;
                _gc.Points -= _upgrades.TankUpgradeCost;
                _upgrades.TankUpgradeCost += 100;

                _gc.UiUpdatePoints();
                MainMenuEvents.UpgradeUpdateUi?.Invoke();
            }
        }

        public void TankAddReloadTime()
        {
            if (_gc.Points >= _upgrades.TankUpgradeCost)
            {
                UnitStats.TankReloadTime += AddReloadTime;
                _gc.Points -= _upgrades.TankUpgradeCost;
                _upgrades.TankUpgradeCost += 100;

                _gc.UiUpdatePoints();
                MainMenuEvents.UpgradeUpdateUi?.Invoke();
            }
        }

        public void TankAddShieldCapacity()
        {
            if (_gc.Points >= _upgrades.TankUpgradeCost)
            {
                UnitStats.TankShieldCapacity += AddShieldCapacity;
                _gc.Points -= _upgrades.TankUpgradeCost;
                _upgrades.TankUpgradeCost += 100;

                _gc.UiUpdatePoints();
                MainMenuEvents.UpgradeUpdateUi?.Invoke();
            }
        }

        // Medic
        public void MedicAddMaxHealth()
        {
            if (_gc.Points >= _upgrades.MedicUpgradeCost)
            {
                UnitStats.MedicHealthMax += AddMaxHealthValue;
                _gc.Points -= _upgrades.MedicUpgradeCost;
                _upgrades.MedicUpgradeCost += 100;

                _gc.UiUpdatePoints();
                MainMenuEvents.UpgradeUpdateUi?.Invoke();
            }
        }

        public void MedicAddDamage()
        {
            if (_gc.Points >= _upgrades.MedicUpgradeCost)
            {
                UnitStats.MedicDamage += AddDamage;
                _gc.Points -= _upgrades.MedicUpgradeCost;
                _upgrades.MedicUpgradeCost += 100;

                _gc.UiUpdatePoints();
                MainMenuEvents.UpgradeUpdateUi?.Invoke();
            }
        }

        public void MedicAddReloadTime()
        {
            if (_gc.Points >= _upgrades.MedicUpgradeCost)
            {
                UnitStats.MedicReloadTime += AddReloadTime;
                _gc.Points -= _upgrades.MedicUpgradeCost;
                _upgrades.MedicUpgradeCost += 100;

                _gc.UiUpdatePoints();
                MainMenuEvents.UpgradeUpdateUi?.Invoke();
            }
        }

        public void MedicAddHealPower()
        {
            if (_gc.Points >= _upgrades.MedicUpgradeCost)
            {
                UnitStats.MedicHealPower += AddHealPower;
                _gc.Points -= _upgrades.MedicUpgradeCost;
                _upgrades.MedicUpgradeCost += 100;

                _gc.UiUpdatePoints();
                MainMenuEvents.UpgradeUpdateUi?.Invoke();
            }
        }
    }
}

