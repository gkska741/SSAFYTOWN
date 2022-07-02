using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using WebSocketSharp;
public class LeaderBoardManager : MonoBehaviour
{
    public Text first;
    public Text Second;
    public Text Third;

    public Text firstScore;
    public Text SecondScore;
    public Text ThirdScore;
    private void Start()
    {
        StartCoroutine(GetRanking());
    }
    IEnumerator GetRanking()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://k6c204.p.ssafy.io:3001/ranking/game1");
        yield return www.SendWebRequest();

        Debug.Log(www);
        Debug.Log(www.downloadHandler.text);

        string Data = www.downloadHandler.text;
        char sp = char.Parse(",");
        string[] subString = Data.Split(sp);

        string Text1 = "1st : ";
        Text1 = Text1 + subString[2].Split(char.Parse(":"))[1].Trim(char.Parse("\""));
        first.text = Text1;
        firstScore.text = "Score : " + subString[1].Split(char.Parse(":"))[1];        


        string Text2 = "2st : ";
        Text2 += subString[6].Split(char.Parse(":"))[1].Trim(char.Parse("\""));
        Second.text = Text2;
        SecondScore.text = "Score : " + subString[5].Split(char.Parse(":"))[1];

        string Text3 = "3st : ";
        Text3 += subString[10].Split(char.Parse(":"))[1].Trim(char.Parse("\""));
        Third.text = Text3;
        ThirdScore.text = "Score : " + subString[9].Split(char.Parse(":"))[1];
    }
    
}
