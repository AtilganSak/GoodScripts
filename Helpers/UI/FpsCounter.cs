using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    public enum Pos
    {
        TopRight,
        TopMid,
        TopLeft,
        MidRight,
        Mid,
        MidLeft,
        BottomRight,
        BottomMid,
        BottomLeft
    }

    /* Assign this script to any object in the Scene to display frames per second */

    public Color fontColor = Color.white;
    public FontStyle fontStyle = FontStyle.Bold;
    public float updateInterval = 0.5f; //How often should the number update
    public int fontSize = 50;
    public Pos pos;

    float accum = 0.0f;
    int frames = 0;
    float timeleft;
    float fps;

    GUIStyle textStyle = new GUIStyle();

    // Use this for initialization
    void Start()
    {
        timeleft = updateInterval;

        textStyle.alignment = TextAnchor.MiddleCenter;
    }

    // Update is called once per frame
    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            fps = (accum / frames);
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    void OnGUI()
    {
        textStyle.fontStyle = fontStyle;
        textStyle.normal.textColor = fontColor;
        textStyle.fontSize = fontSize;

        //Display the fps and round to 2 decimals
        GUI.Label(PosToRect(), Mathf.Floor(fps) + "FPS", textStyle);
    }
    Rect PosToRect()
    {
        switch (pos)
        {
            case Pos.TopRight:
                return new Rect(50, 120, 250, 10);
            case Pos.TopMid:
                return new Rect(Screen.width / 2 - 100, 120, 250, 10);
            case Pos.TopLeft:
                return new Rect(Screen.width - 250, 120, 250, 10);
            case Pos.MidRight:
                return new Rect(50, Screen.height / 2, 250, 10);
            case Pos.Mid:
                return new Rect(Screen.width / 2 - 100, Screen.height / 2, 250, 10);
            case Pos.MidLeft:
                return new Rect(Screen.width - 250, Screen.height / 2, 250, 10);
            case Pos.BottomRight:
                return new Rect(50, Screen.height - 150, 250, 10);
            case Pos.BottomMid:
                return new Rect(Screen.width / 2 - 100 , Screen.height - 150, 250, 10);
            case Pos.BottomLeft:
                return new Rect(Screen.width - 250, Screen.height - 150, 250, 10);
            default:
                return Rect.zero;
        }
    }
}