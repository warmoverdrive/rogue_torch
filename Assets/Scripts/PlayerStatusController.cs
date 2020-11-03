using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Cinemachine;

public class PlayerStatusController : MonoBehaviour
{
	[Header("Design Levers")]
	[SerializeField]
	int hitPoints = 1;
	[SerializeField]
	int hitPointsPerTorch = 1;
	[SerializeField]
	int timeBeforeRespawn = 3;
	[SerializeField]
	float torchOuterRadiusStart = 8,
		torchInnerRadiusStart = 1,
		torchOuterRadiusMax = 16,
		torchOuterSizeIncrement = 1f,
		torchInnerSizeIncriment = 0.25f,
		FXStartIntensity = 1.5f,
		FXIntensityIncrement = 0.25f;

	[Header("Prefabs and References")]
	[SerializeField]
	GameObject playerSprite;
	[SerializeField]
	GameObject torchDropPrefab;

	bool isDead = false;
	int extraTorchHits = 0;

	TorchCounter torchCounter;
	Light2D playerTorch;
	TorchFX torchFX;
	GameController gameController;

	private void Start()
	{
		playerTorch = GetComponentInChildren<Light2D>();
		torchFX = GetComponentInChildren<TorchFX>();
		torchCounter = FindObjectOfType<TorchCounter>();
		gameController = FindObjectOfType<GameController>();
		torchCounter.SetText(hitPoints);
	}

	public void PlayerHit(int damage)
	{
		if (hitPoints - damage <= 0)
		{
			torchCounter.SetText(0);
			PlayerDeath();
		}
		else
		{
			hitPoints -= damage;
			IncrementTorchNegative();
			torchCounter.SetText(hitPoints);
		}
	}

	void PlayerDeath()
	{
		StartCoroutine(gameController.ResetLevel());

		var torch = Instantiate(torchDropPrefab, transform.position, Quaternion.identity);
		gameController.GenerateTorch(torch.GetComponent<TorchMarker>());

		Destroy(gameObject);
	}

	public void GetFlame()
	{
		hitPoints += hitPointsPerTorch;
		IncrementTorchPositive();
		torchCounter.SetText(hitPoints);
	}

	private void IncrementTorchPositive()
	{
		if (playerTorch.pointLightOuterRadius < torchOuterRadiusMax)
		{
			playerTorch.pointLightOuterRadius += torchOuterSizeIncrement;
			playerTorch.pointLightInnerRadius += torchInnerSizeIncriment;
			torchFX.maxRandomLightIntensity += FXIntensityIncrement;
		}
		else extraTorchHits++;
	}

	private void IncrementTorchNegative()
	{
		if (extraTorchHits == 0)
		{
			playerTorch.pointLightOuterRadius -= torchOuterSizeIncrement;
			playerTorch.pointLightInnerRadius -= torchInnerSizeIncriment;
			torchFX.maxRandomLightIntensity -= FXIntensityIncrement;
		}
		else extraTorchHits--;
	}

	public bool IsPlayerDead() { return isDead; }
}
