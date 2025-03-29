using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;

public class ForbiddenRuleChecker
{
    private GameManager.PlayerType[,] _board;
    private const int OMOL_LINE_LENGTH = 15;
    private (int, int) _currentMoveIndex;
    private List<(int, int)> _forbiddenCollection = new();

    public ForbiddenRuleChecker(GameManager.PlayerType[,] board)
    {
        _board = board;
    }

    public List<(int, int)> CheckForbiddenRelease(List<(int, int)> _forbiddenList)
    {
        for (int i = 0; i < _forbiddenList.Count; i++)
        {
            _board[_forbiddenList[i].Item1, _forbiddenList[i].Item2] = GameManager.PlayerType.None;
        }

        Set3X3Forbidden(_forbiddenList);

        return _forbiddenCollection;
    }

    public List<(int, int)> GetForbiddenSpots((int, int) currentMoveIndex)
    {
        _forbiddenCollection.Clear();
        _currentMoveIndex = currentMoveIndex;
        var lineEmpty = FindEmptyPositionList();
        SetOverlineForbidden(lineEmpty);
        Set4X4Forbidden(lineEmpty);
        Set3X3Forbidden(lineEmpty);

        return _forbiddenCollection;
    }

    private List<(int, int)> FindEmptyPositionList()
    {
        List<(int, int)> emptyList = new List<(int, int)>();
        int row = _currentMoveIndex.Item1;
        int col = _currentMoveIndex.Item2;

        int currentCol = col + 1; // ������ Ž��

        while (0 <= currentCol && currentCol < col + 5 && currentCol <= 14)
        {
            if (_board[row, currentCol] == PlayerType.None)
            {
                emptyList.Add((row, currentCol));
            }
            else if (_board[row, currentCol] == PlayerType.PlayerB)
            {
                break;
            }
            ++currentCol;
        }

        currentCol = col - 1; // ���� Ž��

        while (0 <= currentCol && currentCol > col - 5 && currentCol <= 14)
        {
            if (_board[row, currentCol] == PlayerType.None)
            {
                emptyList.Add((row, currentCol));
            }
            else if (_board[row, currentCol] == PlayerType.PlayerB)
            {
                break;
            }
            --currentCol;
        }

        //�Ʒ��� Ž��
        int currentRow = row + 1;

        while (0 <= currentRow && currentRow < row + 5 && currentRow <= 14)
        {
            if (_board[currentRow, col] == PlayerType.None)
            {
                emptyList.Add((currentRow, col));
            }
            else if (_board[currentRow, col] == PlayerType.PlayerB)
            {
                break;
            }

            ++currentRow;
        }

        // ���� Ž��
        currentRow = row - 1;

        while (0 <= currentRow && currentRow > row - 5 && currentRow <= 14)
        {
            if (_board[currentRow, col] == PlayerType.None)
            {
                emptyList.Add((currentRow, col));
            }
            else if (_board[currentRow, col] == PlayerType.PlayerB)
            {
                break;
            }
            --currentRow;
        }

        // ������ �Ʒ� Ž��

        currentRow = row + 1;
        currentCol = col + 1;

        while (currentRow <= 14 && currentCol <= 14 && currentRow < row + 5 && currentCol < col + 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }

            ++currentRow;
            ++currentCol;
        }

        // ���� �� Ž��

        currentRow = row - 1;
        currentCol = col - 1;

        while (currentRow >= 0 && currentCol >= 0 && currentRow > row - 5 && currentCol > col - 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }
            --currentRow;
            --currentCol;
        }


        //�ע� Ž�� ����

        // ���� �Ʒ� Ž��
        currentRow = row + 1;
        currentCol = col - 1;

        while (currentRow <= 14 && currentCol >= 0 && currentRow < row + 5 && currentCol > col - 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }

            ++currentRow;
            --currentCol;
        }

        // ������ �� Ž��
        currentRow = row - 1;
        currentCol = col + 1;

        while (currentRow >= 0 && currentCol <= 14 && currentRow > row - 5 && currentCol < col + 5)
        {
            if (_board[currentRow, currentCol] == PlayerType.None)
            {
                emptyList.Add((currentRow, currentCol));
            }
            else if (_board[currentRow, currentCol] == PlayerType.PlayerB)
            {
                break;
            }

            --currentRow;
            ++currentCol;
        }
        return emptyList;
    }

    private void SetOverlineForbidden(List<(int, int)> emptyList)
    {
        for (int i = 0; i < emptyList.Count; i++)
        {
            if (emptyList[i].Item1 == 0 || emptyList[i].Item2 == 0 || emptyList[i].Item1 == 14 || emptyList[i].Item2 == 14)
            {
                continue;
            }

            int row = emptyList[i].Item1;
            int col = emptyList[i].Item2 + 1;

            int blockIndex = 0;

            for (int j = col; j <= 14 && j < col + 4; j++)
            {
                if (_board[row, j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            row = emptyList[i].Item1;
            col = emptyList[i].Item2 - 1;

            for (int j = col; 0 <= j && j > col - 4; --j)
            {
                if (_board[row, j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            if (blockIndex >= 5)
            {
                _forbiddenCollection.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }

        for (int i = 0; i < emptyList.Count; i++)
        {
            int row = emptyList[i].Item1 + 1;
            int col = emptyList[i].Item2;

            int blockIndex = 0;

            for (int j = row; j <= 14 && j < row + 4; j++)
            {
                if (_board[j, col] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            // �Ʒ��� �˻�
            row = emptyList[i].Item1 - 1;
            col = emptyList[i].Item2;

            for (int j = row; 0 <= j && j > row - 4; --j)
            {
                if (_board[j, col] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            if (blockIndex >= 5)
            {
                _forbiddenCollection.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }

        for (int i = 0; i < emptyList.Count; i++)
        {
            int row = emptyList[i].Item1 + 1;
            int col = emptyList[i].Item2 + 1;

            int blockIndex = 0;

            for (int j = 0; j < 4; j++)
            {
                if (row + j > 14 || col + j > 14)
                {
                    break;
                }

                if (_board[row + j, col + j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            row = emptyList[i].Item1 - 1;
            col = emptyList[i].Item2 - 1;

            for (int j = 0; j < 4; j++)
            {
                if (row - j < 0 || col - j < 0)
                {
                    break;
                }

                if (_board[row - j, col - j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            if (blockIndex >= 5)
            {
                _forbiddenCollection.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }

        for (int i = 0; i < emptyList.Count; i++)
        {
            // ��(���� �Ʒ�) �˻�
            int row = emptyList[i].Item1 + 1;
            int col = emptyList[i].Item2 - 1;

            int blockIndex = 0; // 1,2,3,4

            for (int j = 0; j < 4; j++)
            {
                if (row + j > 14 || col - j < 0)
                {
                    break;
                }

                if (_board[row + j, col - j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            row = emptyList[i].Item1 - 1;
            col = emptyList[i].Item2 + 1;

            for (int j = 0; j < 4; j++)
            {
                if (row - j < 0 || col + j > 14)
                {
                    break;
                }

                if (_board[row - j, col + j] == PlayerType.PlayerA)
                {
                    ++blockIndex;
                }
                else
                {
                    break;
                }
            }

            if (blockIndex >= 5)
            {
                _forbiddenCollection.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }
    }

    private void Set4X4Forbidden(List<(int, int)> emptyList)
    {
        for (int i = 0; i < emptyList.Count; i++)
        {
            const int MAX_TURNING_COUNT = 5;
            const int MAX_VOID_COUNT = 3;
            const int MAX_BLOCK_COUNT = 4;

            int tempForbiddenCount = 0;

            if (emptyList[i].Item1 == 0 || emptyList[i].Item2 == 0 || emptyList[i].Item1 == 14 || emptyList[i].Item2 == 14)
            {
                continue;
            }

            // 가로 검사
            int row = emptyList[i].Item1;
            int col = emptyList[i].Item2 + 1;
            int blockIndex = 1;
            int turningCount = 0;
            int voidCount = 0;
            bool blockedRight = false;
            bool blockedLeft = false;

            for (int j = col; j < OMOL_LINE_LENGTH && j < col + 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++)
            {
                if (_board[row, j] == GameManager.PlayerType.PlayerA)
                    ++blockIndex;
                else if (_board[row, j] == GameManager.PlayerType.PlayerB)
                {
                    blockedRight = true;
                    break;
                }
                else
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            row = emptyList[i].Item1;
            col = emptyList[i].Item2 - 1;
            voidCount = 0;

            for (int j = col; 0 <= j && j > col - 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; --j)
            {
                if (_board[row, j] == GameManager.PlayerType.PlayerA)
                    ++blockIndex;
                else if (_board[row, j] == GameManager.PlayerType.PlayerB)
                {
                    blockedLeft = true;
                    break;
                }
                else
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            if (blockIndex == MAX_BLOCK_COUNT && !(blockedLeft && blockedRight))
            {
                ++tempForbiddenCount;
            }

            // 세로 검사
            row = emptyList[i].Item1 + 1;
            col = emptyList[i].Item2;
            blockIndex = 1;
            turningCount = 0;
            voidCount = 0;
            blockedRight = false;
            blockedLeft = false;

            for (int j = row; j < OMOL_LINE_LENGTH && j < row + 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++)
            {
                if (_board[j, col] == GameManager.PlayerType.PlayerA)
                    ++blockIndex;
                else if (_board[j, col] == GameManager.PlayerType.PlayerB)
                {
                    blockedRight = true;
                    break;
                }
                else
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            row = emptyList[i].Item1 - 1;
            col = emptyList[i].Item2;
            voidCount = 0;

            for (int j = row; 0 <= j && j > row - 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; --j)
            {
                if (_board[j, col] == GameManager.PlayerType.PlayerA)
                    ++blockIndex;
                else if (_board[j, col] == GameManager.PlayerType.PlayerB)
                {
                    blockedLeft = true;
                    break;
                }
                else
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            if (blockIndex == MAX_BLOCK_COUNT && !(blockedLeft && blockedRight))
            {
                ++tempForbiddenCount;
            }

            // 대각 ↘ 검사
            row = emptyList[i].Item1 + 1;
            col = emptyList[i].Item2 + 1;
            blockIndex = 1;
            turningCount = 0;
            voidCount = 0;
            blockedRight = false;
            blockedLeft = false;

            for (int j = 0; j < 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++)
            {
                if (row + j > 14 || col + j > 14) { blockedRight = true; break; }

                if (_board[row + j, col + j] == GameManager.PlayerType.PlayerA)
                    ++blockIndex;
                else if (_board[row + j, col + j] == GameManager.PlayerType.PlayerB)
                {
                    blockedRight = true;
                    break;
                }
                else
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            row = emptyList[i].Item1 - 1;
            col = emptyList[i].Item2 - 1;
            voidCount = 0;

            for (int j = 0; j < 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++)
            {
                if (row - j < 0 || col - j < 0) { blockedLeft = true; break; }

                if (_board[row - j, col - j] == GameManager.PlayerType.PlayerA)
                    ++blockIndex;
                else if (_board[row - j, col - j] == GameManager.PlayerType.PlayerB)
                {
                    blockedLeft = true;
                    break;
                }
                else
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            if (blockIndex == MAX_BLOCK_COUNT && !(blockedLeft && blockedRight))
            {
                ++tempForbiddenCount;
            }

            // 대각 ↙ 검사
            row = emptyList[i].Item1 + 1;
            col = emptyList[i].Item2 - 1;
            blockIndex = 1;
            turningCount = 0;
            voidCount = 0;
            blockedRight = false;
            blockedLeft = false;

            for (int j = 0; j < 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++)
            {
                if (row + j > 14 || col - j < 0) { blockedRight = true; break; }

                if (_board[row + j, col - j] == GameManager.PlayerType.PlayerA)
                    ++blockIndex;
                else if (_board[row + j, col - j] == GameManager.PlayerType.PlayerB)
                {
                    blockedRight = true;
                    break;
                }
                else
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            row = emptyList[i].Item1 - 1;
            col = emptyList[i].Item2 + 1;
            voidCount = 0;

            for (int j = 0; j < 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++)
            {
                if (row - j < 0 || col + j > 14) { blockedLeft = true; break; }

                if (_board[row - j, col + j] == GameManager.PlayerType.PlayerA)
                    ++blockIndex;
                else if (_board[row - j, col + j] == GameManager.PlayerType.PlayerB)
                {
                    blockedLeft = true;
                    break;
                }
                else
                {
                    ++voidCount;
                    ++turningCount;
                }
            }

            if (blockIndex == MAX_BLOCK_COUNT && !(blockedLeft && blockedRight))
            {
                ++tempForbiddenCount;
            }

            if (tempForbiddenCount >= 2)
            {
                _forbiddenCollection.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }

        for (int i = 0; i < emptyList.Count; i++)
        {
            const int MAX_TURN_COUNT = 5;
            int tempForbiddenCount = 0;
            int row = emptyList[i].Item1;
            int col = emptyList[i].Item2;

            int turnCount = 0;
            string tempPattern = "";

            for (int j = col; 0 <= j && j <= 14 && turnCount < MAX_TURN_COUNT; j++)
            {
                ++turnCount;

                if (_board[row, j] == PlayerType.PlayerA || j == emptyList[i].Item2)
                {
                    tempPattern += "A";
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    return;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    tempPattern += ".";
                }
            }

            switch (tempPattern)
            {
                case "A.AAA":
                case "AA.AA":
                case "AAA.A":
                    ++tempForbiddenCount;
                    break;
            }

            row = emptyList[i].Item1;
            col = emptyList[i].Item2 - 1;

            turnCount = 0;
            tempPattern = "";

            for (int j = col; 0 <= j && j <= 14 && turnCount < MAX_TURN_COUNT; j++)
            {
                ++turnCount;

                if (_board[row, j] == PlayerType.PlayerA || j == emptyList[i].Item2)
                {
                    tempPattern += "A";
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    return;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    tempPattern += ".";
                }
            }

            switch (tempPattern)
            {
                case "A.AAA":
                case "AA.AA":
                case "AAA.A":
                    ++tempForbiddenCount;
                    break;// �� ĭ ��� 4
            }

            row = emptyList[i].Item1;
            col = emptyList[i].Item2 - 2;

            turnCount = 0;
            tempPattern = "";

            for (int j = col; 0 <= j && j <= 14 && turnCount < MAX_TURN_COUNT; j++)
            {
                ++turnCount;

                if (_board[row, j] == PlayerType.PlayerA || j == emptyList[i].Item2)
                {
                    tempPattern += "A";
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    return;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    tempPattern += ".";
                }
            }

            switch (tempPattern)
            {
                case "A.AAA":
                case "AA.AA":
                case "AAA.A":
                    ++tempForbiddenCount;
                    break;
            }

            row = emptyList[i].Item1;
            col = emptyList[i].Item2 - 3;

            turnCount = 0;
            tempPattern = "";

            for (int j = col; 0 <= j && j <= 14 && turnCount < MAX_TURN_COUNT; j++)
            {
                ++turnCount;

                if (_board[row, j] == PlayerType.PlayerA || j == emptyList[i].Item2)
                {
                    tempPattern += "A";
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    return;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    tempPattern += ".";
                }
            }

            switch (tempPattern)
            {
                case "A.AAA":
                case "AA.AA":
                case "AAA.A":
                    ++tempForbiddenCount;
                    break;
            }

            row = emptyList[i].Item1;
            col = emptyList[i].Item2 - 4;

            turnCount = 0;
            tempPattern = "";

            for (int j = col; 0 <= j && j <= 14 && turnCount < MAX_TURN_COUNT; j++)
            {
                ++turnCount;

                if (_board[row, j] == PlayerType.PlayerA || j == emptyList[i].Item2)
                {
                    tempPattern += "A";
                }
                else if (_board[row, j] == PlayerType.PlayerB)
                {
                    return;
                }
                else if (_board[row, j] == PlayerType.None)
                {
                    tempPattern += ".";
                }
            }

            switch (tempPattern)
            {
                case "A.AAA":
                case "AA.AA":
                case "AAA.A":
                    ++tempForbiddenCount;
                    break;
            }

            if (tempForbiddenCount > 1)
            {
                _forbiddenCollection.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }
    }

    private void Set3X3Forbidden(List<(int, int)> emptyList) //아니면 바둑돌을 찾았을때의 턴이 몇인지..
    {
        for (int i = 0; i < emptyList.Count; i++)
        {
            const int MAX_BLOCK_COUNT = 3;
            const int MAX_DIRECITON_VOID_COUNT = 2;

            int tempForbiddenCount = 0;

            if (emptyList[i].Item1 == 0 || emptyList[i].Item2 == 0 || emptyList[i].Item1 == 14 || emptyList[i].Item2 == 14)
            {
                continue;
            }

            // 가로 검사
            int row = emptyList[i].Item1;
            int col = emptyList[i].Item2 + 1;
            int blockIndex = 1;
            int voidCount = 0;
            bool isBlocked = false;
            bool isLeftBlocked = false;
            bool isRightBlocked = false;
            
            List<(int, int)> blackList = new List<(int, int)>();

            for (int j = col; j <= 14 && j < col + 4 && voidCount < MAX_DIRECITON_VOID_COUNT; j++)
            {
                if (_board[row, j] == GameManager.PlayerType.PlayerA)
                {
                    ++blockIndex;
                    blackList.Add((row, j));
                }

                else if (_board[row, j] == GameManager.PlayerType.PlayerB)
                {
                    if (Mathf.Abs(j - emptyList[i].Item2) == 1 || Mathf.Abs(j - emptyList[i].Item2) == 2)
                    {
                        isBlocked = true;
                        break;
                    }

                    if (Mathf.Abs(j - emptyList[i].Item2) == 3)
                    {
                        isLeftBlocked = true;
                        break;
                    }
                }
                else if (_board[row, j] == GameManager.PlayerType.None)
                {
                    ++voidCount;
                }
                else if (_board[row, j] == GameManager.PlayerType.PlayerX)
                {
                    isBlocked = true;
                    break;
                }
            }

            row = emptyList[i].Item1;
            col = emptyList[i].Item2 - 1;
            voidCount = 0;

            for (int j = col; 0 <= j && j > col - 4 && voidCount < MAX_DIRECITON_VOID_COUNT; --j)
            {
                if (_board[row, j] == GameManager.PlayerType.PlayerA)
                {
                    ++blockIndex;
                    blackList.Add((row, j));
                }
                else if (_board[row, j] == GameManager.PlayerType.PlayerB)
                {
                    if (Mathf.Abs(j - emptyList[i].Item2) == 1 || Mathf.Abs(j - emptyList[i].Item2) == 2)
                    {
                        isBlocked = true;
                        break;
                    }
                    
                    if (Mathf.Abs(j - emptyList[i].Item2) == 3)
                    {
                        isRightBlocked = true;
                        break;
                    }
                    
                }
                else if (_board[row, j] == GameManager.PlayerType.None)
                {
                    ++voidCount;
                }
                else if (_board[row, j] == GameManager.PlayerType.PlayerX)
                {
                    isBlocked = true;
                    break;
                }
            }

            if (blockIndex == MAX_BLOCK_COUNT && !isBlocked &&
                !(isLeftBlocked && isRightBlocked)) //isLeftBlocked와 isrightBlocked 둘 다 막혀있으면 안되고 isLeftBlocked와 isrightBlocked 둘중 하나는 막혀있어도 됨 
            {
                if (!(Mathf.Abs(blackList[0].Item2 - blackList[1].Item2) > 3)) // 금수에서 가장 끝에 있는 서로의 흑돌이 3칸 초과하면
                {
                    Debug.Log("!!");
                    ++tempForbiddenCount;
                }
            }
            else if (blockIndex > MAX_BLOCK_COUNT)
            {
                break;
            }

            // 세로 검사
            row = emptyList[i].Item1 + 1;
            col = emptyList[i].Item2;
            blockIndex = 1;
            voidCount = 0;
            isBlocked = false;
            isLeftBlocked = false;
            isRightBlocked = false;
            blackList.Clear();

            for (int j = row; j <= 14 && j < row + 4 && voidCount < MAX_DIRECITON_VOID_COUNT; j++)
            {

                if (_board[j, col] == GameManager.PlayerType.PlayerA)
                {
                    ++blockIndex;
                    blackList.Add((j, col));
                }
                else if (_board[j, col] == GameManager.PlayerType.PlayerB)
                {
                    if (Mathf.Abs(j - emptyList[i].Item1) == 1 || Mathf.Abs(j - emptyList[i].Item1) == 2)
                    {
                        isBlocked = true;
                        break;
                    }

                    if (Mathf.Abs(j - emptyList[i].Item1) == 3)
                    {
                        isLeftBlocked = true;
                        break;
                    }
                }
                else if (_board[j, col] == GameManager.PlayerType.None)
                {
                    ++voidCount;
                }
                else if (_board[j, col] == GameManager.PlayerType.PlayerX)
                {
                    isBlocked = true;
                    break;
                }
            }

            row = emptyList[i].Item1 - 1;
            col = emptyList[i].Item2;
            voidCount = 0;

            for (int j = row; 0 <= j && j > row - 4 && voidCount < MAX_DIRECITON_VOID_COUNT; --j)
            {

                if (_board[j, col] == GameManager.PlayerType.PlayerA)
                {
                    ++blockIndex;
                    blackList.Add((j, col));
                }

                else if (_board[j, col] == GameManager.PlayerType.PlayerB)
                {
                    if (Mathf.Abs(j - emptyList[i].Item1) == 1 || Mathf.Abs(j - emptyList[i].Item1) == 2)
                    {
                        isBlocked = true;
                        break;
                    }
                    
                    if (Mathf.Abs(j - emptyList[i].Item1) == 3)
                    {
                        isRightBlocked = true;
                        break;
                    }
                }
                else if (_board[j, col] == GameManager.PlayerType.None)
                {
                    ++voidCount;
                }
                else if (_board[j, col] == GameManager.PlayerType.PlayerX)
                {
                    isBlocked = true;
                    break;
                }
            }


            if (blockIndex == MAX_BLOCK_COUNT && !isBlocked &&
                !(isLeftBlocked && isRightBlocked))
            {
                if (!(Mathf.Abs(blackList[0].Item1 - blackList[1].Item1) > 3)) // 금수에서 가장 끝에 있는 서로의 흑돌이 3칸 초과하면
                {
                    Debug.Log("11");
                    ++tempForbiddenCount;
                }
            }
            else if (blockIndex > MAX_BLOCK_COUNT)
            {
                break;
            }

            // 대각 ↘ 검사
            row = emptyList[i].Item1 + 1;
            col = emptyList[i].Item2 + 1;
            blockIndex = 1;
            voidCount = 0;
            isBlocked = false;
            isLeftBlocked = false;
            isRightBlocked = false;
            blackList.Clear();

            for (int j = 0; j < 4 && voidCount < MAX_DIRECITON_VOID_COUNT; j++)
            {
                if (0 > row + j || row + j >= OMOL_LINE_LENGTH || 0 > col + j || col + j >= OMOL_LINE_LENGTH)
                {
                    break;
                }

                if (_board[row + j, col + j] == GameManager.PlayerType.PlayerA)
                {
                    ++blockIndex;
                    blackList.Add((row + j, col + j));
                }
                else if (_board[row + j, col + j] == GameManager.PlayerType.PlayerB)
                {
                    int deltaRow = Math.Abs((row + j) - emptyList[i].Item1);
                    int deltaCol = Math.Abs((col + j) - emptyList[i].Item2);

                    if (deltaRow == 1 && deltaCol == 1 || deltaRow == 2 && deltaCol == 2)
                    {
                        isBlocked = true;
                        break;
                    }
                    
                    if (deltaRow == 3 && deltaCol == 3)
                    {
                        isLeftBlocked = true;
                        break;
                    }
                }
                else if (_board[row + j, col + j] == GameManager.PlayerType.None)
                {
                    ++voidCount;
                }
                else if (_board[row + j, col + j] == GameManager.PlayerType.PlayerX)
                {
                    isBlocked = true;
                    break;
                }
            }

            row = emptyList[i].Item1 - 1;
            col = emptyList[i].Item2 - 1;
            voidCount = 0;

            for (int j = 0; j < 4 && voidCount < MAX_DIRECITON_VOID_COUNT; j++)
            {
                if (0 > row - j || row - j >= OMOL_LINE_LENGTH || 0 > col - j || col - j >= OMOL_LINE_LENGTH)
                {
                    break;
                }

                if (_board[row - j, col - j] == GameManager.PlayerType.PlayerA)
                {
                    ++blockIndex;
                    blackList.Add((row - j, col - j));
                }
                else if (_board[row - j, col - j] == GameManager.PlayerType.PlayerB)
                {
                    int deltaRow = Math.Abs((row - j) - emptyList[i].Item1);
                    int deltaCol = Math.Abs((col - j) - emptyList[i].Item2);

                    if (deltaRow == 1 && deltaCol == 1 || deltaRow == 2 && deltaCol == 2)
                    {
                        isBlocked = true;
                        break;
                    }
                    
                    if (deltaRow == 3 && deltaCol == 3)
                    {
                        isRightBlocked = true;
                        break;
                    }
                }
                else if (_board[row - j, col - j] == GameManager.PlayerType.None)
                {
                    ++voidCount;
                }
                else if (_board[row - j, col - j] == GameManager.PlayerType.PlayerX)
                {
                    isBlocked = true;
                    break;
                }
            }

            if (blockIndex == MAX_BLOCK_COUNT && !isBlocked &&
                !(isLeftBlocked && isRightBlocked))
            {
                int deltaRow = Mathf.Abs(blackList[0].Item1 - blackList[1].Item1);
                int deltaCol = Mathf.Abs(blackList[0].Item2 - blackList[1].Item2);

                if (deltaRow == deltaCol && deltaRow <= 3) // 대각선에 정확히 있고, 3칸 이내면 금수
                {
                    ++tempForbiddenCount;
                }
            }
            else if (blockIndex > MAX_BLOCK_COUNT)
            {
                break;
            }

            // 대각 ↙ 검사
            row = emptyList[i].Item1 + 1;
            col = emptyList[i].Item2 - 1;
            blockIndex = 1;
            voidCount = 0;
            isBlocked = false;
            isLeftBlocked = false;
            isRightBlocked = false;
            blackList.Clear();

            for (int j = 0; j < 4 && voidCount < MAX_DIRECITON_VOID_COUNT; j++)
            {

                if (0 > row + j || row + j >= OMOL_LINE_LENGTH || 0 > col - j || col - j >= OMOL_LINE_LENGTH)
                {
                    break;
                }

                if (_board[row + j, col - j] == GameManager.PlayerType.PlayerA)
                {
                    ++blockIndex;
                    blackList.Add((row + j, col - j));
                }
                else if (_board[row + j, col - j] == GameManager.PlayerType.PlayerB)
                {
                    int deltaRow = Math.Abs((row + j) - emptyList[i].Item1);
                    int deltaCol = Math.Abs((col - j) - emptyList[i].Item2);

                    if (deltaRow == 1 && deltaCol == 1 || deltaRow == 2 && deltaCol == 2)
                    {
                        isBlocked = true;
                        break;
                    }
                    
                    if (deltaRow == 3 && deltaCol == 3)
                    {
                        isLeftBlocked = true;
                        break;
                    }
                }
                else if (_board[row + j, col - j] == GameManager.PlayerType.None)
                {
                    ++voidCount;
                }
                else if (_board[row + j, col - j] == GameManager.PlayerType.PlayerX)
                {
                    isBlocked = true;
                    break;
                }
            }


            row = emptyList[i].Item1 - 1;
            col = emptyList[i].Item2 + 1;
            voidCount = 0;

            for (int j = 0; j < 4 && voidCount < MAX_DIRECITON_VOID_COUNT; j++)
            {
                if (0 > row - j || row - j >= OMOL_LINE_LENGTH || 0 > col + j || col + j >= OMOL_LINE_LENGTH)
                {
                    break;
                }

                if (_board[row - j, col + j] == GameManager.PlayerType.PlayerA)
                {
                    ++blockIndex;
                    blackList.Add((row - j, col + j));
                }
                else if (_board[row - j, col + j] == GameManager.PlayerType.PlayerB)
                {
                    int deltaRow = Math.Abs((row - j) - emptyList[i].Item1);
                    int deltaCol = Math.Abs((col + j) - emptyList[i].Item2);

                    if (deltaRow == 1 && deltaCol == 1 || deltaRow == 2 && deltaCol == 2)
                    {
                        isBlocked = true;
                        break;
                    }
                    
                    if (deltaRow == 3 && deltaCol == 3)
                    {
                        isRightBlocked = true;
                        break;
                    }
                }
                else if (_board[row - j, col + j] == GameManager.PlayerType.None)
                {
                    ++voidCount;
                }
                else if (_board[row - j, col + j] == GameManager.PlayerType.PlayerX)
                {
                    isBlocked = true;
                    break;
                }
            }

            if (blockIndex == MAX_BLOCK_COUNT && !isBlocked &&
                !(isLeftBlocked && isRightBlocked))
            {
                int deltaRow = Mathf.Abs(blackList[0].Item1 - blackList[1].Item1);
                int deltaCol = Mathf.Abs(blackList[0].Item2 - blackList[1].Item2);

                if (deltaRow == deltaCol && deltaRow <= 3) // 대각선에 정확히 있고, 3칸 이내면 금수
                {
                    ++tempForbiddenCount;
                }
            }
            else if (blockIndex > MAX_BLOCK_COUNT)
            {
                break;
            }

            if (tempForbiddenCount >= 2)
            {
                _forbiddenCollection.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }
    }
}