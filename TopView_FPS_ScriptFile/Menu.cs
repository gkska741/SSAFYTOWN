using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

	public GameObject mainMenuHolder;
	public GameObject leaderBoardMenuHolder;
	GameObject Player;

	void Start()
	{
		if (GameObject.Find("DataManager") != null)
		{
			GameObject DM = GameObject.Find("DataManager");
			Debug.Log(DM);
		}
		else
        {
			Debug.Log("NOT FOUND");
        }
		Screen.SetResolution(1920, 1080, true);
	}

	public void Play()
	{
		SceneManager.LoadScene("FPSGame");
	}

	public void Quit()
	{	
		GameObject DM = GameObject.Find("DataManager");
		DM.GetComponent<DataManager>().NetworkManger.SetActive(true);

		GameObject PM = GameObject.Find("PortalManager");
		PM.GetComponent<PortalManager>().CanvasOn = true;
		SceneManager.LoadScene("Gwangju Campus", LoadSceneMode.Single);
	}

	public void LeaderBoardMenu()
	{
		mainMenuHolder.SetActive(false);
		leaderBoardMenuHolder.SetActive(true);
	}

	public void MainMenu()
	{
		mainMenuHolder.SetActive(true);
		leaderBoardMenuHolder.SetActive(false);
	}
}