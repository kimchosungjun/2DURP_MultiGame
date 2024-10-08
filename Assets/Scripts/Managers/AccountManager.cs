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
	public TitleUI TitleController { get; set; } = null;
	public LoginUIController LobbyController { get; set; } = null;
	const string URL = "https://script.google.com/macros/s/AKfycbwfygVL-scHtrexKmmqpXaDfqqEASMZBbou0BwCOm-e4zJ78BtXlacmuqluL2Gy9Mqa/exec";

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

	#region RENEW : 타이틀 창에서만 호출 (기존 로그인 기록이 있는지 확인)
	public void RenewCache()
    {
		WWWForm form = new WWWForm();
		form.AddField("order", "RENEW");
		StartCoroutine(Post(form, RenewInformation));
	}

	public void  RenewInformation(bool isRenew)
    {
        if (isRenew)
        {
			// 플레이어의 정보로 사용될 정보를 받아옴
			id = postData.playerName;
			scoreValue =  int.Parse(postData.scoreValue);
			TitleController.CanSkipLogin(true);
        }
        else
        {
			// 로그인 캐쉬가 만료되거나 로그인 기록이 없다면 로그인 씬으로 이동
			id = "";
			password = "";
			scoreValue = -1000;
			TitleController.CanSkipLogin(false);
        }
    }
	#endregion

	#region LOGIN / LOGOUT : 로그인, 로그아웃 기능
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
	#endregion

	#region SET / GET : 스코어의 값을 불러오거나 값을 조정하는 기능
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
    #endregion

    #region POST
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
		Debug.Log(postData.result);
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
    #endregion
}