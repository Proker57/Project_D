using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine.MainMenu
{
    public class Buttons : MonoBehaviour
    {
        public void StartGame()
        {
            GameController.Instance.StartBattle();
        }
    }
}

