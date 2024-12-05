using UnityEngine;

public class SwingEndCall : MonoBehaviour
{
    private SwingItem _itemToCall;
    private void Start()
    {
        _itemToCall = GetComponentInParent<SwingItem>();
    }

    public void Call()
    {
        _itemToCall.AllowSwing();
    }
}
