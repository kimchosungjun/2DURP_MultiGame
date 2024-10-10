using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    const int maxRoomCnt = 9;
    public LobbyUIController Lobby { get; set; } = null;

    [Header("DisconnectPanel")]
    public InputField NickNameInput;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public InputField RoomInput;
    public Text WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public Text ListText;
    public Text RoomInfoText;
    public Text[] ChatText;
    public InputField ChatInput;

    [Header("ETC")]
    public Text StatusText;
    public PhotonView PV;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;



    #region 방리스트 갱신
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // 최대페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음버튼
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // 방의 상태가 업데이트 될 때마다 알아서 호출해주는 함수 (JoinLobby가 실행될때부터 방의 상태를 업데이트 해준다.)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList) // 방에 최대 인원수가 들어있거나 방에서 모든 플레이어가 나간 상태라면 목록에서 지워진다. (방이 삭제되는 개념이랑 다른 개념이다.)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion


    #region 서버연결

    public void Init() => Screen.SetResolution(1080, 1920, false); // 핸드폰에서 실행할 땐, 주석처리
    public void Connect() => PhotonNetwork.ConnectUsingSettings(); // 사용자 설정 함수 (포톤 서버에 연결을 시도하는 기능)
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby(); // 포톤 서버에 연결이 성공되면 자동으로 호출되는 콜백 함수를 재정의한 메서드 (OnJoinedLobby 함수가 자동으로 호출)
    public override void OnJoinedLobby()
    {
        PhotonNetwork.LocalPlayer.NickName = GameManager.Instance.Account.AccountData.playerName;
        myList.Clear();
    }

    #endregion

    #region Exit Btn : 서버 연결 끊기
    // 타이틀 씬으로 이동하면서 로그아웃
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        GameManager.Instance.Account.LogOut();
    }
        
    public override void OnDisconnected(DisconnectCause cause) { }
    #endregion

    #region Room : 방 만들기 & 참여하기 & 나가기
    public void CreateRoom()
    {
        int roomCnt = PhotonNetwork.CountOfRooms;
        if(maxRoomCnt<=roomCnt)
        {
            // 경고창 활성화 : 더 이상 방을 생성할 수 없습니다.
            return;
        }

        RoomOptions option = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName+"님의 방", option, TypedLobby.Default);
    }

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnJoinedRoom()
    {
        RoomPanel.SetActive(true);
        RoomRenewal();
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    // 방을 만들지 못하는 문제 발생할 때 호출 : 방의 개수를 초과해서 못 만들때 호출되는 것은 아님
    public override void OnCreateRoomFailed(short returnCode, string message) { Debug.LogError("방을 만들 수 없는 치명적인 문제 발생!!"); }
    
    // 빠른 시작이 불가능할때 호출
    public override void OnJoinRandomFailed(short returnCode, string message) 
    {
        // 매칭할 플레이어를 찾지 못했습니다. 라는 경고 문자 출력
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
    }

    void RoomRenewal()
    {
        ListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
    }
    #endregion


    #region 채팅
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion
}
