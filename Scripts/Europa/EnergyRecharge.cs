using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class EnergyRecharge : MonoBehaviour
    {
        [Tooltip("How fast does it recharge")] public float chargeRate = 1;

        private void OnTriggerStay(Collider other)
        {
            //Try get stats
            Stats stats = other.transform.GetComponentInParent<Stats>();
            if (stats == null)
                stats = other.transform.root.GetComponentInChildren<Stats>();

            //Then recharge slowly
            if (stats != null)
            {
                stats.RechargeEnergy(chargeRate * Time.deltaTime);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Stats stats = other.transform.GetComponentInParent<Stats>();
            if (stats == null)
                stats = other.transform.root.GetComponentInChildren<Stats>();

            //Then recharge slowly
            if (stats != null)
            {
                stats.StopCharging();
            }
        }
    } 
}
