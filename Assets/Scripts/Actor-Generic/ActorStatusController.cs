using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ActorStatusController : MonoBehaviour, IDamagable
{
	[Header("Design Levers")]
	[SerializeField]
	int hitPoints = 1;
	[SerializeField]
	AudioClip hitSound, deathSound;
	[SerializeField]
	AudioClip[] blockSounds;

	bool isDead = false;
	public bool isBlocking = false;

	AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void Hit(int damage) 
	{
		if (isBlocking) audioSource.PlayOneShot(blockSounds[Random.Range(0, blockSounds.Length)]);
		else
		{
			hitPoints -= damage;
			if (hitPoints <= 0)
			{
				isDead = true;
				audioSource.PlayOneShot(deathSound);
			}
			else audioSource.PlayOneShot(hitSound);
		}
	}

	public bool IsDead() { return isDead; }
}
