using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class Stats : MonoBehaviour
    {
        [Tooltip("How much energy does this have")] public float maxEnergy;
        [NonSerialized] public float currentEnergy;
        [Tooltip("Tries to get them from children but may have to be put in manually")] public SkinnedMeshRenderer[] meshRenderers;


        private void Start()
        {
            meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        }

        //Recharges the energy and doesnt charge if full
        public void RechargeEnergy(float amount)
        {
            on = true;
            if (currentEnergy < maxEnergy) return;

            currentEnergy += amount;
        }

        //Stops the charging
        public void StopCharging()
        {
            on = false;
        }

        private float chargeCount;
        private bool on;
        private void FixedUpdate()
        {
            //Changes the blend shape to puff up when gathering energy and closing when not
            if (meshRenderers != null)
            {
                if (on)
                {

                    chargeCount += 50 * Time.deltaTime;
                    foreach (SkinnedMeshRenderer rend in meshRenderers)
                    {
                        rend.SetBlendShapeWeight(0, chargeCount);
                    }
                }
                else
                {
                    chargeCount -= 20 * Time.deltaTime;
                    foreach (SkinnedMeshRenderer rend in meshRenderers)
                    {
                        rend.SetBlendShapeWeight(0, chargeCount);
                    }
                }
                chargeCount = Mathf.Clamp(chargeCount, 0, 100);
            }
        }
    } 
}
