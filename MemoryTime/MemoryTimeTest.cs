using TMPro;
using UnityEngine;

public class MemoryTimeTest : MonoBehaviour
{
    public MemoryTime memoryTime;
    public TMP_Text text;

    private void Start()
    {
        // Example of new
        //memoryTime = new MemoryTime("memoryTime", 60, false, true);

        memoryTime.LoadTime(() =>
        {
            if (!memoryTime.IsFinished)
                memoryTime.StartTimer();
        });
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            memoryTime.StartTimer();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            memoryTime.StopTimer();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            memoryTime.PauseTime();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            memoryTime.Reset();
        }

        memoryTime.ProcessTime();
        text.text = memoryTime.ToString();
    }
}

