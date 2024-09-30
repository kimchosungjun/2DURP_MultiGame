using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AccountManager : MonoBehaviour
{
    const string AccountURL = "https://docs.google.com/spreadsheets/d/1cAeqkRDgntwtmbEiXPO1pTApx3EPTMZa9mZyRRLA5cw/export?formats=tsv";

    IEnumerator Start()
    {
        UnityWebRequest www = UnityWebRequest.Get(AccountURL);

        yield return www.SendWebRequest();

        string _data = www.downloadHandler.text;

        print(_data);
    }
}
