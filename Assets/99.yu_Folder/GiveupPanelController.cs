using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GiveupPanelController : BaseUIController
{
    private PlayerData playerData;

    private void Awake()
    {
        playerData = UserSessionManager.Instance.GetPlayerData();
    }
    public void OnClickGiveupButton()
    {
        Hide(() =>
        {
            //메인씬+프로필패널로 이동
            SceneManager.LoadScene("Login");
            //승점포인트 -1처리하기
            playerData.levelPoint--;
            DBManager.Instance.UpdatePlayerData(playerData);
            UserSessionManager.Instance.SetPlayerData(playerData);
            
            if (UIManager.Instance.currentLevelCountData - Mathf.Abs(playerData.levelPoint) < 0)//기권하여 승급포인트가 강등될 때 처리
            {
                playerData.levelPoint = 0;
                if (playerData.level < 18)
                {
                    playerData.level++;
                }
                
            }
            
            playerData.lose++;
        });
    }

    private void OnDestroy()
    {
        DBManager.Instance.UpdatePlayerData(playerData);
        UserSessionManager.Instance.SetPlayerData(playerData);
    }
}



