using System.Collections.Generic;
using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine.Units
{
    public class Medic : UnitBase
    {
        [Header("Medic Settings")]
        [SerializeField] private int _healPower;
        [SerializeField] private LineRenderer _line;

        private Collider2D _target;
        private GameController _gc;

        private string _typeName;

        protected override void Start()
        {
            base.Start();

            _gc = GameController.Instance;
            _typeName = GetType().Name;

            Type = _typeName;

            _line.startColor = SpriteRenderer.color;
            _line.endColor = SpriteRenderer.color;

            if (_typeName.Equals("Medic"))
            {
                if (IsAlly)
                {
                    _gc.AllyMedicsCurrent++;
                }
                else
                {
                    _gc.EnemyMedicsCurrent++;
                }

                _gc.UiUpdateShipBoards();
            }
        }

        protected override void UseSpecialAbility()
        {
            base.UseSpecialAbility();

            var result = new List<Collider2D>();
            var cf = new ContactFilter2D();
            cf.useTriggers = true;
            if (IsAlly)
            {
                cf.ClearLayerMask();
                cf.SetLayerMask(LayerMask.GetMask("Ally"));
            }
            else
            {
                cf.ClearLayerMask();
                cf.SetLayerMask(LayerMask.GetMask("Enemy"));
            }
            
            Physics2D.OverlapCircle(transform.position, SearchTargetRadius, cf, result);
            _target = result.Count > 0 ? result[0] : null;

            if (_target != null)
            {
                for (var i = 1; i < result.Count; i++)
                {
                    if (result[i].GetComponent<UnitBase>().HealthCurrent < _target.GetComponent<UnitBase>().HealthCurrent) _target = result[i];
                }

                _line.enabled = true;
                Invoke(nameof(DisableLine), .15f);
                _line.SetPosition(0, transform.position);
                _line.SetPosition(1, _target.transform.position);
                _target.GetComponent<UnitBase>().ReceiveHeal(_healPower);
            }
        }

        private void DisableLine()
        {
            _line.enabled = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_typeName.Equals("Medic"))
            {
                if (IsAlly)
                {
                    _gc.AllyMedicsCurrent--;
                }
                else
                {
                    _gc.EnemyMedicsCurrent--;
                }

                _gc.UiUpdateShipBoards();
            }
        }
    }
}

