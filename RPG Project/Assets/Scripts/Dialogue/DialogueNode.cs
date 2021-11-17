using System;
using System.Collections.Generic;
using Core;
using UnityEditor;
using UnityEngine;

namespace Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(0, 0, 200, 185);
        [SerializeField] private bool isPlayerSpeaking;
        [SerializeField] private EDialogueActions onEnterAction;
        [SerializeField] private EDialogueActions onExitAction;
        [SerializeField] private Condition condition;

        public List<string> Children => children;
        public EDialogueActions OnEnterAction => onEnterAction;
        public EDialogueActions OnExitAction => onExitAction;

#if UNITY_EDITOR
        public void AddChild(string newChild)
        {
            Undo.RecordObject(this, "Add new Dialogue link.");
            EditorUtility.SetDirty(this);
            children.Add(newChild);
        }

        public void RemoveChild(string childToRemove)
        {
            Undo.RecordObject(this, "Removing Dialogue link.");
            EditorUtility.SetDirty(this);
            children.Remove(childToRemove);
        }
#endif

        public bool IsPlayerSpeaking
        {
            get => isPlayerSpeaking;
            set
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Changing Who is speaking for a DialogNode.");
                EditorUtility.SetDirty(this);
#endif
                isPlayerSpeaking = value;
            }
        }

        public string Text
        {
            get => text;
            set
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Changing DialogNode text.");
                EditorUtility.SetDirty(this);
#endif
                text = value;
            }
        }
        
        public EDialogueActions ActionEnterAction
        {
            get => onEnterAction;
            set
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Changing DialogNode onEnterAction.");
                EditorUtility.SetDirty(this);
#endif
                onEnterAction = value;
            }
        }
        
        public EDialogueActions ActionExitAction
        {
            get => onExitAction;
            set
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Changing DialogNode onExitAction.");
                EditorUtility.SetDirty(this);
#endif
                onExitAction = value;
            }
        }

        public Rect Rect
        {
            get => rect;
            set
            {
#if UNITY_EDITOR
                Undo.RecordObject(this, "Changing DialogNode rectangle.");
                EditorUtility.SetDirty(this);
#endif
                rect = value;
            }
        }

        public void SetPosition(Vector2 newPosition)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Changing DialogNode rectangle position.");
            EditorUtility.SetDirty(this);
#endif
            rect.position = newPosition;
        }
        
        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }
    }
}