using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchLoader : Singleton<MatchLoader>
{
    public int matchDataCount { get; set; } = 0;

    void Start()
    {
        string matchesDirectory = Path.Combine(Application.persistentDataPath, "matches"); //유저의 PC나 모바일 등에서 앱 데이터를 저장할 수 있는 안전한 경로
        if (!Directory.Exists(matchesDirectory))
        {
            string[] files = Directory.GetFiles(matchesDirectory, "*.json");
            matchDataCount = files.Length; // 기존 파일 수로 초기화 시키기
        }
        else
        {
            matchDataCount = 0;
        }
    }
    
    public List<MatchData> LoadMatches(string nickname)
    {
        string matchesDirectory = Path.Combine(Application.persistentDataPath, "matches"); //유저의 PC나 모바일 등에서 앱 데이터를 저장할 수 있는 안전한 경로
        if (!Directory.Exists(matchesDirectory)) return new List<MatchData>();

        string[] files = Directory.GetFiles(matchesDirectory, $"{nickname}_*.json");
        //matches 폴더 안에 있는 .json 확장자를 가진 파일 중 닉네임으로 시작하는 .json 파일만 가져오게 변경. 

        List<MatchData> matches = new List<MatchData>();
        foreach (string file in files)
        {
            try // 파일 손상 or 형식이 안맞는 경우를 대비한 예외 처리
            {
                string json = File.ReadAllText(file);
                MatchData data = JsonUtility.FromJson<MatchData>(json); //string을 json 형식으로 바꿔서 저장
                if (data != null) matches.Add(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"파일 로드 실패: {file}, 오류: {e.Message}");
            }
        }

        return matches; //여기서 다시 MatchData형식으로 저장되어서 내보내짐.
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}