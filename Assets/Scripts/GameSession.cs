using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
	[SerializeField] private int  score = 0;
	[SerializeField] private TextMeshProUGUI scoreText;
	private void Awake()
	{
		var gameSessionCount = FindObjectsOfType<GameSession>().Length;
		if (gameSessionCount > 1)
		{
			Destroy(gameObject);
		}
		else
		{
			DontDestroyOnLoad(gameObject);
		}
		
	}

	private void Start()
	{
		scoreText.text = score.ToString();
	}

	public void ProcessPlayerDeath()
	{
		ResetGameSession();
	}
	

	private void ResetGameSession()
	{
		SceneManager.LoadScene(2);
		//Destroy(gameObject);
	}

	public void AddToScore(int scorePoint)
	{
		score += scorePoint;
		scoreText.text = score.ToString();
	}
}
