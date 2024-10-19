using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmokStoneEnum;

public class SoloGridManager : MonoBehaviour
{
    [SerializeField, Header("컨트롤러")]
    SoloPlayController controller;

    Dictionary<Vector2Int, OmokStone> gridGroup = new Dictionary<Vector2Int, OmokStone>();
    [SerializeField]
    OmokStone[] omokStone;

    int[] oneDirX = { 0, 1, 1, 1 };
    int[] oneDirY = { 1, 1, 0, -1 };

    public void Clear()
    {
        if (gridGroup.Count > 0)
            gridGroup.Clear();
    }

    public void Awake()
    {
        Clear();
    }

    public bool CanPutStone(Vector2Int position, StoneColor color)
    {
        // 해당 위치에 돌이 있는지? (0)
        if (CheckOverRangeNPlaceStone(position))
            return false;

        // 5목인지? (?)
        if (CheckOmok(position, color))
            return true;

        // 장목인지? (0)
        if (CheckLongMok(position, color))
            return false;

        // 33인지? (0) 
        if (Check33Line(position, color))
            return false;

        // 44인지? (?)
        if (Check44Line(position, color))
            return false;

        return true;
    }

    #region 범위를 벗어났는지와 해당 위치에 돌이 있는지를 확인
    /// <summary>
    /// 범위를 벗어났는지 + 이미 돌이 놓여있는지?
    /// </summary>
    public bool CheckOverRangeNPlaceStone(Vector2Int position)
    {
        if (CheckOverRange(position))
            return true;

        if (CheckPlaceStone(position))
            return true;

        return false;
    }

    /// <summary>
    /// 범위를 벗어났는가?
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool CheckOverRange(Vector2Int position)
    {
        int _posX = Mathf.Abs(position.x);
        int _posY = Mathf.Abs(position.y);

        if (_posX > 9 || _posY > 9)
            return true;
        return false;
    }


    public bool CheckPlaceStone(Vector2Int position)
    {
        if (gridGroup.ContainsKey(position))
            return true;
        return false;
    }
    #endregion

    #region 오목의 조건을 충족했는지를 확인
    /// <summary>
    /// 오목인지 확인
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public bool CheckOmok(Vector2Int position, StoneColor color)
    {
        int _oneDirCnt = oneDirX.Length;
        for (int i = 0; i < _oneDirCnt; i++)
        {
            Vector2Int _deltaValue = new Vector2Int(oneDirX[i], oneDirY[i]);
            if (LinkCnt(position, _deltaValue, color))
                return true;
        }

        return false;
    }

    /// <summary>
    /// 현재 위치에 두었을 때, 개수가 5개 이상인지 확인
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public bool LinkCnt(Vector2Int position, Vector2Int direction, StoneColor color)
    {
        int _cnt = 1;
        Vector2Int _plusPos = position + direction;
        Vector2Int _minusPos = position - direction;

        while (true)
        {
            if (!gridGroup.ContainsKey(_plusPos))
                break;
            else
            {
                if (gridGroup[_plusPos].OmokStoneData.color != color)
                    break;
                _cnt += 1;
                _plusPos += direction;
            }
        }

        while (true)
        {
            if (!gridGroup.ContainsKey(_minusPos))
                break;
            else
            {
                if (gridGroup[_minusPos].OmokStoneData.color != color)
                    break;
                _cnt += 1;
                _minusPos -= direction;
            }
        }

        if (_cnt >= 5 && color == StoneColor.White)
            return true;
        else if (_cnt == 5 && color == StoneColor.Black)
            return true;
        else
            return false;
    }

    #endregion

    #region 장목인지 확인
    public bool CheckLongMok(Vector2Int position, StoneColor color)
    {
        if (color == StoneColor.White)
            return false;

        int _oneDirCnt = oneDirX.Length;
        for (int i = 0; i < _oneDirCnt; i++)
        {
            Vector2Int _deltaValue = new Vector2Int(oneDirX[i], oneDirY[i]);
            int _cnt = 1;
            bool _onceGap = false;

            Vector2Int _plusPos = position + _deltaValue;
            Vector2Int _minusPos = position - _deltaValue;

            while (true)
            {
                if (!gridGroup.ContainsKey(_plusPos) && !_onceGap)
                {
                    _onceGap = true;
                }
                else if (!gridGroup.ContainsKey(_plusPos) && _onceGap)
                {
                    break;
                }
                else
                {
                    if (gridGroup[_plusPos].OmokStoneData.color != color)
                        break;
                    _cnt += 1;
                    _plusPos += _deltaValue;
                }
            }

            while (true)
            {
                if (!gridGroup.ContainsKey(_minusPos) && !_onceGap)
                {
                    _onceGap = true;
                }
                else if (!gridGroup.ContainsKey(_minusPos) && _onceGap)
                {
                    break;
                }
                else
                {
                    if (gridGroup[_minusPos].OmokStoneData.color != color)
                        break;
                    _cnt += 1;
                    _minusPos -= _deltaValue;
                }
            }
            if (_cnt > 5)
                return true;
        }
        return false;
    }

    #endregion

    #region 33인지 확인
    public bool Check33Line(Vector2Int position, StoneColor color)
    {
        if (color == StoneColor.White)
            return false;

        int _check3LineCnt = 0;
        for (int i = 0; i < 4; i++)
        {
            Vector2Int _direction = new Vector2Int(oneDirX[i], oneDirY[i]);
            _check3LineCnt += CheckOpen3Line(position, _direction);
        }

        if (_check3LineCnt >= 2)
            return true;
        return false;
    }

    /// <summary>
    /// 3인지 확인 (열린 4를 만들 수 있는 3을 의미)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public int CheckOpen3Line(Vector2Int position, Vector2Int direction)
    {
        int _cnt = 0;

        int _plusCnt = 0;
        bool _plusOnceGap = false;
        Vector2Int _plusDirection = position + direction;
        Vector2Int _minusDirection = position - direction;

        // 양의 방향으로 탐색
        while (true)
        {
            if (CheckOverRange(_plusDirection))
                break;

            if (!gridGroup.ContainsKey(_plusDirection))
            {
                if (_plusOnceGap)
                    break;
                _plusOnceGap = true;
            }
            else
            {
                if (gridGroup[_plusDirection].OmokStoneData.color == StoneColor.White)
                    break;
                _plusCnt += 1;
            }
            _plusDirection += direction;
        }

        // 음의 방향으로 탐색
        int _minusCnt = 0;
        bool _minusOnceGap = false;
        while (true)
        {
            if (CheckOverRange(_minusDirection))
                break;

            if (!gridGroup.ContainsKey(_minusDirection))
            {
                if (_minusOnceGap)
                    break;
                _minusOnceGap = true;
            }
            else
            {
                if (gridGroup[_minusDirection].OmokStoneData.color == StoneColor.White)
                    break;
                _minusCnt += 1;
            }
            _minusDirection -= direction;
        }

        _cnt = 1 + _minusCnt + _plusCnt;

        if (_cnt != 3)
            return 0;

        // 돌은 둔 위치를 기반으로 양 옆으로 돌이 하나라도 이어져있지 않고 양 방향으로 돌이 1개씩 존재한다면 열린3이 아님.
        if ((!gridGroup.ContainsKey(position + direction) && !gridGroup.ContainsKey(position - direction)) && (_minusCnt == _plusCnt))
            return 0;

        bool _isPlusBlock = ((gridGroup.ContainsKey(_plusDirection) && gridGroup[_plusDirection].OmokStoneData.color == StoneColor.White) || CheckOverRange(_plusDirection)) ? true : false;
        bool _isMinusBlock = ((gridGroup.ContainsKey(_minusDirection) && gridGroup[_minusDirection].OmokStoneData.color == StoneColor.White) || CheckOverRange(_minusDirection)) ? true : false;

        // 양 방향 둘 다 막혀있다면 무조건 닫힌 3임.
        if (_isPlusBlock && _isMinusBlock)
            return 0;
        // 양 방향 둘 다 뚫려있다면 무조건 열린 3임.
        else if (!_isPlusBlock && !_isMinusBlock)
            return 1;
        // 양 방향 중 하나라도 뚫린 경우 : 막힌 곳의 직전 좌표가 뚫려있다면 열린 4를 만들 수 있음.
        else
        {
            // 양의 방향이 막힌 경우
            if (_isPlusBlock)
            {
                Vector2Int _previousDirection = _plusDirection - direction;
                if (_previousDirection == position)
                    return 0;
                // 빈 공간이라면 열려있고 빈 공간이 아니라면 닫힌 3.
                if (!gridGroup.ContainsKey(_previousDirection))
                    return 1;
                else
                    return 0;
            }
            // 음의 방향이 막힌 경우
            else
            {
                Vector2Int _previousDirection = _minusDirection + direction;
                if (_previousDirection == position)
                    return 0;
                // 빈 공간이라면 열려있고 빈 공간이 아니라면 닫힌 3.
                if (!gridGroup.ContainsKey(_previousDirection))
                    return 1;
                else
                    return 0;
            }
        }
    }

    #endregion

    #region 44인지 확인
    public bool Check44Line(Vector2Int position, StoneColor color)
    {
        if (color == StoneColor.White)
            return false;

        int _check4LineCnt = 0;
        for (int i = 0; i < 4; i++)
        {
            Vector2Int _direction = new Vector2Int(oneDirX[i], oneDirY[i]);
            _check4LineCnt += CheckOpen4Line(position, _direction);
        }

        if (_check4LineCnt >= 2)
            return true;
        return false;
    }

    /// <summary>
    /// 4인지 확인 (완전히 닫힌 4가 아님을 의미)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public int CheckOpen4Line(Vector2Int position, Vector2Int direction)
    {
        int _cnt = 0;

        int _plusCnt = 0;
        bool _plusOnceGap = false;
        bool _isHavePlusGapStone = false;
        Vector2Int _plusDirection = position + direction;
        Vector2Int _minusDirection = position - direction;

        // 양의 방향으로 탐색
        while (true)
        {
            if (!gridGroup.ContainsKey(_plusDirection))
            {
                if (CheckOverRange(_plusDirection))
                    break;

                if (_plusOnceGap)
                    break;
                _plusOnceGap = true;
            }
            else
            {
                if (gridGroup[_plusDirection].OmokStoneData.color == StoneColor.White)
                    break;

                if (_plusOnceGap)
                    _isHavePlusGapStone = true;
                _plusCnt += 1;
            }
            _plusDirection += direction;
        }

        // 음의 방향으로 탐색
        int _minusCnt = 0;
        bool _minusOnceGap = false;
        bool _isHaveMinusGapStone = false;
        while (true)
        {
            if (CheckOverRange(_minusDirection))
                break;

            if (!gridGroup.ContainsKey(_minusDirection))
            {
                if (_minusOnceGap)
                    break;
                _minusOnceGap = true;
            }
            else
            {
                if (gridGroup[_minusDirection].OmokStoneData.color == StoneColor.White)
                    break;

                if (_minusOnceGap)
                    _isHaveMinusGapStone = true;
                _minusCnt += 1;
            }
            _minusDirection -= direction;
        }

        _cnt = 1 + _minusCnt + _plusCnt;

        if (_cnt != 4)
            return 0;

        // 돌은 둔 위치를 기반으로 양 옆으로 돌이 하나라도 이어져있지 않고 한 방향에서 3개가 나오지 않는다면 4로 취급하지 않는다.
        if ((!gridGroup.ContainsKey(position + direction) && !gridGroup.ContainsKey(position - direction)) && ((_minusCnt != 0) && (_plusCnt != 0)))
            return 0;


        bool _isPlusBlock = ((gridGroup.ContainsKey(_plusDirection) && gridGroup[_plusDirection].OmokStoneData.color == StoneColor.White) || CheckOverRange(_plusDirection)) ? true : false;
        bool _isMinusBlock = ((gridGroup.ContainsKey(_minusDirection) && gridGroup[_minusDirection].OmokStoneData.color == StoneColor.White) || CheckOverRange(_minusDirection)) ? true : false;

        if ((_isPlusBlock && !_plusOnceGap) && (_isMinusBlock && !_minusOnceGap))
            return 0;

        if (_isHaveMinusGapStone && _isHavePlusGapStone)
            return 0;

        return 1;
    }
    #endregion

    /// <summary>
    /// Put Stone => 추후에 풀링 방식으로 변경 예정. 풀링으로 불러오고 직접 Grid에 데이터 생성해서 추가해주기 (Awake는 비활성화된 물체도 '한번' 호출하는것으로 알고있음)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    public void PutStone(Vector2Int position, StoneColor color)
    {
        OmokStone _stone = Instantiate(omokStone[(int)color]);
        _stone.transform.position = new Vector3(position.x, position.y, 0);
        gridGroup.Add(position, _stone);

        if(CheckOmok(position, color))
        {
            controller.WinGame(color);
            return;
        }
    }
}
