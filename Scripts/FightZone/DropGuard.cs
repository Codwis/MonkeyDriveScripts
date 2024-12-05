using UnityEngine;

public class DropGuard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var t = other.transform.root.GetComponentInChildren<FightStats>();
        if (t != null) t.Die();
    }
}
