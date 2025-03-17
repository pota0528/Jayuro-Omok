using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using yu_namespace;

namespace yu_namespace
{
    
    public class GiveupPanelController : BaseUIController
    {
        public void OnClickGiveupButton()
        {
            Hide(() =>
            {
                //TODO: 메인씬으로 이동
                //SceneManager.LoadScene("");
                Debug.Log("메인으로 이동");
                
            });
        }
    }
}


