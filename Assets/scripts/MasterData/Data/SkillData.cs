using UnityEngine;
using System.Collections.Generic;
using System;

namespace MasterData
{
    [Serializable]
    public class SkillDataRecord : IMasterData
    {
        [field: SerializeField] public ulong Id { get; private set; }
        [field: SerializeField] public string SkillName { get; private set; }
        [field: SerializeField] public string SkillDescription { get; private set; }
        [field: SerializeField] public int SkillType { get; private set; }
        [field: SerializeField] public float Value { get; private set; }


    }
    [CreateAssetMenu(fileName = "SkillData", menuName = "SO/SkillData")]
    public class SkillData : ScriptableObject, IMasterDataContainer<SkillDataRecord>
    {
        [field: SerializeField] public List<SkillDataRecord> Records { get; private set; }

    }
}
