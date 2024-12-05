using UnityEngine;

public class BlockShield : MonoBehaviour
{
    public int maxHitsTillCD;
    public float coolDown;

    private int _currentHits = 0;
    public void TakeHit()
    {
        _currentHits++;
    }
}
