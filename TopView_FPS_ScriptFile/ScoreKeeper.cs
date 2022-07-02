using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; private set; }
    float lastEnemyKillTime;
    int streakCount;
    float streakExpiryTime = 1;

    public string RankingUrl;
    private void Start()
    {
        score = 0;
        Enemy.OndeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;

        RankingUrl = "http://k6c204.p.ssafy.io:3001/score/game1";
    }


    void OnEnemyKilled()
    {
        if (Time.time < lastEnemyKillTime + streakExpiryTime)
        {
            streakCount++;
        }
        else
        {
            streakCount = 0;
        }

        lastEnemyKillTime = Time.time;
        int bonusScore = (int)Mathf.Pow(2, streakCount);
        if (bonusScore > 1000)
        {
            bonusScore = 1000 + streakCount * 5;
        }
        score += (int)(5 + bonusScore);
    }

    void OnPlayerDeath()
    {
        Enemy.OndeathStatic -= OnEnemyKilled;
        StartCoroutine(RankingUpdate());
    }
    IEnumerator RankingUpdate()
    {
        WWWForm form = new WWWForm();

        form.AddField("userId", GameObject.Find("DataManager").GetComponent<DataManager>().UserId);
        form.AddField("score", score);

        UnityWebRequest www = UnityWebRequest.Post(RankingUrl, form);

        yield return www.SendWebRequest();
        Debug.Log(www.result);
        if (www.responseCode == 201)
        {
            Debug.Log("랭킹 업데이트 성공");
        }
        if (www.responseCode == 400)
        {
            Debug.Log(www.error);
        }

        www.Dispose();
    }
}
