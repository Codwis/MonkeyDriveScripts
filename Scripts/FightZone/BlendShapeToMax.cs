using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlendShapeToMax : MonoBehaviour
{
    public float valToReach = 100;
    [Tooltip("take one number off because reasons")]public int sceneToGoTo;
    private SkinnedMeshRenderer _rend;
    private void Start()
    {
        _rend = GetComponent<SkinnedMeshRenderer>();
    }

    public IEnumerator Change(float secs)
    {
        float current = 0;
        while(current <= secs)
        {
            current += Time.deltaTime;
            _rend.SetBlendShapeWeight(0, current / secs);
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(sceneToGoTo + 1);
    }
}
