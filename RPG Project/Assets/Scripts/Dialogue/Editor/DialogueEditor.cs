using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue;
        private GUIStyle nodeStyle;
        private DialogueNode draggedNode = null;
        private Vector2 draggingOffset;
        
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
            // Debug.Log($"OnSelectionChanged. Selected object: {newDialogue}");
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
                    OnGUINode(node);
                }
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggedNode == null)
            {
                // Debug.Log("Start dragging");
                draggedNode = GetNodeAtPoint(Event.current.mousePosition);
                
                if (draggedNode == null) return;

                draggingOffset = draggedNode.rect.position - Event.current.mousePosition;
            }
            else if (draggedNode != null && Event.current.type == EventType.MouseDrag)
            {
                // Debug.Log("Drag move");
                Undo.RecordObject(selectedDialogue, "Moving root Dialogue Node");
                draggedNode.rect.position = Event.current.mousePosition + draggingOffset;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggedNode != null)
            {
                // Debug.Log("Stopping dragging");
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

        private void OnGUINode(DialogueNode node)
        {
            // GUILayout.BeginArea(new Rect(10, 10, 200, 200));
            GUILayout.BeginArea(node.rect, nodeStyle);
            
            EditorGUI.BeginChangeCheck();
                    
            EditorGUILayout.LabelField("Node:", EditorStyles.whiteLabel);
            var newText = EditorGUILayout.TextField(node.text);
            var newId = EditorGUILayout.TextField(node.uniqueId);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update Dialog Text");
                node.text = newText;
                node.uniqueId = newId;
            }
            
            GUILayout.EndArea();
        }
    }
}