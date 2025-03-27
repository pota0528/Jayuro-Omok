using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopPanelController : PanelController
{
    [SerializeField] private CoinPanelController coinPanelController;
    private PlayerData _playerData;

    private int _pendingPurchaseIndex = -1;

    private void Start()
    {
        SetTitleText("전당포");
        _playerData = UserSessionManager.Instance.GetPlayerData();
    }

    public void OnClickCloseButton()
    {
        Hide();
    }

    public void OnClickShopItemButton(int index)
    {
        _pendingPurchaseIndex = index;

        int amount = 0;

        switch (index)
        {
            case 0: amount = 100; break;
            case 1: amount = -100; break; 
            case 2: amount = 300; break;
            case 3: amount = 400; break;
            case 4: amount = 500; break;
            case 5: amount = 600; break;
            case 6: amount = 700; break;
            case 7: amount = 800; break;
            case 8: amount = 900; break;
            case 9: amount = 1000; break;
            default:
                Debug.LogWarning("잘못된 인덱스입니다.");
                return;
        }

        string message = $"충전 : {amount}냥";
        UIManager.Instance.ShowConfirmPopup(message, OnConfirmPurchase);
    }

    private void OnConfirmPurchase()
    {
        int amount = 0;

        switch (_pendingPurchaseIndex)
        {
            case 0: amount = 100; break;
            case 1: amount = -100; break;
            case 2: amount = 300; break;
            case 3: amount = 400; break;
            case 4: amount = 500; break;
            case 5: amount = 600; break;
            case 6: amount = 700; break;
            case 7: amount = 800; break;
            case 8: amount = 900; break;
            case 9: amount = 1000; break;
            default:
                Debug.LogWarning("잘못된 구매 인덱스입니다.");
                return;
        }

        UIManager.Instance.UpdateCoin(amount);
        coinPanelController.InitCoinCount(_playerData.coin);
        _pendingPurchaseIndex = -1;

        // 자현 추가: 게임 씬일 경우 WinLosePanel 동기화
        if (SceneManager.GetActiveScene().name == "Game")
        {
            SetWinLosePanel(UIManager.Instance.NoCoinNextWinLosePanel);
        }
    }

    // 자현 추가
    public void SetWinLosePanel(GameObject winLosePanel)
    {
        winLosePanel.GetComponent<WinLosePanelController>().ShowCoinText(_playerData.coin);
    }
}
