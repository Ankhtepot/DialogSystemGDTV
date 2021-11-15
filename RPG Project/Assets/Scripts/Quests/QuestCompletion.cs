using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] private Quest quest;
        [SerializeField] private string objective;

        public void CompleteObjective()
        {
            Debug.Log($"Objective {objective} completed.");
            FindObjectOfType<QuestList>().CompleteObjective(quest, objective);
        }
    }
}
