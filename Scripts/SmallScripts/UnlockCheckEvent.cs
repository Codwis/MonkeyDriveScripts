using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Codwis
{
    public class UnlockCheckEvent : Event
    {
        [Header("Check Event")]
        public GameObject UnlockableNeededPreview;
        public Unlockable UnlockableNeeded;

        public override void OnTriggerEnter(Collider other)
        {
            if (activated) return;

            if (CheckForUnlockable())
            {
                Destroy(UnlockableNeededPreview);
                base.OnTriggerEnter(other);
            }
        }

        private bool CheckForUnlockable()
        {
            foreach (Unlockable un in UnlockablesHandler.instance.Unlockables)
            {
                if (un == UnlockableNeeded)
                {
                    if (un.Active)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public override void Cleanup()
        {

        }
    } 
}
