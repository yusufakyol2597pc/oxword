using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class HttpHandler : MonoBehaviour
{
    public string m_strResult = "";

    public string GetWords()
    {
        StartCoroutine(RequestWords());
        return m_strResult;
    }

    public IEnumerator RequestWords()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://blooming-oasis-79912.herokuapp.com/get-words");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            m_strResult = www.downloadHandler.text;


            //dynamic word = stuff;
            //string type = stuff.type;
            //Debug.Log(type);
        }
    }
}
