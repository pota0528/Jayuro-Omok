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
            date = DateTime.Now.ToString("dd" + "일 " + "HH" + "시 " + "mm" + "분"), //현재 날짜와 시간
            moves = moves
        };

        int matchDataIndex = MatchLoader.Instance.matchDataCount;
        // JsonUtility.ToJson()은 Unity에서 제공하는 함수고, data는 [System.Serializable]로 직렬화 가능한 상태여야함.
        string json = JsonUtility.ToJson(data); // MatchData 타입을 System.Serialize로 해놓았기 때문에 Json파일로 직렬화 가능
        // data > serialize(이진, 바이트 처리) > json에서 받기
        string path = Path.Combine(Application.persistentDataPath, "matches", $"{nickname}_{data.date}_{matchDataIndex}.json"); //파일이 저장될 경로를 만드는 코드
        //C:\Users\[user name]\AppData\LocalLow\[company name]\[product name]
        // Application.persistentDataPath : 플랫폼에 따라 바뀌는 저장 가능한 경로
        // "matches" : 이건 폴더 이름
        // "{nickname}_{data.date}.json" : 파일 이름을 닉네임과 날짜로 만들겠다는 의미
        Directory.CreateDirectory(Path.GetDirectoryName(path)); //저장할 폴더가 없으면 만들어주는 코드야.
        //만약 "matches" 폴더가 없으면 자동으로 만들어줌
        File.WriteAllText(path, json); //json 문자열을 실제 파일로 저장하는 코드
        Debug.Log($"매치 저장완료 : {path}");
        
        // 저장 후에 matchDataCount 증가시키기
        MatchLoader.Instance.matchDataCount++;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}

// 게임 종료 시, SaveMatch("유저닉네임", movesList) 를 호출.