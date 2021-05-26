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

            HealthMax = IsAlly ? AttackerStats.AllyHealthMax : AttackerStats.EnemyHealthMax;
            Damage = IsAlly ? AttackerStats.AllyDamage : AttackerStats.EnemyDamage;
            ReloadTime = IsAlly ? AttackerStats.AllyReloadTime : AttackerStats.EnemyReloadTime;

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

