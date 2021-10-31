using UnityEngine;
using UnityEngine.Playables;

namespace Cinematics

{
    public class CinematicTrigger : MonoBehaviour
    {
        bool alreadyTriggered = false;
        
        private void OnTriggerEnter(Collider other) 
        {
            if(!alreadyTriggered && other.gameObject.CompareTag("Player"))
            {
                alreadyTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
        }
    }
}