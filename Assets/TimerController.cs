using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TimerController : MonoBehaviour
{
    public Image OuterRing;
    public Image InnerRing;

    public Image PauseorPlay;
    public Sprite Pause;
    public Sprite Play;

    public Vector3 PosDuringPause;
    public Vector3 PosDuringPlay;
    public GameObject PauseCanvas;

    public Image Cell5;
    public Image Cell4;
    public Image Cell3;
    public Image Cell2;
    public Image Cell1;

    bool TriggerPulse = false;

    private void Start()
    {
        OuterRing.transform.DORotate(new Vector3(0, 0, -360), 8f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);

        InnerRing.transform.DORotate(new Vector3(0, 0, 360), 7f, RotateMode.FastBeyond360)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart);
    }


    private void Update()
    {
        if (TimeLoopManager.LoopPaused && PauseCanvas.activeInHierarchy)
        {
            PauseorPlay.sprite = Pause;
            GetComponent<RectTransform>().anchoredPosition = PosDuringPause;
        }
        else
        {
            PauseorPlay.sprite = Play;
            GetComponent<RectTransform>().anchoredPosition = PosDuringPlay;
        }

        if (TimeLoopManager.Instance != null)
        {

            float TotalSeconds = TimeLoopManager.StartTime * 60;

            float SecondsElapsed = Mathf.Max(0, TotalSeconds - TimeLoopManager.SecondsLeft);

            Debug.Log("Seconds elapsed: " + SecondsElapsed + " Total Seconds: " + TotalSeconds);

            if(SecondsElapsed > TotalSeconds / 5)
            {
                Cell5.gameObject.SetActive(false);
            }

            if (SecondsElapsed > (TotalSeconds / 5) * 2)
            {
                Cell4.gameObject.SetActive(false);
            }

            if (SecondsElapsed > (TotalSeconds / 5) * 3)
            {
                Cell3.gameObject.SetActive(false);
            }

            if (SecondsElapsed > (TotalSeconds / 5) * 4)
            {
                Cell2.gameObject.SetActive(false);
            }

            if (SecondsElapsed > (TotalSeconds / 5) * 4.5)
            {
                if (TriggerPulse == false)
                {
                    Color targetColor = Cell1.color;
                    targetColor.a = 0f;

                    Cell1.DOColor(targetColor, 1f)
                         .SetLoops(-1, LoopType.Yoyo)
                         .SetEase(Ease.Linear);
                    TriggerPulse = true;
                }
            }

            if (SecondsElapsed > (TotalSeconds / 5) * 5)
            {
                Cell1.gameObject.SetActive(false);
            }
        }
    }

}
