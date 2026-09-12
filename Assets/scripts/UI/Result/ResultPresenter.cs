using jugyou.batoru.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace jugyou.UI
{
    public class ResultPresenter : MonoBehaviour
    {
        private const string TitleSceneName = "titleS";
        private const string GameSceneName = "batoru";

        [SerializeField] ResultView resultView;
        private ResultModel resultModel;

        private void Start()
        {
            if (resultView == null) return;
            resultModel = new ResultModel();
            resultModel.Initialize();

            resultView.OnRetryAction += RetryGame;
            resultView.OnReturnToTitleAction += ReturnToTitle;

            string message = string.Empty;
            if (resultModel.IsClear == true)
            {
                message = $"ゲームクリア\n\n到達レベル：{resultModel.Level}";
            }
            else
            {
                int minutes = Mathf.FloorToInt(resultModel.SuviedTime / 60f);
                int second = Mathf.FloorToInt(resultModel.SuviedTime - minutes * 60f);
                message = $"ゲームオーバー\n\n生存時間 {minutes:00}:{second:00} \n 到達レベル：{resultModel.Level}";
            }
            resultView.SetResultText(message);
        }
        private void OnDestroy()
        {
            if (resultView != null)
            {
                resultView.OnRetryAction -= RetryGame;
                resultView.OnReturnToTitleAction -= ReturnToTitle;
            }
        }
        private void RetryGame()
        {
            if (GameManager.Instance != null)
            {
                Destroy(GameManager.Instance.gameObject);
            }
            SceneManager.LoadScene(GameSceneName);
        }
        private void ReturnToTitle()
        {
            SceneManager.LoadScene(TitleSceneName);
        }
    }
}
