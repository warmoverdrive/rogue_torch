using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ExitCameraController : MonoBehaviour
{
    CinemachineVirtualCamera exitCamera;

    // Start is called before the first frame update
    void Start()
    {
        exitCamera = GetComponent<CinemachineVirtualCamera>();
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.GetComponent<PlayerStatusController>())
		{
			exitCamera.MoveToTopOfPrioritySubqueue();
		}
	}
}
