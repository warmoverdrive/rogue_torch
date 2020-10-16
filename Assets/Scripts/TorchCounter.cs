using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TorchCounter : MonoBehaviour
{
	Text text;

	private void Start()
	{
		text = GetComponent<Text>();
	}

	public void SetText(int torches) { text.text = "Torches: " + torches; }
}
