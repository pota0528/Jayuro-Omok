using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopPanelController : PanelController
{
    [SerializeField] private CoinPanelController coinPanelController;
    [SerializeField] private GameObject shopItemButtonPrefab; // 동적으로 생성할 버튼 프리팹
    [SerializeField] private Transform shopItemContainer;     // 버튼이 쌓일 부모 오브젝트

    private PlayerData _playerData;
    private int _pendingPurchaseIndex = -1;

    // 각 버튼에 대응하는 코인 금액
    private readonly int[] coinAmounts = { 100, -100, 200, 400, 600, 800, 1000, -1000, 1500, 2000 };

    private void Start()
    {
        SetTitleText("코인상점");
        _playerData = UserSessionManager.Instance.GetPlayerData();
        GenerateShopButtons();
    }

    public void OnClickCloseButton()
    {
        Hide();
    }

    /// <summary>
    /// 버튼을 동적으로 생성
    /// </summary>
    private void GenerateShopButtons()
    {
        for (int i = 0; i < coinAmounts.Length; i++)
        {
            int index = i; // 캡처 방지

            GameObject buttonObj = Instantiate(shopItemButtonPrefab, shopItemContainer);

            // 텍스트 설정
            TMP_Text text = buttonObj.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.text = $"{coinAmounts[i]} 냥";
            }

            // 버튼 이벤트 연결
            Button btn = buttonObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnClickShopItemButton(index));
            }
        }
    }

    /// <summary>
    /// 버튼 클릭 시 호출되는 함수 (팝업 띄움)
    /// </summary>
    public void OnClickShopItemButton(int index)
    {
        _pendingPurchaseIndex = index;

        int amount = coinAmounts[index];
        //SetTitleText($"구매의사");
        string message = $"충전 : {amount}냥";

        UIManager.Instance.ShowConfirmPopup(message, OnConfirmPurchase);
    }

    /// <summary>
    /// 팝업에서 "확인" 클릭 시 호출
    /// </summary>
    private void OnConfirmPurchase()
    {
        if (_pendingPurchaseIndex < 0 || _pendingPurchaseIndex >= coinAmounts.Length) return;

        int amount = coinAmounts[_pendingPurchaseIndex];

        UIManager.Instance.UpdateCoin(amount);
        coinPanelController.InitCoinCount(_playerData.coin);
        _pendingPurchaseIndex = -1;

        // 자현 추가: 게임 씬일 경우 패널 갱신
        if (SceneManager.GetActiveScene().name == "Game")
        {
            SetWinLosePanel(UIManager.Instance.winLosePanelObject);
        }
    }

    // 자현 추가
    public void SetWinLosePanel(GameObject winLosePanel)
    {
        winLosePanel.GetComponent<WinLosePanelController>().ShowCoinText(_playerData.coin);
    }
}
