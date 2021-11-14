using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private string playerName;
        private Dialogue currentDialogue;
        private DialogueNode currentNode;
        private AIConversant currentConversant;
        private bool isChoosing;
        private string currentConversantName;
        public string CurrentConversantName => isChoosing ? playerName : currentConversantName;

        public UnityAction onConversationUpdated;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2);
        }

        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            currentConversant = newConversant;
            currentConversantName = newConversant.ConversantName;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.RootNode;
            TriggerEnterAction();
            onConversationUpdated?.Invoke();
        }

        public void Quit()
        {
            currentDialogue = null;
            TriggerExitAction();
            currentConversant = null;
            currentConversantName = null;
            currentNode = null;
            isChoosing = false;
            onConversationUpdated?.Invoke();
        }

        public bool IsActive()
        {
            return currentDialogue;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            return currentNode 
                ? currentNode.Text 
                : "";
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetPlayerChildren(currentNode);
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerExitAction();
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numOfPlayerResponses = currentDialogue.GetPlayerChildren(currentNode).Count();
            if (numOfPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated?.Invoke();
                return;
            }

            var children = currentDialogue.GetAIChildren(currentNode).ToList();
            TriggerExitAction();
            currentNode = children[Random.Range(0, children.Count)];
            TriggerEnterAction();
            onConversationUpdated?.Invoke();
        }

        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(currentNode).Any();
        }

        private void TriggerEnterAction()
        {
            if (currentNode && !string.IsNullOrEmpty(currentNode.OnEnterAction))
            {
                Debug.Log(currentNode.OnEnterAction);
                TriggerAction(currentNode.OnEnterAction);
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode && !string.IsNullOrEmpty(currentNode.OnExitAction))
            {
                Debug.Log(currentNode.OnExitAction);
                TriggerAction(currentNode.OnExitAction);
            }
        }

        private void TriggerAction(string action)
        {
            foreach (var dialogueTrigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                dialogueTrigger.Trigger(action);
            }
        }
    }
}
