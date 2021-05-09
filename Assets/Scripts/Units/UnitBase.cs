using System.Collections;
using BOYAREngine.Controller;
using BOYAREngine.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace BOYAREngine.Units
{
    public class UnitBase : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] protected int MaxHealth;
        protected int CurrentHealth;

        [Header("Movement")]
        [SerializeField] protected float Speed;
        protected bool CanMove = true;
        protected bool IsMovingToDestination;
        protected Vector2 CurrentDestination;

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
        [SerializeField] private float _specialPassiveIncome;
        private float _specialPassiveIncomeCurrent;

        [Header("UI")]
        [SerializeField] private Image _hpBar;
        [SerializeField] private Image _specialBar;

        [Space]
        [SerializeField] internal bool IsAlly;

        private void Start()
        {
            CurrentHealth = MaxHealth;
            _specialPassiveIncomeCurrent = _specialPassiveIncome;

            GameController.Instance.AddShip?.Invoke(this);
        }

        protected virtual void Update()
        {
            Movement();
            Shoot();
            SpecialAbility();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<BulletBase>() != null)
            {
                var bullet = other.gameObject.GetComponent<BulletBase>();

                if (bullet.IsAlly != IsAlly)
                {
                    CurrentHealth -= bullet.ReceiveDamage();
                    UpdateHpUi();
                    bullet.gameObject.SetActive(false);

                    if (CurrentHealth <= 0)
                    {
                        Death();
                    }
                }
            }

            if (IsAlly)
            {
                if (other.gameObject.GetComponent<Enemy>() == null) return;
                Death();
            }
            else
            {
                if (other.gameObject.GetComponent<Ally>() == null) return;
                Death();
            }
        }

        protected virtual void Death()
        {
            GameController.Instance.RemoveShip?.Invoke(this);
            gameObject.SetActive(false);
        }

        protected virtual void Movement()
        {
            if (!CanMove) return;
            if (!IsMovingToDestination)
            {
                CurrentDestination = SearchForRandomDestination();
                IsMovingToDestination = true;
            }

            if (Vector2.Distance(transform.position, CurrentDestination) <= 0.001f)
            {
                IsMovingToDestination = false;
            }

            transform.position = Vector2.MoveTowards(transform.position, CurrentDestination, Speed * Time.deltaTime);
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

                SpecialCurrent = 0f;
            }
        }

        protected virtual void UseSpecialAbility()
        {

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
            _hpBar.fillAmount = Mathf.InverseLerp(0, MaxHealth, CurrentHealth);
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

