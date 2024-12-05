using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class MovementController : MonoBehaviour
{
    [Header("Rig")]
    [Tooltip("Transform in the Rig rotates when mouse y is moved")]public Transform UpBack;
    [Tooltip("Head Transform in rig")]public Transform Head;

    [Header("Inverse kinematics")]
    [Tooltip("IK Transforms animation rigging")]public Transform LeftHandIK;
    [Tooltip("IK Transforms animation rigging")]public Transform RightHandIK;

    [Header("Misc")]
    [Tooltip("This is the spot where object will be held")]public Transform HoldSpot;
    [Tooltip("How fast does the player move")]public float MovementSpeed = 7;
    [Tooltip("The root pausemenu ui object")]public GameObject PauseMenu;
    [Tooltip("Do the renderers need to be disabled on start")]public bool HideRenderersBool = false;

    [Tooltip("Can the character move sideways with AD")]public bool MoveSideWays = false;
    [Tooltip("Custom spot where player will spawn at only for beating game")]public Transform customSpotOnBeat;

    public HoldableObject CurrentObject { get; private set; }
    public bool Allow { get; set; } = true;

    private Animator _animator;
    private CharacterController _controller;
    private Camera _camera;

    private LayerMask _selfMask;
    private Vector3 _leftOgPos, _rightOgPos;
    private Quaternion _leftOgRot, _rightOgRot;

    private float _mouseY;
    private float _mouseX;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();
        _selfMask = ~(1 << gameObject.layer);

        SetOg();
        if (HideRenderersBool)
        {
            StartCoroutine(HideRenderers()); 
        }

        if(customSpotOnBeat != null && AndroidCutscene.Beaten) //If beat the game spawn near the book and write out some text
        {
            transform.position = customSpotOnBeat.transform.position;

            if(BookColorChanger.AllGotten)
            {
                TextWriter.instance.WriteText("WO OO AH!!! You Got All The Shiny Bananas?!?! COOL", fade: false);
            }
            else
            {
                TextWriter.instance.WriteText("Hey! You Are Back!", fade: false);
            }
        }
    }
    

    private void SetOg() //Saves the og spot and rotation of the hands
    {
        _leftOgPos = LeftHandIK.localPosition;
        _leftOgRot = LeftHandIK.localRotation;

        _rightOgPos = RightHandIK.localPosition;
        _rightOgRot = RightHandIK.localRotation;
    }
    private IEnumerator HideRenderers() //Hides renderers if needed
    {
        Allow = false;
        yield return new WaitForSeconds(0.5f);
        foreach(var t in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            t.enabled = false;
        }
        Allow = true;
    }

    private void Update()
    {
        if (!Allow)
        {
            _animator.SetFloat("_AxisY", 0);
            _animator.SetFloat("_MouseX", 0);
            _animator.SetBool("_Allow", false);
            return; 
        }
        else _animator.SetBool("_Allow", true);

        GravityCheck();
        CheckForInputs();
    }

    private void GravityCheck() //Moves down if player is in air
    {
        _controller.Move(_controller.isGrounded ? Vector3.zero : Vector3.down * 9.41f * Time.deltaTime);
    }

    private const string INTERACT = "Interact";
    private void CheckForInputs() //Checks for inputs 
    {
        if(Input.GetKeyDown(KeyCode.Escape) && PauseMenu != null)
        {
            PauseMenu.GetComponent<SideDreamPauseMenu>().ShowMenu(!PauseMenu.activeSelf);
        }
        if(Input.GetKeyDown(InputHandler.GetInput(INTERACT)))
        {
            Interact();
        }
        if(Input.GetMouseButtonDown(0))
        {
            UseItem();
        }
    }

    private const float MAX_DISTANCE = 5;
    private void Interact() //Tries to interact with an object
    {
        
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, MAX_DISTANCE)) //Else simple raycast check
        {
            if (hit.collider.TryGetComponent<Interactable>(out var interactable))
            {
                if (interactable is HoldableObject && CurrentObject != null) Drop();
                else if (interactable is ShelfScript && CurrentObject != null) Drop();
                else interactable.Interact(transform);
            }
            else
            {
                interactable = hit.collider.GetComponentInParent<Interactable>();
                if(interactable != null)
                {
                    if (interactable is HoldableObject && CurrentObject != null) Drop();
                    else if (interactable is ShelfScript && CurrentObject != null) Drop();
                    else interactable.Interact(transform);

                }
                else if (CurrentObject != null) Drop();
            }
        }
        else
        {
            if (CurrentObject != null) Drop();
        }
    }

    private void UseItem() //Uses an item in hand
    {
        if (CurrentObject == null) return;

        CurrentObject.Use();
    }

    public void Pickup(HoldableObject obj) //Picks up an object with this
    {
        CurrentObject = obj;
        CurrentObject.Pickup(ref RightHandIK, ref LeftHandIK, HoldSpot);
    }

    public void Drop() //Drops an object with this
    {
        if (CurrentObject == null) return;

        CurrentObject.Drop();
        CurrentObject = null;

        //Reset hands
        RightHandIK.SetLocalPositionAndRotation(_rightOgPos, _rightOgRot);
        LeftHandIK.SetLocalPositionAndRotation(_leftOgPos, _leftOgRot);
    }

    #region LOCOMOTION
    private void LateUpdate()
    {
        if (!Allow) return;

        MouseMove();
        Move();
    }
    private void MouseMove() //Moves the player according to mouse movement
    {
        //Mouse Y
        float f = Input.GetAxis("Mouse Y");
        _mouseY += f * Time.deltaTime * PlayerPrefs.GetFloat(SettingScript.SENSITIVITY) * 100;
        _mouseY = Mathf.Clamp(_mouseY, -100, 50);
        float head = _mouseY > 20 ? _mouseY : -20;
        
        UpBack.localEulerAngles = new Vector3(head > 20 ? 20 : _mouseY, 0);

        head = _mouseY > 20 ? _mouseY - 40 : -20;
        Head.localEulerAngles = new Vector3(head, 0);

        //Mouse X
        if(_animator.applyRootMotion)
        {
            _mouseX = Mathf.Lerp(_mouseX, 0, Time.deltaTime);
            _mouseX += Input.GetAxis("Mouse X") * Time.deltaTime;
            _mouseX = Mathf.Clamp(_mouseX, -1, 1);
            _animator.SetFloat("_MouseX", _mouseX);
        }
        else
        {
            _mouseX += Input.GetAxis("Mouse X") * Time.deltaTime * PlayerPrefs.GetFloat(SettingScript.SENSITIVITY) * 100;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _mouseX);
        }
    }


    private void Move() //Moves the player
    {
        float y = Input.GetAxis("Vertical");
        _animator.SetFloat("_AxisY", y);
        _controller.Move(transform.forward * y * Time.deltaTime * MovementSpeed);

        if(MoveSideWays)
        {
            float x = Input.GetAxis("Horizontal");
            _animator.SetFloat("_AxisX", x);
            _controller.Move(transform.right * x * Time.deltaTime * MovementSpeed);
        }
        
    }
    #endregion
}
