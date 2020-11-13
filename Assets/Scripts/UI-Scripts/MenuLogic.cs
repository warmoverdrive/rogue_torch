using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    [SerializeField]
    InputField seedField;
	SeedManager seedManager;

	private void Start()
	{
		seedManager = FindObjectOfType<SeedManager>();
	}

	public void StartGame()
	{
		ParseAndSetSeed();
		SceneController.LoadGame();
	}

	public void PlayAgain()
	{
		seedManager.SetSeed(seedManager.seed);
		SceneController.LoadGame();
	}

	public void QuitGame() => Application.Quit();

	public void ReturnToMainMenu() => SceneController.LoadMainMenu();

	private void ParseAndSetSeed()
	{
		string seedStr = seedField.text;

		if (int.TryParse(seedStr, out int seed))
		{
			seedManager.SetSeed(seed);
		}
		else seedManager.SetSeed(0);
	}
}
