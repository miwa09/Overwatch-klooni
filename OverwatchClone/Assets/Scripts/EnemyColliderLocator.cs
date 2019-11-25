using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderLocator : MonoBehaviour
{
    //Very simple script letting other objects know if we're talking about a head collider or a body one.
    
    public bool isHead = false;
    public bool isBody = false;
}
