using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenConfigSlider : MonoBehaviour
{
    [SerializeField]
    Text valueText;
    [SerializeField]
    Parameter parameter;
    [SerializeField]
    Slider widthSlider,
        heightSlider;

    public enum Parameter
    {
        width,
        height,
        blankRooms
	}

    Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();

		switch (parameter)
		{
			case Parameter.width:
				slider.value = FindObjectOfType<LevelGenManager>().castleWidthInRooms;
				UpdateValueText();
				break;

			case Parameter.height:
                slider.value = FindObjectOfType<LevelGenManager>().castleHeightInRooms;
                UpdateValueText();
                break;

            case Parameter.blankRooms:
                slider.value = FindObjectOfType<LevelGenManager>().blankRooms;
                UpdateValueText();
                break;
        }
    }

	private void Update()
	{
		if (parameter == Parameter.blankRooms)
		{
            slider.maxValue = (widthSlider.value * heightSlider.value) - 2;
		}
	}

	public void UpdateValueText()
	{
		valueText.text = slider.value.ToString();
	}
}
