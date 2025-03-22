using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchLoader : Singleton<MatchLoader>
{
    public int matchDataCount { get; private set; } = 0;

    void Start()
    {
        string matchesDirectory = Path.Combine(Application.persistentDataPath, "matches"); //유저의 PC나 모바일 등에서 앱 데이터를 저장할 수 있는 안전한 경로
        if (!Directory.Exists(matchesDirectory)) return;

        string[] files = Directory.GetFiles(matchesDirectory, "*.json");
        matchDataCount = files.Length;
    }
    
    public List<MatchData> LoadMatches()
    {
        string matchesDirectory = Path.Combine(Application.persistentDataPath, "matches"); //유저의 PC나 모바일 등에서 앱 데이터를 저장할 수 있는 안전한 경로
        if (!Directory.Exists(matchesDirectory)) return new List<MatchData>();

        string[] files = Directory.GetFiles(matchesDirectory, "*.json");
        //matches 폴더 안에 있는 .json 확장자를 가진 모든 파일 경로를 배열로 가져와.
        //예: ["match1.json", "match2.json", ...]

        List<MatchData> matches = new List<MatchData>();
        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            MatchData data = JsonUtility.FromJson<MatchData>(json); //string을 json 형식으로 바꿔서 저장
            matches.Add(data);
        }

        return matches; //여기서 다시 MatchData형식으로 저장되어서 내보내짐.
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}