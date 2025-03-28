using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartTitlePanelController : Singleton<StartTitlePanelController>
{
    [SerializeField] private TextMeshProUGUI subText;
    [SerializeField] private TextMeshProUGUI titleOmokText;
    [SerializeField] private TextMeshProUGUI titleWatchText;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject Clock;
    [SerializeField] private Transform Hour;
    [SerializeField] private Transform Minute;
    [SerializeField] private Transform Second;
    [SerializeField] private TextMeshProUGUI companyText;
    private Animator anim;
    public Sprite soundOffSprite;
    public Sprite soundOnSprite;
    public Button soundButton;
    private delegate void ClockTimeDelegate();
    private ClockTimeDelegate clockDelegate;
    
    private void Awake()
    {
        TitleInit();
    }

    private void Start()
    {
        TitleSetting();
        BGMInit();
    }

    private void BGMInit()
    {
        float savedBGMVolume = PlayerPrefs.GetFloat("BGMParam");
        if (savedBGMVolume <= 0.01f)//슬라이드가 최솟값이라면
        {
            soundButton.GetComponent<Image>().sprite = soundOffSprite;
            
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = soundOnSprite;
        }
    }
    
    public void BGMCheck()
    {
        float savedBGMVolume = PlayerPrefs.GetFloat("BGMParam");
        if (savedBGMVolume <= 0.01f)//슬라이드가 최솟값이라면
        {
            soundButton.GetComponent<Image>().sprite = soundOnSprite;
            PlayerPrefs.SetFloat("BGMParam", 0.5f);
            PlayerPrefs.Save();
            AudioManager.Instance.SetBGMVolume(PlayerPrefs.GetFloat("BGMParam"));
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = soundOffSprite;
            PlayerPrefs.SetFloat("BGMParam", 0.001f);
            PlayerPrefs.Save();
            AudioManager.Instance.SetBGMVolume(PlayerPrefs.GetFloat("BGMParam"));
       
        }
    }
    
    private void Update()
    {
        clockDelegate?.Invoke();
    }
    
    
    public void TitleInit()
    {
        subText.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0);
        companyText.GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0);
        titleOmokText.transform.localScale = Vector3.zero;
        titleWatchText.transform.localScale = Vector3.zero;
        fillImage.fillAmount = 1;
        anim = Clock.GetComponent<Animator>();
    }

    public void TitleSetting()
    {
        subText.DOFade(1f, 2f).OnComplete(() =>
        {
            Sequence sequence1 = DOTween.Sequence();
            sequence1.Join(titleOmokText.transform.DOScale(Vector3.one, 3f))
                .Join(titleOmokText.transform.DORotate(new Vector3(0, 0, 360), 3f, RotateMode.FastBeyond360))
                .OnComplete(() =>
                {
                    titleWatchText.transform.DOScale(Vector3.one, 2f).OnComplete(() =>
                    {
                        Sequence sequence2 = DOTween.Sequence();
                        sequence2.Join(titleWatchText.transform.DOScale(new Vector3(1.3f,1.3f,1.3f), 0.3f))
                            .Join(titleWatchText.DOFade(0.5f, 0.3f)).OnComplete(() =>
                            {
                                Sequence sequence3 = DOTween.Sequence();
                                sequence3.Join(titleWatchText.transform.DOScale(Vector3.one, 0.3f))
                                    .Join(titleWatchText.DOFade(1f, 0.3f));
                                anim.SetTrigger("OnClock");
                                clockDelegate += ClockStart;

                            });

                    });
                    DOVirtual.DelayedCall(4f, () =>
                    {
                        companyText.DOFade(1, 2f);
                    });

                });
            
            });
    }
    
    private void ClockStart()
    {
        DateTime now = DateTime.Now; // 현재 시간 가져오기

        float secondAngle = now.Second * 6f; // 초침 각도
        float minuteAngle = now.Minute * 6f; // 분침 각도
        float hourAngle = (now.Hour % 12) * 30f + now.Minute * 0.5f; // 시침 각도

        // 바늘 회전 적용 (Z축 기준)
        Second.localRotation = Quaternion.Euler(0, 0, -secondAngle);
        Minute.localRotation = Quaternion.Euler(0, 0, -minuteAngle);
        Hour.localRotation = Quaternion.Euler(0, 0, -hourAngle);
    }
    
    private void OnDestroy()
    {
        clockDelegate-=ClockStart;
        DOTween.KillAll();
    }
    
    
}
