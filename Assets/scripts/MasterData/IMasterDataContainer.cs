using System.Collections.Generic;

namespace MasterData
{
    public interface IMasterDataContainer<T> where T : IMasterData
    {
        List<T> Records { get; }
    }
}
