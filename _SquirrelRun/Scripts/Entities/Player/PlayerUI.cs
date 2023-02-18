using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Bars")]
    [SerializeField]
    private Slider _staminaBar;
    [SerializeField]
    private Slider _hungerBar;

    [Header("Body parts visualization")]

    [SerializeField][Tooltip("The maximum alpha value for body part fillers")][Range(0.0f, 1.0f)]
    private float _maxAlphaWounded;

    [Header("")]

    [SerializeField]
    private GameObject _lightBleedingIcon;
    private Text _lightBleedingText;
    [SerializeField]
    private GameObject _heavyBleedingIcon;
    private Text _heavyBleedingText;
    [SerializeField]
    private GameObject _brokenBoneIcon;
    private Text _brokenBoneText;

    [Header("")]

    [SerializeField]
    private Image _headFiller;
    [SerializeField]
    private Image _torsoFiller;
    [SerializeField]
    private Image _stomachFiller;
    [SerializeField]
    private Image _leftArmFiller;
    [SerializeField]
    private Image _rightArmFiller;
    [SerializeField]
    private Image _leftLegFiller;
    [SerializeField]
    private Image _rightLegFiller;

    private void Awake()
    {
        _lightBleedingText = _lightBleedingIcon.GetComponentInChildren<Text>(true);
        _heavyBleedingText = _heavyBleedingIcon.GetComponentInChildren<Text>(true);
        _brokenBoneText = _brokenBoneIcon.GetComponentInChildren<Text>(true);
    }

    /// <summary>
    /// Draws HP on player UI
    /// </summary>
    /// <param name="bodyPartToHP">dictionary that contains body parts as keys and HPs of these body parts as value</param>
    /// <param name="bodyPartToMaxHP">dictionary that contains body parts as keys and max HPs of these body parts as value</param>
    /// <param name="bodyPartToEffect">dictionary that contains body parts as keys and effects on these body parts as value</param>
    public void DrawHP(Dictionary<EntityBodyPart.BodyPartType, float> bodyPartToHP, 
                       Dictionary<EntityBodyPart.BodyPartType, float> bodyPartToMaxHP,
                       Dictionary<EntityBodyPart.BodyPartType, EntityBodyPart.BodyPartEffect> bodyPartToEffect)
    {
        Color baseColor = new Color(1, 0, 0, 0);
        int lightBleedingCount = 0;
        int heavyBleedingCount = 0;
        int brokenBoneCount = 0;

        foreach (EntityBodyPart.BodyPartType key in bodyPartToHP.Keys)
        {
            baseColor.a = _maxAlphaWounded - bodyPartToHP[key] / bodyPartToMaxHP[key] * _maxAlphaWounded;
            EntityBodyPart.BodyPartEffect curEffect = bodyPartToEffect[key];

            if ((bodyPartToEffect[key] & EntityBodyPart.BodyPartEffect.LightBleeding) != 0)
                lightBleedingCount++;
            if ((bodyPartToEffect[key] & EntityBodyPart.BodyPartEffect.HeavyBleeding) != 0)
                heavyBleedingCount++;
            if ((bodyPartToEffect[key] & EntityBodyPart.BodyPartEffect.BrokenBone) != 0)
                brokenBoneCount++;

            switch (key)
            {
                case EntityBodyPart.BodyPartType.Head:
                    _headFiller.color = baseColor;
                    break;
                case EntityBodyPart.BodyPartType.Torso:
                    _torsoFiller.color = baseColor;
                    break;
                case EntityBodyPart.BodyPartType.Stomach:
                    _stomachFiller.color = baseColor;
                    break;
                case EntityBodyPart.BodyPartType.LArm:
                    _leftArmFiller.color = baseColor;
                    break;
                case EntityBodyPart.BodyPartType.RArm:
                    _rightArmFiller.color = baseColor;
                    break;
                case EntityBodyPart.BodyPartType.LLeg:
                    _leftLegFiller.color = baseColor;
                    break;
                case EntityBodyPart.BodyPartType.RLeg:
                    _rightLegFiller.color = baseColor;
                    break;
                default:
                    throw new System.ArgumentException("Wrong key in dictionary!");
            }
        }


        if (lightBleedingCount > 0)
        {
            _lightBleedingIcon.SetActive(true);
            _lightBleedingText.text = "";

            if (lightBleedingCount > 1)
                _lightBleedingText.text = "x" + lightBleedingCount.ToString();
        }
        else
        {
            _lightBleedingIcon.SetActive(false);
        }

        if (heavyBleedingCount > 0)
        {
            _heavyBleedingIcon.SetActive(true);
            _heavyBleedingText.text = "";

            if (heavyBleedingCount > 1)
                _heavyBleedingText.text = "x" + heavyBleedingCount.ToString();
        }
        else
        {
            _heavyBleedingIcon.SetActive(false);
        }

        if (brokenBoneCount > 0)
        {
            _brokenBoneIcon.SetActive(true);
            _brokenBoneText.text = "";

            if (brokenBoneCount > 1)
                _brokenBoneText.text = "x" + brokenBoneCount.ToString();
        }
        else
        {
            _brokenBoneIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Draws stamina on player UI
    /// </summary>
    /// <param name="stamina">current stamina of player</param>
    /// <param name="maxStamina">max stamina of player</param>
    public void DrawStamina(float stamina, float maxStamina)
    {
        _staminaBar.value = stamina / maxStamina;
    }

    /// <summary>
    /// Draws hunger on player UI
    /// </summary>
    /// <param name="hunger">current hunger of player</param>
    /// <param name="maxHunger">max hunger of player</param>
    public void DrawHunger(float hunger, float maxHunger)
    {
        _hungerBar.value = hunger / maxHunger;
    }
}
