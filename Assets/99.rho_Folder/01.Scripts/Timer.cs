using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image[] headCapImage;
    [SerializeField] private TMP_Text[] timeText;
    
    public float CurrentTime { get; private set; }
    private bool _isPaused;
    
    public delegate void GoyaTimerDelegate();
    public GoyaTimerDelegate OnTimeout;

    private float totalTime = 30f;
    
    private void Awake()
    {
        _isPaused = false;
    }

    private void Update()
    {
        if (!_isPaused)
        {
            CurrentTime += Time.deltaTime;
            if (CurrentTime >= totalTime)
            {
                //headCapImage.gameObject.SetActive(false);
                _isPaused = true;
                
                OnTimeout?.Invoke();
            }
            else
            {
                float fillAmount = (totalTime - CurrentTime) / totalTime;
                headCapImage[0].transform.localRotation = 
                    Quaternion.Euler(new Vector3(0, 0, fillAmount * -90));
                
                headCapImage[1].transform.localRotation = 
                    Quaternion.Euler(new Vector3(0, 0, fillAmount * -90));
                
                var timeTextTime = totalTime - CurrentTime;
                timeText[0].text = timeTextTime.ToString("F0");
                timeText[1].text = timeTextTime.ToString("F0");
            }
        }
    }

    public void StartTimer()
    {
        _isPaused = false;
        headCapImage[0].gameObject.SetActive(true);
        headCapImage[1].gameObject.SetActive(true);
    }

    public void PauseTimer()
    {
        _isPaused = true;
    }

    public void InitTimer()
    {
        CurrentTime = 0;
        timeText[0].text = totalTime.ToString("F0");
        timeText[1].text = totalTime.ToString("F0");
        headCapImage[0].gameObject.SetActive(false);
        headCapImage[1].gameObject.SetActive(false);
        _isPaused = true;
    }

    public void ChangeTurnResetTimer()
    {
        InitTimer();
        StartTimer();
    }
}
