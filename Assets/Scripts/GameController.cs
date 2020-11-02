using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    int timeBeforeRespawn = 2;

    List<TorchMarker> torches = new List<TorchMarker>();
    public SpawnPosition[] spawnPositions;
    // TODO add array of enemies

    void Start()
    {
        spawnPositions = FindObjectsOfType<SpawnPosition>();
        // TODO populate array of enemies as a function, first clearing the array
    }

    public void GenerateTorch(TorchMarker torchObject)
	{
        torches.Add(torchObject);
    }

    public IEnumerator ResetLevel()
	{
        yield return new WaitForSeconds(timeBeforeRespawn);

        foreach (SpawnPosition spawnPos in spawnPositions)
		{
            spawnPos.SpawnReset();
		}
        foreach (TorchMarker torch in torches)
		{
            torch.ResetFlame();
		}
	}
}
