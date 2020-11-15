using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MeleeAttack : MonoBehaviour, IAttack
{
	[Header("Design Levers")]
	[SerializeField]
	float attackRange = 1.5f;
	[SerializeField]
	int attackDamage = 1;
	[SerializeField]
	AudioClip[] attackSounds;

	AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void Attack(bool isFacingRight)
	{

		audioSource.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);

		Vector2 direction;
		if (isFacingRight) direction = Vector2.right;
		else direction = Vector2.left;

		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, attackRange);

		Debug.DrawRay(transform.position, direction * attackRange, Color.red, 0.25f);

		foreach (RaycastHit2D hit in hits)
		{
			if (hit.collider.GetComponent<IDamagable>() != null &&
				hit.collider.gameObject != gameObject)
			{
				var damagable = hit.collider.GetComponent<IDamagable>();
				if (!damagable.IsDead()) damagable.Hit(attackDamage);
			}
		}
	}

	public float GetAttackRange() => attackRange;
}
