using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quests
{
    [Serializable]
    public class QuestStatus
    {
        private Quest quest;
        private List<string> completedObjectives = new List<string>();

        public Quest Quest => quest;
        public IEnumerable<string> CompletedObjectives => completedObjectives;
        public int CompletedObjectivesCount => completedObjectives.Count;

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public bool IsObjectiveCompleted(string objective) => completedObjectives.Contains(objective);
    }
}
