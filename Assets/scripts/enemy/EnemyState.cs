using Core.Interface;
using UnityEngine;
using UnityEngine.Events;

namespace jugyou.batoru.enemy
{
    public class EnemyState : MonoBehaviour,IDamageable
    {
        [field:SerializeField] public EnemyDataSO EnemyData {  get; private set; }
        public int CurrentHP {  get; private set; }

        public event UnityAction<EnemyState> OnReturnToPoolAction;
        private void OnEnable()
        {
            if (EnemyData == null) return; 
            CurrentHP = EnemyData.MaxHp;
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
