using UnityEngine;
using TMPro;

public class TutorialSignWriter : MonoBehaviour
{
    [Header("Input")]
    public string textItself;
    public string inputName;

    [Header("Input Showcases")]
    public TMP_Text textBox;
    public SpriteRenderer buttonImage;

    private void Start()
    {
        var t = textItself.Split();

        textBox.text = "";
        for(int i = 0; i < t.Length; i++)
        {
            textBox.text += t[i];
            textBox.text += (i < t.Length - 1) ? "\n" : "";
        }
        buttonImage.sprite = InputHandler.GetInputSprite(inputName);
        Destroy(this);
    }
}
