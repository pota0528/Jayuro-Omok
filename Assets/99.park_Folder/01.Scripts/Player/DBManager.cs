using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class DBManager : Singleton<DBManager>
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

            var existingPlayer = playerCollection.Find(p => p.id == playerData.id).FirstOrDefault();
            if (existingPlayer == null)
            {
                playerCollection.InsertOne(playerData);
                Debug.Log("유저 등록 완료: " + playerData.nickname);
                
                //회원가입 완료 메시지 팝업
                UIManager.Instance.OpenMessagePopup(playerData.nickname+"님\n회원가입 되었습니다.");
            }
            else
            {
                Debug.Log("이미 존재하는 ID 입니다. ");
                //이미 존재 id 메시지 팝업
                UIManager.Instance.OpenMessagePopup("이미 존재하는 ID 입니다.");
            }
        }

        public PlayerData Login(string id, string password)
        {
            var player = playerCollection.Find(p => p.id == id && p.password == password).FirstOrDefault();
            if (player != null)
            {
                Debug.Log("로그인 성공 닉네임: " + player.nickname);
                return player;
            }
            else
            {
                Debug.Log("로그인 실패");
                //TODO:아이디 틀렸을떄, 비밀번호 틀렸을 떄 
                UIManager.Instance.OpenMessagePopup("로그인에 실패하였습니다.");
                return null;
            }
        }

        //데이터 업데이트
        public void UpdatePlayerData(PlayerData playerData)
        {
            if (playerCollection == null)
            {
                Debug.LogError("DB 연결 안됨.");
                return;
            }
            //유저 아이디로 해당 유저 찾기 (id 기준으로 찾고있음)   (조건, 데이터) 
            var filter = Builders<PlayerData>.Filter.Eq(p => p.id, playerData.id);
            
            //업데이트할 내용 설정
            var update = Builders<PlayerData>.Update
                .Set(p => p.coin, playerData.coin)
                .Set(p => p.level, playerData.level)
                .Set(p => p.levelPoint, playerData.levelPoint)
                .Set(p => p.win, playerData.win)
                .Set(p => p.lose, playerData.lose)
                .Set(p => p.imageIndex, playerData.imageIndex);
            
            //필터에 맞는 유저 데이터 업데이터
            var result = playerCollection.UpdateOne(filter, update);

            if (result.MatchedCount > 0)
            {
                Debug.Log(playerData.nickname+" 의 데이터 업데이트 완료.");
            }
            else
            {
                Debug.Log("유저 데이터 업데이트 실패");
            }


        }

      

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
           
        }
    }


