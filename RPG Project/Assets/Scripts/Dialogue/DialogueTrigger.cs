using UnityEngine;
using UnityEngine.Events;

namespace Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private EDialogueActions action;
        [SerializeField] private UnityEvent onTrigger;

        public void Trigger(EDialogueActions actionToTrigger)
        {
            if (actionToTrigger == action)
            {
                onTrigger?.Invoke();
            }
        }
    }
}
