using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string[] objectives;

        public string Title => name;
        public int ObjectivesCount => objectives.Length;

        public IEnumerable<string> Objectives => objectives;
    }
}
