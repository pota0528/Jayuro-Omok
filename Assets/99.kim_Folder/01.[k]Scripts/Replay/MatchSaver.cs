using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchSaver : Singleton<MatchSaver>
{
    public void SaveMatch(string nickname, List<Move> moves)
    {
        MatchData data = new MatchData
        {
            title = nickname,
            date = DateTime.Now.ToString("yyyy-MM-dd"),
            moves = moves
        };

        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, "matches", $"{nickname}_{data.date}.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, json);
        Debug.Log($"매치 저장완료 : {path}");
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}

// 게임 종료 시, SaveMatch("유저닉네임", movesList) 를 호출.