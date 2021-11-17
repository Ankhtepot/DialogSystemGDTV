using System;
using System.Collections.Generic;
using System.Linq;
using Asset_Packs.GameDev.tv_Assets.Scripts.Inventories;
using UnityEngine;

namespace Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private List<Objective> objectives = new List<Objective>();
        [SerializeField] private List<Reward> rewards = new List<Reward>();

        public string Title => name;
        public int ObjectivesCount => objectives.Count;

        public IEnumerable<Objective> Objectives => objectives;

        public IEnumerable<Reward> Rewards => rewards;

        public bool HasObjective(string objectiveRef)
        {
            return objectives.FirstOrDefault(o => o.reference == objectiveRef) != null;
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

        [Serializable]
        public class Reward
        {
            [Min(1)]
            public int number;
            public InventoryItem item;
        }

        [Serializable]
        public class Objective
        {
            public string reference;
            public string description;
        }
    }
}
