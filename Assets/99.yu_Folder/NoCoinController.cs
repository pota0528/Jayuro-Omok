using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using yu_namespace;

namespace yu_namespace
{
    public class NoCoinController : BaseUIController
    {
        [SerializeField] private TextMeshProUGUI coinText;
        
        public void ShowCoinText(int coin)//찬영님이 주시는 데이터 형태로 넣기
        {
            coinText.text = coin.ToString();
        }
        
        public void OnClickShopButton()
        {
            Hide(() =>
            {
                //TODO: 메인씬+상점패널로 이동
                //장운님이 만드신 GameManager.Instance.OpenShopPanel();호출
                Debug.Log("상점으로 이동");
                YuGameManager.Instance.OpenGiveupPanel();
                
                
            });
        }
    
        
    }
}


