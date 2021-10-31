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
    }
}
