using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using UnityEngine.Events;

[System.Serializable]
public class PostData
{
	public string result;
	public string playerName;
	public string scoreValue;
}


public class AccountManager : MonoBehaviour
{
	public LoginUIController LobbyController { get; set; } = null;
	const string URL = "https://script.google.com/macros/s/AKfycbyRc0viRQNOJNNYETkou-uKFjMDCE3JJ6NMbiFlb_bR-DQ0mdqvhpo_QHMVWB8f_IVy/exec";

	[SerializeField] LoginSCO loginInformation = null;
	[SerializeField] PostData postData = null;

	string id = "";
	string password = "";
	int scoreValue= -1000;

    #region Create Account
    public void CheckIDCondition(string _id, UnityAction<bool> _action)
    {
		// ID에 특수문자가 있는지 체크
		if (Regex.IsMatch(_id, @"[^a-zA-Z0-9]"))
        {
			LobbyController.Account.SetIDText("특수문자를 사용할 수 없습니다.");
			return;
        }

		int _length = _id.Length;
		// ID의 길이가 1자이상 8자 이하인지 체크
		if (0 >= _length || _length > 8)
        {
			LobbyController.Account.SetIDText("ID의 길이는 1자 이상 8자 이하여야 합니다.");
			return;
        }

		WWWForm form = new WWWForm();
		form.AddField("order", "OVERLAP");
		form.AddField("id", _id);
		StartCoroutine(Post(form,_action,_id));
	}

	public bool CheckPasswordCondition(string _password)
    {
		// PASSWORD에 특수문자 있는지 체크
		if (Regex.IsMatch(_password, @"[^a-zA-Z0-9]"))
        {
			LobbyController.Account.SetPassowrdText("특수문자를 사용할 수 없습니다.");
			return false;
        }

		// PASSWORD의 길이가 4자 이상 12자 이하인지 체크
		int _length = _password.Length;
		if (4 > _length || _length > 12)
        {
			LobbyController.Account.SetPassowrdText("비밀번호는 4자 이상 12자 이하여야 합니다.");
			return false;
        }

		this.password = _password;
		return true;
    }

	public void Register()
	{
		// 이미 조건체크가 끝난 상태 : 등록만 하면 된다.
		WWWForm form = new WWWForm();
		form.AddField("order", "REGISTER");
		form.AddField("id", id);
		form.AddField("password", password);
		form.AddField("scoreValue", 1000);
		StartCoroutine(Post(form, LobbyController.Account.ShowCreateAccount));
	}
    #endregion

	public void RenewCache()
    {
		LoadLoginInformation _login = LoadLoginRecord();
		WWWForm form = new WWWForm();
		form.AddField("order", "RENEW");
		form.AddField("id", _login.id);
		form.AddField("password", _login.password);
		StartCoroutine(Post(form, RenewInformation));
	}

	public void  RenewInformation(bool isRenew)
    {
        if (isRenew)
        {
			id = postData.playerName;
			scoreValue =  int.Parse(postData.scoreValue);
			Debug.Log("12331232131232");
        }
        else
        {
			Debug.LogError("치명적인 로그인 문제 발생!!");
			Application.Quit();
        }
    }

    public void Login(string _id, string _password)
	{
		WWWForm form = new WWWForm();
		form.AddField("order", "LOGIN");
		form.AddField("id", _id);
		form.AddField("password", _password);
		StartCoroutine(Post(form, LobbyController.Login.CheckLogin));
	}

	
	//void OnApplicationQuit()
	//{
	//	WWWForm form = new WWWForm();
	//	form.AddField("order", "LOGOUT");
	//	StartCoroutine(Post(form));
	//}


	public void SetScoreValue(string _scoreValue)
	{
		WWWForm form = new WWWForm();
		form.AddField("order", "SETSCOREVALUE");
		form.AddField("scoreValue", _scoreValue);
		StartCoroutine(Post(form, RememberScoreValue));
	}


	public void GetScoreValue()
	{
		WWWForm form = new WWWForm();
		form.AddField("order", "GETSCOREVALUE");
		StartCoroutine(Post(form, RememberScoreValue));
	}

	public void RememberScoreValue(bool isRememberScore)
    {
		if (isRememberScore)
			scoreValue = int.Parse(postData.scoreValue);
	}

	IEnumerator Post(WWWForm form, UnityAction<bool> action, string _id="")
    {
		using (UnityWebRequest www = UnityWebRequest.Post(URL, form)) // 반드시 using을 써야한다
		{
			yield return www.SendWebRequest();

			if (www.isDone) Response(www.downloadHandler.text, action, _id);
			else Debug.LogError("응답이 없습니다!");
		}
	}

	void Response(string _jsonText, UnityAction<bool> action, string _id="")
	{
		if (string.IsNullOrEmpty(_jsonText)) return;

		postData = JsonUtility.FromJson<PostData>(_jsonText);
		switch (postData.result)
		{
			case "ERROR":
				action.Invoke(false);
				break;
			case "RENEW":
				action.Invoke(true);
				break;
			case "CANUSE":
				this.id = _id;
				action.Invoke(true);
				break;
			case "LSUCCESS":
				// 로그인 성공
				action.Invoke(true);
				break;
			case "RSUCCESS":
				// 회원가입 성공
				action.Invoke(true);
				break;
			case "SET":
				// 새로 갱신된 점수로 세팅
				action.Invoke(true);
				break;
			case "GET":
				// 새로 갱신된 점수로 세팅
				action.Invoke(true);
				break;
		}
	}

	// 로그인 정보를 암호화하여 저장 (SCO 방식으로)
	public void SaveLoginInformation()
    {
		byte[] _bytesID = System.Text.Encoding.UTF8.GetBytes(id);
		string _encryptID = System.Convert.ToBase64String(_bytesID);
		
		byte[] _bytesPassowrd = System.Text.Encoding.UTF8.GetBytes(password);
		string _encryptPassword = System.Convert.ToBase64String(_bytesPassowrd);

		loginInformation.isLogin = true;
	
		loginInformation.id = _encryptID;
		loginInformation.password = _encryptPassword;

		password = "";

		byte[] _bytesID2 = System.Convert.FromBase64String(loginInformation.id);
		string _decryptID2 = System.Text.Encoding.UTF8.GetString(_bytesID2);

		byte[] _bytesPassowrd2 = System.Convert.FromBase64String(loginInformation.password);
		string _decryptPassword2 = System.Text.Encoding.UTF8.GetString(_bytesPassowrd2);
		Debug.Log(_encryptID);
		Debug.Log(_encryptPassword);
	}

	// 복호화된 로그인 정보 반환
	public LoadLoginInformation LoadLoginRecord()
    {
		byte[] _bytesID = System.Convert.FromBase64String(loginInformation.id);
		string _decryptID = System.Text.Encoding.UTF8.GetString(_bytesID);

		byte[] _bytesPassowrd = System.Convert.FromBase64String(loginInformation.password);
		string _decryptPassword = System.Text.Encoding.UTF8.GetString(_bytesPassowrd);

		LoadLoginInformation result = new LoadLoginInformation(true, _decryptID, _decryptPassword);
		return result;
	}

	// 로그아웃 시 호출
	public void ClearLoginInformation()
    {
		loginInformation.isLogin = false;
		loginInformation.id = "";
		loginInformation.password = "";
	}

	// 로그아웃을 한 적 있는지?
	public bool IsLoginState()
    {
		if (loginInformation.isLogin)
			return true;
		return false;
    }
}