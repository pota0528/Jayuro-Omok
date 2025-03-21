using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Joe_namespace
{
    public class ShopPanelController : PanelController
    {
        [SerializeField] private CoinPanelController coinPanelController;
        
        private void Start()
        {
            SetTitleText("SHOP");
        }

        public void OnClickCloseButton()
        {
            Hide();
        }
        
        public void OnClickShopItemButton(int index)
        {
            switch (index)
            {
                case 0:
                    if (GameManager.Instance.coinCount >= 0)
                    {
                        GameManager.Instance.coinCount += 100;
                        coinPanelController.InitCoinCount(GameManager.Instance.coinCount);
                    }
                
                    break;
                case 1:
                    if (GameManager.Instance.coinCount >= 0)
                    {
                        GameManager.Instance.coinCount += 200;
                        coinPanelController.InitCoinCount(GameManager.Instance.coinCount);
                    }
                    
                    break;
                case 2:
                    if (GameManager.Instance.coinCount >= 0)
                    {
                        GameManager.Instance.coinCount += (int)300f;
                        coinPanelController.InitCoinCount(GameManager.Instance.coinCount);
                    }
                
                    break;
                case 3:
                    if (GameManager.Instance.coinCount >= 0)
                    {
                        GameManager.Instance.coinCount += (int)400f;
                        coinPanelController.InitCoinCount(GameManager.Instance.coinCount);
                    }
                
                    break;
                case 4:
                    if (GameManager.Instance.coinCount >= 0)
                    {
                        GameManager.Instance.coinCount += (int)500f;
                        coinPanelController.InitCoinCount(GameManager.Instance.coinCount);
                    }
                
                    break;
                case 5:
                    if (GameManager.Instance.coinCount >= 0)
                    {
                        GameManager.Instance.coinCount += (int)600f;
                        coinPanelController.InitCoinCount(GameManager.Instance.coinCount);
                    }
                
                    break;
                case 6:
                    if (GameManager.Instance.coinCount >= 0)
                    {
                        GameManager.Instance.coinCount += (int)700f;
                        coinPanelController.InitCoinCount(GameManager.Instance.coinCount);
                    }
                
                    break;
                case 7:
                    if (GameManager.Instance.coinCount >= 0)
                    {
                        GameManager.Instance.coinCount += (int)800f;
                        coinPanelController.InitCoinCount(GameManager.Instance.coinCount);
                    }
                
                    break;
                case 8:
                    if (GameManager.Instance.coinCount >= 0)
                    {
                        GameManager.Instance.coinCount += (int)900f;
                        coinPanelController.InitCoinCount(GameManager.Instance.coinCount);
                    }
                
                    break;
                case 9:
                    if (GameManager.Instance.coinCount >= 0)
                    {
                        GameManager.Instance.coinCount += (int)1000f;
                        coinPanelController.InitCoinCount(GameManager.Instance.coinCount);
                    }
                    
                    break;
           
            }
        }
        
        
        
    }

}
