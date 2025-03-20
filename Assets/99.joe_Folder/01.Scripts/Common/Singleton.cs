using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    // private static bool _isQuitting = false;

    public static T Instance
    {
        get
        {
            // if (_isQuitting)
            // {
            //     Debug.LogWarning($"[Singleton] {typeof(T).Name} 인스턴스를 생성하려 했지만, 게임이 종료 중입니다.");
            //     return null;
            // }

            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    _instance = obj.AddComponent<T>();

                    // 새로 생성된 객체도 씬 전환 시 유지되도록 설정
                    //DontDestroyOnLoad(obj);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            
            // 씬이 변경될 때 호출되는 이벤트 추가
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 필요하면 자식 클래스에서 재정의 가능
    }

    // private void OnApplicationQuit()
    // {
    //     _isQuitting = true;
    // }
}
