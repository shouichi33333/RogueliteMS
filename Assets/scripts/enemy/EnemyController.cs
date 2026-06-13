using UnityEngine;
using UnityEngine.AI;

namespace jugyou.batoru.enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyState enemyState;
        [SerializeField] private NavMeshAgent navMeshAgent;
        private Transform targetPlayer;

        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null)
            {
                targetPlayer = player.transform;
            }
            else
            {
                Debug.LogError("プレイヤータグのオブジェがない！");
            }
            if(navMeshAgent != null && enemyState != null && enemyState.EnemyData != null)
            {
                navMeshAgent.speed = enemyState.EnemyData.MoveSpeed;
            }
        }

        private void Update()
        {
            if(targetPlayer != null && navMeshAgent != null)
            {
                navMeshAgent.SetDestination(targetPlayer.position);
            }
        }

    }
}
