using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator), typeof(FightStats), typeof(CharacterController))]
public class FightMovementController : MonoBehaviour
{
    public Animator cloneAnimator;
    public GameObject shieldObject;
    [NonSerialized] public FightStats _stats;
    private Animator _animator;
    private CharacterController _controller;

    //Animator Consts
    private const string AXIS_X = "_AxisX";
    private const string AXIS_Y = "_AxisY";
    private const string MOUSE_X = "_MouseX";
    //other
    private const string CROUCH = "Crouch";

    private readonly Vector3 blockSpot = new Vector3(0.01975f, -0.00781f, -0.00027f);
    private void Start()
    {
        _stats = GetComponent<FightStats>();
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        IngameDebugConsole.DebugLogConsole.AddCommand("Die", "Kills the player", DieCommand);
    }

    public bool Allow { get; set; } = true;
    private void Update()
    {
        HandsOnPlace();
        if (!Allow)
        {
            _animator.SetFloat(AXIS_X, 0);
            _animator.SetFloat(AXIS_Y, 0);

            cloneAnimator.SetFloat(AXIS_X, 0);
            cloneAnimator.SetFloat(AXIS_Y, 0);
            return;
        }

        Move();
        ExtraInputs();
        GravityCheck();
    }
    private void LateUpdate()
    {
        MouseMove();
    }

    public void TakeDamage()
    {
        _animator.SetTrigger("Hit");
        cloneAnimator.SetTrigger("Hit");
    }

    //Move
    private void Move() //This just moves the player
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //_controller.Move(transform.forward * y * _stats.movSpeed * Time.deltaTime);
        //_controller.Move(transform.right * x * _stats.movSpeed * Time.deltaTime);
        _animator.SetFloat(AXIS_X, x);
        _animator.SetFloat(AXIS_Y, y);

        cloneAnimator.SetFloat(AXIS_X, x);
        cloneAnimator.SetFloat(AXIS_Y, y);
    }

    private void GravityCheck() //Checks if player is in air and if is then move down
    {
        if(!_controller.isGrounded)
        {
            _controller.Move(Vector3.down * 9.8f * Time.deltaTime);
        }
    }

    //Mouse and Aim
    [Header("AIM")]
    [Tooltip("This is the spot where weapon will aim")]public Transform aimSpot;
    [Tooltip("Min or Max amount aimspot can move locally seriously small number")] public float minX, maxX, minY, maxY;
    [NonSerialized] public float _currentX = 0, _currentY = 0;

    [Header("Mouse")]
    [Tooltip("This aims the head")]public MultiAimConstraint headAim;
    [Tooltip("Head transform")]public Transform head;

    private float _mouseX = 0, _mouseY = 0;
    private void MouseMove() //Moves the player according to mouse movement
    {
        float y = Input.GetAxis("Mouse Y");
        float x = Input.GetAxis("Mouse X");

        if (!Input.GetMouseButton(0))
        {
            //Reset weight
            headAim.weight = Mathf.Lerp(headAim.weight, 0, Time.deltaTime * 2);

            // Mouse Y
            _mouseY += y * Time.deltaTime * PlayerPrefs.GetFloat(SettingScript.SENSITIVITY) * 100;
            _mouseY = Mathf.Clamp(_mouseY, -70, 50);

            head.localEulerAngles = new Vector3(_mouseY, head.localEulerAngles.y, head.localEulerAngles.z);

            // Mouse X
            _mouseX += x * Time.deltaTime * PlayerPrefs.GetFloat(SettingScript.SENSITIVITY) * 100;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _mouseX);

            cloneAnimator.transform.eulerAngles = new Vector3(transform.eulerAngles.x, cloneAnimator.transform.eulerAngles.y + x * Time.deltaTime * 5);
        }
        else
        {
            //Resets the _mousey smoothly and set head weight
            _mouseY = Mathf.Lerp(_mouseY, 0, Time.deltaTime * 2);
            headAim.weight = Mathf.Lerp(headAim.weight, 1, Time.deltaTime);

            //Get axises
            y = Input.GetAxisRaw("Mouse Y");
            x = Input.GetAxisRaw("Mouse X");

            //Set axises for aimspot
            _currentX -= x * maxX * Time.deltaTime * PlayerPrefs.GetFloat(SettingScript.SENSITIVITY);
            _currentY += y * maxY * Time.deltaTime * PlayerPrefs.GetFloat(SettingScript.SENSITIVITY);

            //Clamp
            _currentX = Mathf.Clamp(_currentX, minX, maxX);
            _currentY = Mathf.Clamp(_currentY, minY, maxY);

            //Sets the aimspot
            Vector3 point = new Vector3(_currentX, 0, _currentY);

            aimSpot.localPosition = Vector3.Lerp(aimSpot.localPosition, point, Time.deltaTime * 3);

            // Mouse X
            _mouseX += x * Time.deltaTime * PlayerPrefs.GetFloat(SettingScript.SENSITIVITY) * 20;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _mouseX);
        }

        _animator.SetFloat(MOUSE_X, Input.GetAxis("Mouse X"));
        cloneAnimator.SetFloat(MOUSE_X, Input.GetAxis("Mouse X"));
    }

    [Header("Inverse Kinematics")]
    [Tooltip("The lefthand IK target")] public Transform leftHand_IK;
    [Tooltip("The lefthand IK target")] public Transform rightHand_IK;
    [Tooltip("The hold spots for the weapon")] public Transform staff_Left, staff_Right;
    private void HandsOnPlace() //simple method to put ik targets to weapon holding spots
    {
        leftHand_IK.SetPositionAndRotation(staff_Left.position, staff_Left.rotation);
        rightHand_IK.SetPositionAndRotation(staff_Right.position, staff_Right.rotation);
    }

    [Header("Misc")]
    //Extra Inputs
    [Tooltip("The pausemenu for this scene")]public GameObject pauseMenu;
    private void ExtraInputs() //Checks for extra inputs
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //Escape just opens pausemenu or closes it
        {
            if (pauseMenu != null) pauseMenu.GetComponent<SideDreamPauseMenu>().ShowMenu(!pauseMenu.activeSelf);
            else TextWriter.instance.WriteText("There is no escape from here", fade: false);
        }

        if(Input.GetKey(InputHandler.GetInput(CROUCH)))
        {
            _animator.SetBool(CROUCH, true);
        }
        else
        {
            _animator.SetBool(CROUCH, false);
        }
    }

    public string deathText;
    public void OnDeath()
    {
        FinalPhase.DeathCount++;
        _animator.enabled = false;
        TextWriter.instance.WriteText(deathText, sceneToGo: SceneManager.GetActiveScene().buildIndex - 1, fadeFromStart: true);
        this.enabled = false;
    }

    public void DieCommand()
    {
        _stats.Die();
    }
}
