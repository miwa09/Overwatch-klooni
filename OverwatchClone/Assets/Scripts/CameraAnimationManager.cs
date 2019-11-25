﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimationManager : MonoBehaviour
{
    public Animation camAnimation;
    PlayerWeaponMelee playerMelee;
    void Awake()
    {
        camAnimation = gameObject.GetComponent<Animation>(); //Get the animator component automatically
        playerMelee = gameObject.GetComponentInParent<PlayerWeaponMelee>();
    }

    public void CameraMeleeShake() //Simple function for one animation that can be called through other scripts.
    {
        camAnimation.Play();
    }

    public void CameraMeleeDone()
    {
        playerMelee.MeleeDone();
    }

    //private void Update() //To test if it worked
    //{
    //    if (Input.GetKeyDown(KeyCode.J))
    //    {
    //        CameraMeleeShake();
    //    }
    //}
}
