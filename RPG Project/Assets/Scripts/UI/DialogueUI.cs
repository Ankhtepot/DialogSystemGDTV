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
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;

        private void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            nextButton.onClick.AddListener(Next);
            UpdateUI();
        }

        private void UpdateUI()
        {
            AiText.text = playerConversant.GetText();
            nextButton.gameObject.SetActive(playerConversant.HasNext());
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (string choice in playerConversant.GetChoices())
            {
                var newChoice = Instantiate(choicePrefab, choiceRoot);
                newChoice.GetComponentInChildren<TextMeshProUGUI>().text = choice;
            }
        }

        private void Next()
        {
            playerConversant.Next();
            UpdateUI();
        }
    }
}
