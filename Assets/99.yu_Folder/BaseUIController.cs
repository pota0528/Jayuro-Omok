using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace yu_namespace
{
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
            rectTransform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                baseHideDelegate?.Invoke();
                Destroy(gameObject);
            });
    }

    }
}

