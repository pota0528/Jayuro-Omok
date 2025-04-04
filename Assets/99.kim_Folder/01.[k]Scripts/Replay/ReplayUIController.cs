using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ReplayUIController : MonoBehaviour
{
    [SerializeField] private ReplayBlockController replayBlockController; // 리플레이용 오목판
    [SerializeField] private Button exitListButton; // ReplayMatchList에서 나가기 버튼
    [SerializeField] private Button exitButton; // ReplayPanel에서 나가기 버튼
    [SerializeField] private ReplayController replayController; // Replay 로직 관리
    [SerializeField] private MatchListController matchListController; // 매치 리스트 관리
    [SerializeField] private GameObject matchListPanel; // 매치 리스트 패널
    [SerializeField] private GameObject replayPanel; // Replay 패널

    private void Start()
    {
        // 기보보기 버튼 누르면 MatchListPanel만 표시
        matchListPanel.SetActive(true);
        replayPanel.SetActive(false); // ReplayPanel은 비활성화
        
        // 기보 매치 리스트에서 뒤로가기를 누르면 Main Scene으로 다시 돌아감.
        exitListButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Login");
           
        });
        
        // ReplayPanel의 나가기 버튼을 누르면 MatchListPanel로 다시 돌아감.
        exitButton.onClick.AddListener(() =>
        {
            replayBlockController.ResetBoard();
            replayPanel.SetActive(false);
            matchListPanel.SetActive(true);
        });
        
        // 매치 리스트에서 매치를 선택하면 ReplayPanel 표시
        matchListController.MatchSelected += match =>
        {
            replayController.LoadMatch(match); // 선택된 매치를 불러오기
            matchListPanel.SetActive(false); // MatchListPanel은 비활성화
            replayPanel.SetActive(true); // ReplayPanel은 활성화
        };
    }

    public void OnClickSettingButton()
    {
        UIManager.Instance.OpenSettingPopup();
    }
}