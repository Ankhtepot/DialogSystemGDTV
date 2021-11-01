using System;
using UnityEngine;

namespace Dialogue
{
    [Serializable]
    public class DialogueNode
    {
        public string uniqueId;
        public string text;
        public string[] children;
        public Rect rect = new Rect(0, 0, 200, 100);
    }
}
