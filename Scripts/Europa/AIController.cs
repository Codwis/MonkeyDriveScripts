using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Europa
{

    [RequireComponent(typeof(Rigidbody))]
    public class AIController : MonoBehaviour
    {
        [Tooltip("Where will the AI go to feed")] public Transform feedSpot;
        [Tooltip("Spots where the ai can wander to")] public Transform[] wanderSpots;
        [Tooltip("How long should the ai stay around one spot")] public float timePerSpot;
        [Tooltip("A child Object which will smoothly go where the ai will go and lead it")] public Transform spotToGo;

        private Rigidbody rb;
        private int currentSpot = 0; //-1 is feed

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            MoveLookSpot();
            MoveTowardsSpot();
            RotateTowardsSpot();
        }

        //On timed based change the spot to the next one and reset if its the last one
        private void NextSpot()
        {
            int old = currentSpot;

            while (currentSpot == old)
            {
                currentSpot = Random.Range(0, wanderSpots.Length);
            }

            randomNum = Random.Range(2, 10);
        }
        float randomNum = 4;

        //Moves the AI towards the spot
        private void MoveTowardsSpot()
        {
            if (currentSpot == -1)
            {
                //Add Feeding sequence
            }
            transform.position = Vector3.Lerp(transform.position, spotToGo.position, 1 * Time.deltaTime);
        }

        //Moves the AI Spot where the AI is going to
        private void MoveLookSpot()
        {
            spotToGo.position = Vector3.Lerp(spotToGo.position, wanderSpots[currentSpot].position, 0.6f * Time.deltaTime);

            if (Vector3.Distance(transform.position, wanderSpots[currentSpot].position) < randomNum)
            {
                NextSpot();
            }
        }

        //Rotates the AI towards the spot
        private void RotateTowardsSpot()
        {
            Vector3 lookRot = spotToGo.position - transform.position;
            Quaternion lookRotQuaternion = Quaternion.LookRotation(lookRot);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotQuaternion, 3 * Time.deltaTime);
        }
    }

}