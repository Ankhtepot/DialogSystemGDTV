using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue selectedDialogue;
        
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
                foreach (var node in selectedDialogue.AllNodes)
                {
                    EditorGUI.BeginChangeCheck();
                    
                    EditorGUILayout.LabelField("Node:");
                    var newText = EditorGUILayout.TextField(node.text);
                    var newId = EditorGUILayout.TextField(node.uniqueId);
                    
                    if (!EditorGUI.EndChangeCheck()) continue;
                    
                    Undo.RecordObject(selectedDialogue, "Update Dialog Text");
                    node.text = newText;
                    node.uniqueId = newId;
                }
            }
        }
    }
}