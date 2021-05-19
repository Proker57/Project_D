using UnityEngine;

namespace BOYAREngine.MainMenu
{
    public class Achievements : MonoBehaviour
    {
        public string Destroyer10 = "CgkIoLGG7ZMFEAIQAg";
        public string Destroyer9 = "CgkIoLGG7ZMFEAIQAw";

        public void ShowAchievements()
        {
            Social.ShowAchievementsUI();
        }
    }
}

