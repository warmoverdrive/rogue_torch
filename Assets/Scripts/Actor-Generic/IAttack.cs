using System.Collections;
using UnityEngine;

public interface IAttack
{
	void Attack(bool isFacingRight);

	float GetAttackRange();
}
