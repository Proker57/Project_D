using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine.Units
{
    public class Ally : MonoBehaviour
    {
        private GameController _gc;

        private void Awake()
        {
            _gc = GameController.Instance;
        }

        private void AddShip()
        {
            _gc.AllyShips.Add(gameObject);
            _gc.AlliesCountText.text = "Allies: " + _gc.AllyShips.Count;
            _gc.CameraTargetGroup.Targets.Add(gameObject);
        }

        private void RemoveShip()
        {
            _gc.AllyShips.Remove(gameObject);
            _gc.AlliesCountText.text = "Allies: " + _gc.AllyShips.Count;
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

