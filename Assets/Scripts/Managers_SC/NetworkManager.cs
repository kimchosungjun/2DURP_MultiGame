using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    const int maxRoomCnt = 9;
    public LobbyUIController Lobby { get; set; } = null;
    
    [SerializeField] GameSystem gameSystem;

    #region 방리스트 갱신

    List<RoomInfo> existRoomGroup = new List<RoomInfo>();
    public void SortRoom(int _page)
    {
        int _cnt = existRoomGroup.Count-1;
        for(int i=0; i<3; i++)
        {
            int _idx = _page*3 + i;
            if (_idx > _cnt)
                Lobby.SetRoomState(i, 0, false, "");
            else
            {
                int _playerCnt = existRoomGroup[_idx].PlayerCount;
                bool _isInteractable = (_playerCnt < 2) ? true : false;
                Lobby.SetRoomState(i, _playerCnt, _isInteractable, existRoomGroup[_idx].Name);
            }
        }
    }
    
    public void JoinRoom(int _idx)
    {
        //Lobby.SortRoom();
        PhotonNetwork.JoinRoom(existRoomGroup[_idx].Name);
    }

    // 방의 상태가 업데이트 될 때마다 알아서 호출해주는 함수 (JoinLobby가 실행될때부터 방의 상태를 업데이트 해준다.)
    public override void OnRoomListUpdate(List<RoomInfo> roomList) 
    {
        existRoomGroup = roomList;
        if(Lobby!=null)
            Lobby.SortRoom();
    }
    #endregion

    #region 서버연결

    public void Init() => Screen.SetResolution(1080, 1920, false); // 핸드폰에서 실행할 땐, 주석처리
    public void Connect() => PhotonNetwork.ConnectUsingSettings(); // 사용자 설정 함수 (포톤 서버에 연결을 시도하는 기능)
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby(); // 포톤 서버에 연결이 성공되면 자동으로 호출되는 콜백 함수를 재정의한 메서드 (OnJoinedLobby 함수가 자동으로 호출)
    public override void OnJoinedLobby() =>PhotonNetwork.LocalPlayer.NickName = OmokGameManager.Instance.Account.AccountData.playerName;
    #endregion

    #region Exit Btn : 서버 연결 끊기
    // 타이틀 씬으로 이동하면서 로그아웃
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        OmokGameManager.Instance.Account.LogOut();
    }
        
    public override void OnDisconnected(DisconnectCause cause) { }
    #endregion

    #region Room : 방 만들기 & 참여하기 & 나가기
    public void CreateRoom()
    {
        int roomCnt = PhotonNetwork.CountOfRooms;
        if(maxRoomCnt<=roomCnt)
        {
            Lobby.ShowWarnRoom("더 이상 방을 만들 수 없습니다.");
            return;
        }

        RoomOptions option = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };
        option.EmptyRoomTtl = 0; // 방이 비어있는 즉시 삭제된다.
        option.PlayerTtl = 0; // 기본값은 -1이고, 0이면 플레이어의 재접속을 지원하지 않는다.
        PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName+"님의 방", option, TypedLobby.Default);
    }

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    //public void LeaveRoom() => PhotonNetwork.LeaveRoom();
    public void LeaveRoom()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.Log("방에 없다.");
            return;
        }
        Debug.Log("나간다.");
        PhotonNetwork.LeaveRoom();
        OmokGameManager.Instance.Loading.ShowLoading(true); 
        OmokGameManager.Instance.Scene.EnterRoom(SceneNameType.Lobby_Scene);
    }

    public int GetRoomCnt() { return PhotonNetwork.CountOfRooms; }

    // 방에 참여하면 자동으로 호출되는 함수 
    public override void OnJoinedRoom() { OmokGameManager.Instance.Loading.ShowLoading(true); OmokGameManager.Instance.Scene.EnterRoom(SceneNameType.MultiGame_Scene); }

    // 방을 만들지 못하는 문제 발생할 때 호출 : 방의 개수를 초과해서 못 만들때 호출되는 것은 아님
    public override void OnCreateRoomFailed(short returnCode, string message)
    {

        Debug.Log($"Room creation failed with code: {returnCode}, message: {message}");

        // 오류 코드에 따른 처리를 추가할 수 있습니다.
        switch (returnCode)
        {
            case 32758: // 예시 오류 코드
                Debug.Log("Room name is already taken.");
                break;
                // 추가적인 오류 코드 처리
        }
    }
   // => Lobby.ShowWarnRoom("방의 수가 너무 많아 생성할 수 없습니다.");

    // 방에 들어갈 수 없을 때 호출
    public override void OnJoinRoomFailed(short returnCode, string message) => Lobby.ShowWarnRoom("해당 방이 더이상 존재하지 않습니다.");

    // 빠른 시작이 불가능할때 호출
    public override void OnJoinRandomFailed(short returnCode, string message) => Lobby.ShowWarnRoom("매칭할 플레이어를 찾지 못했습니다.");

    // 다른 플레이어가 들어왔을 때 자동 호출
    public override void OnPlayerEnteredRoom(Player newPlayer) 
    {
        GameSystem.Instance.JoinRoom();
    }
    
    // 다른 플레이어가 나갔을 때 자동 호출
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.InRoom) photonView.RPC("CreateGameSystem",RpcTarget.All);
    }

    [PunRPC] public void CreateGameSystem()
    {
        GameObject _system = GameObject.FindWithTag("System");
        if (_system == null)
        {
            _system = Instantiate(gameSystem.gameObject);
            _system.name = "GameSystem";
        }
        else
        {
            // 변수 초기화
            GameSystem.Instance.ClearRoomState();
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }
    #endregion

    #region Game : 게임씬에서 확인해야 하는 내용들
    public bool IsMasterClient()
    {
        if (!PhotonNetwork.InRoom)
            return false;

        if (PhotonNetwork.IsMasterClient)
            return true;
        return false;
    }

    public RoomInfo GetRoomData() { return (PhotonNetwork.CurrentRoom!=null) ? PhotonNetwork.CurrentRoom : null; }
    #endregion

    #region RPC 사용법
    //public void Send()
    //{
    //    PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
    //    ChatInput.text = "";
    //}

    //[PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    //void ChatRPC(string msg)
    //{
    //    bool isInput = false;
    //    for (int i = 0; i < ChatText.Length; i++)
    //        if (ChatText[i].text == "")
    //        {
    //            isInput = true;
    //            ChatText[i].text = msg;
    //            break;
    //        }
    //    if (!isInput) // 꽉차면 한칸씩 위로 올림
    //    {
    //        for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
    //        ChatText[ChatText.Length - 1].text = msg;
    //    }
    //}
    #endregion
}
