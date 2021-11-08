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
        [SerializeField] private Dialogue testDialogue;
        private Dialogue currentDialogue;
        private DialogueNode currentNode;
        private bool isChoosing;

        public UnityAction onConversationUpdated;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2);
            StartDialogue(testDialogue);
        }

        public void StartDialogue(Dialogue newDialogue)
        {
            currentDialogue = newDialogue;
            currentNode = currentDialogue.RootNode;
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
            isChoosing = false;
            currentNode = chosenNode;
            Next();
        }

        public void Next()
        {
            int numOfPlayerResponses = currentDialogue.GetPlayerChildren(currentNode).Count();
            if (numOfPlayerResponses > 0)
            {
                isChoosing = true;
                onConversationUpdated?.Invoke();
                return;
            }

            var children = currentDialogue.GetAIChildren(currentNode).ToList();
            currentNode = children[Random.Range(0, children.Count)];
            onConversationUpdated?.Invoke();
        }

        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(currentNode).Any();
        }
    }
}
