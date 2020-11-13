using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TorchCounter : MonoBehaviour
{
	Text text;
	public int torchesCollected { get; private set; }

	private void Start()
	{
		text = GetComponent<Text>();
	}

	public void SetText(int health, int torches) 
	{ 
		text.text = "Torches: " + health + "/" + torches;
		torchesCollected = torches;
	}
}
