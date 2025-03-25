

    using UnityEngine;
    using UnityEngine.SceneManagement;


    public abstract class Singleton<T> : MonoBehaviour where T: Component
    {
        private static T _instance;
//외부에 공개되는 프로퍼티 
        public static T Instance
        {
            //없으면 생성해서 반환,있으면 그냥 반환 
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();
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
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            //씬 전환시 호출되는 액션 메서드 할당 
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            
        }
   
    }

