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
    }

    public void ExitRoom()
    {
        GameManager.Instance.Network.LeaveRoom();
    }
}
