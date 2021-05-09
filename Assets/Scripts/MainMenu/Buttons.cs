using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine.MainMenu
{
    public class Buttons : MonoBehaviour
    {
        public void StartGame()
        {
            GameController.Instance.MainMenu.SetActive(false);
            GameController.Instance.Game.SetActive(true);
        }
    }
}

