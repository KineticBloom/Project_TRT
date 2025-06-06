using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Debug_LoopWipeUI : MonoBehaviour
{
    [SerializeField] private GameObject segfaultCanvas;

    private void Start()
    {
        TimeLoopManager.LoopElapsed += AnimateWipe;
    }

    private void OnDestroy()
    {
        TimeLoopManager.LoopElapsed -= AnimateWipe;
    }

    private void AnimateWipe(System.Action callback)
    {
        if (callback == null) {
            Debug.LogError("Debug_LoopWipe Error: AnimateWipe failed. callback was null.");
            return;
        }

        StartCoroutine(AnimateWipeRoutine(callback));
    }

    private IEnumerator AnimateWipeRoutine(System.Action callback)
    {
        segfaultCanvas.SetActive(true);
        yield return new WaitForSeconds(3f);
        callback();
    }
}