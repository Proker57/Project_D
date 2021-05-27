using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine.Units
{
    public class Attacker : UnitBase
    {
        [Header("Attacker settings")]
        [SerializeField] private float _speedMultiplier;

        private GameController _gc;

        private string _typeName;

        protected override void Start()
        {
            base.Start();

            HealthMax = IsAlly ? UnitStats.AttackerHealthMax : UnitStats.EnemyAttackerHealthMax;
            Damage = IsAlly ? UnitStats.AttackerDamage : UnitStats.EnemyAttackerDamage;
            ReloadTime = IsAlly ? UnitStats.AttackerReloadTime : UnitStats.EnemyAttackerReloadTime;

            _gc = GameController.Instance;
            _typeName = GetType().Name;

            Type = _typeName;

            if (_typeName.Equals("Attacker"))
            {
                if (IsAlly)
                {
                    _gc.AllyAttackersCurrent++;
                }
                else
                {
                    _gc.EnemyAttackersCurrent++;
                }

                _gc.UiUpdateShipBoards();
            }
        }

        protected override void UseSpecialAbility()
        {
            base.UseSpecialAbility();

            BulletSpeed *= _speedMultiplier;

            Invoke(nameof(RestoreBulletSpeed), SpecialDuration);
        }

        private void RestoreBulletSpeed()
        {
            BulletSpeed /= _speedMultiplier;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_typeName.Equals("Attacker"))
            {
                if (IsAlly)
                {
                    _gc.AllyAttackersCurrent--;
                }
                else
                {
                    _gc.EnemyAttackersCurrent--;
                }

                _gc.UiUpdateShipBoards();
            }
        }
    }
}

