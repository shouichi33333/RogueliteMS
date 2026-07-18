using System;
using System.Collections.Generic;
using UnityEngine;

namespace MasterData
{
    [Serializable]
    public class EnemyDataRecord : IMasterData
    {
        [field: SerializeField] public ulong Id { get; private set; }
        [field: SerializeField] public string EnemyName { get; private set; }
        [field: SerializeField] public int MaxHp { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; }

    }
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "SO/EnemyData")]
    public class EnemyData : ScriptableObject, IMasterDataContainer<EnemyDataRecord>
    {
        [field: SerializeField] public List<EnemyDataRecord> Records { get; private set; }
    }
}
