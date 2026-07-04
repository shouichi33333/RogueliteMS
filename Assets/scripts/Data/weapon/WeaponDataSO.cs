using UnityEngine;
using jugyou.batoru.Enum;

[CreateAssetMenu(menuName = ("Game/WeaponDataSO"))]
public class WeaponDataSO : ScriptableObject
{
    [field: SerializeField] public string WeaponName {  get; private set; }
    [field: SerializeField] public FireType WeponType { get; private set; }
    [field: SerializeField] public int AttackPower {  get; private set; }
    [field: SerializeField] public float FireInterval {  get; private set; }
    [field: SerializeField] public float FireRate { get; private set; }
    [field: SerializeField] public int MaxAmmo { get; private set; }
    [field: SerializeField] public float ReloadTime { get; private set; }
    
}
