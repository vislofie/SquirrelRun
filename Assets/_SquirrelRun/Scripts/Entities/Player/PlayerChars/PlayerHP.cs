using System.Collections.Generic;
using UnityEngine;

public class PlayerHP
{
    private const int _bodyPartsCount = 7;

    private float _overallHP;
    public float OverallHP => _overallHP;

    private EntityBodyPart[] _entityBodyParts = new EntityBodyPart[_bodyPartsCount];

    public Dictionary<EntityBodyPart.BodyPartType, float> BodyPartToHP
    {
        get
        {
            Dictionary<EntityBodyPart.BodyPartType, float> bodyPartToHP = new Dictionary<EntityBodyPart.BodyPartType, float>();

            foreach (EntityBodyPart bodyPart in _entityBodyParts)
                bodyPartToHP.Add(bodyPart.BodyPart, bodyPart.HP);

            return bodyPartToHP;
        }
    }
    public Dictionary<EntityBodyPart.BodyPartType, float> BodyPartToMaxHP
    {
        get
        {
            Dictionary<EntityBodyPart.BodyPartType, float> bodyPartToHP = new Dictionary<EntityBodyPart.BodyPartType, float>();

            foreach (EntityBodyPart bodyPart in _entityBodyParts)
                bodyPartToHP.Add(bodyPart.BodyPart, bodyPart.MaxHP);

            return bodyPartToHP;
        }
    }
    public Dictionary<EntityBodyPart.BodyPartType, EntityBodyPart.BodyPartEffect> BodyPartToEffect
    {
        get
        {
            Dictionary<EntityBodyPart.BodyPartType, EntityBodyPart.BodyPartEffect> bodyPartToEffect = new Dictionary<EntityBodyPart.BodyPartType, EntityBodyPart.BodyPartEffect>();

            foreach (EntityBodyPart bodyPart in _entityBodyParts)
                bodyPartToEffect.Add(bodyPart.BodyPart, bodyPart.Effect);

            return bodyPartToEffect;
        }
    }

    public void Initialize()
    {
        _entityBodyParts[0] = new EntityBodyPart(80, 80, EntityBodyPart.BodyPartType.Head);

        _entityBodyParts[1] = new EntityBodyPart(120, 120, EntityBodyPart.BodyPartType.Torso);
        _entityBodyParts[2] = new EntityBodyPart(110, 110, EntityBodyPart.BodyPartType.Stomach);

        _entityBodyParts[3] = new EntityBodyPart(65, 65, EntityBodyPart.BodyPartType.LArm);
        _entityBodyParts[4] = new EntityBodyPart(65, 65, EntityBodyPart.BodyPartType.RArm);

        _entityBodyParts[5] = new EntityBodyPart(65, 65, EntityBodyPart.BodyPartType.LLeg);
        _entityBodyParts[6] = new EntityBodyPart(65, 65, EntityBodyPart.BodyPartType.RLeg);
    }

    public void OnUpdate()
    {

    }

    public void OnCoroutineUpdate()
    {
        ApplyEffects();

        CalculateOverallHP();
    }

    /// <summary>
    /// Sets hp of given body part
    /// </summary>
    /// <param name="bodyPart"></param>
    /// <param name="hp"></param>
    public void SetHPOfBodyPart(EntityBodyPart.BodyPartType bodyPart, float hp)
    {
        CalculateOverallHP();

        foreach (EntityBodyPart part in _entityBodyParts)
        {
            if (part.BodyPart == bodyPart)
            {
                float leftOver = part.SetHP(hp);

                if (leftOver != 0)
                {
                    if (_overallHP <= leftOver)
                    {
                        // TODO: DIE
                        return;
                    }
                    DisperseLeftOverHP(leftOver);
                }
            }
        }
    }

    /// <summary>
    /// Disperses left over damage from damaged body part between other healthy body parts
    /// </summary>
    /// <param name="leftOverHP"></param>
    private void DisperseLeftOverHP(float leftOverHP)
    {
        Debug.Log("Disperse was called");

        List<EntityBodyPart> aliveBodyParts = new List<EntityBodyPart>();

        foreach (EntityBodyPart bodyPart in _entityBodyParts)
            if (bodyPart.HP > 0)
                aliveBodyParts.Add(bodyPart);

        foreach (EntityBodyPart bodyPart in aliveBodyParts)
        {
            bodyPart.SetHP(bodyPart.HP - leftOverHP / aliveBodyParts.Count);
            Debug.Log(leftOverHP);
        }
    }

    private void CalculateOverallHP()
    {
        _overallHP = 0;

        foreach (EntityBodyPart bodyPart in _entityBodyParts)
            _overallHP += bodyPart.HP;
    }

    /// <summary>
    /// Sets given effect on given body part
    /// </summary>
    /// <param name="bodyPart"></param>
    /// <param name="effect"></param>
    public void SetEffectOfBodyPart(EntityBodyPart.BodyPartType bodyPart, EntityBodyPart.BodyPartEffect effect)
    {
        foreach (EntityBodyPart part in _entityBodyParts)
        {
            if (part.BodyPart == bodyPart)
            {
                part.SetEffect(effect);
            }
        }
    }

    /// <summary>
    /// Applies effects of all effects (k lol)
    /// </summary>
    private void ApplyEffects()
    {
        foreach (EntityBodyPart bodyPart in _entityBodyParts)
        {
            if ((bodyPart.Effect & EntityBodyPart.BodyPartEffect.LightBleeding) != 0)
            {
                SetHPOfBodyPart(bodyPart.BodyPart, bodyPart.HP - 1);

                Debug.Log("Light bleeding");
            }

            if ((bodyPart.Effect & EntityBodyPart.BodyPartEffect.HeavyBleeding) != 0)
            {
                SetHPOfBodyPart(bodyPart.BodyPart, bodyPart.HP - 3);

                Debug.Log("Heavy bleeding");
            }

            if ((bodyPart.Effect & EntityBodyPart.BodyPartEffect.BrokenBone) != 0)
            {
                Debug.Log("Broken bone");
            }
        }

       
    }




    

    
}
