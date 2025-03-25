using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;


    [System.Serializable] 
    public class PlayerData 
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string nickname{get;set;}
      
        public string password { get; set; }
        public int level { get; set; } 
        public int levelPoint { get; set; } 
        public int coin { get; set; }
        public int win { get; set; } 
        public int lose { get; set; } 
        
        public int imageIndex { get; set; }
      //  public List<GameRecord> gameRecords { get; set; } = new List<GameRecord>(); 기보 
    }
    
    public class GameRecord
    {
        //기보 정보 
    }

    

    
