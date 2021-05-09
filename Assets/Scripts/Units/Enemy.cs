using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine.Units
{
    public class Enemy : MonoBehaviour
    {
        private GameController _gc;
        private UnitBase _unitBase;

        private void Awake()
        {
            _gc = GameController.Instance;
            _unitBase = GetComponent<UnitBase>();
        }

        private void AddShip()
        {
            _gc.EnemyShips.Add(gameObject);
            _gc.EnemiesCountText.text = "Enemies: " + _gc.EnemyShips.Count;
            _gc.CameraTargetGroup.Targets.Add(gameObject);
        }

        private void RemoveShip()
        {
            _gc.EnemyShips.Remove(gameObject);
            _gc.EnemiesCountText.text = "Enemies: " + _gc.EnemyShips.Count;
            _gc.Points += _unitBase.HealthMax;
            _gc.CameraTargetGroup.Targets.Remove(gameObject);

            if (_gc.EnemyShips.Count == 0)
            {
                _gc.EndBattle();
            }
        }

        private void OnEnable()
        {
            AddShip();
        }

        private void OnDisable()
        {
            RemoveShip();
        }
    }
}

