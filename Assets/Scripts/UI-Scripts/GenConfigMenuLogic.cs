using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenConfigMenuLogic : MonoBehaviour
{
    [SerializeField]
    Slider widthSlider,
        heightSlider,
        blankSlider;
    [SerializeField]
    GameObject configMenu, mainMenu;

    LevelGenManager genManager;

    // Start is called before the first frame update
    void Start()
    {
        genManager = FindObjectOfType<LevelGenManager>();
    }

    public void Apply()
	{
        genManager.castleWidthInRooms = (int)widthSlider.value;
        genManager.castleHeightInRooms = (int)heightSlider.value;
        genManager.blankRooms = (int)blankSlider.value;
	}

    public void RestoreDefaultValues()
	{
        widthSlider.value = genManager.GetDefaultCastleWidth();
        heightSlider.value = genManager.GetDefaultCastleHeight();
        blankSlider.value = genManager.GetDefaultBlankRooms();

        Apply();
	}

    public void ToggleMenus()
	{
        configMenu.SetActive(!configMenu.activeSelf);
        mainMenu.SetActive(!mainMenu.activeSelf);
	}
}
