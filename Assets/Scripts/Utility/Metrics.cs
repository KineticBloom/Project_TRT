using System;
using System.IO;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public static class Metrics
{
    [Serializable]
    private struct MetricsData
    {
        public float totalPlayTime;
        public int sessionCount;

        public bool tutorialCompleted;
        public bool gameCompleted;

        public float averageFramerate;
        public float totalFrameTime;
        public int totalFrames;
    }

    private static MetricsData _data;
    private static float _sessionStartTime;
    private static readonly string SavePath = Application.persistentDataPath + "/metrics.json";


    #region ========== [ PUBLIC METHODS ] ===========

    /// <summary>
    /// Signal to the Metrics that the player has started playing the game. Put in Start()
    /// </summary>
    public static async void StartSession()
    {
        Load();
        _sessionStartTime = Time.time;
        _data.sessionCount++;

        await InitializeServicesAsync();
    }

    /// <summary>
    /// Signal to the Metrics that the player has stopped playing the game.
    /// </summary>
    public static void EndSession()
    {
        float sessionDuration = Time.time - _sessionStartTime;
        _data.totalPlayTime += sessionDuration;
        Save();
        SendAnalytics();
    }

    /// <summary>
    /// Record frame duration to calculate average framerate
    /// </summary>
    public static void RecordFrame()
    {
        float deltaTime = Time.deltaTime;
        _data.totalFrameTime += deltaTime;
        _data.totalFrames++;

        _data.averageFramerate = _data.totalFrames / _data.totalFrameTime;
    }


    /// <summary>
    /// Record that the Tutorial has been completed at least once
    /// </summary>
    public static void MarkTutorialCompleted()
    {
        _data.tutorialCompleted = true;
        Save();
    }

    /// <summary>
    /// Record that the Game has been beaten at least once
    /// </summary>
    public static void MarkGameCompleted()
    {
        _data.gameCompleted = true;
        Save();
    }

    #endregion

    #region ========== [ PRIVATE METHODS ] ===========

    /// <summary>
    /// Loads saved metrics data into _data
    /// </summary>
    private static void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            _data = JsonUtility.FromJson<MetricsData>(json);
        }
        else
        {
            _data = new MetricsData();
        }
    }

    /// <summary>
    /// Save _data to metrics.json
    /// </summary>
    private static void Save()
    {
        string json = JsonUtility.ToJson(_data, true);
        File.WriteAllText(SavePath, json);
    }

    /// <summary>
    /// Reset saved metrics data
    /// </summary>
    public static void Reset()
    {
        _data = new MetricsData();
        Save();
    }

    /// <summary>
    /// Wait for the AnalyticsService to be initialized before starting data collection
    /// </summary>
    /// <returns></returns>
    private static async Task InitializeServicesAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            AnalyticsService.Instance?.StartDataCollection();
            SendAnalytics();
        }
        catch (ServicesInitializationException e)
        {
            Debug.LogError($"Failed to initialize Unity Services: {e}");
        }
    }

    private static void SendAnalytics()
    {
        SavedAnalyticsEvent eventData = new SavedAnalyticsEvent();

        eventData.TutorialCompleted = _data.tutorialCompleted;
        eventData.GameCompleted = _data.gameCompleted;
        eventData.AverageFramerate = _data.averageFramerate;

        AnalyticsService.Instance.RecordEvent(eventData);
    }

    #endregion
}

public class SavedAnalyticsEvent : Unity.Services.Analytics.Event
{
    public SavedAnalyticsEvent() : base("savedAnalyticsEvent")
    {
    }

    public bool TutorialCompleted { set { SetParameter("tutorialCompleted", value); } }
    public bool GameCompleted { set { SetParameter("gameCompleted", value); } }
    public float AverageFramerate { set { SetParameter("averageFramerate", value); } }

}
