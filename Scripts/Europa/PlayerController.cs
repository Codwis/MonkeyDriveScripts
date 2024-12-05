using System;
using System.Collections;
using UnityEngine;

namespace Europa
{
    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Physics")]
        [NonSerialized] public Rigidbody rigidBody;

        [Header("Camera")]
        public Transform head;
        private Animator animator;
        public SideDreamPauseMenu PauseMenu;

        [Header("Light Related")]
        [Tooltip("Delay before new light can be charged")] public int lightDelay = 5;
        [Tooltip("How long can the player charge up")] public float maxCharge = 10;
        [Tooltip("Light prefab which the player will shoot")] public GameObject lightPrefab;
        [Tooltip("Where does the light spawn")] public Transform lightSpawnSpot;
        public float movSpeed = 500;

        private const string LIGHT = "Light";
        private const string ROTATE = "Rotate";

        private const int CHARGERATE = 5;
        private const int ROTATESPEED = 200;
        private const int LIGHTMOVESPEED = 5;
        private bool allowLight = true;

        private delegate void MyDelegate(float newAmount);
        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            headZOG = head.transform.localEulerAngles.z;
            MyDelegate my = ChangeEuropaSpeed;
            IngameDebugConsole.DebugLogConsole.AddCommand("ChangeEuropaSpeed", "Changes movement speed in Europa", my, "newSpeed default ~10000");
        }
        
        public void ChangeEuropaSpeed(float newSpeed = 10000)
        {
            movSpeed = newSpeed;
        }

        bool edgeReached = false;
        bool positiveEdge;
        private float animX = 0;
        private float headX = 0;
        private float headZOG;
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                PauseMenu.ShowMenu(!PauseMenu.gameObject.activeSelf);
            }

            if(Input.GetKey(InputHandler.GetInput(ROTATE)))
            {
                float x = Input.GetAxis("Mouse X");
                rigidBody.AddTorque(-transform.forward * x * Time.deltaTime * 10, ForceMode.Impulse);
                return;
            }

            if (animator != null)
            {
                //If left mouse is pressed 
                if (Input.GetMouseButton(0))
                {
                    //Adds to the animator the horizontal axis num
                    animX += Input.GetAxis("Mouse X") * Time.deltaTime;
                    animX = Mathf.Clamp(animX, -1, 1);

                    //If the edge was reached last check then
                    if (edgeReached)
                    {

                        //See if the mouse has been moved to opposite direction to prevent just holding down
                        if (positiveEdge)
                        {
                            if (animX < 1)
                                edgeReached = false;
                        }
                        else
                        {
                            if (animX > -1)
                            {
                                edgeReached = false;
                            }
                        }
                    }

                    //If edge was disabled then act normally
                    if (!edgeReached)
                    {
                        //Only move if its actually moved a little
                        float temp = Input.GetAxis("Mouse X");
                        if (temp > 0.1f || temp < -0.1f)
                        {
                            if (temp < 0)
                            {
                                temp = -temp;
                            }

                            //If edge of the animator was reached then stop movement from adding that side until opposite direction is achieved
                            if (animX == 1 || animX == -1)
                            {
                                edgeReached = true;
                                switch (animX)
                                {
                                    case 1:
                                        positiveEdge = true;
                                        break;
                                    case -1:
                                        positiveEdge = false;
                                        break;
                                }
                            }

                            temp = Mathf.Clamp(temp, 0, 2);
                            rigidBody.AddForce(transform.forward * Time.deltaTime * movSpeed * temp, ForceMode.Acceleration);
                            rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, MaxVelocity);
                        }

                    }

                }
                else
                {
                    //if button is not pressed only slightly move it
                    headX -= Input.GetAxis("Mouse X");
                    headX = Mathf.Clamp(headX, -45, 45);

                    Vector3 spot = new Vector3(head.localEulerAngles.x, head.localEulerAngles.y, headZOG + headX);
                    head.localEulerAngles = Vector3.Lerp(head.localEulerAngles, spot, 1 * Time.deltaTime);
                }

                rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, Time.deltaTime);
                animX = Mathf.Lerp(animX, 0, Time.deltaTime);
                animator.SetFloat("MouseX", animX);
            }

            //If light is allowed
            if (allowLight)
            {
                //And key is pressed
                if (Input.GetKeyDown(InputHandler.GetInput(LIGHT)))
                {
                    //Start Charge
                    allowLight = false;
                    StartCoroutine(Charging());
                }
            }
        }

        //Used in Line 102
        private const float MaxVelocity = 3.25f;

        private void LateUpdate()
        {
            float mouseXC = Input.GetAxis("Mouse X");
            float mouseYC = Input.GetAxis("Mouse Y");

            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                //Just add torque from the right axis to turn it smoothly       
                rigidBody.AddTorque(transform.up * mouseXC * Time.deltaTime * ROTATESPEED);
                rigidBody.AddTorque(transform.right * -mouseYC * Time.deltaTime * ROTATESPEED);
            }
            rigidBody.angularVelocity = Vector3.Lerp(rigidBody.angularVelocity, Vector3.zero, Time.deltaTime);


            //Rotate Forward Axis if unstraight
            if (Input.GetMouseButton(1))
            {
                rigidBody.AddTorque(transform.forward * -mouseXC * Time.deltaTime * ROTATESPEED);
            }
        }

        public float lightNormalRange = 10;
        //Charging Function
        private IEnumerator Charging()
        {
            //Reset charge and create object
            float currentCharge = 0;
            GameObject temp = Instantiate(lightPrefab, lightSpawnSpot.position, lightSpawnSpot.rotation, head);

            float[] defaultRanges = SetDefaultRanges();
            float[] SetDefaultRanges()
            {
                float[] tempRanges = new float[temp.GetComponentsInChildren<Light>().Length];
                int i = 0;
                foreach (var item in temp.GetComponentsInChildren<Light>())
                {
                    tempRanges[i] = item.range;
                    i++;
                }
                return tempRanges;
            }


            //While the key is down 
            while (Input.GetKey(InputHandler.GetInput(LIGHT)))
            {
                //Just adds charge and increases the size of the light object and range
                if (currentCharge < maxCharge)
                {
                    int i = 0;
                    foreach (Light li in temp.GetComponentsInChildren<Light>())
                    {
                        li.range = defaultRanges[i] * currentCharge / 100;
                        i++;
                    }
                    temp.transform.localScale = new Vector3(currentCharge / 100, currentCharge / 100, currentCharge / 100);

                    currentCharge += CHARGERATE * Time.deltaTime;
                }

                yield return new WaitForEndOfFrame();
            }

            //Sets the variables to the destroy timer and enables it
            temp.GetComponent<DestroyTimer>().secondsTillDestroy *= (currentCharge / maxCharge);
            temp.GetComponent<DestroyTimer>().enabled = true;
            temp.GetComponent<DestroyTimer>().movDirection = lightSpawnSpot.forward * LIGHTMOVESPEED;

            StartCoroutine(WaitLight());
        }

        //Resetter to allow new light
        private IEnumerator WaitLight()
        {
            yield return new WaitForSeconds(lightDelay);
            allowLight = true;
        }



    }
}

