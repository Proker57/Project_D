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

            PlayGamesPlatform.Instance.IncrementAchievement(_achievements.Destroyer10, 1, (bool success) => { });

            if (EnemiesDestroyed >= 20)
            {
                Social.ReportProgress(_achievements.Destroyer9, 0.0f, (bool success) => { });
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

