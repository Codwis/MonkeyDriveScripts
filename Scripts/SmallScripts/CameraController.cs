using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const string ROTATE = "Rotate";

    [Tooltip("The point which camera rotates around")] public Transform PivotPoint;

    [NonSerialized] public bool Allow = true;
    public CarController_CW _carController;

    private void Start()
    {
        _mouseX = PivotPoint.localEulerAngles.y;
        _mouseY = PivotPoint.localEulerAngles.x;

        if (!PlayerPrefs.HasKey("Sensitivity")) PlayerPrefs.SetFloat("Sensitivity", 1);

        if (_carController == null) _carController = transform.root.GetComponentInChildren<CarController_CW>();
        _originDistance = Vector3.Distance(PivotPoint.transform.position, transform.position);
        _originSpot = transform.localPosition;
    }



    private void Update()
    {
        if (!Allow) return;

        CheckForObjects();
        ApplyWobble();
        UpdateCamera();
    }

    private Quaternion _currentRotation;
    private float _mouseX = 0, _mouseY;
    private void UpdateCamera()
    {
        _mouseX += Input.GetAxis("Mouse X") * PlayerPrefs.GetFloat("Sensitivity") * Time.deltaTime * 100;
        _mouseY += Input.GetAxis("Mouse Y") * PlayerPrefs.GetFloat("Sensitivity") * Time.deltaTime * 100;
        _mouseY = Mathf.Clamp(_mouseY, -30, 30);

        if (!Input.GetKey(InputHandler.GetInput(ROTATE)))
        {
            _mouseX = Mathf.Lerp(_mouseX, Mathf.Clamp(_mouseX, -30, 30), Time.deltaTime * 3);
            _carController.allowTurning = true;
        }
        else
        {
            _carController.allowTurning = false;
        }

        // Calculate the target rotation
        Vector3 targetEulerAngles = _carController.transform.eulerAngles;
        targetEulerAngles.z = 0;
        targetEulerAngles.x = 0;
        targetEulerAngles += new Vector3(_mouseY, _mouseX, 0); // Use only Yaw and Pitch
        Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);

        // Smoothly interpolate the rotation using Slerp
        _currentRotation = Quaternion.Slerp(_currentRotation, targetRotation, Time.deltaTime * 5f);

        // Update the camera position
        PivotPoint.position = _carController.transform.position;

        // Apply the smoothed rotation
        PivotPoint.rotation = _currentRotation;
    }

    //Wobble Things
    private float _wobbleTime = 0f;
    private bool _wobbling = false;
    [Tooltip("Controls the strength of the wobble")] public float WobbleAmplitude = 0.1f;
    [Tooltip("Controls how fast the wobble occurs")] public float WobbleFrequency = 2f;

    private void ApplyWobble()
    {
        float velocityMagnitude = _carController.Rb.velocity.magnitude;

        // Check if velocity is above the threshold to apply wobble
        if (velocityMagnitude > 0.1f)
        {
            //If needed some changes or multiplier here
            float velocityIntensity = velocityMagnitude;

            if (!_wobbling)
            {
                _wobbling = true;
                _wobbleTime = 0f;
            }

            _wobbleTime += Time.deltaTime * velocityIntensity * WobbleFrequency;

            // Apply sinusoidal wobble to the camera's Y rotation
            _mouseY += Mathf.Sin(_wobbleTime) * WobbleAmplitude;
        }
        else
        {
            // Gradually reduce the wobble when below threshold
            _mouseY = Mathf.Lerp(_mouseY, 15, Time.deltaTime * WobbleFrequency);
            _wobbling = false;  // Stop wobbling when velocity is too low
        }

        if (_wobbleTime >= Mathf.PI * 2)
        {
            _wobbleTime = 0f;
        }
    }

    [Header("Checking For Objects")]
    [Tooltip("This should be everything except player, event, collectable")] public LayerMask MaskForCamera;
    private Vector3 _originSpot;
    private float _originDistance;
    private const float smallOffset = 0.9f;
    private void CheckForObjects()
    {
        // Ray from the player to the camera
        Ray ray = new Ray(_carController.transform.position, (transform.position - _carController.transform.position).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, _originDistance, MaskForCamera))
        {
            // If the ray hits an object move the camera closer to the hit point
            float hitDistance = Vector3.Distance(_carController.transform.position, hit.point);
            hitDistance = Mathf.Clamp(hitDistance, 0.1f, 10); //Clamp it so wont go -

            Vector3 dir = (transform.position - _carController.transform.position).normalized; //Get direction
            transform.position = _carController.transform.position + dir * hitDistance * smallOffset;  //Update position according to distance
        }
        else
        {
            //Otherwise just put it to original spot
            transform.localPosition = _originSpot;
        }
    }
}
