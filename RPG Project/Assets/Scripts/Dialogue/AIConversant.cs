using Control;
using UnityEngine;

namespace Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] private string conversantName;
        [SerializeField] private Dialogue dialogue;
        private static PlayerConversant playerConversant;

        public string ConversantName => conversantName;
        
        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!dialogue) return false;
            if (!Input.GetMouseButtonDown(0)) return true;
            
            if (!playerConversant)
            {
                playerConversant = callingController.GetComponent<PlayerConversant>();
            }
                
            playerConversant.StartDialogue(this, dialogue);
            return true;
        }
    }
}
