using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] Dialogue currentDialogue;
        private DialogueNode currentNode;

        private void Awake()
        {
            currentNode = currentDialogue.RootNode;
        }

        public string GetText()
        {
            return currentNode 
                ? currentNode.Text 
                : "";
        }

        public void Next()
        {
            var children = currentDialogue.GetAllChildren(currentNode).ToList();
            currentNode = children[Random.Range(0, children.Count)];
        }

        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(currentNode).Any();
        }
    }
}
