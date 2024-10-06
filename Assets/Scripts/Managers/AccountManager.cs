using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

[System.Serializable]
public class PostData
{
	public string result;
	public string playerName;
	public string scoreValue;
}


public class AccountManager : MonoBehaviour
{
	const string URL = "https://script.google.com/macros/s/AKfycbzUcI8QTBlpTPRynAguZGzYNSsiGgMKjygE1B4O8XWgwdblEe_7sjc_m0GvQsiJKBfE/exec";

	public PostData postData = null;
	string id = "";
	string password = "";

    #region Create Account
    public bool CheckIDCondition(string _id)
    {
		// ID가 중복인지 체크

		// ID에 특수문자가 있는지 체크
		if (Regex.IsMatch(_id, @"[^a-zA-Z0-9]"))
			return false;

		// ID의 길이가 1자이상 8자 이하인지 체크
		int _length = _id.Length;
		if (0 >= _length || _length >8)
			return false;

		this.id = _id;
		return true;
	}

	public bool CheckPasswordCondition(string _password)
    {
		// PASSWORD에 특수문자 있는지 체크
		if (Regex.IsMatch(_password, @"[^a-zA-Z0-9]"))
			return false;

		// PASSWORD의 길이가 4자 이상 12자 이하인지 체크
		int _length = _password.Length;
		if (4 > _length || _length > 12)
			return false;

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
		StartCoroutine(Post(form));
	}
    #endregion

    public void Login(string _id, string _password)
	{
		WWWForm form = new WWWForm();
		form.AddField("order", "LOGIN");
		form.AddField("id", id);
		form.AddField("password", password);
		StartCoroutine(Post(form));
	}

	void OnApplicationQuit()
	{
		WWWForm form = new WWWForm();
		form.AddField("order", "LOGOUT");
		StartCoroutine(Post(form));
	}


	public void SetScoreValue(string _scoreValue)
	{
		WWWForm form = new WWWForm();
		form.AddField("order", "SETSCOREVALUE");
		form.AddField("scoreValue", _scoreValue);
		StartCoroutine(Post(form));
	}


	public void GetScoreValue()
	{
		WWWForm form = new WWWForm();
		form.AddField("order", "GETSCOREVALUE");
		StartCoroutine(Post(form));
	}

	IEnumerator Post(WWWForm form)
	{
		using (UnityWebRequest www = UnityWebRequest.Post(URL, form)) // 반드시 using을 써야한다
		{
			yield return www.SendWebRequest();

			if (www.isDone) Response(www.downloadHandler.text);
			else print("웹의 응답이 없습니다.");
		}
	}


	void Response(string _jsonText)
	{
		if (string.IsNullOrEmpty(_jsonText)) return;

		postData = JsonUtility.FromJson<PostData>(_jsonText);

		switch (postData.result)
		{
			case "ERROR":
				Debug.LogError("해당 기능을 수행하지 못했습니다.");
				break;
			case "SUCCESS":
				break;
			case "SET":
				// 새로 갱신된 점수로 세팅
				break;
			case "GET":
				// 새로 갱신된 점수로 세팅
				break;
		}
	}
}