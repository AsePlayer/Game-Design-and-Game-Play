using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eFSState
{
    idle,
    pre,
    active,
    post
}

public class FloatingScore : MonoBehaviour
{
    [Header("Set Dynamically")]
    public eFSState state = eFSState.idle;

    [SerializeField]
    protected int _score = 0;
    public string scoreString;

    // The score property sets both _score and scoreString
    public int score
    {
        get
        {
            return (_score);
        }
        set
        {
            _score = value;
            scoreString = _score.ToString("N0");
            GetComponent<Text>().text = scoreString;
        }
    }

    public List<Vector2> bezierPts; // The points for the Bezier curve
    public List<float> fontSizes; // The fontSizes for each BezPt

    public float timeStart = -1f; // Time.time value when MoveTo starts
    public float timeDuration = 1f; // Duration of the movement
    public string easingCurve = Easing.InOut; // Easing curve for movement

    // The GameObject that will receive the SendMessage when this is done moving
    public GameObject reportFinishTo = null;

    private RectTransform rectTrans;
    private Text txt;

    // Set up the FloatingScore and movement
    // Note the use of parameters with defaults for eTimeS & eTimeD
    public void Init(List<Vector2> ePts, float eTimeS = 0, float eTimeD = 1)
    {
        rectTrans = GetComponent<RectTransform>();
        rectTrans.anchoredPosition = Vector2.zero;
        txt = GetComponent<Text>();

        bezierPts = new List<Vector2>(ePts);

        if (ePts.Count == 1)
        {
            // If there's only one point, just go there
            transform.position = ePts[0];
            return;
        }

        // If more than one point, configure movement
        if (eTimeS == 0) eTimeS = Time.time;
        timeStart = eTimeS;
        timeDuration = eTimeD;

        // Set the state to pre-movement
        state = eFSState.pre;
    }

    public void FSCallback(FloatingScore fs)
    {
        // When this callback is called by SendMessage, add the score from the
        // FloatingScore to this Scoreboard
        score += fs.score;
    }

    void Update()
    {
        if (state == eFSState.idle) return;

        // Get u from the current time and duration
        float u = (Time.time - timeStart) / timeDuration;
        // Use easing curves to adjust u (apply curves to 0 <= u <= 1)
        float uC = Easing.Ease(u, easingCurve);
        if (u < 0)
        {
            state = eFSState.pre;
            // Move to the position of the first Bezier point
            rectTrans.anchoredPosition = bezierPts[0];
            txt.fontSize = (int)fontSizes[0];
            return;
        }
        else
        {
            if (u >= 1)
            {
                uC = 1;
                state = eFSState.post;
                // Move to the position of the last Bezier point
                rectTrans.anchoredPosition = bezierPts[bezierPts.Count - 1];
                txt.fontSize = (int)fontSizes[fontSizes.Count - 1];
                // If there is a callback GameObject, report this fs is done
                if (reportFinishTo != null)
                {
                    reportFinishTo.SendMessage("FSCallback", this);
                    // Then destroy this gameObject
                    Destroy(gameObject);
                }
                else
                {
                    // If there is no callback, just destroy this gameObject
                    Destroy(gameObject);
                }
                return;
            }
            else
            {
                state = eFSState.active;
            }
        }

        // Use Bezier curve to move to a new Vector2 position
        Vector2 pos = Utils.Bezier(uC, bezierPts);
        rectTrans.anchoredPosition = pos;
        if (fontSizes != null && fontSizes.Count > 0)
        {
            // Use Lerp to move to a new fontSize
            int size = (int)Mathf.Lerp(fontSizes[0], fontSizes[fontSizes.Count - 1], uC);
            txt.fontSize = size;
        }
    }

}



