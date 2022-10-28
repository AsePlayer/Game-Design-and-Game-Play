using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public static int coins;
    public AudioClip clip;
    TMPro.TextMeshProUGUI text;
    void Start ()
    {
        Canvas UI = GameObject.Find("UI").GetComponent<Canvas>();
        text = UI.transform.Find("ScoreText").GetComponent<TMPro.TextMeshProUGUI>();
        text.text = "Nugs Left: 0";
        // Count amount of coins in scene
        
        coins = GameObject.FindGameObjectsWithTag("Coin").Length;

    }
    void Update ()
    {
        // Update text to show amount of coins left
        if(coins > 0)
            text.text = "Nugs Left: " + coins;
        else
            text.text = "You Win!";
    }
    void OnTriggerEnter(Collider other)
    {
        // If player controller has collided with coin, destroy coin and subtract 1 from coins
        if (other.gameObject.tag == "Player")
        {
            coins--;
            // Play cartoon chomp sound effect
            other.gameObject.GetComponent<AudioSource>().PlayOneShot(clip,1f);
            Destroy(gameObject);
        }
    }
    // Check if player controller has collided with coin

}
