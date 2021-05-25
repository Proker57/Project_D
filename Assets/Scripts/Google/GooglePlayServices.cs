using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;

namespace BOYAREngine.MainMenu
{
    public class GooglePlayServices : MonoBehaviour
    {
        [SerializeField] private Text _statusText;
        [Space]
        [SerializeField] private GameObject _signInText;
        [SerializeField] private GameObject _signInButton;
        [SerializeField] private GameObject _signOutButton;
        [SerializeField] private GameObject _achievementsButton;
        [SerializeField] private GameObject _leaderboardsButton;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            var config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }

        public void SignIn()
        {
            PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) => {
                switch (result)
                {
                    case SignInStatus.Success:
                        _statusText.text = result.ToString();
                        IsLoggedIn(true);
                        break;
                    default:
                        _statusText.text = result.ToString();
                        IsLoggedIn(false);
                        break;
                }
            });

            // TODO Debug purpose (DELETE)
#if UNITY_EDITOR
            IsLoggedIn(true);
#endif
        }

        public void SignOut()
        {
            PlayGamesPlatform.Instance.SignOut();
            IsLoggedIn(false);
            _statusText.text = PlayGamesPlatform.Instance.IsAuthenticated().ToString();
        }

        private void IsLoggedIn(bool status)
        {
            _signInText.SetActive(!status);
            _signInButton.SetActive(!status);
            _signOutButton.SetActive(status);
            _achievementsButton.SetActive(status);
            _leaderboardsButton.SetActive(status);
        }

        private void OnApplicationQuit()
        {
            PlayGamesPlatform.Instance.SignOut();
        }
    }
}

