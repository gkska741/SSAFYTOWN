using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{

    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text scoreUI;

    Spawner spawner;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
    }

    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    private void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        if (ScoreKeeper.score > 100000)
        {
            scoreUI.color = Color.blue;
        }
    }

    void OnNewWave(int waveNumber)
    {
        string[] numbers = { "First", "Second", "Third", "Fourth", "Final" };
        newWaveTitle.text = "- " + numbers[waveNumber-1] + " Wave -";
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 2f;
        float speed = 2f;
        float animatePercent = 0;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;
        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;

            if (animatePercent >= 1) 
            {
                animatePercent = 1;
                if (Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-170, 45, animatePercent);
            yield return null;
        }
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }
    public void StartNewGame()
    {
        Debug.Log("StartNewGame");
        SceneManager.LoadScene("FPSGame");
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("FPSMenu");
    }
}
