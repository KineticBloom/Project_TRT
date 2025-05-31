using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAuto : MonoBehaviour
{
    public CameraCore PlaysOnW;
    public CameraCore PlaysOnR;
    public CameraCore PlaysOnT;

    private enum CurrentPlay
    {
        W,T,R,NONE
    }

    CurrentPlay playCurrent;

    private void Start()
    {
        playCurrent = CurrentPlay.NONE;
        PlaysOnW.gameObject.SetActive(false);
        PlaysOnR.gameObject.SetActive(false);
        PlaysOnT.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (playCurrent != CurrentPlay.W)
            {
                playCurrent = CurrentPlay.W;
                Show(playCurrent);
                PlaysOnW.Setup();
            }
            else
            {
                PlaysOnW.TriggerCutscene();
                playCurrent = CurrentPlay.NONE;
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (playCurrent != CurrentPlay.R)
            {
                playCurrent = CurrentPlay.R;
                Show(playCurrent);
                PlaysOnR.Setup();
            }
            else
            {
                PlaysOnR.TriggerCutscene();
                playCurrent = CurrentPlay.NONE;
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (playCurrent != CurrentPlay.T)
            {
                playCurrent = CurrentPlay.T;
                Show(playCurrent);
                PlaysOnT.Setup();
            }
            else
            {
                PlaysOnT.TriggerCutscene();
                playCurrent = CurrentPlay.NONE;
            }
        }
    }

    private void Show(CurrentPlay mode)
    {
        switch (mode)
        {
            case CurrentPlay.W:
                PlaysOnW.gameObject.SetActive(true);
                PlaysOnR.gameObject.SetActive(false);
                PlaysOnT.gameObject.SetActive(false);
                break;
            case CurrentPlay.R:
                PlaysOnW.gameObject.SetActive(false);
                PlaysOnR.gameObject.SetActive(true);
                PlaysOnT.gameObject.SetActive(false);
                break;
            case CurrentPlay.T:
                PlaysOnW.gameObject.SetActive(false);
                PlaysOnR.gameObject.SetActive(false);
                PlaysOnT.gameObject.SetActive(true);
                break;
            case CurrentPlay.NONE:
                PlaysOnW.gameObject.SetActive(false);
                PlaysOnR.gameObject.SetActive(false);
                PlaysOnT.gameObject.SetActive(false);
                break;
        }
    }
}
