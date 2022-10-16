using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Lava : MonoBehaviour
{

    // Check if player controller has collided with lava
    void OnTriggerEnter(Collider other)
    {
        // If player controller has collided with lava, make player controller jump high
        other.gameObject.GetComponent<ThirdPersonController>().JumpHeight = 75;

    }
    


}
