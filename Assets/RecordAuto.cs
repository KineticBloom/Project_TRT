using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAuto : MonoBehaviour
{
    public CameraCore PlaysOnY;
    public CameraCore PlaysOnR;
    public CameraCore PlaysOnT;

    private enum CurrentPlay
    {
        Y,T,R,NONE
    }

    CurrentPlay playCurrent;

    private void Start()
    {
        playCurrent = CurrentPlay.NONE;
        PlaysOnY.gameObject.SetActive(false);
        PlaysOnR.gameObject.SetActive(false);
        PlaysOnT.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (playCurrent != CurrentPlay.Y)
            {
                playCurrent = CurrentPlay.Y;
                Show(playCurrent);
                PlaysOnY.Setup();
            }
            else
            {
                PlaysOnY.TriggerCutscene();
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playCurrent = CurrentPlay.NONE;
            Show(CurrentPlay.NONE);
        }
    }

    private void Show(CurrentPlay mode)
    {
        switch (mode)
        {
            case CurrentPlay.Y:
                PlaysOnY.gameObject.SetActive(true);
                PlaysOnR.gameObject.SetActive(false);
                PlaysOnT.gameObject.SetActive(false);
                break;
            case CurrentPlay.R:
                PlaysOnY.gameObject.SetActive(false);
                PlaysOnR.gameObject.SetActive(true);
                PlaysOnT.gameObject.SetActive(false);
                break;
            case CurrentPlay.T:
                PlaysOnY.gameObject.SetActive(false);
                PlaysOnR.gameObject.SetActive(false);
                PlaysOnT.gameObject.SetActive(true);
                break;
            case CurrentPlay.NONE:
                PlaysOnY.gameObject.SetActive(false);
                PlaysOnR.gameObject.SetActive(false);
                PlaysOnT.gameObject.SetActive(false);
                break;
        }
    }
}
