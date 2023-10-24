using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChars : MonoBehaviour
{
    [Header("Characterstics")]
    [SerializeField]
    private float _maxStamina;
    [SerializeField]
    private float _maxHunger;

    private PlayerHP _hp;
    private float _stamina;
    private float _hunger;

    public float HP
    {
        get
        {
            return _hp.OverallHP;
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
        }
    }

    public float MaxStamina => _maxStamina;
    public float MaxHunger => _maxHunger;
    
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

    private void Awake()
    {
        StartCoroutine(OneSecondCoroutine());

        _hp = new PlayerHP();
        _hp.Initialize();
    }

    private void Update()
    {
        _hp.OnUpdate();
    }

    /// <summary>
    /// Coroutine that is being called each real-time second
    /// </summary>
    /// <returns></returns>
    private IEnumerator OneSecondCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.0f);
            _hp.OnCoroutineUpdate();
        }
    }

    public Dictionary<EntityBodyPart.BodyPartType, float> BodyPartToHP => _hp.BodyPartToHP; // dictionary that contains HP of each body part
    public Dictionary<EntityBodyPart.BodyPartType, float> BodyPartToMaxHP => _hp.BodyPartToMaxHP; // dictionary that contains max HP of each body part
    public Dictionary<EntityBodyPart.BodyPartType, EntityBodyPart.BodyPartEffect> BodyPartToEffect => _hp.BodyPartToEffect; // dictionary that contains effects of each body parts

    /// <summary>
    /// Sets HP to given body part
    /// </summary>
    /// <param name="bodyPart">given body part</param>
    /// <param name="hp">hp</param>
    public void SetHPOfBodyPart(EntityBodyPart.BodyPartType bodyPart, float hp)
    {
        if (!_hp.BodyPartToHP.ContainsKey(bodyPart)) throw new System.ArgumentException("There is no body part with this ID!");
        _hp.SetHPOfBodyPart(bodyPart, hp);
    }
    
    /// <summary>
    /// Sets effects on given body parts
    /// </summary>
    /// <param name="bodyPart">body part</param>
    /// <param name="effect">effect</param>
    public void SetEffectOfBodyPart(EntityBodyPart.BodyPartType bodyPart, EntityBodyPart.BodyPartEffect effect)
    {
        if (!_hp.BodyPartToHP.ContainsKey(bodyPart)) throw new System.ArgumentException("There is no body part with this ID!");
        _hp.SetEffectOfBodyPart(bodyPart, effect);
    }

    /// <summary>
    /// Returns HP of given body part
    /// </summary>
    /// <param name="bodyPart">body part</param>
    /// <returns>float value of HP</returns>
    public float GetHPOfBodyPart(EntityBodyPart.BodyPartType bodyPart)
    {
        if (!_hp.BodyPartToHP.ContainsKey(bodyPart)) throw new System.ArgumentException("There is no body part with this ID!");
        return _hp.BodyPartToHP[bodyPart];
    }

    /// <summary>
    /// Returns max HP of given body part
    /// </summary>
    /// <param name="bodyPart">body part</param>
    /// <returns>float value of max HP</returns>
    public float GetMaxHPOfBodyPart(EntityBodyPart.BodyPartType bodyPart)
    {
        if (!_hp.BodyPartToMaxHP.ContainsKey(bodyPart)) throw new System.ArgumentException("There is no body part with this ID!");
        return _hp.BodyPartToMaxHP[bodyPart];
    }

    /// <summary>
    /// Return effect of given body part
    /// </summary>
    /// <param name="bodyPart">body part</param>
    /// <returns>bit value of effects, summed up together</returns>
    /// <exception cref="System.ArgumentException"></exception>
    public EntityBodyPart.BodyPartEffect GetEffectOfBodyPart(EntityBodyPart.BodyPartType bodyPart)
    {
        if (!_hp.BodyPartToEffect.ContainsKey(bodyPart)) throw new System.ArgumentException("There is no body part with this ID!");
        return _hp.BodyPartToEffect[bodyPart];
    }
}
