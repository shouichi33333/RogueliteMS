using Core.Interface;
using MasterData;
using UnityEngine;
using UnityEngine.Events;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace jugyou.batoru.enemy
{
    public class EnemyState : MonoBehaviour, IDamageable
    {
        private const float flashDuration = 0.1f;

        private const float orbDoropHeightOffset = 0.5f;

        [SerializeField] Renderer[] modelRenderers;

        [SerializeField] GameObject experienceOrbPrefab;

        private Color[] defaultColors;

        private CancellationTokenSource flashCts;

        public EnemyDataRecord EnemyData { get; private set; }
        public int CurrentHP { get; private set; }

        public event UnityAction<EnemyState> OnReturnToPoolAction;

        public event UnityAction OnDamageAction;

        public void initialize(ulong id)
        {
            EnemyData = MasterDataAccessor.Instance.GetById<EnemyDataRecord>(id);
            if (modelRenderers != null)
            {
                defaultColors = new Color[modelRenderers.Length];
                for (int i = 0; i < modelRenderers.Length; i++)
                {
                    if (modelRenderers[i] != null)
                    {
                        defaultColors[i] = modelRenderers[i].material.color;
                    }
                }
            }
        }
        public void Setup()
        {
            if (EnemyData == null) return;
            CurrentHP = EnemyData.MaxHp;
            gameObject.SetActive(true);
            ResetColor();
        }
        public void TakeDamage(int damage)
        {
            if (damage <= 0)
            {
                return;
            }
            CurrentHP -= damage;

            if (CurrentHP <= 0)
            {
                Die();
            }
            else
            {
                OnDamageAction?.Invoke();
                flashCts?.Cancel();
                flashCts?.Dispose();
                flashCts = null;

                flashCts = new CancellationTokenSource();
                CancellationTokenSource tokens = CancellationTokenSource.CreateLinkedTokenSource(flashCts.Token, this.GetCancellationTokenOnDestroy());
                DamageFlashAsync(tokens.Token).Forget();
            }
        }
        private void Die()
        {
            if(experienceOrbPrefab != null)
            {
                Vector3 spownPoint = transform.position + Vector3.up * orbDoropHeightOffset;
                Instantiate(experienceOrbPrefab,spownPoint,Quaternion.identity);
            }
            Debug.Log($"{EnemyData.EnemyName}‚ð“|‚µ‚½");
            this.gameObject.SetActive(false);
            OnReturnToPoolAction?.Invoke(this);
        }

        private void ResetColor()
        {
            if (modelRenderers == null || defaultColors == null) return;
            for (int i = 0; i < modelRenderers.Length; i++)
            {
                if (modelRenderers[i] != null)
                {
                    modelRenderers[i].material.color = defaultColors[i];
                }
            }
        }

        private async UniTaskVoid DamageFlashAsync(CancellationToken token)
        {
            if (modelRenderers == null) return;

            foreach (Renderer renderer in modelRenderers)
            {
                if (renderer != null)
                {
                    renderer.material.color = Color.red;
                }
            }
            bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(flashDuration), cancellationToken: token).SuppressCancellationThrow();

            if (!isCanceled)
            {
                ResetColor();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var player = collision.gameObject.GetComponent<IDamageable>();
            if(player != null && collision.gameObject.CompareTag("Player"))
            {
                player.TakeDamage(10);
            }
        }

    }
}
