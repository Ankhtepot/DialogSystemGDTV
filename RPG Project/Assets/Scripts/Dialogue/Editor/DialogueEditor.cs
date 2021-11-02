using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue;
        [NonSerialized] private GUIStyle nodeStyle;
        [NonSerialized] private DialogueNode draggedNode;
        [NonSerialized] private Vector2 draggingOffset;
        [NonSerialized] private DialogueNode creatingNode;
        [NonSerialized] private DialogueNode deletingNode;
        [NonSerialized] private DialogueNode linkingParentNode;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var target = EditorUtility.InstanceIDToObject(instanceId) as Dialogue;
            if (!target) return false;

            ShowEditorWindow();
            return true;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;
            nodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node0") as Texture2D,
                    textColor = Color.white,
                },
                padding = new RectOffset(10, 10, 10, 10),
                border = new RectOffset(12, 12, 12, 12),
            };
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChange;
        }

        private void OnSelectionChange()
        {
            var newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (!selectedDialogue)
            {
                EditorGUILayout.LabelField("No dialog selected");
            }
            else
            {
                ProcessEvents();
                foreach (var node in selectedDialogue.AllNodes)
                {
                    DrawConnections(node);
                }

                foreach (var node in selectedDialogue.AllNodes)
                {
                    DrawNode(node);
                }

                if (creatingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Created new DialogueNode");
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
                    Undo.RecordObject(selectedDialogue, "Deleted DialogueNode");
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Rect rect = node.rect;
            Vector3 startPosition = new Vector3(rect.xMax, rect.center.y);

            foreach (var childNode in selectedDialogue.GetAllChildren(node))
            {
                rect = childNode.rect;
                Vector3 endPosition = new Vector3(rect.xMin, rect.center.y);

                Vector3 tangentOffset = endPosition - startPosition;
                tangentOffset.y = 0;
                tangentOffset.x *= 0.8f;

                Vector3 startTangent = startPosition + tangentOffset;
                Vector3 endTangent = endPosition - tangentOffset;
                Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, Color.cyan, null, 4f);
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggedNode == null)
            {
                draggedNode = GetNodeAtPoint(Event.current.mousePosition);

                if (draggedNode == null) return;

                draggingOffset = draggedNode.rect.position - Event.current.mousePosition;
            }
            else if (draggedNode != null && Event.current.type == EventType.MouseDrag)
            {
                Undo.RecordObject(selectedDialogue, "Moving root Dialogue Node");
                draggedNode.rect.position = Event.current.mousePosition + draggingOffset;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggedNode != null)
            {
                draggedNode = null;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (var node in selectedDialogue.AllNodes)
            {
                if (node.rect.Contains(point))
                {
                    foundNode = node;
                }
            }

            return foundNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, nodeStyle);

            EditorGUI.BeginChangeCheck();

            var newText = EditorGUILayout.TextField(node.text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialog Text");
                node.text = newText;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(" X Delete"))
            {
                deletingNode = node;
            }

            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    Undo.RecordObject(selectedDialogue, "Started assigning new connection in Dialogue");
                    linkingParentNode = node;
                }
            }
            else
            {
                DrawLinkButtons(node);
            }
            
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode.uniqueId != node.uniqueId 
                && !linkingParentNode.children.Contains(node.uniqueId) 
                && GUILayout.Button("Child") )
            {
                Undo.RecordObject(selectedDialogue, "Add Dialogue Link");
                linkingParentNode.children.Add(node.uniqueId);
                linkingParentNode = null;
            } else if (linkingParentNode.uniqueId != node.uniqueId
                       && linkingParentNode.children.Contains(node.uniqueId)
                       && GUILayout.Button("Unlink"))
            {
                Undo.RecordObject(selectedDialogue, "Unlinking connection in Dialogue");
                linkingParentNode.children.Remove(node.uniqueId);
                linkingParentNode = null;
            }
            else if (linkingParentNode.uniqueId == node.uniqueId 
                     && GUILayout.Button("Cancel"))
            {
                Undo.RecordObject(selectedDialogue, "Canceling creation of new link in Dialogue");
                linkingParentNode = null;
            } 
        }
    }
}