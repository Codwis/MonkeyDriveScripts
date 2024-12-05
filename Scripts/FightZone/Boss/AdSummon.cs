using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdSummon : MonoBehaviour
{
    public GameObject adToSummon;

    public void Summon(Vector3 position)
    {
        var te = Instantiate(adToSummon, position, adToSummon.transform.rotation);
        if(action != null) te.GetComponent<AIStats>().actionToCallOnDeath = action;
        if (_list != null) _list.Add(te);
        if (_turret != null) _turret.occupied = te;

        Destroy(gameObject);
    }

    private Vector3 positionToLand;
    private Vector3 startPos;
    public void SetTargetPos(Vector3 pos)
    {
        positionToLand = pos;
        startPos = transform.position;
    }
    private List<GameObject> _list;
    private Action action;
    private TurretSpawnPoint _turret;
    public void ListToAdd(ref List<GameObject> list, Action toCall)
    {
        action = toCall;
        _list = list;
    }
    public void ListToAdd(ref List<GameObject> list)
    {
        _list = list;
    }
    public void ListToAdd(ref List<GameObject> list, TurretSpawnPoint turret)
    {
        _list = list;
        _turret = turret;
    }



    public float flySpeed = 5;
    private float _currentFloat = 0;
    private void Update()
    {
        if(positionToLand != null)
        {
            _currentFloat += Time.deltaTime * flySpeed;
            transform.position = Vector3.Slerp(startPos, positionToLand, _currentFloat);

            if(Vector3.Distance(transform.position, positionToLand) < 1)
            {
                Summon(transform.position);
            }
        }
    }
}
