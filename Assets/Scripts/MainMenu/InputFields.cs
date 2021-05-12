using BOYAREngine.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace BOYAREngine.MainMenu
{
    public class InputFields : MonoBehaviour
    {
        public void SetAllyAttackersCount(InputField input)
        {
            GameController.Instance.AllyAttackers = int.Parse(input.text);
        }

        public void SetAllyTanksCount(InputField input)
        {
            GameController.Instance.AllyTanks = int.Parse(input.text);
        }

        public void SetEnemyAttackersCount(InputField input)
        {
            GameController.Instance.EnemyAttackers = int.Parse(input.text);
        }

        public void SetEnemyTanksCount(InputField input)
        {
            GameController.Instance.EnemyTanks = int.Parse(input.text);
        }
    }
}

