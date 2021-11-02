using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        private Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            if (nodes.Count == 0)
            {
                DialogueNode rootNode = new DialogueNode()
                {
                    uniqueId = Guid.NewGuid().ToString()
                };
                nodes.Add(rootNode);
            }
            
            OnValidate();
        }
#endif
        private void OnValidate()
        {
            nodeLookup = nodes.ToDictionary(node => node.uniqueId);
        }

        public IEnumerable<DialogueNode> AllNodes => nodes;

        public DialogueNode RootNode => nodes[0];

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            // return parentNode.children.SelectMany(id => nodes.Where(childNode => id == childNode.uniqueId));

            foreach (var childId in parentNode.children)
            {
                if (nodeLookup.ContainsKey(childId))
                {
                    yield return nodeLookup[childId];
                }
            }
        }

        public void CreateNode(DialogueNode parent)
        {
            var newRect = new Rect(parent.rect)
            {
                x = parent.rect.x + parent.rect.width + 50
            };
            var newNode = new DialogueNode
            {
                uniqueId = Guid.NewGuid().ToString(),
                rect = newRect
            };
            parent.children.Add(newNode.uniqueId);
            nodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            OnValidate();

            CleanDanglingChildren(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in AllNodes)
            {
                node.children.Remove(nodeToDelete.uniqueId);
            }
        }
    }
}
