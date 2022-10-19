using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed;

    [SerializeField]
    float gravitationalSpeed;

    [SerializeField]
    float gravity;

    [SerializeField]
    float minDownwardSpeed;

    [SerializeField]
    float lungeWeight;

    [SerializeField]
    float miniImpulseStrength;

    [SerializeField]
    RecoveryMeter recoveryMeter;

    [SerializeField]
    ImpulseCharges impulseCharges;

    [SerializeField]
    CharacterController characterController;

    List<Impulse> currentImpulses;
    Vector3 lockedwasdMove;
    bool locked;

    private void Awake()
    {
        References.player = gameObject;
        References.playerHealthSystem = gameObject.GetComponent<HealthSystem>();
    }
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentImpulses = new List<Impulse>();
    }

    void Update()
    {
        Move();
        ClearFinishedImpulses();
    }

    public void AddImpulse(Impulse i, bool isSurfaceBounce = true)
    {
        currentImpulses.Add(i);
        if (isSurfaceBounce)
        {
            locked = false;
        }
    }

    public bool AccurateIsGrounded()
    {
        return (characterController.isGrounded || gravitationalSpeed > 0);
    }

    public void ResetGravity()
    {
        gravitationalSpeed = 0;
    }

    // Aggregates all input and current impulses, applies to character controller
    void Move()
    {
        // Vertical movement
        if (characterController.isGrounded || gravitationalSpeed >= 0)
        {
            gravitationalSpeed = minDownwardSpeed;
        }
        else
        {
            gravitationalSpeed -= gravity * Time.deltaTime;
        }
        Vector3 gravityMove = new Vector3(0, gravitationalSpeed, 0);

        // WASD movement
        float horizontalMove = Input.GetAxis("Horizontal");
        float forwardsMove = Input.GetAxis("Vertical");
        Vector3 wasdDirection = transform.forward * forwardsMove + transform.right * horizontalMove;
        Vector3 wasdMove = wasdDirection * speed;

        // Lunging-related movement overrides
            // If mid-air jumping, we add some impulse and lock the previous WASD direction until grounded.
        if (Input.GetKeyDown(KeyCode.Space) && !characterController.isGrounded && recoveryMeter.CanRecover())
        {
            AddImpulse(GetLunge());
            impulseCharges.Recharge();
            recoveryMeter.ImpulsedSubtraction();

            lockedwasdMove = wasdMove;
            ResetGravity();
            locked = true;
        }
        if (characterController.isGrounded)
        {
            locked = false;
        }
        if (locked)
        {
            wasdMove = new Vector3(lockedwasdMove.x + wasdMove.x / 2, 
                lockedwasdMove.y + wasdMove.y / 2, 
                lockedwasdMove.z + wasdMove.z / 2); 
        }

        // Impulse-aggregated movement        
        Vector3 impulseMove = GetNetImpulse();

        // Adding all movements up        
        characterController.Move((wasdMove * Time.deltaTime) 
            + (gravityMove * Time.deltaTime) 
            + (impulseMove * Time.deltaTime));
    }

    // Lunge impulse is the WASD movement direction with a vertical offset
    Impulse GetLunge()
    {
        Vector3 lungeDirection = (transform.forward * Input.GetAxisRaw("Vertical") 
            + transform.right * Input.GetAxisRaw("Horizontal"));

        lungeDirection.y = lungeWeight;
        return new Impulse(lungeDirection, miniImpulseStrength);
    }

    void ClearFinishedImpulses()
    {
        currentImpulses.RemoveAll(s => s.IsZero());
    }

    Vector3 GetNetImpulse()
    {
        Vector3 netImpulse = Vector3.zero;
        foreach (Impulse i in currentImpulses)
        {
            i.Tick();
            netImpulse += i.GetCurrentVelocity();
        }

        return netImpulse;
    }
}
