using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUIController : MonoBehaviour
{
    void Awake()
    {
        OmokGameManager.Instance.Network.Lobby = this;
        SortRoom();
    }

    void Update()
    {
        #region Press ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PressExitBtn();
        }
        #endregion
    }

    #region Exit : 뒤로가기 & 게임 종료
    /// <summary>
    /// Call By Exit Btn
    /// </summary>
    public void ReturnTitle()
    {
        OmokGameManager.Instance.Network.Disconnect();
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

    public void PressCreateRoom() => SetActiveStateCreateRoomUI(true);

    public void PressFastGame()
    {
        OmokGameManager.Instance.Network.JoinRandomRoom();
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

    [SerializeField] TMP_InputField inputRoomNameField;
    [SerializeField] GameObject createRoomUIObj;
    [SerializeField] GameObject createRoomWarnObj;
    /// <summary>
    /// 방 만들기를 눌렀을 때 활성화되는 UI
    /// </summary>
    /// <param name="_isActive"></param>
    public void SetActiveStateCreateRoomUI(bool _isActive) { createRoomUIObj.SetActive(_isActive); }
    public void DecideCreateRoom()
    {
        if (inputRoomNameField.text == string.Empty)
            return;
        OmokGameManager.Instance.Network.CreateRoom(inputRoomNameField.text);
        inputRoomNameField.text = string.Empty;
    }

    public void CancelCreateRoom() { SetActiveStateCreateRoomUI(false); inputRoomNameField.text = ""; createRoomWarnObj.SetActive(false); }
    #endregion

    #region Sort Room : 만들어진 방 정렬 & 리스트 버튼
    [Header("방 정렬"), Tooltip("0:이전, 1:다음"),SerializeField] Button[] sortBtns;
    //const int maxSortGroup = 3; // 방 리스트는 세 페이지만 존재한다.
    const int displayRoomCnt = 3;
    int currentSortNum = 0;
  
    [SerializeField, Tooltip("방")] RoomText[] roomTexts;

    public void PressJoinRoom(int _num)
    {
        int _currentRoomNum = displayRoomCnt * currentSortNum + _num;
        OmokGameManager.Instance.Network.JoinRoom(_currentRoomNum);
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
        OmokGameManager.Instance.Network.SortRoom(currentSortNum);
        DecideSortBtnActive();
    }

    public void DecideSortBtnActive()
    {
        if (currentSortNum == 0)
        {
            sortBtns[0].interactable = false;
            sortBtns[1].interactable = true;
        }
        else if (currentSortNum == 1)
        {
            sortBtns[0].interactable = true;
            sortBtns[1].interactable = true;
        }
        else if (currentSortNum == 2)
        {
            sortBtns[0].interactable = true;
            sortBtns[1].interactable = false;
        }
    }

    // 방 정보를 초기화할때 호출
    public void SetRoomState(int _index,int _roomInPlayerNum, bool _isActive, string _roomName = "")
    {
        int _cnt = roomTexts.Length;
        // 방의 인덱스를 초과
        if (_cnt - 1 < _index)
            return;
        roomTexts[_index].RenewRoomState(_roomInPlayerNum, _isActive, _roomName);
    }
    #endregion
}
