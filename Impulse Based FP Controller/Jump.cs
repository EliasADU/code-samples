using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField]
    float strength;

    [SerializeField]
    CharacterController characterController;

    [SerializeField]
    PlayerController playerController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (characterController.isGrounded)
            {
                DoJump();
            }
        }
    }

    void DoJump()
    {
        Impulse jumpImpulse = new Impulse(Vector3.up, strength);

        playerController.AddImpulse(jumpImpulse);
    }
}
