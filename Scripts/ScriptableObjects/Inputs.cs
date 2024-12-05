using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inputs", menuName = "OnlyOneNeeded/Inputs")]
public class Inputs : ScriptableObject
{
    public SingleInput[] inputs;
}

[System.Serializable]
public class SingleInput
{
    [Tooltip("What is the name to get this input with like Forward for W")]public string inputName;
    [Tooltip("What is the key itself")] public KeyCode keyItself;
    public Sprite keyImage;
}
