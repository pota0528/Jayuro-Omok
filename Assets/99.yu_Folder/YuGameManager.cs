using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

namespace yu_namespace
{
    public class YuGameManager : YuSingleton<YuGameManager>
    {
        
        [SerializeField] private GameObject messagePopupPrefab;
        [SerializeField] private GameObject settingPopupPrefab;
        [SerializeField] private GameObject giveupPanelPrefab;
        [SerializeField] private GameObject noCoinPanelPrefab;
        [SerializeField] private GameObject winLosePanelPrefab;
        
        [SerializeField] private RectTransform parent;
       
        
        //메시지팝업 오픈
        public void OpenMessagePopup(string msg)
        {
            
            var messagePopup = Instantiate(messagePopupPrefab, parent);
            messagePopup.GetComponent<MessagePopupController>().Show(msg);
            
        }

        //설정팝업 오픈
        public void OpenSettingPopup()
        {
            var settingPopup = Instantiate(settingPopupPrefab, parent);
            settingPopup.GetComponent<BaseUIController>().Show();
        }
        
        //옵션 패널 오픈
        public void OpenGiveupPanel()
        {
            var giveupPanel = Instantiate(giveupPanelPrefab, parent);
            giveupPanel.GetComponent<BaseUIController>().Show();

        }
        
        //노코인 패널 오픈
        public void OpenNoCoinPanel()
        {
            var noCoinPanel = Instantiate(noCoinPanelPrefab, parent);
            noCoinPanel.GetComponent<BaseUIController>().Show();
            noCoinPanel.GetComponent<NoCoinController>().ShowCoinText(YuConstants.coin);//찬영님이 주시는 데이터 형태로
        }

        public void OpenWinLosePanel()
        {
            var winLosePanel = Instantiate(winLosePanelPrefab, parent);
            winLosePanel.GetComponent<WinLosePanelController>().ShowCoinText(YuConstants.coin);
            int currentLevelCount = winLosePanel.GetComponent<WinLosePanelController>().GetLevelCount(YuConstants.level);//안에 들어가는 수는 level
            bool resultPanel = winLosePanel.GetComponent<WinLosePanelController>().SetResultPanel(currentLevelCount, YuConstants.levelPoint);
                
            if (resultPanel == false)
            {
                Debug.Log("오케이");
                var ResultPanel= Instantiate(messagePopupPrefab, parent);
                if (YuConstants.levelPoint > 0)
                {
                    ResultPanel.GetComponent<MessagePopupController>().Show("승급하셨습니다.");
                }
                else if (YuConstants.levelPoint < 0)
                {
                    ResultPanel.GetComponent <MessagePopupController>().Show("강등되셨습니다.");
                }
                
            }

            
        }

        //테스트팝업오픈
        public void TestShowMessagePopupButton()
        {
            OpenMessagePopup("로그인이 되었습니다.");
        }

        public void TestShowSettingPopupButton()
        {
            OpenSettingPopup();
        }

        public void TestGiveupPanelButton()
        {
            OpenGiveupPanel();
        }

        public void TestNoCoinPanelButton()
        {
            OpenNoCoinPanel();
        }

        

        

    }
}

