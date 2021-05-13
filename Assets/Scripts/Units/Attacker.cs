using UnityEngine;

namespace BOYAREngine.Units
{
    public class Attacker : UnitBase
    {
        [Header("Attacker settings")]
        [SerializeField] private float _speedMultiplier;

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
    }
}

