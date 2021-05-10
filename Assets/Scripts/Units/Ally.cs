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
            _gc.Setup.AlliesCountText.text = "Allies: " + _gc.Setup.AllyShips.Count;
            _gc.Setup.CameraTargetGroup.Targets.Add(gameObject);
        }

        private void RemoveShip()
        {
            _gc.Setup.AllyShips.Remove(gameObject);
            _gc.Setup.AlliesCountText.text = "Allies: " + _gc.Setup.AllyShips.Count;
            _gc.Setup.CameraTargetGroup.Targets.Remove(gameObject);

//            if (_gc.Setup.AllyShips.Count == 0)
//            {
//                Debug.Log("Ally");
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

