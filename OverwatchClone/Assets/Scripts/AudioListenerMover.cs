using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerMover : MonoBehaviour
{
    public GameObject p1;
    public GameObject p2;

    private void FixedUpdate() {
        transform.position = (p1.transform.position + p2.transform.position) / 2;
    }
}
