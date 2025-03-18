using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace Joe_namespace
{

    [RequireComponent(typeof(CanvasGroup))]

    public class PanelController : MonoBehaviour
    {
        //[SerializeField] private TMP_Text titleText;
        //[SerializeField] private GameObject panelObject;
        [SerializeField] private RectTransform panelRectTransform;

        private CanvasGroup _backgroundCanvasGroup;

        public delegate void PanelControllerHideDelegate();



        private void Awake()
        {
            _backgroundCanvasGroup = GetComponent<CanvasGroup>();
        }


        // public void SetTitleText(string title)
        // {
        //     titleText.text = title;
        // }

        // public void OnClickCloseButton()
        // {
        //     HidePanel();
        // }



        public void ShowPanel()
        {

            
            _backgroundCanvasGroup.alpha = 1;
            panelRectTransform.localScale = Vector3.one;
            
            
            _backgroundCanvasGroup.DOFade(1f, 0.3f).SetEase(Ease.Linear);
            panelRectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        public void HidePanel(PanelControllerHideDelegate hideDelegate = null)
        {

            _backgroundCanvasGroup.alpha = 0;
            panelRectTransform.localScale = Vector3.zero;

            _backgroundCanvasGroup.DOFade(0f, 0.3f).SetEase(Ease.Linear);
            panelRectTransform.DOScale(0f, 0.3f)
                .SetEase(Ease.InBack).OnComplete(() =>
                {
                    hideDelegate?.Invoke();
                    Destroy(gameObject);
                });
        }


    }

}
