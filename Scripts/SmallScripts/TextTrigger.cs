using System.Collections;
using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    public string[] textToWrite;
    private int _ind = 0;

    public bool saveThis = true; 
    [Tooltip("This be players ROOT the very tippy top")]public GameObject playerRoot;
    private bool already = false;

    public void Save()
    {
        PlayerPrefs.SetString(textToWrite[0] + MainMenu.CurrentSave, textToWrite[0]);
    }
    public void Load()
    {
        if(PlayerPrefs.HasKey(textToWrite[0] + MainMenu.CurrentSave))
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (already) return;
        already = true;

        if(saveThis)
        {
            TextTriggerManager.instance.triggersToSave.Add(this);
        }

        TextWriter.instance.WriteText(textToWrite[_ind], fade: false);
        _ind++;
        if (_ind >= textToWrite.Length)
        {
            Destroy(GetComponent<Collider>());
        }
        else
        {
            StartCoroutine(SmallDelay());
        }
        
        IEnumerator SmallDelay()
        {
            yield return new WaitForSeconds(3);
            already = false;
        }
    }

}
