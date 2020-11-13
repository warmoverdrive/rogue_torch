using UnityEngine;
using System.Collections;

public class SeedManager : MonoBehaviour
{
	public static SeedManager seedManager;

	public int seed { private set; get; }

	private void Awake()
	{
		if (seedManager == null)
		{
			DontDestroyOnLoad(gameObject);
			seedManager = this;
		}
		else if (seedManager != this)
		{
			Destroy(gameObject);
		}		
	}

	public void SetSeed(int inputSeed)
	{
		if (inputSeed == 0)
		{
			inputSeed = GenerateSeed();
			Random.InitState(inputSeed);
		}
		else Random.InitState(inputSeed);

		seed = inputSeed;
	}

	private int GenerateSeed()
	{
		return Random.Range(-2147483647, 2147483647); // full range of int values
	}
}
