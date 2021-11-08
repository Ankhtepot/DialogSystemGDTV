using System;
using Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DialogueUI : MonoBehaviour
    {
        private PlayerConversant playerConversant;
        [SerializeField] private TextMeshProUGUI AiText;
        [SerializeField] private Button nextButton;
        [SerializeField] private GameObject AIResponse;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;

        private void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUI;
            nextButton.onClick.AddListener(Next);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (!playerConversant.IsActive()) return;
            
            bool isChoosing = playerConversant.IsChoosing();
            AIResponse.SetActive(!isChoosing);
            choiceRoot.gameObject.SetActive(isChoosing);
            
            if (isChoosing)
            {
                BuildChoiceList();
            }
            else
            {
                AiText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                var newChoice = Instantiate(choicePrefab, choiceRoot);
                newChoice.GetComponentInChildren<TextMeshProUGUI>().text = choice.Text;
                Button button = newChoice.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    playerConversant.SelectChoice(choice);
                });
            }
        }

        private void Next()
        {
            playerConversant.Next();
        }

        private void OnDestroy()
        {
            if (playerConversant)
            {
                playerConversant.onConversationUpdated -= UpdateUI;
            }
        }
    }
}
