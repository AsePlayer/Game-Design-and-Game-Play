using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    Vector2 aPosition1;
    Vector2 temp;
    static float speed = 6;
    float gimmex;
    float gimmey;

    public static bool playerDead = false;
    public static int score = 0;

    private float lerpTimer = 0.0f;

    // TextMeshPro
    private TMPro.TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GameObject.Find("Score").GetComponent<TMPro.TMP_Text>();
        scoreText.text = "Score: " + score;

        temp = transform.position;
        float gimmex = temp.x;
        float gimmey = temp.y;
        aPosition1 = new Vector2(-gimmex, -gimmey);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, aPosition1, speed * Time.deltaTime);
        
        if(playerDead)
        {
            speed -= speed / 100.0f;
            lerpTimer += Time.deltaTime / 2.0f;
            GameObject.Find("Player").GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, lerpTimer);
        }
    }

    void OnBecameInvisible()
    {
        score++;
        scoreText.text = "Score: " + score;
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerDead = true;
            // Lerp color to red
            other.gameObject.GetComponent<Movement>().enabled = false;
        }
    }

    public bool isPlayerDead()
    {
        return playerDead;
    }

    public void resetPlayerDead()
    {
        speed = 6;
        score = 0;
        playerDead = false;
    }
}
