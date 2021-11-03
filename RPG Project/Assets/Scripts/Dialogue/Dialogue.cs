using System;
using System.Collections;
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
        private Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            // if (nodes.Count == 0)
            // {
            //     CreateNode(null);
            // }
            //
            // OnValidate();
        }
#endif
        private void OnValidate()
        {
            nodeLookup = nodes.ToDictionary(node => node.name);
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
            var newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue node");

            if (parent)
            {
                var newRect = new Rect(parent.rect)
                {
                    x = parent.rect.x + parent.rect.width + 50
                };
                newNode.rect = newRect;
                parent.children.Add(newNode.name);
            }

            nodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            OnValidate();

            CleanDanglingChildren(nodeToDelete);

            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (var node in AllNodes)
            {
                node.children.Remove(nodeToDelete.name);
            }
        }

        public void OnBeforeSerialize()
        {
            if (nodes.Count == 0)
            {
                CreateNode(null);
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
        }

        // public void OnBeforeSerialize()
        // {
        //     if (nodes.Count == 0)
        //     {
        //         CreateNode(null);
        //     }
        //
        //     if (AssetDatabase.GetAssetPath(this) != "")
        //     {
        //         foreach (DialogueNode node in AllNodes)
        //         {
        //             if (AssetDatabase.GetAssetPath(node) == "")
        //             {
        //                 AssetDatabase.AddObjectToAsset(node, this);
        //             }
        //         }
        //     }
        // }

        public void OnAfterDeserialize()
        {
        }
    }
}