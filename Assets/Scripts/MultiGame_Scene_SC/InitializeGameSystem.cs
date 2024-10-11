using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeGameSystem : MonoBehaviour
{
    void Awake()
    {
        Setting();
    }

    public void Setting()
    {
        if (GameManager.Instance.Network.IsMasterClient())
        {
            GameObject systemObj = GameObject.FindWithTag("System");
            if (systemObj == null)
            {
                // 시스템이 생성되어 있지 않다면 생성
                GameManager.Instance.Network.CreateCommonGameSystem();
                GameSystem _system = FindObjectOfType<GameSystem>();
                #region 만약 게임 시스템이 없다면 ?
                if (_system == null)
                {
                    Debug.LogError("시스템이 없다!!!");
                    return;
                }
                #endregion
                _system.RoomData = GameManager.Instance.Network.GetRoomData();
            }
            else
            {
                // 이미 생성되어있는 상태라면 패스
                return;
            }
        }
    }
}
