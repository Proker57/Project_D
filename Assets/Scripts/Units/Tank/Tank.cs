using UnityEngine;

namespace BOYAREngine.Units
{
    public class Tank : UnitBase
    {
        [SerializeField] private GameObject _forceField;
        //private bool _isSpecialActive;

        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsSpecialActive)
            {
                base.OnTriggerEnter2D(other);
            }
            else
            {
                if (other.gameObject.GetComponent<BulletBase>() != null)
                {
                    var bullet = other.gameObject.GetComponent<BulletBase>();

                    if (bullet.IsAlly != IsAlly)
                    {
                        bullet.BackToPool();
                    }
                }
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
    }
}

