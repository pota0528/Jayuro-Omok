using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab; // 셀 프리팹
    [SerializeField] private Transform content; // 스크롤뷰 콘텐츠

    private List<Item> _items = new List<Item>();

    public void LoadData(List<Item> items)
    {
        _items = items;
        ReloadData();
    }

    private void ReloadData()
    {
        if (content == null)
        {
            Debug.LogError("content가 null입니다!");
            return;
        }

        if (cellPrefab == null)
        {
            Debug.LogError("cellPrefab이 null입니다!");
            return;
        }

        // 기존 셀 제거
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 새 셀 생성
        foreach (var item in _items)
        {
            GameObject cell = Instantiate(cellPrefab, content);
            TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();
            if (textComponent == null)
            {
                Debug.LogError("셀 프리팹에 Text 컴포넌트가 없습니다!");
                continue;
            }
            textComponent.text = item.subtitle;

            Button button = cell.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("셀 프리팹에 Button 컴포넌트가 없습니다!");
                continue;
            }

            if (item.onClick != null)
            {
                button.gameObject.SetActive(true);
                button.onClick.AddListener(() => item.onClick());
            }
        }
    }
}