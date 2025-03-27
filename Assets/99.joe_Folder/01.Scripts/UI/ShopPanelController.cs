using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class ShopPanelController : PanelController
    {
        [SerializeField] private CoinPanelController coinPanelController;
        private PlayerData _playerData;
        private UserPanelController userPanelController;
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
            int amount = 0;
            switch (index)
            {
                
                case 0:
                    if (_playerData.coin >= 0)
                    {
                        amount += 100;
                        UIManager.Instance.UpdateCoin(amount);
                        coinPanelController.InitCoinCount(_playerData.coin);
                        
                    }
                
                    break;
                case 1:
                    if (_playerData.coin >= 0)
                    {
                        amount -= 100;
                        UIManager.Instance.UpdateCoin(amount);
                        coinPanelController.InitCoinCount(_playerData.coin);
                        
                    }
                    
                    break;
                case 2:
                    if (UIManager.Instance.coinCount >= 0)
                    {
                        UIManager.Instance.coinCount += (int)300f;
                        coinPanelController.InitCoinCount(UIManager.Instance.coinCount);
                    }
                
                    break;
                case 3:
                    if (UIManager.Instance.coinCount >= 0)
                    {
                        UIManager.Instance.coinCount += (int)400f;
                        coinPanelController.InitCoinCount(UIManager.Instance.coinCount);
                    }
                
                    break;
                case 4:
                    if (UIManager.Instance.coinCount >= 0)
                    {
                        UIManager.Instance.coinCount += (int)500f;
                        coinPanelController.InitCoinCount(UIManager.Instance.coinCount);
                    }
                
                    break;
                case 5:
                    if (UIManager.Instance.coinCount >= 0)
                    {
                        UIManager.Instance.coinCount += (int)600f;
                        coinPanelController.InitCoinCount(UIManager.Instance.coinCount);
                    }
                
                    break;
                case 6:
                    if (UIManager.Instance.coinCount >= 0)
                    {
                        UIManager.Instance.coinCount += (int)700f;
                        coinPanelController.InitCoinCount(UIManager.Instance.coinCount);
                    }
                
                    break;
                case 7:
                    if (UIManager.Instance.coinCount >= 0)
                    {
                        UIManager.Instance.coinCount += (int)800f;
                        coinPanelController.InitCoinCount(UIManager.Instance.coinCount);
                    }
                
                    break;
                case 8:
                    if (UIManager.Instance.coinCount >= 0)
                    {
                        UIManager.Instance.coinCount += (int)900f;
                        coinPanelController.InitCoinCount(UIManager.Instance.coinCount);
                    }
                
                    break;
                case 9:
                    if (UIManager.Instance.coinCount >= 0)
                    {
                        UIManager.Instance.coinCount += (int)1000f;
                        coinPanelController.InitCoinCount(UIManager.Instance.coinCount);
                    }
                    
                    break;
           
            }
        }
        
        
        
    }