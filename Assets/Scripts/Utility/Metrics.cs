using System;
using System.IO;
using System.Text;
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

        public float averageFramerate;
        public float totalFrameTime;
        public int totalFrames;
    }

    private static MetricsData _data;
    private static float _sessionStartTime;
    private static readonly string SavePath = Application.persistentDataPath + "/metrics.json";
    private static readonly string MetricsFolder = Path.Combine(Application.persistentDataPath, "MetricsLogs");


    #region ========== [ PUBLIC METHODS ] ===========

    /// <summary>
    /// Signal to the Metrics that the player has started playing the game. Put in Start()
    /// </summary>
    public static void StartSession()
    {
        Load();
        _sessionStartTime = Time.time;
        _data.sessionCount++;
    }

    /// <summary>
    /// Signal to the Metrics that the player has stopped playing the game.
    /// </summary>
    public static void EndSession()
    {
        float sessionDuration = Time.time - _sessionStartTime;
        _data.totalPlayTime += sessionDuration;
        Save();
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

    /// <summary>
    /// Reset saved metrics data and logs it to a CSV file.
    /// </summary>
    public static void Reset()
    {
        WriteDataToCSV(_data);
        _data = new MetricsData();
        Save();
    }

    /// <summary>
    /// Reset saved metrics data without logging to CSV.
    /// </summary>
    public static void HardReset()
    {
        _data = new MetricsData();
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

    private static void WriteDataToCSV(MetricsData data)
    {
        if (!Directory.Exists(MetricsFolder))
            Directory.CreateDirectory(MetricsFolder);

        int index = 1;
        string filePath;

        // Give it a sequential filename
        do
        {
            filePath = Path.Combine(MetricsFolder, $"metrics{index}.csv");
            index++;
        } while (File.Exists(filePath));

        var sb = new StringBuilder();

        sb.AppendLine("Field,Value");

        sb.AppendLine($"PlayTime,{data.totalPlayTime}");
        sb.AppendLine($"SessionCount,{data.sessionCount}");
        sb.AppendLine($"TutorialCompleted,{data.tutorialCompleted}");
        sb.AppendLine($"GameCompleted,{data.gameCompleted}");
        sb.AppendLine($"AverageFramerate,{data.averageFramerate}");
        sb.AppendLine($"TotalFrameTime,{data.totalFrameTime}");
        sb.AppendLine($"TotalFrames,{data.totalFrames}");


        File.WriteAllText(filePath, sb.ToString());
    }
    #endregion
}