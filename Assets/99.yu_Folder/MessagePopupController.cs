using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class MessagePopupController : BaseUIController
{
    [SerializeField] private TextMeshProUGUI messageText;

    public GameObject winLosePanelObject;
    public Image winLosePanelImage;
    public Image bottomHandle;
    //메세지 바꾸기
    public void Show(string message)
    {
        base.Show();
        messageText.text = message;
    }
    
    public void ShowResultPanel()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(winLosePanelImage.DOFillAmount(1f,1f))
            .Join(bottomHandle.GetComponent<RectTransform>().DOAnchorPosY(-950f,1f)).OnComplete(
                () =>
                {
                    winLosePanelObject.SetActive(true);
                });
    }
    
    
    
   
}
    


