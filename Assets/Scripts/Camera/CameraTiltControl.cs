using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTiltControl : MonoBehaviour
{
    [SerializeField] private float tiltFactor = 10f;
    [SerializeField] private float lerpSpeed = 5f;
    [SerializeField] AnimationCurve pointerTiltCurve;
    [SerializeField] private float pointerEdgeThreshold = 0.1f;

    public static Vector3 Tilt = Vector3.zero;
    private Vector3 _targetRotTilt = Vector3.zero;


    // Update is called once per frame
    void Update()
    {
        // Controller Camera Controls
        Vector3 input = DetermineInput();

        UpdateRotTilt(input);
    }


    private Vector2 DetermineInput()
    {
        Vector3 axis = GameManager.PlayerInput.GetViewAxis();
        Vector2 input = new(axis.x, axis.z);

        if (GameManager.PlayerInput.LastUsedScheme == GameManager.PlayerInput.KeyboardScheme)
        {
            // input = EdgeScreenInputMethod();
            // input = EdgeCurveMethod();

            Vector2 pointerPos = GameManager.PlayerInput.GetPointerPosition();
            Vector2 res = new(Screen.width, Screen.height);

            float strength =
                Mathf.Pow(pointerPos.x - res.x / 2f, 2)
                / Mathf.Pow(res.x / 2f * (1f - pointerEdgeThreshold), 2)
                + Mathf.Pow(pointerPos.y - res.y / 2f, 2)
                / Mathf.Pow(res.y / 2f * (1f - pointerEdgeThreshold), 2);

            Vector2 direction = (pointerPos - res / 2f).normalized;
            input = Vector2.ClampMagnitude(pointerTiltCurve.Evaluate(strength) * direction, 1f);
        }

        return input;
    }


    private void UpdateRotTilt(Vector2 input)
    {
        float aspectRatio = (float)Screen.currentResolution.width / Screen.currentResolution.height;
        Vector2 maxRotTilt = new Vector2(aspectRatio, 1) * tiltFactor;

        _targetRotTilt = new(input.y * -maxRotTilt.y, input.x * maxRotTilt.x, 0);
        Tilt = Vector3.Lerp(Tilt, _targetRotTilt, Time.deltaTime * lerpSpeed);
    }


    private Vector2 EdgeCurveMethod()
    {
        Vector2 pointerPos = GameManager.PlayerInput.GetPointerPosition();
        Vector2 res = new(Screen.width, Screen.height);

        Vector2 screenToInput = new(
            pointerTiltCurve.Evaluate(pointerPos.x / res.x),
            pointerTiltCurve.Evaluate(pointerPos.y / res.y));

        return Vector2.ClampMagnitude(screenToInput, 1f);
    }


    private Vector2 EdgeScreenInputMethod()
    {
        Vector2 pointerPos = GameManager.PlayerInput.GetPointerPosition();
        Vector2 res = new(Screen.width, Screen.height);

        // https://www.desmos.com/calculator/lihaowp3sz
        var t = 1f / pointerEdgeThreshold;
        var x = Mathf.Clamp01(t * (Mathf.Abs(pointerPos.x / res.x - 0.5f) - 0.5f) + 1);
        var y = Mathf.Clamp01(t * (Mathf.Abs(pointerPos.y / res.y - 0.5f) - 0.5f) + 1);
        x *= pointerPos.x > res.x / 2 ? 1 : -1;
        y *= pointerPos.y > res.y / 2 ? 1 : -1;

        Debug.Log(pointerPos);
        Debug.Log(res);
        return Vector2.ClampMagnitude(new Vector2(x, y), 1f);
    }


    void OnDisable()
    {
        Tilt = Vector3.zero;
    }
}
