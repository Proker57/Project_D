using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine.Units
{
    public class Enemy : MonoBehaviour
    {
        private UnitBase _unitBase;

        private void Awake()
        {
            _unitBase = GetComponent<UnitBase>();
        }

        private void AddShip()
        {
            GameController.Instance.Setup.EnemyShips.Add(gameObject);
            UiUpdateCounter();
            GameController.Instance.Setup.CameraTargetGroup.Targets.Add(gameObject);
        }

        private void RemoveShip()
        {
            GameController.Instance.Setup.EnemyShips.Remove(gameObject);
            UiUpdateCounter();
            GameController.Instance.PointsEarned += _unitBase.HealthMax;
            GameController.Instance.Setup.CameraTargetGroup.Targets.Remove(gameObject);
        }

        private void UiUpdateCounter()
        {
            GameController.Instance.Setup.EnemiesCountText.text = GameController.Instance.Setup.EnemyShips.Count.ToString();
        }

        private void OnEnable()
        {
            AddShip();
        }

        private void OnDisable()
        {
            RemoveShip();

            if (GameController.Instance.Setup.EnemyShips.Count == 0)
            {
                GameController.Instance.EndBattle(true);
            }
        }
    }
}

