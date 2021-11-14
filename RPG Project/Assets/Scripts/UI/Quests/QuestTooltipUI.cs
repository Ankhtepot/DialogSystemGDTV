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
                    status.IsObjectiveCompleted(objective) 
                        ? objectiveCompletedPrefab 
                        : objectivePrefab,
                    objectiveContainer);
                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective;
            }
        }
    }
}