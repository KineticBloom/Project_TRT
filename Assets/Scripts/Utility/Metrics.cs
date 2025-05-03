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
        public float averageFramerate;
        public bool tutorialCompleted;
        public bool gameCompleted;
    }

    private static MetricsData _data;
    private static float _sessionStartTime;
    private static readonly string SavePath = Application.persistentDataPath + "/metrics.json";

    #region ========== [ PUBLIC METHODS ] ===========

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

    public static void Reset()
    {
        _data = new MetricsData();
        Save();
    }

    #endregion
}
