using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Quests
{
    public class QuestList : MonoBehaviour
    {
        private List<QuestStatus> statuses = new List<QuestStatus>();

        public IEnumerable<QuestStatus> QuestStatuses => statuses;
        public event Action onUpdate;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            
            statuses.Add(new QuestStatus(quest));
            onUpdate?.Invoke();
        }

        public bool HasQuest(Quest quest)
        {
            return statuses.FirstOrDefault(status => status.Quest == quest) != null;
        }
    }
}
