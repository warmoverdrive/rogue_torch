using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
			FindObjectOfType<TorchScoreDisplay>().SetTorchScore(FindObjectOfType<TorchCounter>().torchesCollected);

			FindObjectOfType<ExitMenuLogic>().TriggerUIFade();

			FindObjectOfType<PlayerMovementController>().hasExited = true;
	}
}
