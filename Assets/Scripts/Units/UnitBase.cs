using System.Collections;
using BOYAREngine.Controller;
using BOYAREngine.Utils;
using UnityEngine;

namespace BOYAREngine.Units
{
    public class UnitBase : MonoBehaviour
    {
        [SerializeField] protected int Health;
        [SerializeField] internal int Damage;
        [SerializeField] internal int BulletLifeTime;
        [SerializeField] internal float BulletSpeed;
        [SerializeField] protected float Speed;
        [SerializeField] protected float SearchTargetRadius;
        [SerializeField] protected float ShootSpitfireCooldown;
        [SerializeField] protected float ShootBufferDistance;
        [SerializeField] protected float ReloadCooldown;
        [SerializeField] internal bool IsAlly;
        private float _shootCooldownCurrent;
        protected bool CanMove = true;
        protected bool CanShoot = true;
        protected bool IsShooting = true;
        protected bool IsMovingToDestination;

        internal Transform ShootTargetTransform;
        [SerializeField] protected ObjectPool BulletsPool;
        [SerializeField] protected LayerMask TargetMask;
        protected Vector2 CurrentDestination;

        private void Start()
        {
            GameController.Instance.AddShip?.Invoke(this);
        }

        protected virtual void Update()
        {
            Movement();
            Shoot();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Bullet>() != null)
            {
                var bullet = other.gameObject.GetComponent<Bullet>();

                if (bullet.IsAlly != IsAlly)
                {
                    Health -= bullet.ReceiveDamage();
                    bullet.gameObject.SetActive(false);

                    if (Health <= 0)
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
                StartCoroutine(Spitfire());
                IsShooting = false;
            }
        }

        private IEnumerator Spitfire()
        {
            
            for (var i = 0; i < BulletsPool.Amount; i++)
            {
                yield return new WaitForSeconds(ShootSpitfireCooldown);
                SearchForTarget();
                ShootBullet();
            }

            Invoke(nameof(ReloadWeapons), ReloadCooldown);
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

        public Vector2 SearchForRandomDestination()
        {
            return new Vector2(Random.Range(-8.8f, 8.6f), Random.Range(-4.3f, 5.37f));
        }

        public void SearchForTarget()
        {
            var hit = Physics2D.OverlapCircleAll(transform.position, SearchTargetRadius, TargetMask);
            ShootTargetTransform = hit.Length > 0 ? hit[Random.Range(0, hit.Length)].transform : null;
        }
    }
}

