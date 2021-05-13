using System.Collections;
using UnityEngine;

namespace BOYAREngine.Units
{
    public class BulletBase : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [HideInInspector] public UnitBase Ship;
        [HideInInspector] public bool IsAlly;
        private float _speed;
        private bool _isAlive;

        private Vector3 _direction;

        private void Start()
        {
            _speed = Ship.BulletSpeed;
            IsAlly = Ship.IsAlly;
            _isAlive = true;

            SetColors();
        }

        private void Update()
        {
            if (_isAlive && (Ship.ShootTargetTransform != null))
            {
                StopCoroutine(nameof(BackToPool));
                _direction = Ship.ShootTargetTransform.position - transform.position;
                _direction.Normalize();
                //Invoke(nameof(BackToPool), Ship.BulletLifeTime);
                StartCoroutine(nameof(BackToPool));
                _isAlive = false;
            }

            _rigidbody2D.velocity = _direction * _speed;
        }

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<UnitBase>() != null)
            {
                var ship = other.gameObject.GetComponent<UnitBase>();
            
                if (ship.IsAlly != IsAlly)
                {
                    ship.ReceiveDamage(Ship.Damage);
                    Ship.SpecialCurrent++;
                    Ship.UpdateSpecialUi();
                    Deactivate();
                }
            }
        }

        private IEnumerator BackToPool()
        {
            yield return new WaitForSeconds(Ship.BulletLifeTime);
            Deactivate();
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void SetColors()
        {
            _spriteRenderer.color = Ship.SpriteRenderer.color;

            if (IsAlly)
            {
                if (ColorUtility.TryParseHtmlString("#B1FF55", out var color))
                {
                    _spriteRenderer.color = color;
                }
            }
            else
            {
                if (ColorUtility.TryParseHtmlString("#FF91E7", out var color))
                {
                    _spriteRenderer.color = color;
                }
            }
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

