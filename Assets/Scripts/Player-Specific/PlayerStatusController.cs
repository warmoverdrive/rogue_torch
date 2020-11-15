﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Cinemachine;

public class PlayerStatusController : MonoBehaviour, IDamagable
{
	[Header("Design Levers")]
	[SerializeField]
	int hitPoints = 1;
	[SerializeField]
	int hitPointsPerTorch = 1;
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
	int torchesCollected;

	TorchCounter torchCounter;
	Light2D playerTorch;
	TorchFX torchFX;
	GameController gameController;
	PlayerAction action;
	ShadowCaster2D shadowCaster;

	private void Start()
	{
		torchesCollected = hitPoints;
		playerTorch = GetComponentInChildren<Light2D>();
		shadowCaster = GetComponent<ShadowCaster2D>();
		torchFX = GetComponentInChildren<TorchFX>();
		action = GetComponent<PlayerAction>();
		torchCounter = FindObjectOfType<TorchCounter>();
		gameController = FindObjectOfType<GameController>();
		torchCounter.SetText(hitPoints, torchesCollected);
	}

	public void Hit(int damage)
	{
		if (action.isBlocking) return;
		else
		{
			if (hitPoints - damage <= 0)
			{
				torchCounter.SetText(0, torchesCollected);
				isDead = true;
				PlayerDeath();
			}
			else
			{
				hitPoints -= damage;
				IncrementTorchNegative();
				torchCounter.SetText(hitPoints, torchesCollected);
			}
		}
	}

	void PlayerDeath()
	{
		var torch = Instantiate(torchDropPrefab, transform.position, Quaternion.identity);
		gameController.GenerateTorch(torch.GetComponent<TorchMarker>());

		gameController.TriggerReset();

		Destroy(GetComponentInChildren<SpriteRenderer>());
		Destroy(shadowCaster);
	}

	public void GetFlame()
	{
		if (isDead) return;
		hitPoints += hitPointsPerTorch;
		torchesCollected += hitPointsPerTorch;
		IncrementTorchPositive();
		torchCounter.SetText(hitPoints, torchesCollected);
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

	public bool IsDead() { return isDead; }
}
