using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace park_namespace
{
    public class PlayerManager : MonoBehaviour
    {

        public string nickname;
        public string id;
        public string password;
        public int level;
        public int levelPoint;
        public int coin;
        public int win;
        public int lose;

        public void SetPlayerData(PlayerData playerData)
        {
            nickname = playerData.nickname;
            id = playerData.id;
            password = playerData.password;
            level = playerData.level;
            levelPoint = playerData.levelPoint;
            coin = playerData.coin;
            win = playerData.win;
            lose = playerData.lose;
        }
    }
}

