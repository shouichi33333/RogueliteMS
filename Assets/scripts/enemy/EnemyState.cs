using Core.Interface;
using UnityEngine;

namespace jugyou.batoru.enemy
{
    public class EnemyState : MonoBehaviour,IDamageable
    {
        private const int MAX_HP = 100;
        public int CurrentHP {  get; private set; }
        public void Awake()
        {
            CurrentHP = MAX_HP;
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
            Destroy(gameObject);
        }
    }
}
