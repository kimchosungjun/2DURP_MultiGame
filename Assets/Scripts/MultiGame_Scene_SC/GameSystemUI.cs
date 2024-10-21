using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameSystemUI : MonoBehaviour
{
    [SerializeField] MultiGameController controller;
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

    public bool IsPlayingGame { get; set; } = false;

    #region UI 초기 값 설정
    void Awake()
    {
        InitUISetting();
    }

    public void InitUISetting()
    {
        stoneColorImages[0].gameObject.SetActive(false);
        stoneColorImages[1].gameObject.SetActive(false);
        timeSlider.maxValue = turnTimer;
        timeSlider.value = timeSlider.maxValue;
        DecideActiveState_BeforeUI(true);
        turnPanels[0].SetActive(false);
        turnPanels[1].SetActive(false);
        putBtn.interactable = false;
        //InitPlayerName();
    }
    #endregion


    [SerializeField, Header("게임 시작 전 UI")] GameObject beforeUIObj;
    [SerializeField] Button matchBtn;
    public void DecideActiveState_BeforeUI(bool _isActive)
    {
        bool _isFullRoom = OmokGameManager.Instance.Network.IsFullRoom();
        if (_isFullRoom)
            matchBtn.interactable = true;
        else
            matchBtn.interactable = false;

        beforeUIObj.SetActive(_isActive);
    }

    // 대결 버튼 
    public void ReadyMatchGame() { DecideActiveState_BeforeUI(false); }

    // 나가기 버튼
    public void LeaveRoom() { OmokGameManager.Instance.Network.LeaveRoom(); }

    [SerializeField, Header("타이머 슬라이더")] Slider timeSlider;
    [SerializeField] float turnTimer;

    public void StartTimer()
    {
        StopTimer();
        StartCoroutine(TurnTimer());
    }

    IEnumerator TurnTimer()
    {
        float timer = 0f;
        timeSlider.value = turnTimer;
        while (timer < turnTimer)
        {
            timer += Time.deltaTime;
            timeSlider.value = turnTimer - timer;
            yield return null;
        }
        timeSlider.value = 0f;
        System.ReverseTurnByTime();
    }

    public void StopTimer() { StopAllCoroutines(); }

    [SerializeField, Header("돌 두기 버튼")] Button putBtn;
    [SerializeField, Header("턴을 나타내주는 Panel, 0은 내꺼, 1은 상대방꺼")] GameObject[] turnPanels;
    public void SetTurnUI(bool _isMasterTurn)
    {
        bool _isIamMaster = OmokGameManager.Instance.Network.IsMasterClient();
        if (_isIamMaster == _isMasterTurn)
        {
            // 내 턴
            putBtn.interactable = true;
            turnPanels[0].SetActive(false);
            turnPanels[1].SetActive(true);
            controller.IsMyTurn = true;
        }
        else
        {
            // 상대 턴
            putBtn.interactable = false;
            turnPanels[0].SetActive(true);
            turnPanels[1].SetActive(false);
            controller.IsMyTurn = false;
        }
        StartTimer();
    }

    // 상대방이 나갔을 때 호출 : 강제종료
    public void ClearUIState()
    {
        InitUISetting();
        InitPlayerName();
        if (IsPlayingGame)
        {
            ShowWinText(nickNameTexts[0].text + "님이 승리했습니다!");
            IsPlayingGame = false;
        }
    }

    [SerializeField, Header("0은 내 이름, 1은 상대방 이름")] TextMeshProUGUI[] nickNameTexts;
    public void InitPlayerName()
    {
        List<string> playerNickNameGroup = OmokGameManager.Instance.Network.GetPlayerNames();
        int nickNameCnt = playerNickNameGroup.Count;
        nickNameTexts[0].text = playerNickNameGroup[0];
        nickNameTexts[1].text = (nickNameCnt < 2) ? "" : playerNickNameGroup[1];
    }

    [SerializeField, Header("승리 UI")] TextMeshProUGUI winText;
    public void ShowWinText(string _winText)
    {
        StopTimer();
        controller.StopGame = true;
        winText.text = _winText;
        winText.gameObject.SetActive(true);
        Invoke("CloseWinText", 3f);
    }
    public void CloseWinText() { winText.gameObject.SetActive(false); }

    [SerializeField, Header("0은 나, 1은 상대방 : 돌의 색 이미지")] Image[] stoneColorImages;
    [SerializeField, Header("0은 흰돌, 1은 검은돌")] Sprite[] stoneSprites;
    public void SetStoneColor(bool isMasterBlack) 
    {
        bool isIamMaster = OmokGameManager.Instance.Network.IsMasterClient();
        if (isMasterBlack)
        {
            if (isIamMaster)
            {
                controller.SetStoneColor(true);
                stoneColorImages[0].sprite = stoneSprites[1];
                stoneColorImages[1].sprite = stoneSprites[0];
            }
            else
            {
                controller.SetStoneColor(false);
                stoneColorImages[0].sprite = stoneSprites[0];
                stoneColorImages[1].sprite = stoneSprites[1];
            }
        }
        else
        {
            if (isIamMaster)
            {
                controller.SetStoneColor(false);
                stoneColorImages[0].sprite = stoneSprites[0];
                stoneColorImages[1].sprite = stoneSprites[1];
            }
            else
            {
                controller.SetStoneColor(true);
                stoneColorImages[0].sprite = stoneSprites[1];
                stoneColorImages[1].sprite = stoneSprites[0];
            }
        }
        stoneColorImages[0].gameObject.SetActive(true);
        stoneColorImages[1].gameObject.SetActive(true);
    }
}
