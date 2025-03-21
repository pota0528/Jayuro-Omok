
using UnityEngine;
using DG.Tweening;
using TMPro;

//캔버스 그룹이 꼭 있어야한다.
    [RequireComponent(typeof(CanvasGroup))]
    public class PanelController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;

//팝업창 
        [SerializeField] private RectTransform panelRectTransform;
        //배경   
        private CanvasGroup _backgroundcanvasGroup;

        public delegate void PanelControllerHideDelegate();
    
        protected virtual void Awake()
        {
            _backgroundcanvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetTitleText(string title)
        {
            titleText.text = title;
        }
        
        
        /// <summary>
        /// panel 표시 함수 
        /// </summary>
        public void Show()
        {
            _backgroundcanvasGroup.alpha = 0;
            panelRectTransform.localScale = Vector3.zero;

            _backgroundcanvasGroup.DOFade(1, 0.5f).SetEase(Ease.Linear);
            panelRectTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        }

        /// <summary>
        /// 패널 숨기기 함수 
        /// </summary>
        public void Hide(PanelControllerHideDelegate hideDelegate = null)
        {
            _backgroundcanvasGroup.alpha = 1;
            panelRectTransform.localScale = Vector3.one;
     
            _backgroundcanvasGroup.DOFade(0, 0.5f).SetEase(Ease.Linear);
            panelRectTransform.DOScale(0, 0.5f)
                .SetEase(Ease.InBack).OnComplete(() =>
                {
                    hideDelegate?.Invoke();
                    Destroy(gameObject);
                });
     
        }
    }


