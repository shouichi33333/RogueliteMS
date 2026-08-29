using System;
using System.Linq;
using jugyou.batoru.Player;
using MasterData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace jugyou.batoru.Manager
{
    [Serializable]
    public class SkillButtonUI
    {
        public Button button;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI dectText;
    }

    public class LevelUpManager : MonoBehaviour
    {
        public static LevelUpManager Instance { get; private set; }

        [Header("UIê›íË")]
        [SerializeField] GameObject skillSelectPanel;
        [SerializeField] SkillButtonUI[] skillButtons = new SkillButtonUI[3];

        private PlayerInptActions inputActions;
        private PlayerController playerController;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            Time.timeScale = 1.0f;

            if(skillSelectPanel != null)
            {
                skillSelectPanel.SetActive(false);
            }
        }

        public void OnLevelUp(PlayerInptActions currentInput,PlayerController playerController_)
        {
            inputActions = currentInput;
            playerController = playerController_;

            var allSkils = MasterDataAccessor.Instance.GetAll<SkillDataRecord>();
            var chosenSkill = allSkils.OrderBy(v => System.Guid.NewGuid()).Take(3).ToList();

            for(int i = 0; i < 3; i++)
            {
                var skill = chosenSkill[i];
                var ui = skillButtons[i];

                ui.nameText.text = skill.SkillName;
                ui.dectText.text = skill.SkillDescription;

                ui.button.onClick.RemoveAllListeners();
                ui.button.onClick.AddListener(() => OnSkillSelected(skill));

            }

            if(skillSelectPanel != null)
            {
                skillSelectPanel.SetActive(true);
            }

            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if(inputActions != null)
            {
                inputActions.Player.Disable();
            }

        }
        private void OnSkillSelected(SkillDataRecord selectedSkill)
        {
            if(playerController != null)
            {
                playerController.ApplySkill(selectedSkill);
            }

            if(skillSelectPanel != null)
            {
                skillSelectPanel.SetActive(false);
            }

            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if(inputActions != null)
            {
                inputActions.Player.Enable();
            }
        }
    }
}
