using UnityEngine;

public class FreecamController : MonoBehaviour
{
    private float _mouseX = 0, _mouseY = 0;
    private void Start() //set the starting angles to match transform
    {
        _mouseX = transform.eulerAngles.y;
        _mouseY = transform.eulerAngles.x;
        gameObject.SetActive(false);
    }

    private const float XTRA_SPEED = 10;
    void Update() 
    {
        //First as always gets mouse axises and changes the eulers
        _mouseX += Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat(SettingScript.SENSITIVITY, 1) * Time.deltaTime * 100;
        _mouseY -= Input.GetAxis("Mouse Y") * PlayerPrefs.GetFloat(SettingScript.SENSITIVITY, 1) * Time.deltaTime * 100;

        transform.eulerAngles = new Vector3(_mouseY, _mouseX);

        //Then get keyboard movement and move it
        float x = Input.GetAxis("Horizontal") * XTRA_SPEED * Time.deltaTime;
        float y = Input.GetAxis("Vertical") * XTRA_SPEED * Time.deltaTime;

        Vector3 dir = transform.forward * y;
        dir += transform.right * x;

        transform.position += dir;
    }
}
