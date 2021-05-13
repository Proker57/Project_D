using UnityEngine;

namespace BOYAREngine.Units
{
    public class Tank : UnitBase
    {
        [Header("Tank settings")]
        [SerializeField] private GameObject _forceField;

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
    }
}

