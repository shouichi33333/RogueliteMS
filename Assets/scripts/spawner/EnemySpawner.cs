using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using jugyou.batoru.enemy;

namespace jugyou.batoru.spawner
{
    public class EnemySpawner : MonoBehaviour
    {
        private const float spawnInterval = 3;
        private const float maxDistance = 2;
        private const int maxEnemy = 20;

        [SerializeField] GameObject enemyPrefab;
        [SerializeField] private Transform[] spawnPoint;

        private Queue<EnemyState> enemys = new Queue<EnemyState>();
        private void Start()
        {
            spawnLoopAsync().Forget();
        }
        async UniTaskVoid spawnLoopAsync()
        {
            var token = this.GetCancellationTokenOnDestroy();
            for(int i = 0;i < maxEnemy; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab,this.transform);
                EnemyState enemyState = enemy.GetComponent<EnemyState>();
                enemyState.gameObject.SetActive(false);
                enemys.Enqueue(enemyState);
            }

            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(spawnInterval), cancellationToken: token);
                spownEnemy();
            }
        }


        private void spownEnemy()
        {
            if (enemyPrefab == null && spawnPoint == null) return;

            int random = UnityEngine.Random.Range(0, spawnPoint.Length);
            Transform spawn = spawnPoint[random];

            Vector3 sefe = spawn.position;
            if (NavMesh.SamplePosition(spawn.position, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
            {
                sefe = hit.position;

            }
            else
            {
                Debug.Log("スポーンポイントの近くにナブメッシュがない");
                return;
            }
            EnemyState Estate = null;
            {
                //if (enemys.Count <= 0)
                //{
                //    foreach (Transform t in this.transform)
                //    {
                //        GameObject go = t.gameObject;
                //        if (go.activeSelf == false)
                //        {
                //            EnemyState Este = go.GetComponent<EnemyState>();
                //            if (Este != null) enemys.Enqueue(Este);
                //        }
                //    }
                //    if (enemys.Count == 0)
                //    {
                //        Debug.Log("敵が最大");
                //        return;
                //    }
                //}
            }
            if(enemys.Count > 0)
            {
                Estate = enemys.Dequeue();
            }
            else
            {
                //Debug.Log("敵がキューにいない");
                return;
            }

            Estate.OnReturnToPoolAction -= ReturnToPool;
            Estate.OnReturnToPoolAction += ReturnToPool;

            Estate.gameObject.transform.position = spawn.position;
            Estate.gameObject.transform.rotation = Quaternion.identity;
            Estate.gameObject.SetActive(true);

            //Debug.Log("敵出現！");
        }

        private void ReturnToPool(EnemyState enemy)
        {
            enemys.Enqueue(enemy);
            enemy.OnReturnToPoolAction -= ReturnToPool;
        }
    }
}
