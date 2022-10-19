using UnityEngine;

public class SurfaceImpulser : MonoBehaviour
{
    [SerializeField]
    float maxImpulseDistance;

    [SerializeField]
    float maxPullDistance;

    [SerializeField]
    float impulseStrength;

    [SerializeField]
    float pullStrength;

    [SerializeField]
    ImpulseCharges impulseChargesSetter;

    [SerializeField]
    AimReticleBehavior reticleBehavior;

    [SerializeField]
    AimParticleHandler aimParticleHandler;

    [SerializeField]
    PlayerController player;

    [SerializeField]
    Transform cameraTransform;

    [SerializeField]
    PlayerController playerController;

    void Update()
    {
        ResetChargesIfGrounded();

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            SetAimParticles();
        }
        else
        {
            reticleBehavior.Activate(false);
        }

        if (Input.GetMouseButtonUp(0) && impulseChargesSetter.HasChargesLeft())
        {
            // Pushes
            ImpulseIfPossible(true);
        }
        else if (Input.GetMouseButtonUp(1) && impulseChargesSetter.HasChargesLeft())
        {
            // Pulls
            ImpulseIfPossible(false);
        }
    }

    void ResetChargesIfGrounded()
    {
        if (playerController.AccurateIsGrounded())
        {
            impulseChargesSetter.ResetCharges();
        }
    }

    void SetAimParticles()
    {
        Vector3 pointingTo;

        if (GetVectorToPointedSurface(out pointingTo))
        {
            float validDistance = maxImpulseDistance;
            if (Input.GetMouseButton(1))
            {
                validDistance = maxPullDistance;
            }
            if (pointingTo.magnitude <= validDistance)
            {
                reticleBehavior.Activate(true);
            }
            else
            {
                reticleBehavior.Activate(false);
            }
        }
        else
        {
            reticleBehavior.Activate(false);
        }
    }

    void ImpulseIfPossible(bool isPush)
    {
        Vector3 pointingTo;

        // Executes only if collided with a surface
        if (GetVectorToPointedSurface(out pointingTo))
        {
            float validDistance = maxImpulseDistance;
            if (!isPush)
            {
                validDistance = maxPullDistance;
            }
            if (Time.timeScale == 1f || Time.timeScale == 0.5f)
            {
                if (pointingTo.magnitude <= validDistance)
                {
                    float strength = impulseStrength;
                    if (!isPush)
                    {
                        pointingTo = -pointingTo;
                        strength = pullStrength;
                    }
                    player.AddImpulse(new Impulse(pointingTo.normalized, strength));
                    impulseChargesSetter.SpendCharge();
                    if(strength == pullStrength)
                    {
                        aimParticleHandler.PullParticles(cameraTransform.position + pointingTo, 
                            pointingTo);
                    }
                    aimParticleHandler.BlastParticles(cameraTransform.position - pointingTo, pointingTo);
                    player.ResetGravity();
                }
            }
        }
    }

    // Raycasts to nearest surface, if it returns true, pointingTo will be a vector to the surface hit point.
    bool GetVectorToPointedSurface(out Vector3 pointingTo)
    {
        RaycastHit collisionPoint;
        var layerMask = ~(1 << 7);
        bool hit = Physics.Raycast(cameraTransform.transform.position, 
            cameraTransform.forward, 
            out collisionPoint, 
            Mathf.Infinity, 
            layerMask);

        if (hit)
        {
            if(collisionPoint.collider.gameObject.tag != "Player")
            {
                pointingTo = cameraTransform.transform.position - collisionPoint.point;
                return true;
            }
        }

        pointingTo = Vector3.zero;
        return false;
    }
}
