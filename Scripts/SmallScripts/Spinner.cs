using UnityEngine;

public class Spinner : MonoBehaviour
{
    [Tooltip("How much should it spin")]public Vector3 SpinAmount;
    [Tooltip("How much should it move multiplies the sin value which is -1 to 1 Default 0.01f")]public float UpAmplitude = 0.01f;
    [Tooltip("How fast does it go up and down Default 10")]public float UpFrequency = 10;
    public bool UpDown = true;
    private float _upTime = 0;
    private void FixedUpdate()
    {
        transform.localEulerAngles += SpinAmount * Time.deltaTime; //Rotate the object

        if(UpDown)
        {
            //Then UpDown motion using Sine pattern
            _upTime += Time.deltaTime * UpFrequency; //Adds up time so the sine goes forward
            //Direct calculate the sine to the position instead variable
            transform.localPosition += new Vector3(0, Mathf.Sin(_upTime) * UpAmplitude);
        }
    }
}
