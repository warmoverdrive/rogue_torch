﻿using UnityEngine;
using System.Collections;

public class ActorStatusController : MonoBehaviour, IDamagable
{
	[Header("Design Levers")]
	[SerializeField]
	int hitPoints = 1;

	bool isDead = false;
	public bool isBlocking = false;

	public void Hit(int damage) 
	{
		if (isBlocking) return;
		else
		{
			hitPoints -= damage;
			if (hitPoints <= 0) isDead = true;
		}
	}

	public bool IsDead() { return isDead; }
}
