using BOYAREngine.Controller;
using UnityEngine;

namespace BOYAREngine.MainMenu
{
    public class Buttons : MonoBehaviour
    {
        public void StartGame()
        {
            GameController.Instance.MainMenuObject.SetActive(false);
            GameController.Instance.GameObject.SetActive(true);
        }
    }
}

