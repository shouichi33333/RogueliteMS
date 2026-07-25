using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace jugyou.batoru.enemy
{
    public class EnemyController : MonoBehaviour
    {
        private const string playerTagName = "Player";
        private const float knockBackForce = 2f;
        private const float knockBackDurarion = 0.15f;

        [SerializeField] private EnemyState enemyState;
        [SerializeField] private NavMeshAgent navMeshAgent;
        private Transform targetPlayer;

        private CancellationTokenSource hitCts;

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

        private void OnEnable()
        {
            if(enemyState != null)
            {
                enemyState.OnDamageAction -= HandleDamage;
                enemyState.OnDamageAction += HandleDamage;
            }
        }

        private void OnDisable()
        {
            if(enemyState != null)
            {
                enemyState.OnDamageAction -= HandleDamage;
            }
            if(navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
            {
                navMeshAgent.isStopped = false;
            }
        }

        private void Update()
        {
            if(targetPlayer != null && navMeshAgent != null)
            {
                navMeshAgent.SetDestination(targetPlayer.position);
            }
        }

        private async UniTaskVoid KnockBackAsync(CancellationToken token)
        {
            if(navMeshAgent == null)
            {
                return;
            }
            bool wasStopped = navMeshAgent.isStopped;
            navMeshAgent.isStopped = true;

            if(targetPlayer != null)
            {
                Vector3 dir = (transform.position - targetPlayer.position).normalized;
                dir.y = 0;
                transform.position += dir * knockBackForce;
            }

            bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(knockBackDurarion),cancellationToken:token).SuppressCancellationThrow();

            if (!isCanceled && navMeshAgent.isActiveAndEnabled)
            {
                navMeshAgent.isStopped = wasStopped;
            }
        }

        private void HandleDamage()
        {
            hitCts?.Cancel();
            hitCts?.Dispose();
            hitCts = null;
            hitCts = new CancellationTokenSource();
            CancellationTokenSource tokens = CancellationTokenSource.CreateLinkedTokenSource(hitCts.Token,this.GetCancellationTokenOnDestroy());
            KnockBackAsync(tokens.Token).Forget();
        }
    }
}
