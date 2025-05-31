using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using DG.Tweening;

public class CameraCore : MonoBehaviour
{
    public bool UseCameraRotationAnim = false;
    public Vector3 StartCameraRotation;
    public Vector3 EndCameraRotation;
    public float RotationTime;
    public float RotationDelay;
    public Ease EaseMode;

    private PlayableDirector playableDirector;
    private CinemachineVirtualCamera cam;

    private void Start()
    {
        playableDirector = GetComponentInChildren<PlayableDirector>();
        cam = playableDirector.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void Setup()
    {
        cam.gameObject.transform.DOKill();
        StopAllCoroutines();
        playableDirector.Stop();
        if (UseCameraRotationAnim)
        {
            cam.gameObject.transform.localEulerAngles = StartCameraRotation;
        }
        cam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0;
    }

    public void TriggerCutscene()
    {
        if (UseCameraRotationAnim)
        {
            cam.gameObject.transform.localEulerAngles = StartCameraRotation;
            if (RotationDelay <= 0)
            {
                cam.gameObject.transform.DOLocalRotate(EndCameraRotation, RotationTime, RotateMode.Fast).SetEase(EaseMode);
            }
            else
            {
                StartCoroutine(Rotate());
            }
        }
        playableDirector.Play();
    }

    IEnumerator Rotate()
    {
        yield return new WaitForSeconds(RotationDelay);
        cam.gameObject.transform.DOLocalRotate(EndCameraRotation, RotationTime, RotateMode.Fast).SetEase(EaseMode);

    }
}
