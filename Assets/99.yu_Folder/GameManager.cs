using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

namespace yu_namespace
{
    public class GameManager : MonoBehaviour
    {
        
        [SerializeField] private GameObject MessagePopupPrefab;
        //[SerializeField] private GameObject SettingPopupPrefab;
        [SerializeField] private RectTransform parent;
        
        //메시지팝업 오픈
        public void OpenMessagePopup(string msg)
        {
            var messagePopup = Instantiate(MessagePopupPrefab, parent);
            messagePopup.GetComponent<PopupController>().Show(msg);
        }

        //설정팝업 오픈
        public void OpenSettingPopup()
        {
            
        }

        //테스트팝업오픈
        public void TestShowButton()
        {
            OpenMessagePopup("로그인이 되었습니다.");
        }

        

    }
}

