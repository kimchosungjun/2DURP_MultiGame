using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomName;
    [SerializeField] TextMeshProUGUI roomInPlayerNum;
    [SerializeField] Button room;

    public void RenewRoomState(int _roomInPlayerNum , bool _isActive, string _roomName="")
    {
        roomName.text = _roomName;
        room.interactable = _isActive;
        roomInPlayerNum.text = _roomInPlayerNum.ToString() + " / 2";
    }
}
