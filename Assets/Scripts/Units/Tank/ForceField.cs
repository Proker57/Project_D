using UnityEngine;

namespace BOYAREngine.Units
{
    public class ForceField : MonoBehaviour
    {
        [SerializeField] private UnitBase _unitBase;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _alpha;

        private void Start()
        {
            _spriteRenderer.color = new Color(_unitBase.SpriteRenderer.color.r, _unitBase.SpriteRenderer.color.g, _unitBase.SpriteRenderer.color.b, _alpha);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<BulletBase>() != null)
            {
                var bullet = other.gameObject.GetComponent<BulletBase>();

                if (bullet.IsAlly != _unitBase.IsAlly)
                {
                    bullet.gameObject.SetActive(false);
                }
            }
        }
    }
}

