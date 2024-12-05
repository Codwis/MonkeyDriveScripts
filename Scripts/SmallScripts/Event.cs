using System;
using System.Collections;
using UnityEngine;


namespace Codwis
{
    public class Event : MonoBehaviour
    {
        [Header("OPTIONAL COSMETIC")]
        [Tooltip("Which particles get played")] public ParticleSystem[] Particles;
        [Tooltip("Which audio will it play")] public AudioClipPreset AudioToPlay;
        [Tooltip("Gets destroyed when particles play")] public GameObject ObjectToRemove;

        [Header("MAIN")]
        [Tooltip("Which Transform should move LOCALLY")] public Transform TransformToMove;
        [Tooltip("How fast should the transform move")] public float TransformSpeed = 1;
        [Tooltip("The new spot LOCAL")] public Vector3 TransformsNewSpot;
        [Tooltip("The new rotation of the transform euler LOCAL")] public Vector3 TransformsNewRot;
        [Tooltip("Will this event auto save the game")] public bool SaveGame = true;

        [Header("Unlockables ONLY CHOOSE ONE PER EVENT")]
        [Tooltip("Which Abilitied does this unlock")] public Ability AbilityUnlock;
        [Tooltip("Which Unlockable does this unlock cosmetic mostly")] public Unlockable Unlock;

        public virtual void Invoke(Transform source)
        {
            if (SaveGame)
            {
                GameHandler.Save(MainMenu.CurrentSave, GameHandler.Player.transform, source.root.GetComponentInChildren<PlayerCollectables>(), source.root.GetComponentInChildren<PlayerUnlockables>());
            }
            CameraItself = Camera.main;
            CameraOriginPos = CameraItself.transform.position;
            CameraOriginRot = CameraItself.transform.rotation;
            
            StartCoroutine(MoveCamera());
        }

        private Action _actionToCall;
        public virtual void Invoke(Action callBack)
        {
            _actionToCall = callBack;
            CameraItself = Camera.main;
            StartCoroutine(MoveCamera());
        }


        public bool activated { get; protected set; } = false;
        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<HoldableObject>()) return;
            if (!other.transform.root.GetComponentInChildren<CarController_CW>()) return;

            if (activated) return;
            activated = true;

            Invoke(other.transform);
        }

        public virtual void Cleanup()
        {
            foreach (var parti in Particles) Destroy(parti);
            if (_actionToCall == null) Destroy(this);
        }

        [Header("CAMERA")]
        [Tooltip("Transform where the camera will go temporary")] public Transform CameraSpot;
        [Tooltip("How long does it take for camera to move lower is faster seconds")] public float MaxIndex = 3;

        [NonSerialized] public Vector3 CameraOriginPos;
        [NonSerialized] public Quaternion CameraOriginRot;
        [NonSerialized] public Camera CameraItself;
        private bool CarExists = true;
        private IEnumerator MoveCamera(bool back = false) //This just moves the camera
        {
            if(GameHandler.Player != null)
            {
                var temp = GameHandler.Player.GetComponentInChildren<PlayerUnlockables>();
                if (Unlock != null) temp.AddUnlockable(Unlock);
                if (AbilityUnlock != null) temp.AddAbility(AbilityUnlock);
            }

            if (CameraSpot == null) //if there is not spot then just skip
            {
                if (!back)
                {
                    Cosmetics();
                }
                else
                {
                    Done();
                    Cleanup();
                }

                StopCoroutine(MoveCamera());
                yield break;
            }

            //Stop Inputs from happening
            if (CameraItself.transform.root.GetComponentInChildren<CarController_CW>()) CarExists = true;
            else CarExists = false;

            if(CarExists)
            {
                CameraItself.transform.root.GetComponentInChildren<CameraController>().Allow = false;
                CameraItself.transform.root.GetComponentInChildren<CarController_CW>().Allow = false;
            }
            else
            {
                CameraItself.transform.root.GetComponentInChildren<MovementController>().Allow = false;
            }

            //Set starting points for the LERP 
            Vector3 startPoint = !back ? CameraOriginPos : CameraSpot.position;
            Quaternion startRot = !back ? CameraOriginRot : CameraSpot.rotation;

            float index = 0;
            while (index < MaxIndex) //Moves the camera
            {
                Move(back ? CameraOriginPos : CameraSpot.position, back ? CameraOriginRot : CameraSpot.rotation);
                yield return new WaitForEndOfFrame();
            }

            //if going back then allow inputs again and destroy the event
            if (back)
            {
                if(CarExists)
                {
                    CameraItself.transform.root.GetComponentInChildren<CameraController>().Allow = true;
                    CameraItself.transform.root.GetComponentInChildren<CarController_CW>().Allow = true;
                }
                else
                {
                    CameraItself.transform.root.GetComponentInChildren<MovementController>().Allow = true;
                }
                Cleanup();

                StopCoroutine(MoveCamera());
                yield break;
            }

            Cosmetics(); //Call Cosmetics

            void Move(Vector3 spot, Quaternion rot) //Small local method to move the camera and rotate it to given parameters
            {
                index += Time.deltaTime;
                CameraItself.transform.position = Vector3.Lerp(startPoint, spot, index / MaxIndex);
                CameraItself.transform.rotation = Quaternion.Lerp(startRot, rot, index / MaxIndex);
            }
        }

        private void Cosmetics() //Plays audio and particles
        {
            if (AudioToPlay.Clip != null) GameHandler.instance.PlayEffectAudio(AudioToPlay);
            if (Particles != null)
            {
                foreach (ParticleSystem part in Particles) part.Play();
            }

            if (ObjectToRemove != null) Destroy(ObjectToRemove);
            Enabling = true;
        }
        public virtual void Done(Transform other)
        {
            if(_actionToCall != null)
            {
                _actionToCall();
                _actionToCall = null;
                Cleanup();
            }
        }

        public virtual void Done()
        {
            if (_actionToCall != null)
            {
                _actionToCall();
                _actionToCall = null;
                Cleanup();
            }
        }


        #region TransformMovement
        private void Update()
        {
            CheckEnabling();
        }
        public bool Enabling { get; private set; } = false;
        //Method to check if the event is moving the object
        private void CheckEnabling()
        {
            if (Enabling)
            {
                if (TransformToMove != null) //Moves the transform to the new position and rotation
                {
                    if (TransformsNewSpot != Vector3.zero) //Move
                        TransformToMove.localPosition = Vector3.Lerp(TransformToMove.localPosition, TransformsNewSpot, Time.deltaTime * TransformSpeed);
                    if (TransformsNewRot != Vector3.zero) //Rotate
                        TransformToMove.localRotation = Quaternion.Lerp(TransformToMove.localRotation, Quaternion.Euler(TransformsNewRot), Time.deltaTime * TransformSpeed);

                    //When it goes close enough stop this and move camera back
                    if (Vector3.Distance(TransformToMove.localPosition, TransformsNewSpot) < 0.1f)
                    {
                        StartCoroutine(MoveCamera(true));

                        Enabling = false;
                    }
                }
            }
        }
        #endregion
    } 
}
