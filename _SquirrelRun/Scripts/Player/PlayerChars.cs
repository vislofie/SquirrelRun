using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChars : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private Slider _hpBar;
    [SerializeField]
    private Slider _staminaBar;
    [SerializeField]
    private Slider _hungerBar;

    [Header("Characterstics")]
    [SerializeField]
    private float _maxHP;
    [SerializeField]
    private float _maxStamina;
    [SerializeField]
    private float _maxHunger;

    private float _hp;
    private float _stamina;
    private float _hunger;

    public float HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = Mathf.Clamp(value, 0, _maxHP);
            _hpBar.value = _hp / _maxHP;
        }
    }
    public float Stamina
    {
        get
        {
            return _stamina;
        }
        set
        {
            _stamina = Mathf.Clamp(value, 0, _maxStamina);
            _staminaBar.value = _stamina / _maxStamina;
        }
    }
    public float Hunger
    {
        get
        {
            return _hunger;
        }
        set
        {
            _hunger = Mathf.Clamp(value, 0, _maxHunger);
            _hungerBar.value = _hunger / _maxHunger;
        }
    }
    
    [Header("Stamina costs")]
    [SerializeField][Tooltip("This variable is a multiplier for Time.deltaTime")]
    private float _staminaCostForRunning = 1.0f;
    [SerializeField][Tooltip("This variable is a multiplier for Time.deltaTime")]
    private float _staminaCostForClimbing = 1.0f;
    [SerializeField]
    private float _staminaCostForGroundJump = 8.0f;
    [SerializeField]
    private float _staminaCostForFromSurfaceJump = 10.0f;
    [SerializeField][Tooltip("How much stamina is going to be gained per second while resting\n" +
                             "This variable is a multiplier for Time.deltaTime")]
    private float _staminaRestValue = 0.5f;
    [SerializeField][Tooltip("Just look it up in PlayerBrain, it's a lot to write about\n" +
                             "This variable is a multiplier for Time.deltaTime")]
    private float _restExpValue = 0.5f;

    public float StaminaCostForRunning => _staminaCostForRunning;
    public float StaminaCostForClimbing => _staminaCostForClimbing;
    public float StaminaCostForGroundJump => _staminaCostForGroundJump;
    public float StaminaCostForFromSurfaceJump => _staminaCostForFromSurfaceJump;
    public float StaminaRestVlaue => _staminaRestValue;
    public float RestExpValue => _restExpValue;

    
}
