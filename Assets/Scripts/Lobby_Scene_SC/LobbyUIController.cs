using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUIController : MonoBehaviour
{
    void Awake()
    {
        GameManager.Instance.Network.Lobby = this;   
    }

    void Update()
    {
        // 뒤로가기 누름 : 종료창 활성화
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PressExitBtn();
        }
    }

    #region Exit : 뒤로가기 & 게임 종료
    /// <summary>
    /// Call By Exit Btn
    /// </summary>
    public void ReturnTitle()
    {
        GameManager.Instance.Network.Disconnect();
    }

    [Header("게임 나가기"),SerializeField] GameObject[] exitPanels;
    public void PressExitBtn()
    {
        if(exitPanels[0].activeSelf)
        {
            exitPanels[0].SetActive(false);
            exitPanels[1].SetActive(false);
            return;
        }
        exitPanels[0].SetActive(true);
        exitPanels[1].SetActive(true);
    }

    public void PressYesExit() { Application.Quit(); }
    #endregion

    #region Room : 방 만들기 & 빠른 시작 & 경고문자
    [Header("방 만들기"), SerializeField] TextMeshProUGUI warnText;
    [SerializeField] GameObject warnTextObj;

    public void PressCreateRoom()
    {
        GameManager.Instance.Network.CreateRoom();
    }

    public void PressFastGame()
    {
        GameManager.Instance.Network.JoinRandomRoom();
    }

    public void ShowWarnRoom(string _text)
    {
        warnText.text = _text;
        warnTextObj.SetActive(true);
    }

    public void CheckWarn()
    {
        warnTextObj.SetActive(false);
    }
    #endregion

    #region Sort Room : 만들어진 방 정렬 & 리스트 버튼
    [Header("방 정렬"), Tooltip("0:이전, 1:다음"),SerializeField] Button[] sortBtns;
    const int maxSortGroup = 3; // 방 리스트는 세 페이지만 존재한다.
    const int displayRoomCnt = 3;
    int currentSortNum = 0;
  
    [SerializeField, Tooltip("방")] RoomText[] roomTexts;

    public void PressJoinRoom(int _num)
    {
        int _currentRoomNum = displayRoomCnt * currentSortNum + _num;
        GameManager.Instance.Network.JoinRoom(_currentRoomNum);
    }

    public void PressPrevListBtn()
    {
        if (currentSortNum <= 0)
            return;
        currentSortNum -= 1;
        SortRoom();
    }

    public void PressNextListBtn()
    {
        if (currentSortNum >= 2)
            return;
        currentSortNum += 1;
        SortRoom();
    }

    public void SortRoom()
    {
        // 방 정보를 입력
        GameManager.Instance.Network.SortRoom(currentSortNum);
        DecideSortBtnActive();
    }

    public void DecideSortBtnActive()
    {
        if (currentSortNum == 0)
            sortBtns[0].interactable = false;
        else if (currentSortNum == 1)
        {
            sortBtns[0].interactable = true;
            sortBtns[1].interactable = true;
        }
        else if (currentSortNum == 2)
            sortBtns[1].interactable = false;
    }

    // 방 정보를 초기화할때 호출
    public void SetRoomState(int _index,int _roomInPlayerNum, bool _isActive, string _roomName = "")
    {
        int _cnt = roomTexts.Length;
        if (_cnt - 1 < _index)
        {
            Debug.LogError("인덱스 범위를 초과했습니다.");
            return;
        }

        roomTexts[_index].RenewRoomState(_roomInPlayerNum, _isActive, _roomName);
    }
    #endregion
}
