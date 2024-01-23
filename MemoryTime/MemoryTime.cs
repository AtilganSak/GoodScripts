using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;

[System.Serializable]
public class MemoryTime
{
    [Tooltip("The key used for registration. Make sure you use a unique key. (Saving PlayerPrefs)")]
    [SerializeField] string key;
    [Tooltip("Start time for process.")]
    public float time;
    [Tooltip("If there is internet it uses API time, otherwise it uses local time.")]
    public bool internetTime;
    [Tooltip("Time is not registration. It only works in runtime.")]
    public bool indepentTime;

    bool isFinished;
    /// <summary>
    /// Is time over? Including in offline situations.
    /// </summary>
    public bool IsFinished { get => isFinished; }

    bool paused;
    /// <summary>
    /// Is time paused?
    /// </summary>
    public bool IsPaused { get => paused; }

    bool isRunning;
    /// <summary>
    /// Is time processing?
    /// </summary>
    public bool IsRunning { get => isRunning; }

    float currentTime;

    const string API_URL = "time.nist.gov";

    public Action onStarted;
    public Action<bool> onPaused;
    public Action onStopped;
    public Action onCompleted;

    string STARTED_TIME_KEY
    {
        get
        {
            return key + "StartedTime";
        }
    }

    public MemoryTime() { }
    public MemoryTime(string _key, float _time)
    {
        key = _key;
        time = _time;
    }
    public MemoryTime(string _key, float _time, bool _internetTime, bool _indepentTime)
    {
        key = _key;
        time = _time;
        internetTime = _internetTime;
        indepentTime = _indepentTime;
    }

    public void ProcessTime()
    {
        if (!isFinished && !paused && isRunning)
        {
            currentTime -= UnityEngine.Time.deltaTime;
            if (currentTime < 0)
            {
                StopTimer();

                onCompleted?.Invoke();
            }
        }
    }
    public void PauseTime()
    {
        paused = !paused;
        isRunning = !paused;

        onPaused?.Invoke(paused);
    }
    public void StartTimer()
    {
        if (isRunning) return;

        isFinished = false;
        isRunning = true;

        onStarted?.Invoke();    

        if (!PlayerPrefs.HasKey(STARTED_TIME_KEY) && !indepentTime)
        {
            if (internetTime && IsOnline())
            {
                GetInternetTime((time) =>
                {
                    Debug.Log("Saved Time: " + time);

                    string serializedDateTime = time.ToString("yyyy-MM-dd HH:mm:ss");
                    PlayerPrefs.SetString(STARTED_TIME_KEY, serializedDateTime);
                    PlayerPrefs.Save();
                });
                try
                {
                }
                catch (Exception e)
                {
                    Debug.LogError(e);

                    SaveLocalTime();
                }
            }
            else
            {
                SaveLocalTime();
            }
        }
    }
    public void StopTimer()
    {
        if (isRunning)
        {
            isRunning = false;
            currentTime = time;

            onStopped?.Invoke();    
        }
    }
    public void Reset()
    {
        StopTimer();
        paused = false;
        isFinished = false;
        PlayerPrefs.DeleteKey(STARTED_TIME_KEY);
    }
    public void LoadTime(Action onLoadedTime)
    {
        if (PlayerPrefs.HasKey(STARTED_TIME_KEY) && !indepentTime)
        {
            if (internetTime && IsOnline())
            {
                try
                {
                    GetInternetTime((time) =>
                    {
                        CheckTimeDifference(time);

                        onLoadedTime?.Invoke();
                    });
                    return;
                }
                catch (WebException e)
                {
                    Debug.LogError(e.Message);

                    CheckTimeDifference(DateTime.Now);
                }
            }
            else
            {
                CheckTimeDifference(DateTime.Now);
            }
        }
        else
        {
            currentTime = time;
        }
        onLoadedTime?.Invoke();
    }    
    private void GetInternetTime(Action<DateTime> callback)
    {
        // Try get time from internet
        var client = new TcpClient(API_URL, 13);
        using (var streamReader = new StreamReader(client.GetStream()))
        {
            var response = streamReader.ReadToEnd();
            int counter = 0;
            while (response == "")
            {
                counter++;
                if (counter >= 100)
                    break;
            }
            var utcDateTimeString = response.Substring(7, 17);
            DateTime localDateTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            callback?.Invoke(localDateTime);
        }
    }
    private void CheckTimeDifference(DateTime nowDate)
    {
        Debug.Log("Now Data: " + nowDate);

        string serializedDateTime = PlayerPrefs.GetString(STARTED_TIME_KEY, string.Empty);
        DateTime lastStartDate = DateTime.ParseExact(serializedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        TimeSpan timeDifference = nowDate - lastStartDate;
        if (timeDifference.TotalSeconds > time)
        {
            isFinished = true;
        }
        else
        {
            currentTime = time - (float)timeDifference.TotalSeconds;
        }
    }
    private bool IsOnline()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
    private void SaveLocalTime()
    {
        string serializedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        PlayerPrefs.SetString(STARTED_TIME_KEY, serializedDateTime);
        PlayerPrefs.Save();
    }
    public override string ToString()
    {
        if (currentTime > 3600)
            return TimeSpan.FromSeconds(currentTime).ToString(@"hh\:mm\:ss");
        else if (currentTime > 59)
            return TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss");
        else
            return currentTime.ToString();
    }
}