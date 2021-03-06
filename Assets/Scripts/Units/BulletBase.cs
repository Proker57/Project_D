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
        private float _lifeTime;
        private bool _isAlive;
        private bool _canHit;

        private Vector3 _direction;

        private void Start()
        {
            _speed = Ship.BulletSpeed;
            IsAlly = Ship.IsAlly;
            _isAlive = true;
            _canHit = true;

            SetColors();
        }

        private void Update()
        {
            if (_isAlive && (Ship.ShootTargetTransform != null))
            {
                //StopCoroutine(nameof(BackToPool));

                _direction = Ship.ShootTargetTransform.position - transform.position;
                _direction.Normalize();
                //Invoke(nameof(BackToPool), Ship.BulletLifeTime);
                //StartCoroutine(nameof(BackToPool));
                _isAlive = false;
            }

            _lifeTime -= Time.deltaTime;
            if (_lifeTime <= 0.01f)
            {
                Deactivate();
            }

            _rigidbody2D.velocity = _direction * _speed;
        }

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<UnitBase>() != null)
            {
                if (_canHit)
                {
                    _canHit = false;
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
                gameObject.layer = LayerMask.NameToLayer("AllyBullet");
                if (ColorUtility.TryParseHtmlString("#B1FF55", out var color))
                {
                    _spriteRenderer.color = color;
                }
            }
            else
            {
                if (ColorUtility.TryParseHtmlString("#FF0000", out var color))
                {
                    gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
                    _spriteRenderer.color = color;
                }
            }
        }

        private void OnEnable()
        {
            if (Ship != null)
            {
                _speed = Ship.BulletSpeed;
                _lifeTime = Ship.BulletLifeTime;
            }

            _isAlive = true;
            _canHit = true;
        }

        private void OnDisable()
        {
            _isAlive = false;
        }
    }
}

