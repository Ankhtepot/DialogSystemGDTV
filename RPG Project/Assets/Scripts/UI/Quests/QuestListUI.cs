using System;
using Control;
using Quests;
using UnityEngine;

namespace UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] private QuestItemUI questPrefab;
        private QuestList questList;

        private void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.onUpdate += Redraw;
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var status in questList.QuestStatuses)
            {
                Instantiate(questPrefab, transform).Setup(status);
            }
        }

        private void OnDisable()
        {
            if (questList)
            {
                questList.onUpdate -= Redraw;
            }
        }
    }
}
