using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class NoCoinController : BaseUIController
{
    [SerializeField] private TextMeshProUGUI coinText;
    UserPanelController userPanelController;
    PlayerData playerData;

    private void Awake()
    {
        playerData = UserSessionManager.Instance.GetPlayerData();
    }

    public void ShowCoinText(int coin) //찬영님이 주시는 데이터 형태로 넣기
    {
        coinText.text = coin.ToString();
        playerData.coin = coin;
        
    }

    public void OnClickShopButton()
    {
        Hide(() =>
        {
            //TODO: 메인씬+상점패널로 이동
            Debug.Log("상점으로 이동");
            UIManager.Instance.OpenShopPanel();
            userPanelController.UpdataUI();
        });
    }

    private void OnDestroy()
    {
        UserSessionManager.Instance.SetPlayerData(playerData);
        DBManager.Instance.UpdatePlayerData(playerData);
    }
}