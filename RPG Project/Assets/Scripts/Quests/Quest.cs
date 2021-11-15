using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private List<string> objectives = new List<string>();

        public string Title => name;
        public int ObjectivesCount => objectives.Count;

        public IEnumerable<string> Objectives => objectives;

        public bool HasObjective(string objective)
        {
            return objectives.Contains(objective);
        }

        public static Quest GetByName(string questName)
        {
            foreach (var quest in Resources.LoadAll<Quest>(""))
            {
                if (quest.name == questName)
                {
                    return quest;
                }   
            }

            return null;
        }
    }
}
