using UnityEngine;

namespace BOYAREngine.MainMenu
{
    public class Achievements : MonoBehaviour
    {
        public string Beginner = "CgkIoLGG7ZMFEAIQAQ";
        public string Destroyer10 = "CgkIoLGG7ZMFEAIQAg";
        public string Destroyer9 = "CgkIoLGG7ZMFEAIQAw";
        public string Destroyer8 = "CgkIoLGG7ZMFEAIQBA";

        public void ShowAchievements()
        {
            Social.ShowAchievementsUI();
        }
    }
}

