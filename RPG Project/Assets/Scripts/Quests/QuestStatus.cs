using System;
using System.Collections.Generic;
using UI.Quests;
using UnityEngine;

namespace Quests
{
    [Serializable]
    public class QuestStatus
    {
        private Quest quest;
        private List<string> completedObjectives = new List<string>();
        
        [Serializable]
        private class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;
        }

        public Quest Quest => quest;
        public int CompletedObjectivesCount => completedObjectives.Count;

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(object objectState)
        {
            var state = objectState as QuestStatusRecord;
            quest = Quest.GetByName(state?.questName);
            completedObjectives = state?.completedObjectives;
        }

        public bool IsObjectiveCompleted(string objective) => completedObjectives.Contains(objective);

        public bool AddCompletedObjective(string objective)
        {
            if (!quest.HasObjective(objective) || IsObjectiveCompleted(objective)) return false;
            
            completedObjectives.Add(objective);
            return true;
        }

        public object CaptureState()
        {
            return new QuestStatusRecord()
            {
                questName = quest.name,
                completedObjectives = completedObjectives
            };
        }
    }
}
