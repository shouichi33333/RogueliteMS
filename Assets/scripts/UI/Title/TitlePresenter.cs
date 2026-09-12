using UnityEngine;
using UnityEngine.SceneManagement;

namespace jugyou.UI
{
    public class TitlePresenter : MonoBehaviour
    {
        private const string BattleSceneName = "batoru";

        [SerializeField] TitleView titleView;
        private TitleModel titleModel;

        private void Start()
        {
            titleModel = new TitleModel();
            titleModel.Initialize();

            titleView.StartButtonClick += GameStart;
            titleView.AsobiText(titleModel.test);
        }

        private void OnDestroy()
        {
            titleView.StartButtonClick -= GameStart;
        }

        private void GameStart()
        {
            SceneManager.LoadScene(BattleSceneName);
        }
    }
}
