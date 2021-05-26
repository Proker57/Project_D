using BOYAREngine.Units;
using UnityEngine;
using UnityEngine.UI;

namespace BOYAREngine.Upgrades
{
    public class UpdateUi : MonoBehaviour
    {
        [SerializeField] private string _type;
        [Space]
        [SerializeField] private Text _maxHealthCost;
        [SerializeField] private Text _maxHealthCurrent;
        [Space]
        [SerializeField] private Text _damageCost;
        [SerializeField] private Text _damageCurrent;
        [Space]
        [SerializeField] private Text _reloadTimeCost;
        [SerializeField] private Text _reloadTimeCurrent;

        public void UiUpdate(string type)
        {
            switch (type)
            {
                case "Attacker":
                    _maxHealthCurrent.text = AttackerStats.AllyHealthMax.ToString();
                    //_damageCost.text = 
                    _damageCurrent.text = AttackerStats.AllyDamage.ToString();
                    //_reloadTimeCost.text = 
                    _reloadTimeCurrent.text = AttackerStats.AllyReloadTime.ToString();
                    break;
                case "Tank":
                    _maxHealthCurrent.text = TankStats.AllyHealthMax.ToString();
                    //_damageCost.text = 
                    _damageCurrent.text = TankStats.AllyDamage.ToString();
                    //_reloadTimeCost.text = 
                    _reloadTimeCurrent.text = TankStats.AllyReloadTime.ToString();
                    break;
                case "Medic":
                    _maxHealthCurrent.text = MedicStats.AllyHealthMax.ToString();
                    //_damageCost.text = 
                    _damageCurrent.text = MedicStats.AllyDamage.ToString();
                    //_reloadTimeCost.text = 
                    _reloadTimeCurrent.text = MedicStats.AllyReloadTime.ToString();
                    break;
            }
            
        }

        private void OnEnable()
        {
            UiUpdate(_type);
        }
    }
}

