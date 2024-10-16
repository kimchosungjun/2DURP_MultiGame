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

    // RPC : 포톤 뷰
    [Header("RPC"),SerializeField] PhotonView pv;
    public PhotonView PV { get { return pv; } }
    #region Relate Game System
    
    // 턴
    TurnOwner owner = TurnOwner.System;
    public TurnOwner Owner { get => owner; set => pv.RPC(nameof(SetTurn), RpcTarget.AllBuffered, value); }
    [PunRPC] void SetTurn(TurnOwner _turnOwner)
    {
        // 주의 : 프로퍼티로 선언하면 무한 루프 걸림 
        owner = _turnOwner;

        // 시스템의 턴일 때 : 타이머 UI 초기화 & 정지
        if (owner == TurnOwner.System)
        {
            SystemUI.ClearTimer();
            return;
        }

        // 플레이어들의 턴일 때 : 게임이 시작함
        if (pv.IsMine)
        {
            // 내가 방장인 경우
            if (owner == TurnOwner.Chief)
                SystemUI.AcceptPutBtnState(true);
            else if (owner == TurnOwner.Player)
                SystemUI.AcceptPutBtnState(false);
        }
        else
        {
            // 내가 방장이 아닌 경우
            if (owner == TurnOwner.Player)
            {
                SystemUI.AcceptPutBtnState(true);
            }
            else if (owner == TurnOwner.Chief)
            {
                SystemUI.AcceptPutBtnState(false);
            }
        }
    }

    // 방장이 검은돌인지
    bool isMasterBlack = true;
    public bool IsMasterBlack { get => isMasterBlack; set => pv.RPC(nameof(SetMasterBlack), RpcTarget.AllBuffered, value); }
    [PunRPC] void SetMasterBlack(bool _isMasterBlack) => isMasterBlack = _isMasterBlack;

    // 게임의 상태를 나타내주는 UI
    GameSystemUI gameSystemUI = null;
    public GameSystemUI SystemUI
    {
        get
        {
            if (gameSystemUI == null)
            {
                GameObject uiObj = GameObject.FindWithTag("SystemUI");
                gameSystemUI = uiObj.GetComponent<GameSystemUI>();
            }
            return gameSystemUI;
        }
    }

    /// <summary>
    /// 방을 만들었을 때, 혹은 다른 플레이어가 나갔을 때 호출한다.
    /// </summary>
    /// <param name="_isCreateOrClear"></param>
    public void InitRoomState() => pv.RPC("RPC_ClearRoomState", RpcTarget.AllBuffered);
    public void EnterRoom() => pv.RPC("RPC_AcceptBeforeStartGameUI", RpcTarget.AllBuffered);

    [PunRPC] public void RPC_ClearRoomState()
    {
        SystemUI.AcceptActiveBeforeGameUI(true);
        SystemUI.SetRoomInPlayerNames();
        Owner = TurnOwner.System;
        IsMasterBlack = true;
    }

    [PunRPC] public void RPC_AcceptBeforeStartGameUI()
    {
        StopAllCoroutines();
        if (OmokGameManager.Instance.Network.MultiInit.IsFullRoom())
            StartCoroutine(MeasureBeforeGameTime());
        SystemUI.AcceptActiveBeforeGameUI(true);
        SystemUI.SetRoomInPlayerNames();
    }
    #endregion

    #region Join

    IEnumerator MeasureBeforeGameTime()
    {
        if (!pv.IsMine)
            yield break;

        float _time = 0f;
        float _timer = 5f;
        while (_time < _timer)
        {
            _time += Time.deltaTime;
            yield return null;
        }
        StartGame();
    }

    public void StartGame() { pv.RPC("RPC_StartGame", RpcTarget.All); }

    [PunRPC] public void RPC_StartGame() 
    {
        SystemUI.AcceptActiveBeforeGameUI(false);
        SystemUI.DecideDolColor(IsMasterBlack);
        if (isMasterBlack)
            Owner = TurnOwner.Chief;
        else
            Owner = TurnOwner.Player;

        IsMasterBlack = !isMasterBlack;
    } 
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


    #endregion

    public void PutStone()
    {

    }
    
}
