using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public enum PlayerDol
{
    White =0,
    Black = 1,
    None=2
}

public class GameSystemUI : MonoBehaviour
{
    bool isTouchScreen = false;
    Vector2Int currentCoordinate = Vector2Int.zero;
    private void Start()
    {
        AcceptActiveBeforeGameUI(true);
    }

    private void Update()
    {
        if (!isMyTurn)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 _touchPoint = Camera.main.ScreenToWorldPoint(touch.position);
            int _touchPointX = Mathf.RoundToInt(_touchPoint.x);
            int _touchPointY = Mathf.RoundToInt(_touchPoint.y);
            Vector2Int _coordinate = new Vector2Int(_touchPointX, _touchPointY);
            currentCoordinate = _coordinate;
            if (!isTouchScreen)
                isTouchScreen = true;
        }
    }

    #region 시작 전 UI
    [Header("시작 전 게임 UI"), SerializeField] GameObject beforeGameUI;
    [SerializeField] Button matchBtn;
    public void AcceptActiveBeforeGameUI(bool _isActive)
    {
        beforeGameUI.SetActive(_isActive);
        if (OmokGameManager.Instance.Network.MultiInit.IsFullRoom())
            matchBtn.interactable = true;
        else
            matchBtn.interactable = false;
    }

    public void MatchGame() => beforeGameUI.SetActive(false);

    public void ExitRoom() => OmokGameManager.Instance.Network.LeaveRoom();
    #endregion

    #region 이름 

    [Header("상대이름"), SerializeField] TextMeshProUGUI otherName;
    [Header("내이름"), SerializeField] TextMeshProUGUI myName;

    public void SetRoomInPlayerNames()
    {
        List<string> names = OmokGameManager.Instance.Network.GetPlayerNames();
        if(names.Count==2)
        {
            myName.text = names[0];
            otherName.text = names[1];
        }
        else
        {
            myName.text = names[0];
            otherName.text = string.Empty;
        }
    }
    #endregion

    #region 돌 두기

    [Header("바둑돌 두기"), SerializeField] Button putBtn;
    [SerializeField] GameObject myindicateTurn;
    [SerializeField] GameObject otherindicateTurn;

    bool isMyTurn = false;
    PlayerDol dolColor = PlayerDol.None;
    public void DecideDolColor(bool _isBlack)
    {
        bool isIamMaster = PhotonNetwork.IsMasterClient;
        
        if (isIamMaster)
        {
            if (_isBlack)
                dolColor = PlayerDol.Black;
            else
                dolColor = PlayerDol.White;
        }
        else
        {
            if (_isBlack)
                dolColor = PlayerDol.White;
            else
                dolColor = PlayerDol.Black;
        }
    }

    public void AcceptPutBtnState(bool _isActive)
    {
        isTouchScreen = false;
        isMyTurn = _isActive;
        myindicateTurn.SetActive(!_isActive);
        otherindicateTurn.SetActive(_isActive);
        putBtn.gameObject.SetActive(_isActive);
        StopAllCoroutines();
        StartCoroutine(MeasureTurnTime());
    }

    public void PressPutStone()
    {
        if (!isMyTurn)
            return;

        if (!isTouchScreen)
            return;

        if (dolColor == PlayerDol.White)
        {
            //GameSystem.Instance.Grid.CanPutStone(currentCoordinate, dolColor)
        }
        else
        {

        }
    }
 

    [Header("타이머"), SerializeField] Slider timeSlider;
    public IEnumerator MeasureTurnTime()
    {
        float _time = 0f;
        float _timer = 30f;
        timeSlider.value = 30f;
        while (_time < _timer)
        {
            _time += Time.deltaTime;
            timeSlider.value = _timer - _time;
            yield return null;
        }
        timeSlider.value = 0f;
        
        if(isMyTurn) // 내 턴이였을 때만 상대방에게 턴을 넘겨준다.
            GameSystem.Instance.ReverseTurn();
    }    

    public void ClearTimer()
    {
        StopAllCoroutines();
        timeSlider.value = 30f;
    }

    #endregion
}
