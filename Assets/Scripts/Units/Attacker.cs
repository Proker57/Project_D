using UnityEngine;

namespace BOYAREngine.Units
{
    public class Attacker : UnitBase
    {
        protected override void UseSpecialAbility()
        {
            base.UseSpecialAbility();

            BulletSpeed *= 3;

            Invoke(nameof(RestoreBulletSpeed), SpecialDuration);
        }

        private void RestoreBulletSpeed()
        {
            BulletSpeed /= 3;
        }
    }
}

