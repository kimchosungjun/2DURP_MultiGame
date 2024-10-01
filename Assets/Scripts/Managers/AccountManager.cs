using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AccountManager : MonoBehaviour
{
    const string AccountURL = "https://script.google.com/macros/s/AKfycbxCZ06V110BfGdXhyvZYBdKkAPFGUsS3PccPFfDcM-nWJ0ufnctpy4_Rvks_TEADQUv/exec";

    IEnumerator Start()
    {
        UnityWebRequest www = UnityWebRequest.Get(AccountURL);

        yield return www.SendWebRequest();

        string _data = www.downloadHandler.text;

        print(_data);
    }

    public bool IsOverlapID()
    {
        return false;
    }

    public void RegistAccount(string id, string password)
    {
        WWWForm _form = new WWWForm();
        _form.AddField("order", "register");
        _form.AddField("id", id);
        _form.AddField("password", password);
        StartCoroutine(DispatchPost(_form));
    }

    IEnumerator DispatchPost(WWWForm form)
    {
        using(UnityWebRequest _www = UnityWebRequest.Post(AccountURL, form))
        {
            yield return _www.SendWebRequest();
            if (_www.isDone)
            {
                Debug.Log("완료");
            }
            else
            {
                Debug.Log("응답이 없습니다.");
            }
        }
    }
}
