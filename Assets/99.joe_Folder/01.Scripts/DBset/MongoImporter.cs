using System.Collections.Generic;
using System.IO;
using MongoDB.Driver;
using UnityEditor;
using UnityEngine;

public static class JsonUtilityWrapper
{
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
    }

    public static List<T> FromJsonList<T>(string json)
    {
        string wrapped = "{\"items\":" + json + "}";
        return JsonUtility.FromJson<Wrapper<T>>(wrapped)?.items;
    }
}

public class MongoImporter
{
    //[MenuItem("Tools/Import Players JSON to MongoDB")]
    public static void ImportJsonToMongoDB()
    {
        string filePath = Path.Combine(Application.dataPath, "players.json");

        if (!File.Exists(filePath))
        {
            Debug.LogError("❌ 파일 없음: " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        List<PlayerDataForJson> rawPlayers = JsonUtilityWrapper.FromJsonList<PlayerDataForJson>(json);

        if (rawPlayers == null || rawPlayers.Count == 0)
        {
            Debug.LogWarning("⚠️ 데이터 없음");
            return;
        }

        // 변환
        List<PlayerData> players = new List<PlayerData>();
        foreach (var p in rawPlayers)
        {
            players.Add(new PlayerData
            {
                id = p.id,
                nickname = p.nickname,
                password = p.password,
                level = p.level,
                levelPoint = p.levelPoint,
                coin = p.coin,
                win = p.win,
                lose = p.lose,
                //score = p.score,
                imageIndex = p.imageIndex
            });
        }

        var client = new MongoClient("mongodb://localhost:27017");
        var db = client.GetDatabase("Jayuro_omok");
        var collection = db.GetCollection<PlayerData>("PlayerData");

        collection.DeleteMany(Builders<PlayerData>.Filter.Empty);
        collection.InsertMany(players);

        Debug.Log($"✅ MongoDB에 유저 {players.Count}명 삽입 완료!");
    }
}