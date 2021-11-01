using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "IEnumerables Primer/Quest", order = 0)]
public class Quest : ScriptableObject
{
    [SerializeField] private string[] tasks;

    public IEnumerable<string> GetTasks()
    {
        foreach (var task in tasks)
        {
            yield return task;
        }
    }
}
