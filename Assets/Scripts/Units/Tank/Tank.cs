using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine.Units
{
    public class Tank : UnitBase
    {
        [Header("Tank settings")]
        [SerializeField] private GameObject _forceField;

        private GameController _gc;

        private string _typeName;

        protected override void Start()
        {
            base.Start();

            _gc = GameController.Instance;
            _typeName = GetType().Name;

            Type = _typeName;

            HealthMax = IsAlly ? TankStats.AllyHealthMax : TankStats.EnemyHealthMax;
            Damage = IsAlly ? TankStats.AllyDamage : TankStats.EnemyDamage;
            ReloadTime = IsAlly ? TankStats.AllyReloadTime : TankStats.EnemyReloadTime;

            if (_typeName.Equals("Tank"))
            {
                if (IsAlly)
                {
                    _gc.AllyTanksCurrent++;
                }
                else
                {
                    _gc.EnemyTanksCurrent++;
                }

                _gc.UiUpdateShipBoards();
            }
        }

        protected override void Death()
        {
            _forceField.SetActive(false);
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsSpecialActive)
            {
                base.OnTriggerEnter2D(other);
            }
        }

        protected override void UseSpecialAbility()
        {
            base.UseSpecialAbility();

            _forceField.SetActive(true);

            Invoke(nameof(ForceFieldOff), SpecialDuration);
        }

        private void ForceFieldOff()
        {
            _forceField.SetActive(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_typeName.Equals("Tank"))
            {
                if (IsAlly)
                {
                    _gc.AllyTanksCurrent--;
                }
                else
                {
                    _gc.EnemyTanksCurrent--;
                }

                _gc.UiUpdateShipBoards();
            }
        }
    }
}

