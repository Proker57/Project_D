using UnityEngine;

namespace BOYAREngine.Units
{
    public class Attacker : UnitBase
    {
        private void Awake()
        {

        }

        protected override void UseSpecialAbility()
        {
            BulletSpeed *= 3;

            Invoke(nameof(RestoreBulletSpeed), 6f);
        }

        private void RestoreBulletSpeed()
        {
            BulletSpeed /= 3;
        }
    }
}

