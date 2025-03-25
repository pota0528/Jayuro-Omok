using System.Collections.Generic;
using System;

[System.Serializable]
public struct Move
{
    public int row; // 행
    public int col; // 열
    public string color; // "흑" 또는 "백"
}

[System.Serializable]
public class MatchData //json으로 저장될 것들은 struct이 아니라 class가 좋다고 함, null을 가질 수 없어서 오류 발생
{
    public string title; // 유저 닉네임
    public string date; // 저장 날짜
    public List<Move> moves; // 이동 목록
}