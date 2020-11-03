using UnityEngine;


public interface IDamagable
{
	bool IsDead();
	void Hit(int damage);
}