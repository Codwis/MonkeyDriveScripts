using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCheck : MonoBehaviour
{
    private MovementController _main;
    private void Start()
    {
        _main = GetComponentInParent<MovementController>();
    }
}
