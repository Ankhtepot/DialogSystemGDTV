using System;
using System.Collections.Generic;
using System.Linq;
using Asset_Packs.GameDev.tv_Assets.Scripts.Inventories;
using Core;
using Saving;
using UnityEngine;
using UnityEngine.Events;

namespace Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        private readonly List<QuestStatus> statuses = new List<QuestStatus>();

        public IEnumerable<QuestStatus> QuestStatuses => statuses;
        public event Action onUpdate;

        public void AddQuest(Quest quest)
        {
            if (TryGetQuestStatus(quest, out _)) return;
            
            statuses.Add(new QuestStatus(quest));
            onUpdate?.Invoke();
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            if (TryGetQuestStatus(quest, out var status))
            {
                status.AddCompletedObjective(objective);
                if (status.IsComplete())
                {
                    GiveReward(quest);
                }
                onUpdate?.Invoke();
            }
        }

        public object CaptureState()
        {
            return statuses.Select(status => status.CaptureState()).ToList();
        }

        public void RestoreState(object state)
        {
            var stateList = state as List<object>;

            if (stateList == null) return;
            
            statuses.Clear();

            foreach (var objectState in stateList)
            {
                statuses.Add(new QuestStatus(objectState));
            }
        }
        
        private bool TryGetQuestStatus(Quest quest, out QuestStatus status)
        {
            var foundStatus =  statuses.FirstOrDefault(s => s.Quest == quest);

            if (foundStatus == null)
            {
                status = null;
                return false;
            }

            status = foundStatus;
            return true;
        }

        private bool HasQuest(string nameOfQuest)
        {
            return statuses.FirstOrDefault(s => s.Quest.Title == nameOfQuest) != null;
        }
        
        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.Rewards)
            {
                bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);

                if (!success)
                {
                    GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
                }
            }
        }

        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            switch (predicate)
            {
                case EPredicate.HasQuest:
                    return parameters.Any(HasQuest);
                case EPredicate.CompletedQuest when !parameters.Any(HasQuest):
                    return false;
                case EPredicate.CompletedQuest:
                {
                    foreach (var parameter in parameters)
                    {
                        if (!TryGetQuestStatus(Quest.GetByName(parameter), out var status)) continue;
                        if (!status.IsComplete()) return false;
                    }

                    return true;
                }
                default:
                    return null;
            }
        }
    }
}
