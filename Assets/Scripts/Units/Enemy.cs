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
            _gc.Setup.EnemiesCountText.text = "Enemies: " + _gc.Setup.EnemyShips.Count;
            _gc.Setup.CameraTargetGroup.Targets.Add(gameObject);
        }

        private void RemoveShip()
        {
            _gc.Setup.EnemyShips.Remove(gameObject);
            _gc.Setup.EnemiesCountText.text = "Enemies: " + _gc.Setup.EnemyShips.Count;
            _gc.Points += _unitBase.HealthMax;
            _gc.Setup.CameraTargetGroup.Targets.Remove(gameObject);

//            if (_gc.Setup.EnemyShips.Count == 0)
//            {
//                Debug.Log("Enemy");
//                _gc.EndBattle();
//            }
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

