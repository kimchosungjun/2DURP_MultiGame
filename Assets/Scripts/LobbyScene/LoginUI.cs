using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LoginUI : MonoBehaviour
{
    enum LoginType
    {
        ID = 0,
        PASSWORD = 1
    }

    UnityAction sucessLogin = null;
    UnityAction failLogin = null;

    [Header("로그인 관련")]    
    [SerializeField] Toggle rememberToggle;
    [SerializeField] TMP_InputField[] loginInputs;
    [SerializeField] Button loginBtn;

    [Header("로그인 실패")]
    [SerializeField] TextMeshProUGUI warnText;
    [SerializeField] Animator warnTextAnim;

    public void Init()
    {
        sucessLogin = null;
        failLogin = null;

        sucessLogin += PassLoginSystem;
        failLogin += FailLoginSystem;
        loginBtn.onClick.AddListener(() => PressLogin());
    }    

    

    /// <summary>
    /// 아이디 창을 눌렀을 때 호출
    /// </summary>
    public void ResetLogin()
    {
        warnText.gameObject.SetActive(false);
    }

    public void PressLogin()
    {
        string _id = loginInputs[(int)LoginType.ID].text;
        string _password = loginInputs[(int)LoginType.PASSWORD].text;
        GameManager.Instance.Account.Login(_id, _password);
    }

    public void CheckLogin(bool isLogin)
    {
        if (isLogin)
            sucessLogin.Invoke();
        else
            failLogin.Invoke();
    }

    public void PassLoginSystem()
    {
        // 다음 씬으로 넘어가거나 ui 비활성화
        Debug.Log("로그인 성공!");
    }

    public void FailLoginSystem()
    {
        warnText.gameObject.SetActive(true);
        warnTextAnim.SetTrigger("Shake");

        // 아이디는 기억하지 않을 때만 초기화
        if (!rememberToggle.isOn)
            loginInputs[(int)LoginType.ID].text = "";

        loginInputs[(int)LoginType.PASSWORD].text = "";
    }
}
