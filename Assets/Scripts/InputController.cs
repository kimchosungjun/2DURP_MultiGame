using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField]
    GameObject greenStone;

    [SerializeField]
    GameObject redStone;

    bool isBlack = true;
    bool canPut = false;
    Vector2Int currentCoordinate = Vector2Int.zero;

    public void Init()
    {
        greenStone.SetActive(false);
        redStone.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 _touchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int _touchPointX = Mathf.RoundToInt(_touchPoint.x);
            int _touchPointY = Mathf.RoundToInt(_touchPoint.y);

            Vector2Int _coordinate = new Vector2Int(_touchPointX, _touchPointY);
            currentCoordinate = _coordinate;

            if (isBlack)
            {
                if (GameSystem.Instance.Grid.CanPutStone(_coordinate, OmokStoneEnum.StoneColor.Black))
                {
                    canPut = true;
                    redStone.SetActive(false);
                    greenStone.SetActive(true);
                    greenStone.transform.position = new Vector3(_touchPointX, _touchPointY, 0);
                }
                else
                {
                    canPut = false;
                    greenStone.SetActive(false);
                    redStone.SetActive(true);
                    redStone.transform.position = new Vector3(_touchPointX, _touchPointY, 0);
                }
            }
            else
            {
                if (GameSystem.Instance.Grid.CanPutStone(_coordinate, OmokStoneEnum.StoneColor.White))
                {
                    canPut = true;
                    redStone.SetActive(false);
                    greenStone.SetActive(true);
                    greenStone.transform.position = new Vector3(_touchPointX, _touchPointY, 0);
                }
                else
                {
                    canPut = false;
                    greenStone.SetActive(false);
                    redStone.SetActive(true);
                    redStone.transform.position = new Vector3(_touchPointX, _touchPointY, 0);
                }
            }

        }
        else if (Input.GetMouseButtonDown(1) && canPut)
        {
            if (isBlack)
                GameSystem.Instance.Grid.PutStone(currentCoordinate, OmokStoneEnum.StoneColor.Black);
            else
                GameSystem.Instance.Grid.PutStone(currentCoordinate, OmokStoneEnum.StoneColor.White);
            canPut = false;
            isBlack = !isBlack;
            redStone.SetActive(false);
            greenStone.SetActive(false);
        }
    }


    public void Execute()
    {
        // 내 턴인지? => 내 턴이 아니라면 실행 불가
        //if (!isMyTurn)
        //{
        //  return;
        //}

        // 터치가 발생한 경우
        if (Input.touchCount > 0)
        {
            Touch _firstTouch = Input.GetTouch(0);

            if (_firstTouch.phase == TouchPhase.Began)
            {
                Vector3 _touchPoint = Camera.main.ScreenToWorldPoint(_firstTouch.position);
                _touchPoint.z = 0;
                int _touchPointX = Mathf.RoundToInt(_touchPoint.x);
                int _touchPointY = Mathf.RoundToInt(_touchPoint.y);

                Vector2Int _coordinate = new Vector2Int(_touchPointX, _touchPointY);

                // 놓을 수 있는지 확인
                if( GameSystem.Instance.Grid.CanPutStone(_coordinate, OmokStoneEnum.StoneColor.White)) 
                {
                    // 놓을 수 있다면 해당 위치에 게임 오브젝트 활성화 (초록색) + Put 버튼 활성화
                    redStone.SetActive(false);
                    greenStone.SetActive(true);
                    greenStone.transform.position = new Vector3(_touchPointX, _touchPointY, 0);
                }
                else
                {
                    // 놓을 수 없다면 해당 위치에 빨간 게임 오브젝트 활성화 + Put 버튼 비활성화
                    greenStone.SetActive(false);
                    redStone.SetActive(true);
                    redStone.transform.position = new Vector3(_touchPointX, _touchPointY, 0);
                }

            }
        }
    }
}
