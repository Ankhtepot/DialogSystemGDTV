using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private Quest quest;
    void Start()
    {
        foreach (var task in quest.GetTasks())
        {
            Debug.Log($"Has task: {task}");
        }
    }
}
