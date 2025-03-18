using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

namespace yu_namespace
{
    public class GameManager : Singleton<GameManager>
    {
        
        [SerializeField] private GameObject messagePopupPrefab;
        [SerializeField] private GameObject settingPopupPrefab;
        [SerializeField] private GameObject giveupPanelPrefab;
        [SerializeField] private GameObject noCoinPanelPrefab;
        
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

