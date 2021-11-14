using System.Linq;
using Quests;
using TMPro;
using UnityEngine;

namespace UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI progress;

        public QuestStatus QuestStatus { get; private set; }

        public void Setup(QuestStatus status)
        {
            QuestStatus = status;
            Quest quest = status.Quest;
            title.text = quest.Title;
            progress.text = $"{status.CompletedObjectivesCount}/{quest.ObjectivesCount}";
        }
    }
}
