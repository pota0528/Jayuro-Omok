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
    [SerializeField] private TextMeshProUGUI mainResultText;
    [SerializeField] private Image mainWinImage;
    [SerializeField] private Image mainLoseImage;

    private void Awake()
    {
        mainResultText.DOFade(0f,0);
        mainLoseImage.DOFade(0f,0);
    }

    public void MainLosePanelOpen()
    {
        mainResultText.text = "패배!";
        mainLoseImage.DOFade(1f, 5f).OnComplete(() =>
        {
            mainResultText.DOFade(1f, 5f);

        });
    }

    public void MainWinPanelOpen()
    {
        
    }
}
