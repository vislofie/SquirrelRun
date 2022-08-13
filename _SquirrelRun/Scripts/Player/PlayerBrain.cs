using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerBrain : MonoBehaviour
{
    private PlayerMovement _movement;
    private PlayerAnimation _animation;
    private PlayerChars _chars;
    private PlayerFood _food;
    private PlayerInventory _inventory;

    private TriggerReceiver _triggerReceiver;

    private Camera _mainCamera;

    private float _horizontal;
    private float _vertical;

    private float _mouseX;
    private float _mouseY;

    private float _restExp;
    private bool _restReset;
    
    [SerializeField]
    private int _targetFrameRate;

    #region STANDARD-FUNCTIONS

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _animation = GetComponent<PlayerAnimation>();
        _chars = GetComponent<PlayerChars>();
        _food = GetComponent<PlayerFood>();
        _inventory = GetComponent<PlayerInventory>();

        _triggerReceiver = GetComponentInChildren<TriggerReceiver>();

        _mainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;

        _restReset = false;

        Application.targetFrameRate = _targetFrameRate;
    }

    private void Start()
    {
        _chars.HP = 10000;
        _chars.Stamina = 10000;
        _chars.Hunger = 0;
    }

    private void Update()
    {
        TakeInput();
        ActByInput();

        _chars.Hunger += _food.HungerRate * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        _movement.MoveOnInput(_horizontal, _vertical);
    }

    #endregion

    #region INPUT-AND-ACCORDING-ACTIONS

    /// <summary>
    /// Takes WASD and mouse input to further work with it
    /// </summary>
    private void TakeInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");
    }

    /// <summary>
    /// Acts depending on the key input and WASD and mouse input
    /// </summary>
    private void ActByInput()
    {
        bool movingAtAll = (Mathf.Abs(_horizontal) + Mathf.Abs(_vertical)) != 0; // if character is idle

        if (!movingAtAll)
        {
            if (!_restReset) // if stopped moving and didn't reset the rest exponent value
            {
                _restExp = 0.0f; // reset exponent argument
                _restReset = true;
            }

            _restExp += Time.deltaTime * _chars.RestExpValue;
        }
        else if (movingAtAll)
        {
            if (_restReset) // if started moving and the rest exponent value was reset
            {
                _restReset = false;
                _restExp = 0.0f; // reset exponent argument
            }
        }

        _chars.Stamina += Time.deltaTime * _chars.StaminaRestVlaue * 0.5f * Mathf.Exp(_restExp);





        if (_movement.Climbing) // if character is climbing atm
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                _movement.StopClimbing();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _movement.PrepareForJump();
            }
            else if (Input.GetKeyUp(KeyCode.Space) && _chars.Stamina > _chars.StaminaCostForFromSurfaceJump) // if space got up and there is enough stamina to make a jump
            {
                if (_movement.UnPrepareForJump())
                    _chars.Stamina -= _chars.StaminaCostForFromSurfaceJump;
            }

            if (movingAtAll)
                _chars.Stamina -= Time.deltaTime * _chars.StaminaCostForClimbing;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && _movement.WallNearby)
            {
                _movement.StartClimbing();
            }

            if (_movement.OnGround && Input.GetKeyDown(KeyCode.Space) && _chars.Stamina > _chars.StaminaCostForGroundJump) // if space was pressed and there is enough stamina to make a jump
            {
                _movement.Jump();
                _chars.Stamina -= _chars.StaminaCostForGroundJump;
            }

            _chars.Stamina += Time.deltaTime * _chars.StaminaRestVlaue;
        }

        _movement.RotateOnInput(_mouseX, _mouseY);

        if (_chars.Stamina < 1.0f)
        {
            _movement.StopClimbing(); // if there is not enough stamina to pass through
            _movement.StopRunning();  // climbing and running threshold, stop running and climbing 
        }





        if (Input.GetKeyDown(KeyCode.LeftShift) && movingAtAll) // if character is moving and shift key was pressed
        {
            _movement.StartRunning(); // start running obv
            _mainCamera.fieldOfView = 100;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)) // if character released shift key
        {
            _movement.StopRunning(); // well stop running genius
            _mainCamera.fieldOfView = 90;
        }

        if (_movement.Running)
            _chars.Stamina -= Time.deltaTime * _chars.StaminaCostForRunning;



        if (_inventory.SlotEmpty) // if there is nothing in the inventory slot atm
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Collectable collectable;
                if (_triggerReceiver.GetClosestCollectable(out collectable)) // gets closest collectable detected by trigger receiver
                {
                    _inventory.FillSlot(collectable); // fills that empty slot with the collectable that was found
                }
            }
        }
        else // if there is something in the inventory slot
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _inventory.FreeSlot(); // we drop it boiz, if player pressed F
            }
            else if (Input.GetKeyDown(KeyCode.Q)) // but if player pressed Q
            {
                if (_inventory.CurrentCollectable.GetType() == typeof(CollectableFood)) // and it's also a food collectable
                {
                    CollectableFood col = _inventory.CurrentCollectable as CollectableFood;
                    FoodItem fItem = col.AttachedItem as FoodItem;

                    if (!col.Peeled && fItem.Peelable) // if this collectable was not peeled
                    {
                        _animation.ActivateAnimationTrigger(PlayerAnimation.AnimationTrigger.PeelOffFood); // peel it
                        _animation.ActivateAnimationTriggerInGivenTime(PlayerAnimation.AnimationTrigger.PeelOffEnd, fItem.TimeToPeel, OnPeelEnd);
                    }
                    else // and if it was peeled
                    {
                        _animation.ActivateAnimationTrigger(PlayerAnimation.AnimationTrigger.EatFood); // eat it
                        _animation.ActivateAnimationTriggerInGivenTime(PlayerAnimation.AnimationTrigger.EatFoodEnd, fItem.TimeToEat, OnEatEnd);
                    }
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// Gets called when peeling animation ends
    /// </summary>
    private void OnPeelEnd()
    {
        _inventory.PeelCurrent();
    }

    /// <summary>
    /// Gets called when eating animation ends
    /// </summary>
    private void OnEatEnd()
    {
        float hungerChange = _inventory.EatCurrent(); // we get the hunger reduce value from the eaten food
        _chars.Hunger -= hungerChange; // and then substract hunger with that value
    }


}
