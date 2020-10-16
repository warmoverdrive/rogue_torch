using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TorchMarker : MonoBehaviour
{
    [Header("Design Levers")]
    [SerializeField]
    float lightOuterRadiusMax = 6;
    [SerializeField]
    float lightOuterRadiusMin = 1,
        lightInnerRadiusMax = 1,
        lightInnerRadiusMin = 0.1f;

    bool hasFlame = true;

    Animator animator;
    Light2D thisTorch;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        thisTorch = GetComponentInChildren<Light2D>();

        animator.SetTrigger("isDead");
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        PlayerStatusController playerStatus = collision.gameObject.GetComponent<PlayerStatusController>();

        if (playerStatus && hasFlame)
		{
            GivePlayerFlame(playerStatus);
		}
	}

    private void GivePlayerFlame(PlayerStatusController player)
	{
        animator.SetBool("gotFlame", true);
        thisTorch.pointLightOuterRadius = lightOuterRadiusMin;
        thisTorch.pointLightInnerRadius = lightInnerRadiusMin;
        player.GetFlame();
        hasFlame = false;
    }

    public void ResetFlame()
	{
        hasFlame = true;
        animator.SetBool("gotFlame", false);
        animator.SetTrigger("isDead");
        thisTorch.pointLightOuterRadius = lightOuterRadiusMax;
        thisTorch.pointLightInnerRadius = lightInnerRadiusMax;
    }
}
