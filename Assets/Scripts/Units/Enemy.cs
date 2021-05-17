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
            _gc.Setup.EnemyShips.Add(gameObject);
            UiUpdateCounter();
            _gc.Setup.CameraTargetGroup.Targets.Add(gameObject);
        }

        private void RemoveShip()
        {
            _gc.Setup.EnemyShips.Remove(gameObject);
            UiUpdateCounter();
            _gc.PointsEarned += _unitBase.HealthMax;
            _gc.Setup.CameraTargetGroup.Targets.Remove(gameObject);
        }

        private void UiUpdateCounter()
        {
            _gc.Setup.EnemiesCountText.text = _gc.Setup.EnemyShips.Count.ToString();
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

