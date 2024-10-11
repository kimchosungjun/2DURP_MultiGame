using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ExitRoom();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LogRoomCnt();
        }
    }

    public void ExitRoom()
    {
        GameManager.Instance.Network.LeaveRoom();
    }

    public void LogRoomCnt()
    {
        Debug.Log(GameManager.Instance.Network.GetRoomCnt());
    }
}
