using System.Collections.Generic;
using static GameManager;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ForbiddenRuleChecker
{
    private GameManager.PlayerType[,] _board;
    private const int OMOL_LINE_LENGTH = 15;
    private (int, int) _currentMoveIndex;
    private List<(int, int)> _forbiddenCollection = new();

    public ForbiddenRuleChecker(GameManager.PlayerType[,] board, (int, int) currentMoveIndex)
    {
        _board = board;
        _currentMoveIndex = currentMoveIndex;
    }

    public List<(int, int)> GetForbiddenSpots()
    {
        _forbiddenCollection.Clear();

        var lineEmpty = FindEmptyPositionList();
        SetOverlineForbidden(lineEmpty);
        Set4X4Forbidden(lineEmpty);
        List < (int, int) > tempForbiddenList = Set3X3Forbidden(lineEmpty);
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

    private List<(int, int)> Set3X3Forbidden(List<(int, int)> emptyList)
    {
        List<(int, int)> tempForbiddenList = new List<(int, int)> ();
        for (int i = 0; i < emptyList.Count; i++)
        {
            const int MAX_TURNING_COUNT = 5;
            const int MAX_VOID_COUNT = 4;
            const int MAX_BLOCK_COUNT = 3;

            int tempForbiddenCount = 0;

            // 가로 검사
            int row = emptyList[i].Item1;
            int col = emptyList[i].Item2 + 1;
            int blockIndex = 1;
            int turningCount = 0;
            int voidCount = 0;
            bool blockedRight = false;
            bool blockedLeft = false;

            for (int j = col; j <= 14 && j < col + 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++)
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

            for (int j = row; j <= 14 && j < row + 4 && turningCount < MAX_TURNING_COUNT && voidCount < MAX_VOID_COUNT; j++)
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
                tempForbiddenList.Add((emptyList[i].Item1, emptyList[i].Item2));
            }
        }

        return tempForbiddenList;
    }

    


}
