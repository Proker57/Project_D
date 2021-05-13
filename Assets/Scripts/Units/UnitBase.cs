using System.Collections;
using System.Collections.Generic;
using BOYAREngine.Ui;
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
        internal int HealthCurrent;

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
        [SerializeField] protected ObjectPool BulletsPool;
        protected LayerMask TargetMask;
        internal Transform ShootTargetTransform;

        [Header("Special")]
        [HideInInspector] public float SpecialCurrent;
        [SerializeField] private float _specialMax;
        [SerializeField] protected float SpecialDuration;
        private float _specialDurationCurrent;
        [SerializeField] private float _specialPassiveIncome;
        private float _specialPassiveIncomeCurrent;
        protected bool IsSpecialActive;
        private bool _canShowSpecialCountdown;

        [Header("UI")]
        [SerializeField] private GameObject _hpGo;
        [SerializeField] private Image _hpBar;
        [SerializeField] private Image _hpFrame;
        [SerializeField] private GameObject _specialGo;
        [SerializeField] private Image _specialBar;
        [SerializeField] private Image _specialFrame;
        [SerializeField] private GameObject _specialCountdownGo;
        [SerializeField] private Image _specialCountdownBar;
        [SerializeField] private Image _specialCountdownFrame;

        [Header("FX")]
        public SpriteRenderer SpriteRenderer;
        [SerializeField] private ParticleSystem _deathFx;

        [Space]
        internal bool IsAlly;

        protected virtual void Start()
        {
            SetBattleSide();

            HealthCurrent = HealthMax;
            _specialPassiveIncomeCurrent = _specialPassiveIncome;

            CanMove = true;
            CanShoot = true;
            _canShowSpecialCountdown = true;
            SpriteRenderer.enabled = true;
            //_hpGo.SetActive(true);
            //_specialGo.SetActive(true);
            //_specialCountdownGo.SetActive(false);
        }

        protected virtual void Update()
        {
            Movement();
            Shoot();
            SpecialAbility();
        }

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
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
                fxMain.startColor = SpriteRenderer.color;
                _deathFx.Play();

                SpriteRenderer.enabled = false;
                CanMove = false;
                CanShoot = false;

                _hpGo.SetActive(false);
                _specialGo.SetActive(false);
                _specialCountdownGo.SetActive(false);

                Invoke(nameof(Death), 1f);
            }

            UpdateHpUi();
        }

        public void ReceiveHeal(int healAmount)
        {
            HealthCurrent += healAmount;

            if (HealthCurrent >= HealthMax)
            {
                HealthCurrent = HealthMax;
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

            if (Vector2.Distance(transform.position, _currentDestination) <= 0.01f)
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
            if (_canShowSpecialCountdown)
            {
                _specialCountdownGo.SetActive(true);
            }

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
            var result = new List<Collider2D>();
            var cf = new ContactFilter2D();
            cf.useTriggers = true;
            cf.SetLayerMask(TargetMask);

            Physics2D.OverlapCircle(transform.position, SearchTargetRadius, cf, result);
            ShootTargetTransform = result.Count > 0 ? result[Random.Range(0, result.Count)].transform : null;
        }

        private void SetBattleSide()
        {
            if (IsAlly)
            {
                gameObject.layer = LayerMask.NameToLayer("Ally");
                TargetMask = LayerMask.GetMask("Enemy");
                gameObject.AddComponent<Ally>();

                // Body color
                if (ColorUtility.TryParseHtmlString("#00FF24", out var color))
                {
                    SpriteRenderer.color = color;
                }
                // Hp color
                if (ColorUtility.TryParseHtmlString("#9AEE49", out var hpColor))
                {
                    _hpBar.color = hpColor;
                    _hpFrame.color = hpColor;
                }
                // "Special" color
                if (ColorUtility.TryParseHtmlString("#97B27D", out color))
                {
                    _specialBar.color = color;
                    _specialFrame.color = color;
                }
                // Special countdown color
                if (ColorUtility.TryParseHtmlString("#A4B297", out color))
                {
                    _specialCountdownBar.color = color;
                    _specialCountdownFrame.color = color;
                }
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Enemy");
                TargetMask = LayerMask.GetMask("Ally");
                gameObject.AddComponent<Enemy>();

                // Body color
                if (ColorUtility.TryParseHtmlString("#FF0000", out var color))
                {
                    SpriteRenderer.color = color;
                }
                // Hp color
                if (ColorUtility.TryParseHtmlString("#FF0000", out color))
                {
                    _hpBar.color = color;
                    _hpFrame.color = color;
                }
                // "Special" color
                if (ColorUtility.TryParseHtmlString("#B17C7C", out color))
                {
                    _specialBar.color = color;
                    _specialFrame.color = color;
                }
                // Special countdown color
                if (ColorUtility.TryParseHtmlString("#B19F9F", out color))
                {
                    _specialCountdownBar.color = color;
                    _specialCountdownFrame.color = color;
                }
            }
        }

        private void ToggleShipUi(int type)
        {
            switch (type)
            {
                case 0:
                    _hpGo.SetActive(!_hpGo.activeSelf);
                    break;
                case 1:
                    _specialGo.SetActive(!_specialGo.activeSelf);
                    break;
                case 2:
                    _canShowSpecialCountdown = !_canShowSpecialCountdown;
                    if (_specialCountdownGo.activeSelf)
                    {
                        _specialCountdownGo.SetActive(!_specialCountdownGo.activeSelf);
                    }

                    if (_specialDurationCurrent > 0.001f && _canShowSpecialCountdown)
                    {
                        _specialCountdownGo.SetActive(true);
                    }
                    break;
            }
        }

        private void OnEnable()
        {
            UiSettings.ToggleShipUi += ToggleShipUi;
        }

        private void OnDisable()
        {
            UiSettings.ToggleShipUi -= ToggleShipUi;
        }
    }
}

