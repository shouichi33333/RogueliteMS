using Cysharp.Threading.Tasks;
using jugyou.batoru.Player;
using jugyou.batoru.spawner;
using UnityEngine;
using MasterData;
using TMPro;
using UnityEngine.SceneManagement;

namespace jugyou.batoru.Manager
{
    public class GameManager : MonoBehaviour
    {
        private const string ResultSceneName = "rizaruto";

        public static GameManager Instance { get; private set; }
        [SerializeField] private PlayerController player = null;
        [SerializeField] private EnemySpawner enemySpawner = null;
        [SerializeField] TextMeshProUGUI timerText = null;
        [SerializeField] float gameClearTime = 180f;

        public bool IsGameClear { get; private set; }
        public float SurvivedTime { get; private set; }
        public int FinalLecel { get; private set; }

        private float currentTime = 0f;
        private bool isGameActive = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        private void Start()
        {
            Setup().Forget();
        }
        private async UniTaskVoid Setup()
        {
            await MasterDataAccessor.Instance.InitializeAsync();

            if (player != null)
            {
                player.Setup();
            }
            if (enemySpawner != null)
            {
                enemySpawner.Setup();
            }
            IsGameClear = false;
            currentTime = gameClearTime;
            isGameActive = true;
        }


        private void Update()
        {
            if (!isGameActive) return;

            if (Time.timeScale == 0f) return;

            currentTime -= Time.deltaTime;
            SurvivedTime = gameClearTime - currentTime;

            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(currentTime / 60);
                int seconds = Mathf.FloorToInt(currentTime - minutes * 60f);
                timerText.SetText($"{minutes:00}:{seconds:00}");
            }

            if (currentTime <= 0)
            {
                GameClear();
            }

        }

        private void GameClear()
        {
            isGameActive = false;
            IsGameClear = true;
            FinalLecel = player != null ? player.CurrntLevel : 0;

            Debug.Log("ƒQƒ€ƒNƒŠ");
            GoToResultScene();
        }

        public void GameOver()
        {
            isGameActive = false;
            IsGameClear = true;
            FinalLecel = player != null ? player.CurrntLevel : 0;

            Debug.Log("ƒQƒ€ƒIƒo");
            GoToResultScene();
        }

        private void GoToResultScene()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene(ResultSceneName);
        }

    }
}
