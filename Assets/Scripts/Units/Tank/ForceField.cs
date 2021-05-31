using UnityEngine;

namespace BOYAREngine.Units
{
    public class ForceField : MonoBehaviour
    {
        [SerializeField] private UnitBase _unitBase;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _alpha;
        private int _capacity;

        private void Start()
        {
            _spriteRenderer.color = new Color(_unitBase.SpriteRenderer.color.r, _unitBase.SpriteRenderer.color.g, _unitBase.SpriteRenderer.color.b, _alpha);

            _capacity = _unitBase.IsAlly ? UnitStats.TankShieldCapacity : UnitStats.EnemyTankShieldCapacity;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<BulletBase>() != null)
            {
                var bullet = other.gameObject.GetComponent<BulletBase>();

                if (bullet.IsAlly != _unitBase.IsAlly)
                {
                    _capacity -= bullet.Ship.Damage;
                    bullet.gameObject.SetActive(false);

                    if (_capacity <= 0)
                    {
                        Deactivate(bullet);
                    }
                }
            }
        }

        private void Deactivate(BulletBase bullet)
        {
            gameObject.SetActive(false);
        }
    }
}

