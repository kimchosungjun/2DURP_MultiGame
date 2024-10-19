using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OmokStoneEnum;

public class SoloPlayController : MonoBehaviour
{
    public bool StopGame { get; set; } = false;

    [SerializeField, Header("UI")]
    SoloGameUI soloUI;

    [SerializeField]
    GameObject greenStone;

    [SerializeField]
    GameObject redStone;

    [Header("격자 매니저 : 오목 체크"), SerializeField]
    SoloGridManager gridManager;

    [Header("플레이어 1,2의 Put Btn"),SerializeField, Tooltip("0:Player1, 1:Player2")]
    Button[] putBtns;

    bool isBlack;
    bool isPlayer1Black;

    bool canPut ;
    Vector2Int currentCoordinate = Vector2Int.zero;

    public void Awake()
    {
        canPut = false;
        isBlack = true;
        isPlayer1Black = true;
        greenStone.SetActive(false);
        redStone.SetActive(false);
        SetActiveBtn();
    }

    // 게임이 끝난 후 다시 재대결 할때 호출
    public void Rematch()
    {
        canPut = false;
        isBlack = true;
        isPlayer1Black = !isPlayer1Black;
        greenStone.SetActive(false);
        redStone.SetActive(false);
        gridManager.Clear();
        SetActiveBtn();
    }

    public void SetActiveBtn()
    {
        if (isBlack)
        {
            if(isPlayer1Black)
            {
                putBtns[0].interactable = true;
                putBtns[1].interactable = false;
            }
            else
            {
                putBtns[1].interactable = true;
                putBtns[0].interactable = false;
            }
        }
        else
        {
            if (isPlayer1Black)
            {
                putBtns[0].interactable = false;
                putBtns[1].interactable = true;
            }
            else
            {
                putBtns[1].interactable = true;
                putBtns[0].interactable = false;
            }
        }
    }

    public void Update()
    {
        // 승자가 정해지면 잠시 게임 중단
        if (StopGame)
            return;

        if (Input.touchCount>0)
        {
            // UI 터치를 방지
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            Touch _firstTouch = Input.GetTouch(0);
            Vector3 _touchPoint = Camera.main.ScreenToWorldPoint(_firstTouch.position);
            _touchPoint.z = 0;
            int _touchPointX = Mathf.RoundToInt(_touchPoint.x);
            int _touchPointY = Mathf.RoundToInt(_touchPoint.y);
            Vector2Int _coordinate = new Vector2Int(_touchPointX, _touchPointY);
            currentCoordinate = _coordinate;

            if (isBlack)
            {
                if (gridManager.CanPutStone(_coordinate, OmokStoneEnum.StoneColor.Black))
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
                if (gridManager.CanPutStone(_coordinate, OmokStoneEnum.StoneColor.White))
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
    }


    public void PutStone()
    {
        if (!canPut)
            return;

        if (isBlack)
            gridManager.PutStone(currentCoordinate, OmokStoneEnum.StoneColor.Black);
        else
            gridManager.PutStone(currentCoordinate, OmokStoneEnum.StoneColor.White);

        canPut = false;
        isBlack = !isBlack;
        redStone.SetActive(false);
        greenStone.SetActive(false);
        SetActiveBtn();
    }

    public void WinGame(StoneColor color)
    {
        bool _player1Win = true;
        switch (color)
        {
            case StoneColor.White:
                if (isPlayer1Black)
                    _player1Win = false;
                else
                    _player1Win = true;
                break;
            case StoneColor.Black:
                if (isPlayer1Black)
                    _player1Win = true;
                else
                    _player1Win = false;
                break;
        }
        soloUI.Victory(_player1Win);
    }
}
