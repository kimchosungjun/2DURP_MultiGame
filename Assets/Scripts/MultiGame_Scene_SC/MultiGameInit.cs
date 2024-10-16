using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MultiGameInit : MonoBehaviour
{
    private void Awake()
    {
        OmokGameManager.Instance.Loading.ShowLoading(false);
        InitSetting();
    }

    public void InitSetting()
    {
        // 1) 네트워크 매니저에 연결 => CreateGameSystem 호출이 목적 (상대방이 게임을 나갔을 때 호출)
        OmokGameManager.Instance.Network.MultiInit = this;

        // 2) 게임 시스템이 없다면 생성       
        CreateGameSystem();
    }

    public void CreateGameSystem()
    {
        GameObject gameSystemObj = GameObject.FindWithTag("System");
        if (gameSystemObj != null)
        {
            GameSystem.Instance.InitRoomState();
            GameSystem.Instance.EnterRoom();
            return;
        }
        gameSystemObj = PhotonNetwork.Instantiate("GameSystem", Vector3.zero,Quaternion.identity);
        gameSystemObj.name = "GameSystem";
        GameSystem gameSystem = gameSystemObj.GetComponent<GameSystem>();
        gameSystem.InitRoomState();
        gameSystem.EnterRoom();
    }

    public bool IsFullRoom()
    {
        NetworkManager net = OmokGameManager.Instance.Network;
        if (net.GetRoomInPlayers().Count == 2)
            return true;
        return false;
    }
}
