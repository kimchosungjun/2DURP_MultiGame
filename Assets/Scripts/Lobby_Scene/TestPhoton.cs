using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 실행하면 에러가 발생한다.

public class TestPhoton : MonoBehaviour
{
    string gameVersion = "1";
    private void Awake()
    {
        //이렇게 하면 PhotonNetwork를 사용할 수 있습니다.
        //마스터 클라이언트와 같은 룸에 있는 모든 클라이언트의 LoadLevel() 레벨을 자동으로 동기화합니다
        PhotonNetwork.AutomaticallySyncScene = true;

        //      PhotonNetwork.AutomaticallySyncScene가 true일때
        //      MasterClient와 PhotonNetwork.LoadLevel()을 호출할 수 있고 모든 연결된 플레이어들을 동일한 레벨을 자동으로 불러올 것이다.
    }

    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}
