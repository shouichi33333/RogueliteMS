using UnityEngine;

[CreateAssetMenu(menuName = "Game/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    [field: SerializeField] public string EnemyName {  get; private set; }
    [field: SerializeField] public int MaxHp {  get; private set; }
    [field: SerializeField] public float MoveSpeed {  get; private set; }
}
