using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(CanvasGroup))]
public class BaseUIController : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;

    public delegate void BaseHideDelegate();

    //패널 보이기
    public void Show()
    {
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(1, 0.3f).SetEase(Ease.Flash);
    }

    //패널 숨기다 지우기
    public void Hide(BaseHideDelegate baseHideDelegate = null)
    {
        rectTransform.DOScale(0, 0.3f).SetEase(Ease.Flash).OnComplete(() =>
        {
            baseHideDelegate?.Invoke();
            Destroy(gameObject);
        });
    }

    public void OnConfirmButtonClick()
    {
        Hide();
    }

    public void OnClickExitButton()
    {
        Hide(() =>
        {
            //TODO: 메인씬+프로필패널로 이동
            SceneManager.LoadScene("Main");
        });
    }
}


