using System.Collections;
using BOYAREngine.Controller;
using BOYAREngine.Ui;
using BOYAREngine.Utils;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BOYAREngine.Units
{
    public class UnitBase : MonoBehaviour
    {
        [Header("Base")]
        public int HealthMax;
        internal int HealthCurrent;
        public string Type;

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
        private bool _isSpecialCountdownActive;
        private bool _canUseSpecialAbility;

        [Header("UI")]
        [SerializeField] private SpriteRenderer _hpBarSprite;
        [SerializeField] private SpriteRenderer _hpFrameSprite;
        [Space]
        [SerializeField] private SpriteRenderer _specialBarSprite;
        [SerializeField] private SpriteRenderer _specialFrameSprite;
        [Space]
        [SerializeField] private SpriteRenderer _specialCountdownBarSprite;
        [SerializeField] private SpriteRenderer _specialCountdownFrameSprite;
        [Space]
        public GameObject NameTagGo;
        [SerializeField] private Image _nameTagPanel;
        [SerializeField] private Text _nameTagText;

        [Header("FX")]
        public SpriteRenderer SpriteRenderer;
        [SerializeField] private ParticleSystem _deathFx;
        [SerializeField] private BoxCollider2D _boxCollider2D;

        [Space]
        internal bool IsAlly;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void Start()
        {
            SetBattleSide();

            HealthCurrent = HealthMax;
            _specialPassiveIncomeCurrent = _specialPassiveIncome;

            CanMove = true;
            CanShoot = true;
            _canShowSpecialCountdown = true;
            _canUseSpecialAbility = true;
            _isSpecialCountdownActive = true;
            SpriteRenderer.enabled = true;
            _boxCollider2D.enabled = true;
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
                if (other.gameObject.GetComponent<Enemy>() != null)
                {
                    ShipCollided(other);
                }
            }
            else
            {
                if (other.gameObject.GetComponent<Ally>() != null)
                {
                    ShipCollided(other);
                }
            }
        }

        public void ReceiveDamage(int damage)
        {
            HealthCurrent -= damage;

            if (HealthCurrent <= 0)
            {
                HealthCurrent = 0;
                Statistic.EnemyDestroyed?.Invoke();

                var fxMain = _deathFx.main;
                fxMain.startColor = SpriteRenderer.color;
                _deathFx.Play();

                SpriteRenderer.enabled = false;
                _boxCollider2D.enabled = false;
                CanMove = false;
                CanShoot = false;
                _canUseSpecialAbility = false;
                _canShowSpecialCountdown = false;

                _hpBarSprite.enabled = false;
                _hpFrameSprite.enabled = false;
                _specialBarSprite.enabled = false;
                _specialFrameSprite.enabled = false;
                _specialCountdownBarSprite.enabled = false;
                _specialCountdownFrameSprite.enabled = false;
                

                NameTagGo.SetActive(false);

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
            ReceiveDamage(HealthCurrent / 2);
            unitBase.ReceiveDamage(unitBase.HealthCurrent / 2);
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
                //MiniTask.Run(0f, () => { _currentDestination = SearchForRandomDestination(); });
                _currentDestination = SearchForRandomDestination();

                _isMovingToDestination = true;
            }

            if (Vector2.Distance(transform.position, _currentDestination) <= 0.01f)
            {
                _isMovingToDestination = false;
            }
        }

        private void FixedUpdate()
        {
            if (CanMove)
            {
                _rb.position = Vector2.MoveTowards(transform.position, _currentDestination, MoveSpeed * Time.deltaTime);
            }
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

            if (SpecialCurrent >= _specialMax && _canUseSpecialAbility)
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
                _specialCountdownBarSprite.enabled = true;
                _specialCountdownFrameSprite.enabled = true;
            }

            StartCoroutine(SpecialCountdown());
            SpecialCurrent = 0f;
        }

        private IEnumerator SpecialCountdown()
        {
            while (_specialDurationCurrent >= 0.001f)
            {
                _specialDurationCurrent -= Time.deltaTime;
                _specialCountdownBarSprite.transform.localScale = new Vector3(Mathf.Clamp(_specialDurationCurrent, 0f, 2.32f) * _specialDurationCurrent / SpecialDuration, 1.52f, 1f);
                yield return new WaitForEndOfFrame();
            }

            IsSpecialActive = false;
            _specialCountdownBarSprite.enabled = false;
            _specialCountdownFrameSprite.enabled = false;
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
            _hpBarSprite.transform.localScale = new Vector3(Mathf.Clamp(HealthCurrent, 0f, 2.32f) * HealthCurrent / HealthMax, 1.52f, 1f);    // 2.32f
        }

        public void UpdateSpecialUi()
        {
            _specialBarSprite.transform.localScale = new Vector3(Mathf.Clamp(SpecialCurrent, 0f, 2.32f) * SpecialCurrent / _specialMax, 1.52f, 1f);    // 2.32f
        }

        public Vector2 SearchForRandomDestination()
        {
            return new Vector2(Random.Range(-20f, 20f), Random.Range(-10, 10f));
        }

        public void SearchForTarget()
        {
            if (IsAlly)
            {
                var enemyShips = GameController.Instance.Setup.EnemyShips;
                ShootTargetTransform = enemyShips[Random.Range(0, enemyShips.Count)].transform;
            }
            else
            {
                var allyShips = GameController.Instance.Setup.AllyShips;
                ShootTargetTransform = allyShips[Random.Range(0, allyShips.Count)].transform;
            }
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
                // NameTag
                if (ColorUtility.TryParseHtmlString("#00FF24", out color))
                {
                    _nameTagPanel.color = color;
                    _nameTagText.color = color;
                }
                // Hp color
                if (ColorUtility.TryParseHtmlString("#9AEE49", out color))
                {
                    _hpBarSprite.color = color;
                    _hpFrameSprite.color = color;
                }
                // "Special" color
                if (ColorUtility.TryParseHtmlString("#97B27D", out color))
                {
                    _specialBarSprite.color = color;
                    _specialFrameSprite.color = color;
                }
                // Special countdown color
                if (ColorUtility.TryParseHtmlString("#A4B297", out color))
                {
                    _specialCountdownBarSprite.color = color;
                    _specialCountdownFrameSprite.color = color;
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
                // Name Tag
                if (ColorUtility.TryParseHtmlString("#FF0000", out color))
                {
                    _nameTagPanel.color = color;
                    _nameTagText.color = color;
                }
                // Hp color
                if (ColorUtility.TryParseHtmlString("#FF0000", out color))
                {
                    _hpBarSprite.color = color;
                    _hpFrameSprite.color = color;
                }
                // "Special" color
                if (ColorUtility.TryParseHtmlString("#B17C7C", out color))
                {
                    _specialBarSprite.color = color;
                    _specialFrameSprite.color = color;
                }
                // Special countdown color
                if (ColorUtility.TryParseHtmlString("#B19F9F", out color))
                {
                    _specialCountdownBarSprite.color = color;
                    _specialCountdownFrameSprite.color = color;
                }
            }
        }

        private void ToggleShipUi(int type)
        {
            switch (type)
            {
                case 0:
                    _hpBarSprite.enabled = !_hpBarSprite.enabled;
                    _hpFrameSprite.enabled = !_hpFrameSprite.enabled;
                    break;
                case 1:
                    _specialBarSprite.enabled = !_specialBarSprite.enabled;
                    _specialFrameSprite.enabled = !_specialFrameSprite.enabled;
                    break;
                case 2:
                    _canShowSpecialCountdown = !_canShowSpecialCountdown;
                    _isSpecialCountdownActive = !_isSpecialCountdownActive;

                    _specialCountdownBarSprite.enabled = _isSpecialCountdownActive;
                    _specialCountdownFrameSprite.enabled = _isSpecialCountdownActive;

                    if (_specialDurationCurrent > 0.001f && _canShowSpecialCountdown)
                    {
                        _specialCountdownBarSprite.enabled = true;
                        _specialCountdownFrameSprite.enabled = true;
                    }
                    break;
                case 3:
                    NameTagGo.SetActive(!NameTagGo.activeSelf);
                    break;
            }
        }

        private void OnEnable()
        {
            UiSettings.ToggleShipUi += ToggleShipUi;
        }

        protected virtual void OnDisable()
        {
            UiSettings.ToggleShipUi -= ToggleShipUi;
            StopAllCoroutines();
        }
    }
}

