using BOYAREngine.MainMenu;
using GooglePlayGames;
using UnityEngine;

namespace BOYAREngine
{
    public class Statistic : MonoBehaviour
    {
        public delegate void EnemyDestroyedEvent();
        public static EnemyDestroyedEvent EnemyDestroyed;

        public int EnemiesDestroyed;

        [SerializeField] private Achievements _achievements;

        private void OnEnemyDestroyed()
        {
            EnemiesDestroyed++;
            // 20
            if (EnemiesDestroyed <= 20)
            {
                PlayGamesPlatform.Instance.IncrementAchievement(_achievements.Destroyer10, 1, (bool success) => { });
            }
            // 100
            if (EnemiesDestroyed == 20) PlayGamesPlatform.Instance.RevealAchievement(_achievements.Destroyer9, (bool success) => { });
            if (EnemiesDestroyed > 20 && EnemiesDestroyed <= 120)
            {
                PlayGamesPlatform.Instance.IncrementAchievement(_achievements.Destroyer9, 1, (bool success) => { });
            }
            // 500
            if (EnemiesDestroyed == 120) PlayGamesPlatform.Instance.RevealAchievement(_achievements.Destroyer8, (bool success) => { });
            if (EnemiesDestroyed > 120 && EnemiesDestroyed <= 620)
            {
                PlayGamesPlatform.Instance.IncrementAchievement(_achievements.Destroyer8, 1, (bool success) => { });
            }
        }

        private void OnEnable()
        {
            EnemyDestroyed += OnEnemyDestroyed;
        }

        private void OnDisable()
        {
            EnemyDestroyed -= OnEnemyDestroyed;
        }
    }
}

