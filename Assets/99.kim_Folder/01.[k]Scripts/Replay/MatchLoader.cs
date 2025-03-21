using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchLoader : Singleton<MatchLoader>
{
    public List<MatchData> LoadMatches()
    {
        string matchesDirectory = Path.Combine(Application.persistentDataPath, "matches");
        if (!Directory.Exists(matchesDirectory)) return new List<MatchData>();

        string[] files = Directory.GetFiles(matchesDirectory, "*.json");
        List<MatchData> matches = new List<MatchData>();
        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            MatchData data = JsonUtility.FromJson<MatchData>(json);
            matches.Add(data);
        }

        return matches;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}