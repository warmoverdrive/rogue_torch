using UnityEngine;
using System.Collections;

public class LevelGenManager : MonoBehaviour
{
	public static LevelGenManager levelGenManager;

	[SerializeField]
	int defaultBlankRooms = 5, 
		defaultCastleWidthInRooms = 4,
		defaultCastleHeightInRooms = 6;

	[Header("Configured Settings")]
	public int blankRooms = 5;
	public int castleWidthInRooms = 4;
	public int castleHeightInRooms = 6;

	public int seed { private set; get; }

	public int GetDefaultBlankRooms() => defaultBlankRooms;
	public int GetDefaultCastleWidth() => defaultCastleWidthInRooms;
	public int GetDefaultCastleHeight() => defaultCastleHeightInRooms;

	private void Awake()
	{
		if (levelGenManager == null)
		{
			DontDestroyOnLoad(gameObject);
			levelGenManager = this;
		}
		else if (levelGenManager != this)
		{
			Destroy(gameObject);
			return;
		}
		
		SetDefaults();
	}

	public void SetDefaults()
	{
		blankRooms = defaultBlankRooms;
		castleWidthInRooms = defaultCastleWidthInRooms;
		castleHeightInRooms = defaultCastleHeightInRooms;
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
