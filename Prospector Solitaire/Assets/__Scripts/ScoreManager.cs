using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eScoreEvent 
{
    draw, // Drawing a card
    mine, // Removing a mine card
    mineGold, // Getting gold from a mine
    gameWin, // Won the round
    gameLoss // Lost the round
}

public class ScoreManager : MonoBehaviour
{
    static private ScoreManager S; // A private Singleton

    static public int SCORE_FROM_PREV_ROUND = 0;
    static public int HIGH_SCORE = 0;

    [Header("Set Dynamically")]
    // Fields to track score info
    public int chain = 0;
    public int scoreRun = 0;
    public int score = 0;

    void Awake()
    {
        if (S == null)
        {
            S = this; // Set the private Singleton
        }
        else
        {
            Debug.LogError("ERROR: ScoreManager.Awake(): S is already set!");
        }
        // Check for a high score in PlayerPrefs
        if (PlayerPrefs.HasKey("ProspectorHighScore"))
        {
            HIGH_SCORE = PlayerPrefs.GetInt("ProspectorHighScore");
        }
        // Add the score from last round, which will be >0 if it was a win
        score += SCORE_FROM_PREV_ROUND;
        // And reset the SCORE_FROM_PREV_ROUND
        SCORE_FROM_PREV_ROUND = 0;
    }

    static public void EVENT(eScoreEvent evt)
    {
        try
        {
            S.Event(evt);
        }
        catch (System.NullReferenceException nre)
        {
            Debug.LogError("ScoreManager:EVENT() called while S=null.\n" + nre);
        }
    }

    void Event(eScoreEvent evt)
    {
        switch (evt)
        {
            // Same things need to happen whether it's a draw, a win, or a loss
            case eScoreEvent.draw: // Drawing a card
            case eScoreEvent.gameWin: // Won the round
                // If it's a win, add the score from this round
                // static fields are NOT reset by SceneManager.LoadScene()
                SCORE_FROM_PREV_ROUND = score;
                print("You won this round! Round score: " + score);
                break;
            case eScoreEvent.gameLoss: // Lost the round
                if(HIGH_SCORE <= score)
                {
                    print("You got the high score! High score: " + score);
                    HIGH_SCORE = score;
                    PlayerPrefs.SetInt("ProspectorHighScore", score);
                }
                else
                {
                    print("Your final score for the round was: " + score);
                }
                break;
            case eScoreEvent.mine: // Removing a mine card
                chain++; // Increase the score chain
                scoreRun += chain; // Add score for this card to scoreRun
                break;
            case eScoreEvent.mineGold: // Getting gold from a mine
                // Adding a score for getting gold
                scoreRun += Mathf.Max(100, 10 * chain);
                break;
            default:
                print("score: " + score + " scoreRun: " + scoreRun + " chain: " + chain);
                break;
        }
    }

    static public int CHAIN { get { return S.chain; } }
    static public int SCORE { get { return S.score; } }
    static public int SCORE_RUN { get { return S.scoreRun; } }
}
