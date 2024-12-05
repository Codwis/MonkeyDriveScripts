using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Codwis
{
	public class EndEvent : MonoBehaviour
	{
        public Ability abilityToUnlock;
        public string textToSay = "";
        public int sceneToGoTo;
        public void Invoke(Transform source)
        {
            var type = abilityToUnlock.GetType();
            GameHandler.abilityToUnlockOnLoad = type;
            TextWriter.instance.WriteText(textToSay, true, sceneToGo: sceneToGoTo, fadeFromStart: true);
        }
    } 
}
