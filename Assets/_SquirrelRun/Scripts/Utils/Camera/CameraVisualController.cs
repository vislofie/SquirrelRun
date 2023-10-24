using System.Collections.Generic;
using UnityEngine;

public class CameraVisualController : MonoBehaviour
{
    public enum CameraEffect { FieldOfView }

    private Camera _camera;
    private List<ICameraEffect> _effects = new List<ICameraEffect>();

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        _effects = new List<ICameraEffect>(GetComponents<ICameraEffect>());
    }

    public void ChangeCameraEffectValue(CameraEffect effect, float value)
    {
        foreach (ICameraEffect cameraEffect in _effects)
        {
            if (cameraEffect.EffectType == effect)
            {
                cameraEffect.ChangeEffectValue(_camera, value);
                return;
            }
        }

        throw new System.Exception("Can't find the effect on CameraVisualController!");
    }
}
