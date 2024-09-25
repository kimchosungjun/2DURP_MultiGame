using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmokStoneEnum;

public class OmokGridManager : MonoBehaviour
{
    Dictionary<Vector2Int, OmokStone> gridGroup = new Dictionary<Vector2Int, OmokStone>();
    OmokStone[] omokStone = new OmokStone[2];

    int[] deltaX = { 0, 1, 1, 1 };
    int[] deltaY = { 1, 1, 0, -1 };

    #region Call By Manager
    public void Init()
    {
        omokStone[0] = GameManager.Instance.Resource.LoadResource<OmokStone>("W_OmokStone", ResourceType.Stone);
        omokStone[1] = GameManager.Instance.Resource.LoadResource<OmokStone>("B_OmokStone", ResourceType.Stone);
    }

    public void Clear()
    {
        if (gridGroup.Count > 0)
            gridGroup.Clear();
    }
    #endregion

    public bool CanPutStone(Vector2Int position, StoneColor color)
    {
        // 바둑판 범위를 벗어났을 때
        if (!CheckGridRange(position))
            return false;

        if (color == StoneColor.Black)
        {
            if (gridGroup.ContainsKey(position))
                return false;
            return CanPutBlackStone(position);
        }
        else
        {
            // 백돌은 놓을 때 아무런 조건이 없다.
            if (gridGroup.ContainsKey(position))
                return false;
            return true;
        }
    }

    public void PutStone(Vector2Int position, StoneColor color)
    {
        OmokStone _stone = Instantiate(omokStone[(int)color]);
        _stone.transform.position = new Vector3(position.x, position.y, 0);
        if (gridGroup.ContainsKey(position))
            return;
        gridGroup.Add(position, _stone);

        // 게임 승리 조건 확인
        if (CheckCompleteOmok(position, color))
        {
            Debug.Log("승리!!!");
            return;
        }
    }

    #region Check Grid

    // 바둑판 밖을 벗어난 곳에 두려고 하는지 확인
    public bool CheckGridRange(Vector2Int position)
    {
        int _xPos = Mathf.Abs(position.x);
        int _yPos = Mathf.Abs(position.y);

        if (_xPos > 9 || _yPos > 9)
            return false;
        return true;
    }

    // 흑돌의 33, 44, 6목 이상인지 확인
    public bool CanPutBlackStone(Vector2Int position)
    {
        Debug.Log("블랙돌");
        if (CheckOver6Stone(position))
            return false;

        if (CheckOpen33Stone(position))
            return false;

        if (CheckOpen44Stone(position))
            return false;

        return true;
    }

    // 열린 33이 되는지 확인
    public bool CheckOpen33Stone(Vector2Int position)
    {
        int _openLineCnt = 0;
        for (int i = 0; i < 4; i++)
        {
            Vector2Int _deltaVec = new Vector2Int(deltaX[i], deltaY[i]);
            if (CheckOpenLineStone(position, _deltaVec, 3))
                _openLineCnt += 1;
            if (_openLineCnt >= 2)
                return true;
        }
        return false;
    }

    // 열린 44가 되는지 확인
    public bool CheckOpen44Stone(Vector2Int position)
    {
        int _openLineCnt = 0;
        for (int i = 0; i < 4; i++)
        {
            Vector2Int _deltaVec = new Vector2Int(deltaX[i], deltaY[i]);
            if (CheckOpenLineStone(position, _deltaVec, 4))
                _openLineCnt += 1;
            if (_openLineCnt >= 2)
                return true;
        }
        return false;
    }

    // 열려있는지 확인
    public bool CheckOpenLineStone(Vector2Int position, Vector2Int deltaVec, int length)
    {
        int _cnt = 1;
        bool _onceGap = false;

        Vector2Int _plusVec = position + deltaVec;
        Vector2Int _minusVec = position - deltaVec;

        // Plus Direction
        while (CheckGridRange(_plusVec))
        {
            if (!gridGroup.ContainsKey(_plusVec) && _onceGap)
            {
                _onceGap = true;
            }
            else if (gridGroup.ContainsKey(_plusVec) && gridGroup[_plusVec].OmokStoneData.color == StoneColor.Black)
            {
                _cnt += 1;
            }
            else
            {
                break;
            }
            _plusVec += deltaVec;
        }

        bool _isOpenPlus = (CheckGridRange(_plusVec) && !gridGroup.ContainsKey(_plusVec));
        bool _isBlockPlus = (CheckGridRange(_plusVec) && (gridGroup.ContainsKey(_plusVec)) && gridGroup[_plusVec].OmokStoneData.color == StoneColor.White);

        _onceGap = false;

        // Minus Direction
        while (CheckGridRange(_minusVec))
        {
            if (!gridGroup.ContainsKey(_minusVec) && _onceGap)
            {
                _onceGap = true;
            }
            else if (gridGroup.ContainsKey(_minusVec) && gridGroup[_minusVec].OmokStoneData.color == StoneColor.Black)
            {
                _cnt += 1;
            }
            else
            {
                break;
            }
            _minusVec -= deltaVec;
        }

        bool _isOpenMinus = (CheckGridRange(_minusVec) && !gridGroup.ContainsKey(_minusVec));
        bool _isBlockMinus = (CheckGridRange(_minusVec) && (gridGroup.ContainsKey(_minusVec)) && gridGroup[_minusVec].OmokStoneData.color == StoneColor.White);

        return _isOpenPlus && _isOpenMinus && (_cnt == length) && !_isBlockPlus && !_isBlockMinus;
    }

    // 6목 이상인지 확인
    public bool CheckOver6Stone(Vector2Int position)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2Int _deltaVec = new Vector2Int(deltaX[i], deltaY[i]);
            if (CheckLinkStoneCnt(position, _deltaVec,StoneColor.Black) >=6)
                return true;
        }
        return false;
    }
   
    // 오목을 완성했는지를 확인
    public bool CheckCompleteOmok(Vector2Int position, StoneColor color)
    {
        int _deltaCnt = deltaX.Length;
        for(int i=0; i <_deltaCnt; i++)
        {
            Vector2Int _deltaVec = new Vector2Int(deltaX[i], deltaY[i]);
            int _cnt = CheckLinkStoneCnt(position, _deltaVec, color);

            // 흑의 다목은 이미 막혀있기에 5 이상으로 둬도 무방할듯
            if (_cnt >= 5) 
                return true;
        }
        return false;
    }
    
    // 같은 색의 돌이 몇 개 연속으로 이어졌는지 확인
    public int CheckLinkStoneCnt(Vector2Int position,Vector2Int deltaVec, StoneColor color)
    {
        int _cnt = 1;

        Vector2Int _plusDirection = position+ deltaVec;
        Vector2Int _minusDirection = position + (-1)*deltaVec;
        
        while (true)
        {
            if (!gridGroup.ContainsKey(_plusDirection))
                break;
            if (gridGroup[_plusDirection].OmokStoneData.color == color)
            {
                _cnt += 1;
                _plusDirection += deltaVec;
            }
            else
                break;
        }

        while (true)
        {
            if (!gridGroup.ContainsKey(_minusDirection))
                break;
            if (gridGroup[_minusDirection].OmokStoneData.color == color)
            {
                _cnt += 1;
                _minusDirection += (-1) * deltaVec;
            }
            else
                break;
        }
        return _cnt;
    }

    #endregion
}
