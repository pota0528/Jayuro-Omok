using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private Image playerAImage;
    [SerializeField] private Image playerBImage;
    [SerializeField] private Button gameOverButton;

    public enum GameUIMode
    {
        Init,
        TurnA,
        TurnB,
        GameOver
    }
    
    private const float DisableAlpha = 0.5f;
    private const float EnableAlpha = 1f;

    public void SetGameUIMode(GameUIMode mode)
    {
        switch (mode)
        {
            case GameUIMode.Init:
                playerAImage.color = new Color32(221, 221, 221, 255);
                playerBImage.color = new Color32(221, 221, 221, 255);
                break;
            case GameUIMode.TurnA:
                playerAImage.color = new Color32(105, 255, 132, 255);
                playerBImage.color = new Color32(221, 221, 221, 255);
                break;
            case GameUIMode.TurnB:
                playerAImage.color = new Color32(221, 221, 221, 255);
                playerBImage.color = new Color32(105, 255, 132, 255);
                break;
            case GameUIMode.GameOver:
                playerAImage.gameObject.SetActive(false);
                playerBImage.gameObject.SetActive(false);
                break;
        }
    }

    // public void OnClickGameOverButton()
    // {
    //     GameManager.Instance.OpenConfirmPanel("게임을 종료하시겠습니까?", () =>
    //     {
    //         GameManager.Instance.ChangeToMainScene();
    //     });
    // }

    // public void OnClickSettingsButton() 
    // {
    //     GameManager.Instance.OpenSettingsPanel();
    // }

    // public void OnClickBackButton()
    // {
    //     GameManager.Instance.OpenConfirmPanel("게임을 종료하시겠습니까?", () =>
    //     {
    //         GameManager.Instance.ChangeToMainScene();
    //     });
    // }
}