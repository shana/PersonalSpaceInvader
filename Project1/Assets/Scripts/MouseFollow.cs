﻿using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour {

	public float rotationSpeed = 0.1f;
	private Transform target;
	private PlayerBehaviour player;

	void Start()
	{
		player = transform.root.gameObject.GetComponent<PlayerBehaviour>();
		var enemy = player.enemy;
		if (enemy)
			target = enemy.transform;
	}

	void Update () 
	{
		RotateTowardsTarget(target);
	}

	private void RotateTowardsTarget(Transform target)
	{
		if (!target)
			return;

		target = player.enemy.transform;
		var direction = (target.position - transform.position).normalized;
		var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
		angle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, Time.deltaTime * rotationSpeed);
		var lookRotation = Quaternion.Euler(0, 0, angle);
		
		transform.rotation = lookRotation;
	}
}
