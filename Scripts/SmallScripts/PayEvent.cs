using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Codwis
{
    public class PayEvent : Event
    {
        [Header("Pay Event Related")]
        [Tooltip("Which Collectable is needed use unreachable Reference object")] public Collectable CollectableNeeded;
        [Tooltip("How many of the given collectables are needed")] public int AmountNeeded = 1;
        [Header("Text")]
        [Tooltip("This will be the preview objects text scale")] public Vector3 PreviewTextScale = new Vector3(0.01f, 0.01f, 0.01f);
        [Tooltip("This will be text preview objects local position leave zero for normal")] public Vector3 PreviewTextPadding = Vector3.zero;
        [Header("Preview")]
        [Tooltip("This will be the Preview objects localposition")] public Vector3 PreviewOffset = new Vector3(0, 0.2f, 0);
        [Tooltip("The preview objects LOCAL scale")] public Vector3 PreviewObjectScale = new Vector3(0.5f, 0.5f, 0.5f);

        private GameObject preview;
        private TextMeshPro text;

        private void Awake()
        {
            SetupPreview();
            GetComponent<Collider>().isTrigger = true;
        }
        private void SetupPreview() //Setup a preview to showcase how much is needed
        {
            text = GetComponentInChildren<TextMeshPro>();
            if (text == null) //If there is no textmesh in child then create one and set the values
            {
                text = new GameObject().AddComponent<TextMeshPro>();
                text.transform.SetParent(transform);
                text.transform.localPosition = PreviewOffset;
                text.fontSize = 70;
                text.transform.localScale = PreviewTextScale;
            }
            text.outlineWidth = 0.2f;
            text.color = Color.cyan;
            text.outlineColor = Color.black;
            text.text = AmountNeeded.ToString();

            //Create the preview object the collectable needed
            preview = Instantiate(CollectableNeeded.gameObject, transform);
            Destroy(preview.GetComponent<Collectable>()); //remove so it cant be picked up

            //Set the preview transform
            preview.transform.position = text.transform.position;
            preview.transform.localScale = PreviewObjectScale;
            text.transform.SetParent(preview.transform);

            if (PreviewTextPadding != Vector3.zero)
            {
                text.GetComponent<RectTransform>().localPosition = PreviewTextPadding;
            }
        }

        public override void Cleanup() //Cleanup preview object
        {
            Destroy(preview);
            Destroy(text);
        }
        public override void OnTriggerEnter(Collider other)
        {
            if (!CheckCollectables(other)) return;
            base.OnTriggerEnter(other);
        }

        private bool CheckCollectables(Collider other) //Checks if the player has enough of the given collectable
        {
#if UNITY_EDITOR
            return true;
#else
        var temp = other.transform.root.GetComponentInChildren<PlayerCollectables>();
        return temp.GetAmount(CollectableNeeded) >= AmountNeeded;
#endif
        }
    } 
}
