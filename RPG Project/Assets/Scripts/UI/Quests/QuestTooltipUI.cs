using System.Linq;
using Quests;
using TMPro;
using UnityEngine;

namespace UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Transform objectiveContainer;
        [SerializeField] private GameObject objectivePrefab;
        [SerializeField] private GameObject objectiveCompletedPrefab;
        [SerializeField] private TextMeshProUGUI rewards;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.Quest;
            title.text = quest.Title;

            foreach (Transform child in objectiveContainer.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var objective in quest.Objectives)
            {
                var objectiveInstance = Instantiate(
                    status.IsObjectiveCompleted(objective.reference) 
                        ? objectiveCompletedPrefab 
                        : objectivePrefab,
                    objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective.description;
            }
            
            rewards.text = GetRewardText(status);
        }

        private string GetRewardText(QuestStatus status)
        {
            string rewardsText = "";
            int rewardsCount = status.Quest.Rewards.Count();

            if (rewardsCount == 0) return "No reward";
            
            for (int i = 0; i < rewardsCount; i++)
            {
                Quest.Reward reward = status.Quest.Rewards.ElementAt(i);
                rewardsText += $"{(i > 0 ? ", " : "")}{(reward.number > 1 ? reward.number.ToString() : "")} {reward.item.GetDisplayName()}";
            }

            return rewardsText;
        }
    }
}