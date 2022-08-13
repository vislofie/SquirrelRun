using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public enum AnimationTrigger { PeelOffFood = 0, PeelOffEnd = 1, EatFood = 2, EatFoodEnd = 3};
    private Animator _animator;

    private IEnumerator _delayedCallTrigger = null;

    private bool _delayedAnimationInQueue = false;
    public bool DelayedAnimationInQueue => _delayedAnimationInQueue;
    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Activates trigger from animator component
    /// </summary>
    /// <param name="trigger">name of the trigger from enum AnimationTrigger</param>
    public void ActivateAnimationTrigger(AnimationTrigger trigger)
    {
        if (_delayedCallTrigger != null)
        {
            Debug.LogWarning("Tried to start another animation, but there was already one waiting. Removing the new call");
            return;
        }
        _animator.SetTrigger(trigger.ToString());
    }

    /// <summary>
    /// Activates trigger from animator component in given time and takes a method to call on the moment the trigger is activated
    /// </summary>
    /// <param name="trigger">name of the trigger from enum AnimationTrigger</param>
    /// <param name="time">after how much time the trigger is going to be activated</param>
    /// <param name="onEndAction">the method that is going to be called when the trigger is activated</param>
    public void ActivateAnimationTriggerInGivenTime(AnimationTrigger trigger, float time, Action onEndAction = null)
    {
        if (_delayedCallTrigger != null)
        {
            Debug.LogWarning("Tried to start another animation, but there was already one waiting. Removing the new call");
            return;
        }

        _delayedCallTrigger = DelayedCallTrigger(trigger, time, onEndAction);
        StartCoroutine(_delayedCallTrigger);

        _delayedAnimationInQueue = true;
    }

    private IEnumerator DelayedCallTrigger(AnimationTrigger trigger, float time, Action onEndAction)
    {
        yield return new WaitForSeconds(time);
        _animator.SetTrigger(trigger.ToString());

        _delayedCallTrigger = null;

        if (onEndAction != null) onEndAction();

        _delayedAnimationInQueue = false;
    }
}
