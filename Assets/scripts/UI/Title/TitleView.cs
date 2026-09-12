using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace jugyou.UI
{
    public class TitleView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI asobi;
        [SerializeField] Button startButton;
        public event Action StartButtonClick;

        private void Awake()
        {
            if(startButton != null)
            {
                startButton.onClick.AddListener(() => StartButtonClick());
            }
        }

        public void AsobiText(string text)
        {
            asobi.text = text;
        }
    }
}
