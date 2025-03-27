using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopConfirmPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action _onConfirm;

    public void Show(string message, Action onConfirm)
    {
        messageText.text = message;
        _onConfirm = onConfirm;
        gameObject.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            _onConfirm?.Invoke();
            Close();
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(Close);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
