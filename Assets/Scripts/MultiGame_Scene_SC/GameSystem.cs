using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public enum TurnOwner
{
    System=0,
    Chief=1,
    Player=2
}

public class GameSystem : MonoBehaviourPunCallbacks
{
    #region Singleton & Managers
    static GameSystem instance = null;
    public static GameSystem Instance
    {
        get
        {
            if (instance == null)
                return null;
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        if (gameSystemUI == null)
            gameSystemUI = FindObjectOfType<GameSystemUI>();
    }

    [SerializeField] OmokGridManager gridManager;
    public OmokGridManager Grid { get { return gridManager; } }
    #endregion

    [Header("RPC"),SerializeField] PhotonView pv;

    #region Relate Game System
    // 턴
    TurnOwner owner = TurnOwner.System;
    public TurnOwner Owner { get => owner; set => pv.RPC(nameof(SetTurn), RpcTarget.AllBuffered, value); }
    [PunRPC]
    void SetTurn(TurnOwner _turnOwner)
    { 
        owner = _turnOwner;
        AcceptTurnChange();
    }

    // 방장이 검은돌인지
    bool isMasterBlack = true;
    public bool IsMasterBlack { get => isMasterBlack; set => pv.RPC(nameof(SetMasterBlack), RpcTarget.AllBuffered, value); }
    [PunRPC] void SetMasterBlack(bool _isMasterBlack) => isMasterBlack = _isMasterBlack;
    
    public void ClearRoomState()
    {
        isFullRoom = false;
        gameSystemUI.AcceptActiveBeforeGameUI(true);
        gameSystemUI.SetOtherName(string.Empty);
        Owner = TurnOwner.System; 
        IsMasterBlack = true;
        pv.RPC("AcceptBeforeStartGameUI", RpcTarget.AllBuffered, false);
    }
    #endregion

    GameSystemUI gameSystemUI = null;
    bool isFullRoom = false;
    public bool IsFullRoom { get => isFullRoom;  }
    #region Join
    public void EnterRoom(string _otherPlayerName)
    {
        isFullRoom = true;
        pv.RPC("AcceptBeforeStartGameUI", RpcTarget.All, true);
        gameSystemUI.SetOtherName(_otherPlayerName);
    }

    [PunRPC] public void AcceptBeforeStartGameUI(bool _isActive)
    {
        if (_isActive == false)
            StopAllCoroutines();
        StartCoroutine(MeasureBeforeGameTime());
        gameSystemUI.AcceptActiveBeforeGameUI(_isActive);
    }

    IEnumerator MeasureBeforeGameTime()
    {
        float _time = 0f;
        float _timer = 5f;
        while (_time < _timer)
        {
            _time += Time.deltaTime;
            yield return null;
        }
        StartGame();
    }

    public void StartGame()
    {
        gameSystemUI.AcceptActiveBeforeGameUI(false);
        DecideDol();
    }

    public void DecideDol() 
    {
        if (pv.IsMine)
        {
            if (isMasterBlack)
                pv.RPC("DecideDolColor", RpcTarget.All, true);
            else
                pv.RPC("DecideDolColor", RpcTarget.All, false);
        }
        else
        {
            if (isMasterBlack)
                pv.RPC("DecideDolColor", RpcTarget.All, false);
            else
                pv.RPC("DecideDolColor", RpcTarget.All, true);
        }

        if (isMasterBlack)
            Owner = TurnOwner.Chief;
        else
            Owner = TurnOwner.Player;

        IsMasterBlack = !isMasterBlack;
    }
    [PunRPC] public void DecideDolColor(bool _isBlack) { gameSystemUI.DecideDolColor(_isBlack); }
    #endregion

    #region Turn
    public void ReverseTurn()
    {
        if (owner == TurnOwner.Chief)
            Owner = TurnOwner.Player;
        else if (owner == TurnOwner.Player)
            Owner = TurnOwner.Chief;
    }

    public void ChangeTurn()
    {
        pv.RPC("AcceptTurnChange", RpcTarget.All);   
    }

    [PunRPC] public void AcceptTurnChange()
    {
        if(owner==TurnOwner.System)
        {
            gameSystemUI.ClearTimer();
        }

        if (pv.IsMine)
        {
            // 내가 방장인 경우
            if(owner== TurnOwner.Chief)
            {
                gameSystemUI.AcceptPutBtnState(true);
            }
            else if(owner== TurnOwner.Player)
            {
                gameSystemUI.AcceptPutBtnState(false);
            }
        }
        else
        {
            // 내가 방장이 아닌 경우
            if (owner == TurnOwner.Player)
            {
                gameSystemUI.AcceptPutBtnState(true);
            }
            else if(owner == TurnOwner.Chief)
            {
                gameSystemUI.AcceptPutBtnState(false);
            }
        }
    }
    #endregion

    public void PutStone()
    {

    }
    
}
