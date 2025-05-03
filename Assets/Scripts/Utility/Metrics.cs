using System;
using System.IO;
using UnityEngine;

public static class Metrics
{
    [Serializable]
    private struct MetricsData
    {
        public float totalPlayTime;
        public int sessionCount;

        public bool tutorialCompleted;
        public bool gameCompleted;
    }

    private static MetricsData _data;
    private static float _sessionStartTime;
    private static readonly string SavePath = Application.persistentDataPath + "/metrics.json";

    #region ========== [ PUBLIC METHODS ] ===========

    public static void StartSession()
    {
        Load();
        _sessionStartTime = Time.time;
        _data.sessionCount++;
    }

    public static void EndSession()
    {
        float sessionDuration = Time.time - _sessionStartTime;
        _data.totalPlayTime += sessionDuration;
        Save();
    }

    public static void MarkTutorialCompleted()
    {
        _data.tutorialCompleted = true;
        Save();
    }

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

    #endregion
}
