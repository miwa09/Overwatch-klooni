using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UltBarScript : MonoBehaviour
{
    public PlayerAbilitiesBrigitte brigitteScript;
    public PlayerAbilitiesSoldier76 soldierScript;
    float maxUlt;
    float ult;
    public Image bar;
    public Image notReadyPrompt;
    public Image readyPrompt;
    bool ultReady;
    
    void Start()
    {
        if (soldierScript != null) {
            maxUlt = soldierScript.ultChargeMax;
            ult = soldierScript.ultCharge;
        }
        if (brigitteScript != null) {
            maxUlt = brigitteScript.maxUltCharge;
            ult = brigitteScript.ultCharge;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (brigitteScript != null) {
            ult = brigitteScript.ultCharge;
            if (brigitteScript.ultReady) {
                ultReady = true;
            } else ultReady = false;
        }
        if (soldierScript != null) {
            ult = soldierScript.ultCharge;
            if (soldierScript.ultReady) {
                ultReady = true;
            } else ultReady = false;
        }
        if (ultReady) {
            readyPrompt.enabled = true;
            notReadyPrompt.enabled = false;
        } else {
            readyPrompt.enabled = false;
            notReadyPrompt.enabled = true;
        }
        bar.fillAmount = ult / maxUlt;
    }
}
