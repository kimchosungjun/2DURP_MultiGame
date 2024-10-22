using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using OmokStoneEnum;
public class GameSystem : MonoBehaviourPunCallbacks
{
    /*********************** 컴포넌트 **********************************/
    [SerializeField, Header("PhotonView : RPC")] PhotonView pv;
    [SerializeField, Header("Muliti Game UI")] GameSystemUI ui;
    public GameSystemUI UI
    {
        get
        {
            // 찾아서 연결
            if (ui == null)
                ui = FindObjectOfType<GameSystemUI>();
            // 없으면 null
            if (ui == null)
                return null;
            return ui;
        }
    }
    MultiGameController controller;
    public MultiGameController Controller
    {
        get
        {
            // 찾아서 연결
            if (controller == null)
                controller = FindObjectOfType<MultiGameController>();
            // 없으면 null
            if (controller == null)
                return null;
            return controller;
        }
    }

    /*********************** 변수 ***************************************/
    bool isBlack = false; // 현재 검은돌의 턴인지
    bool isMasterBlack= false; // 현재 방장이 검은돌인지 

    public bool IsBlack { get => isBlack; set => pv.RPC(nameof(SetBlack), RpcTarget.AllBuffered, value); }
    [PunRPC] void SetBlack(bool _isBlack) => isBlack = _isBlack;

    public bool IsMasterBlack { get => isMasterBlack; set => pv.RPC(nameof(SetMasterBlack), RpcTarget.AllBuffered, value); }
    [PunRPC] void SetMasterBlack(bool _isMasterBlack) => isMasterBlack = _isMasterBlack;

    StoneColor winColor = StoneColor.Default;
    public StoneColor WinColor { get => winColor; set => pv.RPC(nameof(SetWinColor), RpcTarget.AllBuffered, value); }
    [PunRPC] void SetWinColor(StoneColor _winColor) => winColor = _winColor;
    [SerializeField] OmokGridManager gridManager;
    public OmokGridManager GridManager { get { return gridManager; } }
    /*********************** 초기 설정 *********************************/
    void Awake()
    {
        InitSettingValue();
        BeforeGameStart();
        StartCoroutine(BeforeGameStartTimer());
    }

    // 처음 설정 : 생성 시
    public void InitSettingValue()
    {
        IsBlack = true;
        IsMasterBlack = !isMasterBlack;
    }

    public void BeforeRematch()
    {
        InitSettingValue();
        BeforeGameStart();
        RematchClear();
        StartCoroutine(BeforeGameStartTimer(true));
    }

    public void RematchClear() { pv.RPC("RPC_RematchClear", RpcTarget.AllBuffered); }
    [PunRPC] void RPC_RematchClear() { gridManager.RematchClear(); if (UI != null) { UI.StopTimer(); UI.GameWinClearUI(); } }

    #region Game 시작 전 : 나가기 & 대결 버튼 활성화 
    public void BeforeGameStart()
    {
        pv.RPC("RPC_BeforeGameStart", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_BeforeGameStart()
    {
        if (UI == null)
            return;
        UI.DecideActiveState_BeforeUI(true);
        UI.InitPlayerName();
    }
    #endregion

    // 생성시 & 재대결 시 호출 : 5초 후 게임 시작
    public void StartBeforeGameTimer() { StartCoroutine(BeforeGameStartTimer()); }
    public void StopAllTimer() { StopAllCoroutines(); }
    IEnumerator BeforeGameStartTimer(bool _isRematch=false)
    {
        if (!_isRematch)
        {
            if (!PhotonNetwork.IsMasterClient)
                yield break;
        }
       
        float timer = 0f;
        while (timer < 5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        GameStart();
    }

    #region Game 시작 : 턴 (타이머) 실행 & 시작 전 UI 비활성화 
    public void GameStart()
    {
        pv.RPC("RPC_GameStart", RpcTarget.AllBuffered);
        pv.RPC("RPC_StartTurnTimer", RpcTarget.AllBuffered);
        SetTurn();
    }

    // 시작 전 게임 UI(대결 & 나가기) 비활성화
    [PunRPC] void RPC_GameStart()
    {
        if (UI == null)
            return;
        UI.IsPlayingGame = true;
        UI.DecideActiveState_BeforeUI(false);

        if (Controller == null)
            return;
        Controller.StopGame = false;
    }
    // 타이머 UI 활성화
    [PunRPC] void RPC_StartTurnTimer()
    {
        if (UI == null)
            return;
        UI.StartTimer();
        UI.SetStoneColor(IsMasterBlack);
    }

    // 턴을 변경 : RPC로 선언되어 있음
    public void ReverseTurnByTime()  { if (!PhotonNetwork.IsMasterClient) return; IsBlack = !IsBlack; SetTurn(); }
    public void ReverseTurnByPut() { IsBlack = !IsBlack; SetTurn(); }
    public void SetTurn()
    {
        if (isBlack)
        {
            // 흑돌의 턴
            if (isMasterBlack)
            {
                // 방장이 검은돌이면 : 방장의 턴
                pv.RPC("RPC_SetTurn", RpcTarget.AllBuffered, true);
            }
            else
            {
                // 참가자의 턴
                pv.RPC("RPC_SetTurn", RpcTarget.AllBuffered, false);
            }
        }
        else
        {
            // 흰돌의 턴
            if (isMasterBlack)
            {
                // 방장이 검은돌이면 : 참가자의 턴
                pv.RPC("RPC_SetTurn", RpcTarget.AllBuffered, false);
            }
            else
            {
                // 방장의 턴
                pv.RPC("RPC_SetTurn", RpcTarget.AllBuffered, true);
            }
        }
    }
    [PunRPC] void RPC_SetTurn(bool _isMasterTurn)
    {
        if (UI == null)
            return;
        UI.SetTurnUI(_isMasterTurn);

        if (Controller == null)
            return;
        Controller.TurnOffIndiacateStone();
    }
    #endregion

    // 게임 시작 타이머 종료
    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void EndGame()
    {
        BeforeRematch();
    }

    #region UI 초기화
    public void ClearUIState()
    {
        pv.RPC("RPC_ClearUIState", RpcTarget.AllBuffered);
    }
    [PunRPC] void RPC_ClearUIState()
    {
        if (UI == null)
            return;
        //UI.ClearUIState();
    }
    #endregion
}
