using UnityEngine;

namespace BOYAREngine.Units
{
    public class BulletBase : MonoBehaviour
    {
        public UnitBase Ship;
        public bool IsAlly;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        private float _speed;
        private bool _isAlive;

        private Vector3 _direction;

        private void Start()
        {
            _speed = Ship.BulletSpeed;
            IsAlly = Ship.IsAlly;
            _isAlive = true;
        }

        private void Update()
        {
            if (_isAlive && (Ship.ShootTargetTransform != null))
            {
                _direction = Ship.ShootTargetTransform.position - transform.position;
                _direction.Normalize();
                Invoke(nameof(BackToPool), Ship.BulletLifeTime);
                _isAlive = false;
            }

            _rigidbody2D.velocity = _direction * _speed;
        }

        internal virtual void BackToPool()
        {
            gameObject.SetActive(false);
        }

        public int ReceiveDamage()
        {
            Ship.SpecialCurrent++;
            Ship.UpdateSpecialUi();

            return Ship.Damage;
        }

        private void OnEnable()
        {
            if (Ship != null) _speed = Ship.BulletSpeed;
            _isAlive = true;
        }

        private void OnDisable()
        {
            _isAlive = false;
        }
    }
}

