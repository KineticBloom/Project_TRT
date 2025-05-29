using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateRevealLeaveButton : MonoBehaviour
{
    public Button leaveButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(SetActive());
    }

    private void OnDisable()
    {
        leaveButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SetActive()
    {
        yield return new WaitForSeconds(1f);

        leaveButton.interactable = true;
    }
}
