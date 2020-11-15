using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
	[SerializeField]
	float volume = 0.33f,
		fadeInTime = 1f;
	float fadeTimeElapsed = 0f;
	MusicPlayer musicPlayer;
	AudioSource audioSource;

	private void Awake()
	{
		if ( musicPlayer == null)
		{
			DontDestroyOnLoad(gameObject);
			musicPlayer = this;
		}
		else if (musicPlayer != this)
		{
			Destroy(gameObject);
		}

		audioSource = GetComponent<AudioSource>();

		StartCoroutine(FadeInAudio());
	}

	private IEnumerator FadeInAudio()
	{
		while (fadeTimeElapsed < fadeInTime)
		{
			audioSource.volume = Mathf.Lerp(0, volume, fadeTimeElapsed / fadeInTime);

			fadeTimeElapsed += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}
	}
}
