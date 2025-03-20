using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Cell : MonoBehaviour
{
    [SerializeField] private TMP_Text subtitleText;

    public int Index { get; private set; }

    public void SetItem(Item item, int index)
    {
        //image.sprite = Resources.Load<Sprite>(item.imageFileName);
        subtitleText.text = item.subtitle;
        Index = index;
    }
}