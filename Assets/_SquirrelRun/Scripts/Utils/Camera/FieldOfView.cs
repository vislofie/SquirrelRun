using UnityEngine;

public class FieldOfView : MonoBehaviour, ICameraEffect
{
    public CameraVisualController.CameraEffect EffectType { get; private set; }
    public float Value { get; private set; }

    public FieldOfView()
    {
        EffectType = CameraVisualController.CameraEffect.FieldOfView;
    }

    public void ChangeEffectValue(Camera associatedCamera, float value)
    {
        associatedCamera.fieldOfView = value;
        Value = value;
    }
}
