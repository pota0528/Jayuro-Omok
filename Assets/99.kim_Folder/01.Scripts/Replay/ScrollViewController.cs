using System.Collections.Generic;
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
        // 기존 셀 제거
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 새 셀 생성
        foreach (var item in _items)
        {
            GameObject cell = Instantiate(cellPrefab, content);
            cell.GetComponentInChildren<Text>().text = item.subtitle;
            Button button = cell.GetComponent<Button>();
            if (item.onClick != null && button != null)
            {
                button.onClick.AddListener(() => item.onClick());
            }
        }
    }
}