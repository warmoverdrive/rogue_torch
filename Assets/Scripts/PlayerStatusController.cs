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
	ShadowCaster2D shawdowCaster;

	Vector3 spawnPos;

	List<TorchMarker> torches = new List<TorchMarker>();

	private void Start()
	{
		spawnPos = transform.position;
		playerTorch = GetComponentInChildren<Light2D>();
		torchFX = GetComponentInChildren<TorchFX>();
		shawdowCaster = GetComponent<ShadowCaster2D>();
		torchCounter = FindObjectOfType<TorchCounter>();

		torchCounter.SetText(hitPoints);
	}

	public void PlayerHit(int damage)
	{
		if (hitPoints - damage <= 0)
		{
			torchCounter.SetText(0);
			StartCoroutine(PlayerDeath());
		}
		else
		{
			hitPoints -= damage;
			IncrementTorchNegative();
			torchCounter.SetText(hitPoints);
		}
	}

	private IEnumerator PlayerDeath()
	{
		HidePlayer();
		yield return new WaitForSeconds(timeBeforeRespawn);
		ResetPlayer();
		ResetTorches();
		torchCounter.SetText(hitPoints);
	}

	private void HidePlayer()
	{
		isDead = true;
		playerSprite.SetActive(false);
		shawdowCaster.enabled = false;
		var newTorch = Instantiate(torchDropPrefab, transform.position, Quaternion.identity);
		torches.Add(newTorch.GetComponent<TorchMarker>());
	}

	private void ResetPlayer()
	{
		transform.position = spawnPos;
		playerSprite.SetActive(true);
		shawdowCaster.enabled = true;
		ResetPlayerTorch();
		isDead = false;
	}

	private void ResetTorches()
	{
		foreach (var torch in torches)
		{
			torch.ResetFlame();
		}
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

	private void ResetPlayerTorch()
	{
		playerTorch.pointLightOuterRadius = torchOuterRadiusStart;
		playerTorch.pointLightInnerRadius = torchInnerRadiusStart;
		torchFX.maxRandomLightIntensity = FXStartIntensity;
	}

	public bool IsPlayerDead() { return isDead; }
}
