using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MulitGameSceneController : MonoBehaviourPunCallbacks
{
    [Header("게임씬의 시스템 : 1개만 존재"), SerializeField]
    GameSystem gameSystem;

    void Awake()
    {
        Setting();
    }

    #region Init Setting : 한번만
    public void Setting()
    {
        if (OmokGameManager.Instance.Network.IsMasterClient())
        {
            GameObject systemObj = GameObject.FindWithTag("System");
            if (systemObj == null)
            {
                // 시스템이 생성되어 있지 않다면 생성
                CreateCommonGameSystem();
                GameSystem _system = FindObjectOfType<GameSystem>();
                #region 만약 게임 시스템이 없다면 ?
                if (_system == null)
                {
                    Debug.LogError("시스템이 없다!!!");
                    return;
                }
                #endregion
                _system.RoomData = OmokGameManager.Instance.Network.GetRoomData();
            }
            else
            {
                // 이미 생성되어있는 상태라면 패스
                return;
            }
        }
    }
    public void CreateCommonGameSystem() { photonView.RPC("CreateGameSystem", RpcTarget.AllBuffered); }
    [PunRPC]
    public void CreateGameSystem()=> Instantiate(gameSystem.gameObject);
    #endregion
}
