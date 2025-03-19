using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace yu_namespace
{
    public class MessagePopupController : BaseUIController
    {
        [SerializeField] private TextMeshProUGUI messageText;

        //메세지 바꾸기
        public void Show(string message)
        {
            base.Show();
            messageText.text = message;
        }
        
        
    }
    
}

