using System.Collections;
using BOYAREngine.Utils;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BOYAREngine.Units
{
    public class UnitBase : MonoBehaviour
    {
        [Header("Health")]
        public int HealthMax;
        protected int HealthCurrent;

        [Header("Movement")]
        [SerializeField] protected float MoveSpeed;
        protected bool CanMove = true;
        private bool _isMovingToDestination;
        private Vector2 _currentDestination;

        [Header("Weapon Settings")]
        [SerializeField] internal int Damage;
        [SerializeField] internal int BulletLifeTime;
        [SerializeField] internal float BulletSpeed;
        [SerializeField] protected float ShootInterval;
        [SerializeField] protected float ReloadTime;
        [SerializeField] protected float ShootBufferDistance;
        [SerializeField] protected float SearchTargetRadius;
        protected bool CanShoot = true;
        protected bool IsShooting = true;
        [SerializeField] protected LayerMask TargetMask;
        [SerializeField] protected ObjectPool BulletsPool;
        internal Transform ShootTargetTransform;

        [Header("Special")]
        [HideInInspector] public float SpecialCurrent;
        [SerializeField] private float _specialMax;
        [SerializeField] protected float SpecialDuration;
        private float _specialDurationCurrent;
        [SerializeField] private float _specialPassiveIncome;
        private float _specialPassiveIncomeCurrent;
        protected bool IsSpecialActive;

        [Header("UI")]
        [SerializeField] private GameObject _hpGo;
        [SerializeField] private Image _hpBar;
        [SerializeField] private GameObject _specialGo;
        [SerializeField] private Image _specialBar;
        [SerializeField] private GameObject _specialCountdownGo;
        [SerializeField] private Image _specialCountdownBar;

        [Header("FX")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private ParticleSystem _deathFx;

        [Space]
        [SerializeField] internal bool IsAlly;

        private void Start()
        {
            HealthCurrent = HealthMax;
            _specialPassiveIncomeCurrent = _specialPassiveIncome;

            CanMove = true;
            CanShoot = true;
            _spriteRenderer.enabled = true;
            _hpGo.SetActive(true);
            _specialGo.SetActive(true);
            _specialCountdownGo.SetActive(false);
        }

        protected virtual void Update()
        {
            Movement();
            Shoot();
            SpecialAbility();
        }

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<BulletBase>() != null)
            {
                var bullet = other.gameObject.GetComponent<BulletBase>();

                if (bullet.IsAlly != IsAlly)
                {
                    ReceiveDamage(bullet.ReceiveDamage());
                    bullet.gameObject.SetActive(false);
                }
            }

            if (IsAlly)
            {
                if (other.gameObject.GetComponent<Enemy>() == null) return;
                ShipCollided(other);
            }
            else
            {
                if (other.gameObject.GetComponent<Ally>() == null) return;
                ShipCollided(other);
            }
        }

        public void ReceiveDamage(int damage)
        {
            HealthCurrent -= damage;

            if (HealthCurrent <= 0)
            {
                HealthCurrent = 0;

                var fxMain = _deathFx.main;
                fxMain.startColor = _spriteRenderer.color;
                _deathFx.Play();

                _spriteRenderer.enabled = false;
                CanMove = false;
                CanShoot = false;

                _hpGo.SetActive(false);
                _specialGo.SetActive(false);
                _specialCountdownGo.SetActive(false);

                Invoke(nameof(Death), 1f);
            }

            UpdateHpUi();
        }

        private void ShipCollided(Collider2D other)
        {
            var unitBase = other.GetComponent<UnitBase>();
            ReceiveDamage(unitBase.HealthCurrent);
            unitBase.ReceiveDamage(HealthCurrent);
            
        }

        protected virtual void Death()
        {
            gameObject.transform.parent.gameObject.SetActive(false);
        }

        protected virtual void Movement()
        {
            if (!CanMove) return;
            if (!_isMovingToDestination)
            {
                MiniTask.Run(0f, () => { _currentDestination = SearchForRandomDestination(); });
                //_currentDestination = SearchForRandomDestination();
                _isMovingToDestination = true;
            }

            if (Vector2.Distance(transform.position, _currentDestination) <= 0.001f)
            {
                _isMovingToDestination = false;
            }

            transform.position = Vector2.MoveTowards(transform.position, _currentDestination, MoveSpeed * Time.deltaTime);
        }

        protected void Shoot()
        {
            if (!CanShoot) return;
            if (IsShooting)
            {
                StartCoroutine(RapidFire());
                IsShooting = false;
            }
        }

        private void SpecialAbility()
        {
            _specialPassiveIncomeCurrent -= Time.deltaTime;
            if (_specialPassiveIncomeCurrent <= 0f)
            {
                SpecialCurrent++;
                _specialPassiveIncomeCurrent = _specialPassiveIncome;
                UpdateSpecialUi();
            }

            if (SpecialCurrent >= _specialMax)
            {
                UseSpecialAbility();
            }
        }

        protected virtual void UseSpecialAbility()
        {
            IsSpecialActive = true;
            _specialDurationCurrent = SpecialDuration;
            _specialCountdownGo.SetActive(true);

            StartCoroutine(SpecialCountdown());
            SpecialCurrent = 0f;
        }

        private IEnumerator SpecialCountdown()
        {
            while (_specialDurationCurrent >= 0.001f)
            {
                _specialDurationCurrent -= Time.deltaTime;
                _specialCountdownBar.fillAmount = Mathf.InverseLerp(0f, SpecialDuration, _specialDurationCurrent);
                yield return null;
            }

            IsSpecialActive = false;
            _specialCountdownGo.SetActive(false);
        }

        private IEnumerator RapidFire()
        {
            for (var i = 0; i < BulletsPool.Amount; i++)
            {
                yield return new WaitForSeconds(ShootInterval);
                SearchForTarget();
                ShootBullet();
            }

            Invoke(nameof(ReloadWeapons), ReloadTime);
        }

        private void ReloadWeapons()
        {
            IsShooting = true;
        }

        private void ShootBullet()
        {
            var bullet = BulletsPool.GetPooledObject();
            if (bullet != null && ShootTargetTransform != null)
            {
                var direction = ShootTargetTransform.position - transform.position;
                direction.Normalize();
                direction *= ShootBufferDistance;
                bullet.transform.position = transform.position + direction;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
            }
        }

        private void UpdateHpUi()
        {
            _hpBar.fillAmount = Mathf.InverseLerp(0, HealthMax, HealthCurrent);
        }

        public void UpdateSpecialUi()
        {
            _specialBar.fillAmount = Mathf.InverseLerp(0, _specialMax, SpecialCurrent);
        }

        public Vector2 SearchForRandomDestination()
        {
            return new Vector2(Random.Range(-20f, 20f), Random.Range(-10, 10f));
        }

        public void SearchForTarget()
        {
            var hit = Physics2D.OverlapCircleAll(transform.position, SearchTargetRadius, TargetMask);
            ShootTargetTransform = hit.Length > 0 ? hit[Random.Range(0, hit.Length)].transform : null;
        }
    }
}

