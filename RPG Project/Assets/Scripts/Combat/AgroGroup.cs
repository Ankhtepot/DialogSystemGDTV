using System;
using UnityEngine;

namespace Combat
{
    public class AgroGroup : MonoBehaviour
    {
        [SerializeField] private Fighter[] fighters;
        [SerializeField] private bool activateOnStart;

        private void Start()
        {
            Activate(activateOnStart);
        }

        public void Activate(bool shouldActivate)
        {
            foreach (var fighter in fighters)
            {
                fighter.GetComponent<CombatTarget>().enabled = shouldActivate;
                fighter.enabled = shouldActivate;
            }
        }
    }
}
