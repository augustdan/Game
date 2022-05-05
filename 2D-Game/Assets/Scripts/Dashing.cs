using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
	[SerializeField]
	float dashForce;
	[SerializeField]
	float durationVar;
	float dashDuration = 0.1f;
	[SerializeField]
	float varCD;
	float dashCoolDown = 5f;
	[SerializeField]
	bool canDash;
	Rigidbody2D playerRb;
	PlayerController playerController;
	private bool isFacingRight = true;

	void Start()
	{
		playerRb = GetComponent<Rigidbody2D>();
		playerController = GetComponent<PlayerController>();
		varCD = dashCoolDown;
		durationVar = dashDuration;
		canDash = true;
	}

	void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.LeftShift) && canDash)
		{
			if (isFacingRight)
			{
				playerRb.AddForce(transform.right * dashForce);
			}
			else
			{
				playerRb.AddForce(-transform.right * dashForce);
			}
			durationVar -= Time.deltaTime;
			if (durationVar < 0)
			{
				canDash = false;
				durationVar = dashDuration;
			}
		}

		if (canDash == false)
		{
			varCD -= Time.deltaTime;
			if (varCD <= 0)
			{
				canDash = true;
				varCD = dashCoolDown;

			}
		}
	}
}
