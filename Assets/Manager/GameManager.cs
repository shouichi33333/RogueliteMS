using Cysharp.Threading.Tasks;
using jugyou.batoru.Player;
using jugyou.batoru.spawner;
using UnityEngine;
using MasterData;

namespace jugyou.batoru.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private PlayerController player = null;
        [SerializeField] private EnemySpawner enemySpawner = null;

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
        }
    }
}
