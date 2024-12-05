using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Codwis
{
	public class AbilityCheckEvent : Event
	{
        [Header("Check Event")]
        public GameObject AbilityNeededPreview;
        public Ability AbilityNeeded;

        public override void OnTriggerEnter(Collider other)
        {
            if (activated) return;

            if (CheckForAbility())
            {
                Destroy(AbilityNeededPreview);
                base.OnTriggerEnter(other);
            }
        }

        private bool CheckForAbility()
        {
            foreach (Ability un in UnlockablesHandler.instance.Abilities)
            {
                if (un.GetType() == AbilityNeeded.GetType())
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
