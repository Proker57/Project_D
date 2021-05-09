using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine.Units
{
    public class Enemy : MonoBehaviour
    {
        private GameController _gc;

        private void Awake()
        {
            _gc = GameController.Instance;
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
            _gc.CameraTargetGroup.Targets.Remove(gameObject);
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

