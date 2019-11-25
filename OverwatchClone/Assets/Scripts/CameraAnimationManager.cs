using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimationManager : MonoBehaviour
{
    Animation camAnimation;
    void Awake()
    {
        camAnimation = gameObject.GetComponent<Animation>(); //Get the animator component automatically
    }

    public void CameraMeleeShake() //Simple function for one animation that can be called through other scripts.
    {
        camAnimation.Play();
    }

    //private void Update() //To test if it worked
    //{
    //    if (Input.GetKeyDown(KeyCode.J))
    //    {
    //        CameraMeleeShake();
    //    }
    //}
}
