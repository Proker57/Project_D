using System.Collections.Generic;
using System.Linq;
using BOYAREngine.Controller;
using Cinemachine;
using UnityEngine;

namespace BOYAREngine.Utils
{
    public class CameraTargetGroup : MonoBehaviour
    {
        [SerializeField] private CinemachineTargetGroup _targetGroup;
        private List<GameObject> _targets;

        public void FindAllTargets()
        {
            foreach (var ally in GameController.Instance.AllyShips)
            {
                _targetGroup.AddMember(ally.transform, 2f, 2f);
            }

            foreach (var enemy in GameController.Instance.EnemyShips)
            {
                _targetGroup.AddMember(enemy.transform, 1f, 2f);
            }
        }
    }
}

