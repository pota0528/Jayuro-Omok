using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using UnityEngine;

namespace park_namespace
{
    public class DBManager : MonoBehaviour
    {
        private MongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<PlayerData> playerCollection;

        void Start()
        {
            //MongoDB 연결
            string connectionString = "mongodb://localhost:27017"; // 로컬 MongoDB
            client = new MongoClient(connectionString);
            database = client.GetDatabase("Jayuro_omok"); // DB 이름
            playerCollection = database.GetCollection<PlayerData>("PlayerData"); // 컬렉션 이름

            if (playerCollection == null)
            {
                Debug.LogError("DB 연결 확인 요망 ");
            }
        }

        //이미 있는 아이디인지 파악 
        public void RegisterPlayer(PlayerData playerData)
        {
            if (playerCollection == null)
            {
                Debug.LogError("DB 연결 안됨");
                return;
            }

            var existingPlayer = playerCollection.Find(p=>p.id==playerData.id).FirstOrDefault();
            if (existingPlayer == null)
            {
                playerCollection.InsertOne(playerData);
                Debug.Log("유저 등록 완료: "+ playerData.nickname);
            }
            else
            {
                Debug.Log("이미 존재하는 ID 입니다. ");
            }
        }

        public PlayerData Login(string id, string password)
        {
            var player = playerCollection.Find(p=>p.id==id&&p.password==password).FirstOrDefault();
            if (player != null)
            {
                Debug.Log("로그인 성공 닉네임: "+player.nickname);
                return player;
            }
            else
            {
                Debug.Log("로그인 실패");
                //TODO:아이디 틀렸을떄, 비밀번호 틀렸을 떄 
                return null;
            }
        }
    }

}
