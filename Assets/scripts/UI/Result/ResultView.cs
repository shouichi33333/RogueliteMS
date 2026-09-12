using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace jugyou.UI
{
    public class ResultView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI resultText;
        [SerializeField] Button retryButton;
        [SerializeField] Button returnToTitleButton;
        public event UnityAction OnRetryAction;
        public event UnityAction OnReturnToTitleAction;

        private void Awake()
        {
            if (retryButton != null)
            {
                retryButton.onClick.AddListener(() => OnRetryAction?.Invoke());
            }
            if (returnToTitleButton != null)
            {
                returnToTitleButton.onClick.AddListener(() => OnReturnToTitleAction?.Invoke());
            }
        }
        public void SetResultText(string text)
        {
            if(resultText != null)
            {
                resultText.text = text;
            }
        }
    }
}
