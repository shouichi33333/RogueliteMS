using Core.Interface;
using MasterData;
using UnityEngine;
using UnityEngine.Events;

namespace jugyou.batoru.enemy
{
    public class EnemyState : MonoBehaviour,IDamageable
    {
        public EnemyDataRecord EnemyData {  get; private set; }
        public int CurrentHP {  get; private set; }

        public event UnityAction<EnemyState> OnReturnToPoolAction;

        public void initialize(ulong id)
        {
            EnemyData = MasterDataAccessor.Instance.GetById<EnemyDataRecord>(id);
        }
        public void Setup()
        {
            if (EnemyData == null) return;
            CurrentHP = EnemyData.MaxHp;
            gameObject.SetActive(true);
        }
        public void TekeDamage(int damage)
        {
            if(damage <= 0)
            {
                return;
            }
            CurrentHP -= damage;
            if(CurrentHP <= 0)
            {
                Die();
            }
        }
        private void Die()
        {
            Debug.Log($"{EnemyData.EnemyName}‚ð“|‚µ‚½");
            this.gameObject.SetActive(false);
            OnReturnToPoolAction?.Invoke(this);
        }
    }
}
