using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerCameraEffectsInteraction : MonoBehaviour
{
    [SerializeField]
    [Range(60, 120)]
    private float _normalFOV;

    [SerializeField]
    [Range(60, 180)]
    private float _runningFOV;

    [SerializeField]
    private Camera _camera;

    private PlayerInput _input;
    private CameraVisualController _visualController;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _visualController = _camera.GetComponent<CameraVisualController>();

        _input.OnSprintPressed += IncreaseFOV;
        _input.OnSprintUp += ReturnFOVToNormal;
    }

    private void IncreaseFOV()
    {
        _visualController.ChangeCameraEffectValue(CameraVisualController.CameraEffect.FieldOfView, _runningFOV);
    }

    private void ReturnFOVToNormal()
    {
        _visualController.ChangeCameraEffectValue(CameraVisualController.CameraEffect.FieldOfView, _normalFOV);
    }
}
