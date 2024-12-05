using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class CarController_CW : MonoBehaviour
{
    private const string HOP = "Hop";
    private const string WIGGLE_L = "Wiggle_L";
    private const string WIGGLE_R = "Wiggle_R";
    private const string BOOST = "Boost";
    private const string SAVE = "Save";
    private const string LOAD = "Load";
    private const string BUCKET = "Bucket";
    private const string SCAN = "Scan";

    [Header("Physics")]
    [Tooltip("Which pull type is it front or back or both")] public DriveSystem DriveSystem;
    [Tooltip("How much torque is added to wheels -default 150")] public float TorqueAmount = 100;
    [Tooltip("How fast does it apply breaks")] public float BrakingTorque = 50;
    [Tooltip("How much do the wheels turn -default 25")] public float TurnAngle = 25;
    [Tooltip("How much strenght is there in wiggling")] public float wiggleStrenght = 30;

    public Rigidbody Rb { get; private set; }
    public bool Allow { get; set; } = true;

    [Header("Tires/Wheels")]
    [Tooltip("The wheel colliders")] public WheelCollider Fl_WheelCol;
    [Tooltip("The wheel colliders")] public WheelCollider Fr_WheelCol, Bl_WheelCol, Br_WheelCol;
    [Tooltip("The wheels themselfs")]public Transform Fl_Wheel, Fr_Wheel, Bl_Wheel, Br_Wheel;

    [Header("Cosmetical")]
    [Tooltip("Transform for the steering wheel")]public Transform SteeringWheel;
    [Tooltip("The thing that points in speedmeter")]public Transform SpeedMeterTick;
    [Tooltip("Particles in wheels")]public ParticleSystem[] WheelParticles;
    [Header("Gas Pedal")]
    [Tooltip("The Ik Target of the right foot")]public Transform FootIkTarget;
    [Tooltip("The skinned renderer of the pedals on car")]public SkinnedMeshRenderer Pedals;
    [Tooltip("How much displacement should gas down have added to local pos")]public Vector3 displacementOnGas;

    [Header("Audio")]
    public AudioSource MainAudio;
    public AudioSource HonkSound;
    public AudioClipPreset VehicleHonkSound;
    public AudioSource VehicleCrashSound;

    private Vector3 _steeringWheelOg = new Vector3(-35, 0, 0);
    private Vector3 _footIkOg;
    private void Awake()
    {
        _ogTorque = TorqueAmount;
        Rb = GetComponent<Rigidbody>();
        _footIkOg = FootIkTarget.localPosition;

        if (VehicleCrashSound != null) _crashOgVolume = VehicleCrashSound.volume;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        if (!Allow) //If For example event being active stop movement and dont allow any further
        {
            Rb.velocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
            if(_rockets != null)_rockets.StopLoop();
            if(_boosts != null) _boosts.StopLoop();
            MainAudio.volume = 0;

            foreach (var item in WheelParticles)
            {
                item.Stop();
            }

            return;
        }
        AddInputs();
        CheckForAdditionalInputs();
        UpdateWheels();
        ChangeMeterTick();
    }


    public bool allowTurning = true;
    private float _x;
    private void AddInputs() //Adds inputs to the car 
    {
        if(allowTurning)
        {
            //Turn wheels
            _x += Input.GetAxis("Mouse X") * Time.deltaTime * 2;
            _x = Mathf.Clamp(_x, -1, 1);

            Fl_WheelCol.steerAngle = _x * TurnAngle;
            Fr_WheelCol.steerAngle = _x * TurnAngle;

            ChangeSteeringWheelRot(_x);
        }

        float y = -Input.GetAxis("Vertical");
        ChangeFootPedal(y);
        MainAudio.volume = Mathf.Abs(y / 2.5f);
        switch (DriveSystem) // Depending on drive system adds torque to wheels
        {
            case DriveSystem.Front:
                AddTorque(Fl_WheelCol, y * TorqueAmount);
                AddTorque(Fr_WheelCol, y * TorqueAmount);
                break;
            case DriveSystem.Back:
                AddTorque(Bl_WheelCol, y * TorqueAmount);
                AddTorque(Br_WheelCol, y * TorqueAmount);
                break;
            case DriveSystem.Both:
                AddTorque(Fl_WheelCol, y * TorqueAmount);
                AddTorque(Fr_WheelCol, y * TorqueAmount);
                AddTorque(Bl_WheelCol, y * TorqueAmount);
                AddTorque(Br_WheelCol, y * TorqueAmount);
                break;
        }
    }

    private void ChangeFootPedal(float y) //Small thing to make the pedal go down and foot with it for that +1
    {
        y = MathF.Abs(y);
        Pedals.SetBlendShapeWeight(1, y * 100);

        FootIkTarget.localPosition = Vector3.Lerp(_footIkOg, _footIkOg + displacementOnGas, y);
    }
    private void ChangeSteeringWheelRot(float x)
    {
        if (SteeringWheel != null) // Turn the steering wheel May need to be changed
        {
            Vector3 steer = (x > 0) ? new Vector3(-35, 0, -220) : new Vector3(-35, 0, 220);
            SteeringWheel.localRotation = Quaternion.Lerp(Quaternion.Euler(_steeringWheelOg), Quaternion.Euler(steer), Mathf.Abs(x));
        }
    }
    private void ChangeMeterTick() //Changes the speed meter tick with speed
    {
        if (SpeedMeterTick == null) return;

        Vector3 rot = new Vector3(0, Rb.velocity.magnitude * 2);
        SpeedMeterTick.localEulerAngles = -rot;
    }

    private void AddTorque(WheelCollider col, float amount) //Just simple function to add specified amount of torque to specified wheel
    {
        col.motorTorque = amount;

        foreach (var particle in WheelParticles) //Particle activation
        {
            //Get emission module
            var emission = particle.emission; 
            float emissionAmount;

            //Math
            emissionAmount = -amount > 0 ? -amount : 10;                                       ///If input is higher than 0 set it as amount
            emissionAmount *= Rb.velocity.magnitude > 0.1f ? Rb.velocity.magnitude : 1;        ///Multiply with velocity if there is velocity
            emissionAmount = -amount > 0 || Rb.velocity.magnitude > 0.1f ? emissionAmount : 0; ///If there is no input and not enough velocity set it to 0
            emissionAmount = Mathf.Clamp(emissionAmount, 0, 50);                               ///Then limit it
            
            //Set new value
            emission.rateOverTimeMultiplier = emissionAmount;
            
            //If the particle systems arent playing then set them to play
            if (!particle.isPlaying) particle.Play();
        }
    }

    //Hop
    public float HopMulti { get; private set; } = 1000;
    private readonly float _hopDelay = 1.5f;
    private bool _allowHop = true;

    //Rockets
    private RocketJump _rockets = null;
    private bool _rocketsInitiated = false;
    private float _maxRocketHold = 2;
    private float _startTime = 0;

    //Boosts
    private RocketBoost _boosts = null;
    private readonly float _boostExtra = 10;
    private float _ogTorque;
    private float _currentFuel;
    private bool _allowBoost = true;
    private const float REFUEL_TIME = 6;

    //Scanner
    private ScannerAbility _scanner = null;
    private bool _allowScan = true;

    //SmallAbilities
    private FloatAbility _floatAbility = null;
    private Bucket _bucket = null;

    //Misc
    private bool _allowHonk = true;
    private void CheckForAdditionalInputs() //Checks for additional keys like brake or engine
    {
        //Hopping with Rocket jump
        Hop();
        void Hop()
        {
            if (!_allowHop) return;
            if (_rockets != null) //So timer always resets so in air cant stack the time
            {
                if (Input.GetKeyUp(InputHandler.GetInput(_rockets.InputName)))
                {
                    _rocketsInitiated = false;
                }
            }
            if (!CheckOnGround() && !_rocketsInitiated)
            {
                _rocketsInitiated = false;
                if (_rockets != null) _rockets.GreyOutUi(false);
                if (_rockets != null) _rockets.StopLoop();
                return; 
            }

            if (_rockets != null) _rockets.GreyOutUi(true);

            if (_rockets != null) //If there are rockets then allow holding down HOP for a big jump
            {
                if(Input.GetKey(InputHandler.GetInput(_rockets.InputName)) && !_rocketsInitiated) //First set the time
                {
                    _rockets.Charge(true);
                    _rocketsInitiated = true;
                    _startTime = Time.time;
                }
                else if(Input.GetKey(InputHandler.GetInput(_rockets.InputName)) && _rocketsInitiated)
                {
                    if(Time.time - _startTime >= _maxRocketHold) //check if the time goes above the max
                    {
                        FinalHop(_maxRocketHold + 1 * 10, true); //add one so it doesnt demultiply and extra power
                    }
                }
                else if(Input.GetKeyUp(InputHandler.GetInput(_rockets.InputName))) //if let go it will jump with given time and no extra
                {
                    _rockets.CleanUp();
                    FinalHop(Time.time - _startTime + 1); //add one so it doesnt demultiply so it cant be 0-1
                }
            }
            else if(Input.GetKey(InputHandler.GetInput(HOP)))
            {
                FinalHop(1); //otherwise no extra boost at all
            }

            void FinalHop(float multiplier, bool blast = false) //This just does the jump
            {
                _rocketsInitiated = false;
                if (blast)
                {
                    _rockets.Use();
                }
                if(_rockets != null) _rockets.StopLoop();

                Rb.AddForce(Vector3.up * HopMulti * multiplier, ForceMode.Impulse);

                _allowHop = false;
                StartCoroutine(AllowHop());
            }

            IEnumerator AllowHop()
            {
                if (_rockets != null) _rockets.GreyOutUi(false);
                yield return new WaitForSeconds(_hopDelay);
                _allowHop = true;
            }
        }

        //Boosting ability
        Boosts();
        void Boosts()
        {
            if (!_allowBoost) return; //Dont allow boosts if out of fuel

            if (Input.GetKeyDown(InputHandler.GetInput(BOOST)) && _boosts != null) //Start the boost loop on first press
            {
                StopCoroutine(StartRefuel());
                _boosts.UseLoop();
            }
            if (Input.GetKey(InputHandler.GetInput(BOOST)) && _boosts != null) //Main force applier and fuel remover
            {
                if (_currentFuel <= 0) //If out of fuel stop the boosts and set timer on
                {
                    _boosts.StopLoop();
                    StartCoroutine(AllowBoosts());
                    return;
                }

                //Otherwise remove fuel and apply force
                _currentFuel -= REFUEL_TIME * Time.deltaTime;
                Rb.AddForce(-transform.forward * 1000 * Time.deltaTime * _boostExtra, ForceMode.Impulse);
            }
            else if (Input.GetKeyUp(InputHandler.GetInput(BOOST)) && _boosts != null) //If key is let up stop boosts
            {
                StartCoroutine(StartRefuel());
                _boosts.StopLoop();
            }

            IEnumerator AllowBoosts() //Timer to allow boosts again after delay
            {
                _allowBoost = false;
                _boosts.GreyOutUi(false);

                yield return new WaitForSeconds(REFUEL_TIME);

                _currentFuel = _boosts.MaxFuel;
                _allowBoost = true;
                _boosts.GreyOutUi(true);
            }

            IEnumerator StartRefuel() //Reset fuel if let go of the key
            {
                yield return new WaitForSeconds(REFUEL_TIME);
                _currentFuel = _boosts.MaxFuel;
            }
        }

        //Bucket Ability
        Bucket();
        void Bucket()
        {
            if(_bucket != null)
            {
                if(Input.GetKeyDown(InputHandler.GetInput(BUCKET)))
                {
                    _bucket.Use();
                }
            }
        }

        //Scan Ability
        Scan();
        void Scan()
        {
            if (!_allowScan || _scanner == null) return;

            if (Input.GetKeyDown(InputHandler.GetInput(SCAN)))
            {
                _scanner.Use();
                StartCoroutine(ScannerReset());
            }

            IEnumerator ScannerReset()
            {
                _scanner.GreyOutUi(false);
                _allowScan = false;
                yield return new WaitForSeconds(_scanner.cooldown);
                _allowScan = true;
                _scanner.GreyOutUi(true);
            }
        }

        //Honking sound
        Honk();
        void Honk()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0) && _allowHonk)
            {
                if(VehicleHonkSound != null)
                {
                    HonkSound.volume = VehicleHonkSound.Volume;
                    HonkSound.PlayOneShot(VehicleHonkSound.Clip);
                    StartCoroutine(AllowHonk());
                }
            }

            IEnumerator AllowHonk()
            {
                _allowHonk = false;
                yield return new WaitForSeconds(VehicleHonkSound.Clip.length);
                _allowHonk = true;
            }
        }

        //Quick saving and loading
        SaveLoad();
        void SaveLoad()
        {
            if(Input.GetKeyDown(InputHandler.GetInput(SAVE)))
            {
                GameHandler.Save(MainMenu.CurrentSave, transform,
                    transform.root.GetComponentInChildren<PlayerCollectables>(), transform.root.GetComponentInChildren<PlayerUnlockables>());
            }
            else if(Input.GetKeyDown(InputHandler.GetInput(LOAD)))
            {
                GameHandler.Load();
            }
        }

        if(Input.GetMouseButton(1) && BrakesOn)
        {
            Fl_WheelCol.brakeTorque = BrakingTorque;
            Fr_WheelCol.brakeTorque = BrakingTorque;
            Bl_WheelCol.brakeTorque = BrakingTorque;
            Br_WheelCol.brakeTorque = BrakingTorque;
        }
        else
        {
            Fl_WheelCol.brakeTorque = 0;
            Fr_WheelCol.brakeTorque = 0;
            Bl_WheelCol.brakeTorque = 0;
            Br_WheelCol.brakeTorque = 0;
        }

        //Wiggling by adding torque
        if (Input.GetKey(InputHandler.GetInput(WIGGLE_L)))
        {
            Rb.AddTorque(-transform.forward * wiggleStrenght * Time.deltaTime, ForceMode.Impulse);
        }
        if(Input.GetKey(InputHandler.GetInput(WIGGLE_R)))
        {
            Rb.AddTorque(transform.forward * wiggleStrenght * Time.deltaTime, ForceMode.Impulse);
        }
    }
    public bool BrakesOn = false;

    public bool CheckOnGround() //Just checks if any of the wheels is touching the ground
    {
        if (Bl_WheelCol.isGrounded) return true;
        if (Br_WheelCol.isGrounded) return true;
        if (Fl_WheelCol.isGrounded) return true;
        if (Fr_WheelCol.isGrounded) return true;

        return false;
    }

    public void EnableAbility(Ability abilityToEnable) //This enables given ability
    {
        switch(abilityToEnable)
        {
            case RocketJump jump:
                EnableRockets(jump);
                break;
            case RocketBoost boosts:
                EnableRocketBoost(boosts);
                break;
            case FloatAbility flo:
                EnableFloating(flo);
                break;
            case Bucket buck:
                EnableBucket(buck);
                break;
            case ScannerAbility scanner:
                EnableScanner(scanner);
                break;
        }

        void EnableRockets(RocketJump rockets) //Sets the rockets
        {
            _rockets = rockets;
        }
        void EnableRocketBoost(RocketBoost boost)
        {
            _currentFuel = boost.MaxFuel;
            _boosts = boost;
        }
        void EnableFloating(FloatAbility fl)
        {
            _floatAbility = fl;
        }
        void EnableBucket(Bucket bucket)
        {
            _bucket = bucket;
        }
        void EnableScanner(ScannerAbility scanner)
        {
            _scanner = scanner;
        }
    }
    public bool FloatAbilityActive()
    {
        return _floatAbility;
    }
    public void HideAbilityUi()
    {
        if (_bucket != null) _bucket.HideUi();
    }

    private void UpdateWheels() //Updates the rotation and postion of the wheel transform to the one in collider
    {
        Vector3 pos;
        Quaternion rot;

        //Front wheel
        Fl_WheelCol.GetWorldPose(out pos, out rot);
        Fl_Wheel.SetPositionAndRotation(pos, rot);
        Fr_WheelCol.GetWorldPose(out pos, out rot);
        Fr_Wheel.SetPositionAndRotation(pos, rot);

        //Back wheels
        Bl_WheelCol.GetWorldPose(out pos, out rot);
        Bl_Wheel.SetPositionAndRotation(pos, rot);
        Br_WheelCol.GetWorldPose(out pos, out rot);
        Br_Wheel.SetPositionAndRotation(pos, rot);
    }

    private bool _collisionSound = true;
    private float _crashOgVolume;
    private void OnCollisionEnter(Collision collision)
    {
        if (!_collisionSound || VehicleCrashSound == null) return;

        float newVol = _crashOgVolume * collision.relativeVelocity.magnitude / 6;
        newVol = Mathf.Clamp(newVol, 0, 0.15f);
        VehicleCrashSound.volume = newVol;
        VehicleCrashSound.Play();

        StartCoroutine(smallDelay());
        IEnumerator smallDelay()
        {
            _collisionSound = false;
            yield return new WaitForSeconds(0.25f);
            _collisionSound = true;
        }
    }
}
public enum DriveSystem { Front, Back, Both }
