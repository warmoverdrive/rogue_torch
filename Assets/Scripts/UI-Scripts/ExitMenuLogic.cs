using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitMenuLogic : MonoBehaviour
{
    [SerializeField]
    float fadeInDuration = 1f;

    Image backgroundPanel;
    Text[] menuTexts;
    Button[] menuButtons;

    // Start is called before the first frame update
    void Start()
    {
        backgroundPanel = GetComponent<Image>();
        menuTexts = GetComponentsInChildren<Text>();
        menuButtons = GetComponentsInChildren<Button>();

        ToggleAllButtons(false);
        FadeAllElements(0.0f, 0.001f);
    }

    public void TriggerUIFade()
    {
        ToggleAllButtons(true);
        FadeAllElements(1.0f, fadeInDuration);
    }

    private void FadeAllElements(float targetAlpha, float duration)
	{
        backgroundPanel.CrossFadeAlpha(targetAlpha, duration, true);
        foreach (Text menuText in menuTexts) menuText.CrossFadeAlpha(targetAlpha, duration, true);
    }

    private void ToggleAllButtons(bool isInteractable)
	{
        foreach (Button button in menuButtons) button.interactable = isInteractable;
	}
}
