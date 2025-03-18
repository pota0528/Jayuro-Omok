using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

    public class ProfilePanelController : PanelController
    {
        //TODO: 버튼 클릭하면 그 이미지 정보를 저장하기 

        public void OnClickConfirmButton()
        {
            Debug.Log("확인");
            Hide();
        }
   
        public void OnClickBackButton()
        {
            Hide();
        }
    }


