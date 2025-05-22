using UnityEngine;
using Cinemachine.Utility;
using Cinemachine;

/// <summary>
/// Custom add-on module for Cinemachine Virtual Camera that adds a final rotational offset to the camera
/// Modified version of CinemachineCameraOffset
/// </summary>

[AddComponentMenu("")] // Hide in menu
[ExecuteAlways]
[SaveDuringPlay]
public class CinemachineCameraTilt : CinemachineExtension
{
    /// <summary>
    /// Applies the specified offset to the camera state
    /// </summary>
    /// <param name="vcam">The virtual camera being processed</param>
    /// <param name="stage">The current pipeline stage</param>
    /// <param name="state">The current virtual camera state</param>
    /// <param name="deltaTime">The current applicable deltaTime</param>
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Aim) return;

        Quaternion goal = Quaternion.Euler(
            state.RawOrientation.eulerAngles + 
            CameraTiltControl.Tilt);

        state.OrientationCorrection = Quaternion.Inverse(state.RawOrientation) * goal;
    }
}
