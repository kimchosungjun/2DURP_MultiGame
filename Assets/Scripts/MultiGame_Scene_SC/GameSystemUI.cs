using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    bool isMyTurn = false;
    PlayerDol dolColor = PlayerDol.None;
    public void DecideDolColor(bool _isBlack) { if (_isBlack) dolColor = PlayerDol.Black; else dolColor = PlayerDol.White; }

    [Header("시작 전 게임 UI"), SerializeField] GameObject beforeGameUI;
    public void AcceptActiveBeforeGameUI(bool _isActive)
    {
        beforeGameUI.SetActive(_isActive);
    }

    [Header("바둑돌 두기"), SerializeField] Button putBtn;
    [SerializeField] GameObject indicateTurn;
    public void AcceptPutBtnState(bool _isActive)
    {
        isTouchScreen = false;
        isMyTurn = _isActive;
        indicateTurn.SetActive(_isActive);
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
        GameSystem.Instance.ReverseTurn();
    }    

    public void ClearTimer()
    {
        StopAllCoroutines();
        timeSlider.value = 30f;
    }
}
