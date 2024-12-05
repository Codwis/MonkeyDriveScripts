using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
public class TextWriter : MonoBehaviour
{
    #region Singleton
    public static TextWriter instance;
    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }
    #endregion

    [Tooltip("This will be used mainly for the text writing")]public TMP_Text textDisplay;
    [Tooltip("This is the object which says any key to continue")]public GameObject AnyKeyObject;

    private TMP_Text _currentText;

    [Header("Audio")]
    [Tooltip("This will be played each word")]public AudioClip audioPer;
    [Tooltip("This is where its played from")]public AudioSource source;

    [NonSerialized] public Animator animator;

    private void Start()
    {
        if (textDisplay == null) Debug.LogError("NO TEXTDISPLAY in " + gameObject.name);
        if (source == null) Debug.LogError("NO audio source in " + gameObject.name);

        animator = GetComponentInChildren<Animator>(true);
    }
    void Tttets()
    {
        WriteText("It Can Get A Little Scary");
    }

    
    private void Update()
    {
        if(Input.anyKeyDown) //Speeds up the text if any key is pressed
        {
            if(_currentTarget != null)
            {
                index++;
                WriteNext();
            }
            else if(AnyKeyObject.activeSelf)
            {
                //Here put scene transition for later
                if(_fade)
                {
                    animator.SetBool("Fade", true);
                }
                if(SceneToGo > -1)
                {
                    MainMenu.NewGame = false;
                    SceneManager.LoadScene(SceneToGo + 1);
                }

                _currentText.text = null;
                ToCallWhenDone?.Invoke();
                AnyKeyObject.SetActive(false);
            }
        }
    }


    public IEnumerator Spam(int scene, AsyncOperation op) //Used once at End little spam to add spooky
    {
        float t = 0;
        animator.SetBool("Fade", true);
        while (t < 1f)
        {
            t += Time.deltaTime;
            if(t > 0.5f) op.allowSceneActivation = true;

            _currentText.text += " IT " + " HURTS ";
            source.pitch = UnityEngine.Random.Range(0.1f, 2.0f);
            source.PlayOneShot(audioPer);

            _currentText.text += " IT " + " HURTS ";
            source.pitch = UnityEngine.Random.Range(0.1f, 2.0f);
            source.PlayOneShot(audioPer);

            _currentText.text += " IT " + " HURTS ";
            source.pitch = UnityEngine.Random.Range(0.1f, 2.0f);
            source.PlayOneShot(audioPer);

            yield return new WaitForEndOfFrame();
        }

        
    }

    private bool _fade = false;
    private string[] _currentTarget = null;
    private int SceneToGo = -1;
    private Action ToCallWhenDone = null;
    public void WriteText(string textToWrite, bool fade = true, TMP_Text customTextBox = null, int sceneToGo = -1, bool fadeFromStart = false, Action callBack = null) //Main global function
    {
        //Set the text box and empty it
        if (customTextBox != null) _currentText = customTextBox;
        else _currentText = textDisplay;

        _currentText.text = "";

        if (sceneToGo != -1) SceneToGo = sceneToGo;
        ToCallWhenDone = callBack ?? null;

        _fade = fade;
        if (fade) animator.SetBool("Fade", fadeFromStart);
        //Shows the hide thing splits the text start the coroutine
        if(textToWrite.Contains("%NB%"))
        {
            textToWrite = textToWrite.Replace("%NB%", PlayerPrefs.GetInt(PlayerCollectables.BANANA_COUNT + MainMenu.CurrentSave, 0).ToString());
            if (PlayerPrefs.GetInt(PlayerCollectables.BANANA_COUNT + MainMenu.CurrentSave, 0) >= PlayerPrefs.GetInt(CollectableHandler.BANANA_TOTAL, 0))
            {
                textToWrite = @"Why did You Collect Every Banana In The Game You Ask? ""Well Done Btw""";
            }
            else if(PlayerPrefs.GetInt(PlayerCollectables.BANANA_COUNT + MainMenu.CurrentSave, 0) <= 0)
            {
                textToWrite = "Why You Collected 0 BANANAS?!? How did you Even Manage To Dodge all of Them?! but";
            }
            else if (PlayerPrefs.GetInt(PlayerCollectables.BANANA_COUNT + MainMenu.CurrentSave, 0) <= 12)
            {
                textToWrite = @"Why did you Collect the Bare Mininum amount of bananas ""Thats impressive"" you ask?";
            }
        }

        _currentTarget = textToWrite.Split(" ");
        StartCoroutine(Writer());
    }


    private int index = 0;
    private IEnumerator Writer() //Main writer timer
    {
        index = 0;
        while(_currentTarget != null || index < _currentTarget.Length)
        {
            WriteNext();
            yield return new WaitForSeconds(0.30f);
            index++;
        }
    }

    private void WriteNext() //Main Writer function which writes the next text
    {
        if (index > _currentTarget.Length - 1)
        {
            AnyKeyObject.SetActive(true);
            StopAllCoroutines();
            _currentTarget = null;
            return;
        }

        _currentText.text += " " + _currentTarget[index];
        source.pitch = UnityEngine.Random.Range(0.5f, 1.5f);
        source.PlayOneShot(audioPer);
    }
}
