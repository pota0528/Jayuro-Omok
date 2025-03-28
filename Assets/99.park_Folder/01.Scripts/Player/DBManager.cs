using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using UnityEngine;
using UnityEngine.SceneManagement;
using BCrypt.Net;

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
                //비밀번호 해시화
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(playerData.password);
                //해시된 비밀번호로 새 playerData 생성 
                playerData.password = hashedPassword;
                
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

        public (PlayerData, string) Login(string id, string password)
        {
            var player = playerCollection.Find(p => p.id == id).FirstOrDefault();

            if (player == null)
            {
                // 아이디가 존재하지 않으면
                UIManager.Instance.OpenMessagePopup("아이디가 존재하지 않습니다.");
                return (null, "아이디가 존재하지 않습니다.");
               
            }

            //암호화 된 비밀번호 검증 
            if (!BCrypt.Net.BCrypt.Verify(password, player.password))
            {
                // 비밀번호가 틀리면
                UIManager.Instance.OpenMessagePopup("비밀번호가 틀렸습니다..");
                return (null, "비밀번호가 틀렸습니다.");
               
            }

            // 아이디와 비밀번호가 일치하면 로그인 성공
            Debug.Log("로그인 성공 닉네임: " + player.nickname);
            return (player, "로그인 성공");
        }

        //데이터 업데이트
        public void UpdatePlayerData(PlayerData playerData,bool updateImageOnly=false)
        {
            if (playerCollection == null)
            {
                Debug.LogError("DB 연결 안됨.");
                return;
            }
            //유저 아이디로 해당 유저 찾기 (id 기준으로 찾고있음)   (조건, 데이터) 
            var filter = Builders<PlayerData>.Filter.Eq(p => p.id, playerData.id);
            UpdateDefinition<PlayerData> update;
            
            
            
            
            //프로필 이미지 인덱스만 업데이트 
            if (updateImageOnly)
            {
                update = Builders<PlayerData>.Update.Set(p => p.imageIndex, playerData.imageIndex);
            }
            //업데이트할 내용 설정
            else
            {
                update = Builders<PlayerData>.Update
                    .Set(p => p.coin, playerData.coin)
                    .Set(p => p.level, playerData.level)
                    .Set(p => p.levelPoint, playerData.levelPoint)
                    .Set(p => p.win, playerData.win)
                    .Set(p => p.lose, playerData.lose)
                    .Set(p => p.imageIndex, playerData.imageIndex);
            }


            //필터에 맞는 유저 데이터 업데이터
                var result = playerCollection.UpdateOne(filter, update);

                if (result.MatchedCount > 0)
                {
                    Debug.Log(playerData.nickname + " 의 데이터 업데이트 완료.");
                }
                else
                {
                    Debug.Log("유저 데이터 업데이트 실패");
                }


            
        }
  

        
        public List<PlayerData> GetAllPlayers()
        {
            if (playerCollection == null)
            {
                Debug.LogError("DB 연결 안됨.");
                return new List<PlayerData>();
            }

            return playerCollection.Find(_ => true).ToList();
        }
        

        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
           
        }
    }


