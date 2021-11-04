using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField] private Vector2 newNodeOffset = new Vector2(50, 0);
        private Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();
        
        private void OnValidate()
        {
            nodeLookup = nodes.ToDictionary(node => node.name);
        }

        public IEnumerable<DialogueNode> AllNodes => nodes;

        public DialogueNode RootNode => nodes[0];

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            // return parentNode.children.SelectMany(id => nodes.Where(childNode => id == childNode.uniqueId));

            foreach (var childId in parentNode.Children)
            {
                if (nodeLookup.ContainsKey(childId))
                {
                    yield return nodeLookup[childId];
                }
            }
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            var newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue node");

            Undo.RecordObject(this, "Created new DialogueNode");
            AddNode(newNode);
        }
        
        private DialogueNode MakeNode(DialogueNode parent)
        {
            var newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            if (parent)
            {
                var newRect = new Rect(parent.Rect)
                {
                    x = parent.Rect.xMax + newNodeOffset.x,
                    y = parent.Rect.yMin + (parent.Children.Count * 100) + newNodeOffset.y
                };
                newNode.IsPlayerSpeaking = !parent.IsPlayerSpeaking;
                newNode.Rect = newRect;
                parent.AddChild(newNode.name);
            }

            return newNode;
        }
        
        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted DialogueNode");
            nodes.Remove(nodeToDelete);
            OnValidate();

            CleanDanglingChildren(nodeToDelete);

            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in AllNodes)
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                AddNode(MakeNode(null));
            }

            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                foreach (var node in AllNodes)
                {
                    if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(node)))
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}