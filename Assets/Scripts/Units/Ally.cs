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
            _gc.Setup.AllyShips.Add(gameObject);
            UiUpdateCounter();
            _gc.Setup.CameraTargetGroup.Targets.Add(gameObject);
        }

        private void RemoveShip()
        {
            _gc.Setup.AllyShips.Remove(gameObject);
            UiUpdateCounter();
            _gc.Setup.CameraTargetGroup.Targets.Remove(gameObject);
        }

        private void UiUpdateCounter()
        {
            _gc.Setup.AlliesCountText.text = _gc.Setup.AllyShips.Count.ToString();
        }

        private void OnEnable()
        {
            AddShip();
        }

        private void OnDisable()
        {
            RemoveShip();

            if (_gc.Setup.AllyShips.Count == 0)
            {
                _gc.EndBattle(true);
            }
        }
    }
}

