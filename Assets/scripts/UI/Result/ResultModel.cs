using jugyou.batoru.Manager;

namespace jugyou.UI
{
    public class ResultModel
    {
        public bool IsClear { get; private set; }
        public int Level { get; private set; }
        public float SuviedTime { get; private set; }

        public void Initialize()
        {
            if (GameManager.Instance != null)
            {
                IsClear = GameManager.Instance.IsGameClear;
                Level = GameManager.Instance.FinalLecel;
                SuviedTime = GameManager.Instance.SurvivedTime;
            }
        }
    }
}
