using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace kim_namespace
{
    public class MatchSave : MonoBehaviour
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
    }

// 게임 종료 시, SaveMatch("유저닉네임", movesList) 를 호출.
}