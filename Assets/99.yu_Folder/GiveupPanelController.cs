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
            //TODO: 메인씬+프로필패널로 이동
            SceneManager.LoadScene("Main");
            //todo: 승점포인트 -1처리하기
            playerData.levelPoint--;
        });
    }

    private void OnDestroy()
    {
        DBManager.Instance.UpdatePlayerData(playerData);
        UserSessionManager.Instance.SetPlayerData(playerData);
    }
}



