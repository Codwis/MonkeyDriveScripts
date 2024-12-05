using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationScript : MonoBehaviour
{
    #region Singleton
    public static CustomizationScript instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Main")]
    [Tooltip("The handy colorpicker on canvas")]public FlexibleColorPicker colorPicker;
    [Tooltip("This is where the materials will be taken from")]public MeshRenderer rendererToTakeMatsFrom;

    [Header("Setup")]
    [Tooltip("Where does the each mat ui parent to GRID ui object")]public Transform MatUiParent;
    [Tooltip("Prefab for grid object for each material")]public GameObject MatUiPrefab;
    public List<CustomizationUiPieceSetup> Pieces { get; private set; } = new List<CustomizationUiPieceSetup>();

    private void Start() //Setups the ui pieces
    {
        ///Set start variables
        Material[] mats = rendererToTakeMatsFrom.materials;
        int i = 1;

        foreach (var item in mats) ///Goes through all the materials
        {    
            ///Makes new object and gets the setup and sets it up with given variables
            var temp = Instantiate(MatUiPrefab, MatUiParent);
            var cust = temp.GetComponent<CustomizationUiPieceSetup>();
            cust.Setup(item, "Colour: ", i, this);
            Pieces.Add(cust);

            i++;
        }
        gameObject.SetActive(false); ///Hides the ui after its done setting up
    }

    public void HideColorPicker() //This hides the color picker addon
    {
        _currentMat = null;
        _currentImage = null;
        colorPicker.gameObject.SetActive(false);
        colorPicker.onColorChange.RemoveListener(OnChange);
    }

    private Material _currentMat;
    private Image _currentImage;
    public void ShowColorPicker(ref Material target, ref Image image) //This shows the colorpicker and sets target to the given material and image
    {
        _currentImage = image;
        _currentMat = target;
        colorPicker.SetColor(target.color);

        colorPicker.gameObject.SetActive(true);
        colorPicker.onColorChange.RemoveListener(OnChange);
        colorPicker.onColorChange.AddListener(OnChange);
    }

    public void OnChange(Color newColor) //When Color changes change the color of the mat and image
    {
        _currentImage.color = newColor;
        _currentMat.color = newColor;
    }

    [Header("Scale")]
    [Tooltip("Just the textbox which shows the currentscale")]public TMP_Text scaleText;
    private float _scaleTarget;

    public void OnSizeChange(float newSize) //When scale slider changes
    {
        //Set the text
        newSize = System.MathF.Round(newSize, 2);
        _scaleTarget = newSize;
        scaleText.text = "Current Scale: " + newSize.ToString();

        //Restart the coroutine
        StopCoroutine(ScaleChanger());
        StartCoroutine(ScaleChanger());
    }

    //Scaling
    private const float SCALE_CHANGE_SPEED = 2f;
    private const int MAX_INDEX = 6;
    private const float PIVOT_MULTI = 1;
    public static bool CanClose { get; private set; } = true;
    public IEnumerator ScaleChanger()
    {
        //Set start variables
        float index = 0;

        LayerMask selfMask = ~(1 << gameObject.layer);
        Vector3 startScale = transform.root.localScale;

        Transform player = GameHandler.Player.transform;
        Vector3 startPos = player.position;

        //Disable player movement
        DisablePlayer(true);

        while (index < MAX_INDEX)
        {
            //First scale the player up or down fixed amount at time
            index += Time.deltaTime * SCALE_CHANGE_SPEED;
            Vector3 scale = Vector3.Lerp(startScale, new Vector3(_scaleTarget, _scaleTarget, _scaleTarget), index / MAX_INDEX);
            transform.root.localScale = scale;

            //Adjust position from the START position  -important!
            if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit, 50, selfMask))
            {
                float adjustedOffset = transform.root.localScale.y;
                Vector3 pos = new Vector3(startPos.x, hit.point.y + PIVOT_MULTI * adjustedOffset, startPos.z);
                player.position = pos;
            }

            yield return new WaitForEndOfFrame();
        }

        DisablePlayer(false);

        static void DisablePlayer(bool on) //Disables or Enables the players movement and rotates the player right way
        {
            GameHandler.Player.Allow = !on;

            var old = GameHandler.Player.transform.localEulerAngles;
            GameHandler.Player.transform.localEulerAngles = new Vector3(old.x, old.y, 0);

            CanClose = !on;
        }
    }

}

[System.Serializable]
public class ColorFormat //Used in saving
{
    public float G;
    public float B;
    public float R;
    public ColorFormat(Color col)
    {
        R = col.r;
        G = col.g;
        B = col.b;
    }
}
