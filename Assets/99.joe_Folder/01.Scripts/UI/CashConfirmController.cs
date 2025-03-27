using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashConfirmController : PanelController
{
    [SerializeField] private CoinPanelController coinPanelController;
    private PlayerData _playerData;
    //private UserPanelController userPanelController;


    public void OnClickConfirmButton()
    {
        Hide(() =>
        {
            int amount = 0;
            if (_playerData.coin >= 0)
            {
                amount += 100;
                UIManager.Instance.UpdateCoin(amount);
                coinPanelController.InitCoinCount(_playerData.coin);
                        
            }
        });
    }


    public void OnClickCancelButton()
    {
        Hide();
    }
    
    
}
