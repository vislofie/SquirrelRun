using UnityEngine;

public interface ICameraEffect
{
    public abstract CameraVisualController.CameraEffect EffectType { get; }
    public abstract float Value { get; }
    public abstract void ChangeEffectValue(Camera associatedCamera, float value);
}
