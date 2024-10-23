using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using OmokStoneEnum;

public class MultiGameController : MonoBehaviour
{
    public bool StopGame { get; set; } = false;
    public bool IsMyTurn { get; set; } = false;
    bool canPut;
    StoneColor playerStoneColor;
    Vector2Int currentCoordinate = Vector2Int.zero;

    GameSystem system = null;
    public GameSystem System
    {
        get
        {
            if (system == null)
                system = FindObjectOfType<GameSystem>();
            if (system == null)
                return null;
            return system;
        }
    }

    [SerializeField] GameObject[] greenNRedStone;

    void Awake()
    {
        greenNRedStone[0].gameObject.SetActive(false);
        greenNRedStone[1].gameObject.SetActive(false);
    }

    public void SetStoneColor(bool _isPlayerBlack) 
    {
        if (_isPlayerBlack)
            playerStoneColor = StoneColor.Black;
        else
            playerStoneColor = StoneColor.White;

        canPut = false;
    }

    void Update()
    {
        // 승자가 정해지면 잠시 게임 중단
        if (StopGame)
            return;

        if (!IsMyTurn)
            return;

        if (Input.touchCount > 0)
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

            // Grid 범위를 벗어나면 입력을 받지 않는다.
            if (System.GridManager.CheckOverRange(_coordinate))
                return;

            currentCoordinate = _coordinate;

            if (System.GridManager.CanPutStone(_coordinate, playerStoneColor))
            {
                canPut = true;
                greenNRedStone[0].SetActive(true);
                greenNRedStone[0].transform.position = new Vector3(currentCoordinate.x, currentCoordinate.y, 0);
                greenNRedStone[1].SetActive(false);
            }
            else
            {
                canPut = false;
                greenNRedStone[1].SetActive(true);
                greenNRedStone[1].transform.position = new Vector3(currentCoordinate.x, currentCoordinate.y, 0);
                greenNRedStone[0].SetActive(false);
            }
        }
    }


    public void PutStone()
    {
        if (!canPut)
            return;
        
        System.GridManager.PutStone(currentCoordinate, playerStoneColor);
        // 턴을 넘겨줌
        greenNRedStone[0].gameObject.SetActive(false);
        greenNRedStone[1].gameObject.SetActive(false);
        canPut = false;
    }

    public void TurnOffIndiacateStone()
    {
        greenNRedStone[0].gameObject.SetActive(false);
        greenNRedStone[1].gameObject.SetActive(false);
    }
}
