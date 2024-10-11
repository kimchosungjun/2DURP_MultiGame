using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum TurnOwner
{
    System=0,
    Chief=1,
    Player=2
}

public class GameSystem : MonoBehaviour
{
    RoomInfo roomData = null;
    public RoomInfo RoomData { get { if (roomData == null) return null; return roomData; } set { roomData = value; } }

    bool isChiefPutBlack = false; // 방장이 검은돌을 두었는지
    bool changePlayer = false;
    bool isGameStart = false;
    bool isFullRoom = false;

    [Header("게임 시스템")]
    [SerializeField, Range(0f,5f)] float waitingTime = 5f;
    [SerializeField, Range(0f, 30f)] float turnTime = 30f;

    float waitingTimer = 0f;
    float turnTimer = 0f;

    TurnOwner owner = TurnOwner.System;

    void Awake()
    {
        InitSetting();
    }

    // 사람이 나가거나 처음 생성될때 호출
    public void InitSetting()
    {
        owner = TurnOwner.System;

        isFullRoom = false;
        changePlayer = false;
        isGameStart = false;
        isChiefPutBlack = false;

        waitingTimer = 0f;
        turnTimer = 0f;
    }

    public void EnterRoom()
    {
        if (!PhotonNetwork.InRoom)
            return ;

        int _cnt = PhotonNetwork.CurrentRoom.PlayerCount;
        if (_cnt == 2)
            return ;
        return ;
    }

    void Update()
    {
        #region 방에 사람이 가득 차 있을 때
        if (isFullRoom)
        {
            if (isGameStart)
            {
                TurnTime();
            }
            else
            {
                if (!changePlayer)
                {
                    changePlayer = true;
                    GameStart();
                }
                else
                {
                    waitingTimer += Time.deltaTime;
                    if (waitingTimer > waitingTime)
                    {
                        waitingTimer = 0f;
                        GameStart();
                    }
                }
            }
        }
        #endregion

        #region 방에 사람이 없을 때
        if (!isFullRoom)
        {
            if (isGameStart)
            {
                // 남아있는 사람의 승리!
                isGameStart = false;
            }


        }
        #endregion
    }

    public void GameStart()
    {
        isGameStart = true;
    }

    

    public void TurnTime()
    {
        turnTimer += Time.deltaTime;
        if(turnTimer> turnTime)
        {
            turnTimer = 0f;
            ChangePlayerTurn();
        }
    }

    public void ChangePlayerTurn()
    {
        if(owner==TurnOwner.Chief)
        {
            owner = TurnOwner.Player;
        }
        else if(owner==TurnOwner.Player)
        {
            owner = TurnOwner.Chief;
        }

        // 타이머 UI 리셋
        // 돌 두는 턴 회수 & 부여
    }

    public void GameOver()
    {

    }
}
