using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateAccountUI : MonoBehaviour
{
    enum CreateAccountType
    {
        ID =0,
        PASSWORD=1,
        REPEAT_PASSWORD=2,
        PANEL=3,
        FRAME=4,
        COMPLETE = 5,
        MAX=6
    }

    [Header("회원가입")]
    string createPlayerID="";
    string createPlayerPassword = "";

    [SerializeField, Tooltip("회원가입 버튼")] Button accountBtn;
    [SerializeField] GameObject[] createAccountObjects;
    [SerializeField] TMP_InputField[] inputAccountFields;

    #region Initialize UI
    public void Init()
    {
        // 회원가입창이 열려있지 않도록 초기화
        int _cnt = (int)CreateAccountType.MAX;
        for (int i = 0; i < _cnt; i++)
        {
            createAccountObjects[i].SetActive(false);
        }

        // 회원가입 로그들이 남아있지 않도록 초기화
        int _fieldCnt = inputAccountFields.Length;
        for (int k = 0; k < _fieldCnt; k++)
        {
            inputAccountFields[k].text = "";
        }
    }

    public void ActiveCreateAccountUI()
    {
        createAccountObjects[(int)CreateAccountType.PANEL].SetActive(true);
        createAccountObjects[(int)CreateAccountType.FRAME].SetActive(true);
        createAccountObjects[(int)CreateAccountType.ID].SetActive(true);
    }
    #endregion

    #region ID
    bool isShowOverlapID = false;
    [Header("아이디 생성")]
    [SerializeField] TextMeshProUGUI warnOverlapIDText;
    [SerializeField] Animator warnOverlapIDAnim;
    [SerializeField] Button createIDBtn;

    /// <summary>
    /// ID 입력
    /// </summary>
    /// <param name="_value"></param>
    public void IDInput(string _value)
    {
        string _inputID = _value;
        _inputID = _inputID.Replace(" ", "");

        if(isShowOverlapID)
        {
            isShowOverlapID = false;
            warnOverlapIDText.gameObject.SetActive(false);
        }
        
        // id 길이에 따라 버튼 활성, 비활성화
        if (_inputID.Length >= 4 && _inputID.Length<9)
        {
            if(!createIDBtn.gameObject.activeSelf)
                createIDBtn.gameObject.SetActive(true);
        }
        else
        {
            if (createIDBtn.gameObject.activeSelf)
                createIDBtn.gameObject.SetActive(false);
        }
    }

    public void ClickCreateID()
    {
        createIDBtn.interactable = false;
        string _inputID = inputAccountFields[(int)CreateAccountType.ID].text;
        _inputID = _inputID.Replace(" ", "");
        GameManager.Instance.Account.CheckIDCondition(_inputID, CheckOverlapID);
    }

    public void CheckOverlapID(bool isNotOverlap)
    {
        if (isNotOverlap)
        {
            // 중복이 아니라면?
            createPlayerID = inputAccountFields[(int)CreateAccountType.ID].text;
            inputAccountFields[(int)CreateAccountType.ID].text = "";
            createAccountObjects[(int)CreateAccountType.ID].SetActive(false);
            createAccountObjects[(int)CreateAccountType.PASSWORD].SetActive(true);
            createIDBtn.interactable = true;
        }
        else
        {
            // 중복이라면?
            SetIDText("해당 ID는 중복된 ID입니다.");
        }
    }

    public void SetIDText(string _text)
    {
        warnOverlapIDText.text = _text;
        createIDBtn.interactable = true;
        inputAccountFields[(int)CreateAccountType.ID].text = "";
        warnOverlapIDText.gameObject.SetActive(true);
        isShowOverlapID = true;
        warnOverlapIDAnim.SetTrigger("Shake");
    }
    #endregion

    #region PASSWORD

    [Header("패스워드 생성")]
    bool isHaveSpacePassword = false;
    [SerializeField] Button createPasswordBtn;
    [SerializeField] Animator passwordWarnAnim;
    [SerializeField] TextMeshProUGUI passwordWarnText;

    public void PASSWORDInput(string _value)
    {
        if (_value.Contains(' '))
        {
            if (!isHaveSpacePassword)
                passwordWarnText.gameObject.SetActive(true);
            isHaveSpacePassword = true;
        }
        else
        {
            if (isHaveSpacePassword)
                passwordWarnText.gameObject.SetActive(false);
            isHaveSpacePassword = false;
        }

        if (passwordWarnText.gameObject.activeSelf)
            passwordWarnText.gameObject.SetActive(false);

        string _inputPassword = _value;
        int _cnt = _inputPassword.Length;
        if (_cnt > 4 && !passwordWarnText.gameObject.activeSelf)
            createPasswordBtn.gameObject.SetActive(true);
        else if(_cnt<4 || passwordWarnText.gameObject.activeSelf)
            createPasswordBtn.gameObject.SetActive(false);
    }

    public void SetPassowrdText(string _text)
    {
        if (createPasswordBtn.gameObject.activeSelf)
            createPasswordBtn.gameObject.SetActive(false);
        inputAccountFields[(int)CreateAccountType.PASSWORD].text = "";
        passwordWarnText.text = _text;
        passwordWarnText.gameObject.SetActive(true);
        passwordWarnAnim.SetTrigger("Shake");
    }

    public void DecidePassword()
    {
        if (!GameManager.Instance.Account.CheckPasswordCondition(inputAccountFields[(int)CreateAccountType.PASSWORD].text))
            return;
        
        createPlayerPassword = inputAccountFields[(int)CreateAccountType.PASSWORD].text;
        inputAccountFields[(int)CreateAccountType.PASSWORD].text = "";
        createAccountObjects[(int)CreateAccountType.PASSWORD].SetActive(false);
        createAccountObjects[(int)CreateAccountType.REPEAT_PASSWORD].SetActive(true);
    }

    #endregion

    #region REPEAT_PASSWORD
    [Header("패스워드 재입력")]
    [SerializeField] TextMeshProUGUI warnDifferentText;
    [SerializeField] Animator warnDifferentAnim;
    [SerializeField] Button repeatPasswordBtn;

    public void RepeatPASSWORDInput(string _value)
    {
        string _inputPassword = _value;
        _inputPassword = _inputPassword.Replace(" ", "");
        inputAccountFields[(int)CreateAccountType.REPEAT_PASSWORD].text = _inputPassword;

        if (warnDifferentAnim.gameObject.activeSelf)
            warnDifferentAnim.gameObject.SetActive(false);

        if (_inputPassword.Length > 4)
            repeatPasswordBtn.gameObject.SetActive(true);
        else
            repeatPasswordBtn.gameObject.SetActive(false);
    }

    public void PressRepeatPassword()
    {
        string _repeat = inputAccountFields[(int)CreateAccountType.REPEAT_PASSWORD].text;
        if (_repeat.Equals(createPlayerPassword))
        {
            // 아이디와 패스워드를 DB에 저장
            inputAccountFields[(int)CreateAccountType.REPEAT_PASSWORD].text = "";
            createAccountObjects[(int)CreateAccountType.REPEAT_PASSWORD].SetActive(false);
            createAccountObjects[(int)CreateAccountType.COMPLETE].SetActive(true);
            GameManager.Instance.Account.Register();
        }
        else
        {
            if (repeatPasswordBtn.gameObject.activeSelf)
                repeatPasswordBtn.gameObject.SetActive(false);
            inputAccountFields[(int)CreateAccountType.REPEAT_PASSWORD].text = "";
            warnDifferentText.gameObject.SetActive(true);
            warnDifferentAnim.SetTrigger("Shake");
        }
    }
    #endregion

    #region COMPLETE
    [SerializeField] Button completeBtn;
    [SerializeField] TextMeshProUGUI completeText;

    public void ClickComplete()
    {
        ClearAccount();
    }

    public void ShowCreateAccount(bool isCreateAccount)
    {
        if (isCreateAccount)
            completeText.text = "회원가입 성공!";
        else
            completeText.text = "회원가입 실패!";
    }
    #endregion


    /// <summary>
    /// Escape 버튼을 눌렀을 때 호출 : 회원가입중이였다면 true, 아니라면 false 호출
    /// </summary>
    /// <returns></returns>
    public bool ClearAccount()
    {
        // 회원가입중이라면?
        if(createAccountObjects[(int)CreateAccountType.FRAME].activeSelf)
        {
            int _cnt = (int)CreateAccountType.MAX;
            for (int i=0; i< _cnt; i++)
            {
                createAccountObjects[i].SetActive(false);
            }

            int _fieldCnt = inputAccountFields.Length;
            for(int k=0; k<_fieldCnt; k++)
            {
                inputAccountFields[k].text = "";
            }

            createPlayerID = "";
            createPlayerPassword = "";

            // ID 경고문자 비활성화
            if (warnOverlapIDText.gameObject.activeSelf)
                warnOverlapIDText.gameObject.SetActive(false);

            // 패스워드 경고문자 비활성화
            if (passwordWarnText.gameObject.activeSelf)
                passwordWarnText.gameObject.SetActive(false);

            // 재입력 경고문자 비활성화
            if (warnDifferentAnim.gameObject.activeSelf)
                warnDifferentAnim.gameObject.SetActive(false);

            return true;
        }
        // 회원가입을 하지 않고 있었다면?
        return false;
    }
}
