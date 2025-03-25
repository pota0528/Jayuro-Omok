using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainWinLosePanelController : MonoBehaviour
{
    [Header("mainPanel")] 
    [SerializeField] private TextMeshProUGUI mainLoseText;
    [SerializeField] private Image mainWinImage;
    [SerializeField] private Image mainLoseImage;
    [SerializeField] private TextMeshProUGUI mainWinText;

    private void Awake()
    {
        mainLoseText.DOFade(0f,0);
        mainLoseImage.DOFade(0f,0);
        mainLoseImage.DOFillAmount(0f, 0f);
        
        mainWinText.DOFade(0f,0);
        mainWinImage.DOFillAmount(0f,0f);
    }

    public void MainLosePanelOpen()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(mainLoseImage.DOFillAmount(1f, 2f)).Join(mainLoseImage.DOFade(1f, 2f)).OnComplete(() =>
        {
            mainLoseText.DOFade(1f, 1f);
        });
    }
    
    public void MainWinPanelOpen()
    {
        Sequence sequence2 = DOTween.Sequence();
        sequence2.Join(mainWinImage.DOFillAmount(1f, 2f)).Join(mainWinImage.DOFade(1f, 2f)).OnComplete(() =>
        {
            mainWinText.DOFade(1f, 1f);
        });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
