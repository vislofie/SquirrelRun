using System;
using UnityEngine;

public class EntityBodyPart
{
    [Flags]
    public enum BodyPartEffect
    {
        None = 0,
        LightBleeding = 0b0000001,
        HeavyBleeding = 0b0000010,
        BrokenBone = 0b0000100
    };

    public enum BodyPartType { Head, Torso, Stomach, RArm, LArm, RLeg, LLeg };

    private BodyPartType _bodyPart;
    public BodyPartType BodyPart => _bodyPart;

    private BodyPartEffect _effect;
    public BodyPartEffect Effect
    {
        get
        {
            return _effect;
        }
        set
        {
            _effect = value;
        }
    }

    private float _hp;
    public float HP => _hp;

    private WriteOnce<float> _maxHP = new WriteOnce<float>();
    public float MaxHP => _maxHP.Value;

    public float SetHP(float newHP)
    {
        float leftOver = 0;
        if (newHP < 0)
        {
            leftOver = -newHP;
            _hp = 0;
        }
        else
        {
            _hp = Mathf.Clamp(newHP, 0, _maxHP.Value);
        }

        return leftOver;
    }

    public void SetEffect(BodyPartEffect effect)
    {
        _effect = _effect | effect;
    }

   

    public EntityBodyPart(float hp, float maxHP, BodyPartType bodyPartType)
    {
        _bodyPart = bodyPartType;
        _maxHP.Value = maxHP;
        SetHP(hp);
    }

}
