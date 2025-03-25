using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using DG.Tweening;

   public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject Prefab;

        //[SerializeField] int poolSize;
        [SerializeField] private RectTransform parent;

        //Renderer component;

        private Queue<GameObject> _pool;
        private static ObjectPool _instance;

        public static ObjectPool Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            _instance = this;
            _pool = new Queue<GameObject>();

            // for (int i = 0; i < poolSize; i++)
            // {
            //     CreateNewObject();
            // }
        }

        /// <summary>
        /// 새로운 오브잭트 풀 생성 메서트
        /// </summary>
        private void CreateNewObject()
        {
            GameObject newObject = Instantiate(Prefab, parent);
            //newObject.name = "ObjectPool" + _pool.Count;


            // component = newObject.GetComponent<Renderer>();
            // component.material.color = new Color(1, 0, 0, 1);
            newObject.SetActive(false);
            _pool.Enqueue(newObject);
        }

        /// <summary>
        /// 풀에있는 오브젝트 반환 메서드
        /// </summary>
        /// <returns></returns>
        public GameObject GetObject()
        {
            if (_pool.Count == 0) CreateNewObject();

            GameObject dequeObject = _pool.Dequeue();
            dequeObject.SetActive(true);
            return dequeObject;
        }

        /// <summary>
        /// 사용한 오브젝트를 오브젝트 풀로 되돌려 주는 메서드
        /// </summary>
        /// <param name="returnObject"></param>
        public void ReturnObject(GameObject returnObject)
        {
            returnObject.SetActive(false);

            //returnObject.SetActive(true);
            //returnObject.transform.SetParent(parent, false); // 부모 설정 유지
            //returnObject.transform.SetAsFirstSibling(); // 뒤로이동
            //returnObject.transform.SetAsLastSibling(); // 가장 위로 이동

            _pool.Enqueue(returnObject);
        }
    }