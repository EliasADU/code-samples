using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseCharges : MonoBehaviour
{
    [SerializeField]
    int chargesAmount;

    [SerializeField]
    int rechargeAmount;

    [SerializeField]
    int chargesLeft;

    [SerializeField]
    KnobManager chargeManager;

    void Start()
    {
        chargesLeft = chargesAmount;
        chargeManager.SetKnobAmount(chargesLeft);
    }

    public bool HasChargesLeft()
    {
        return chargesLeft != 0;
    }

    public void ResetCharges()
    {
        chargesLeft = chargesAmount;
        chargeManager.UpdateKnobAmount(chargesLeft);
    }

    public void SpendCharge()
    {
        if (Time.timeScale != 0f)
        {
            chargesLeft--;
            UpdateMarkers();
        }
    }

    public void Recharge()
    {
        chargesLeft += rechargeAmount;
        UpdateMarkers();
    }

    void UpdateMarkers()
    {
        chargesLeft = Mathf.Clamp(chargesLeft, 0, chargesAmount);
        chargeManager.UpdateKnobAmount(chargesLeft);
    }
}
