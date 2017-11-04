using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControllerBehaviour))]
public class PlayerBehaviour : MonoBehaviour
{
	#region publicReferences
		[Header("Movement")]
		public float moveSpeed = 6.0f;
		public float accelTimeGrounded = .1f;
		[Header("Jumping")]
		public float jumpHeight = 4.0f;
		public float timeToJumpApex = .4f;
		public float accelTimeAirborne = .2f;
		[Header("Shooting")]
		public Transform turretRight;
		public Transform turretLeft;
		public GameManager manager;
	#endregion

	#region privateReferences
		ControllerBehaviour controller; // the player controller component
		float gravity; // the gravity of the player
		float jumpVelocity; // the force of the player jump
		float velocityXSmoothing; // how much the player velocity will smooth in the x axis
		Vector3 velocity; // the player's velocity vector
		SpriteRenderer playerRenderer;
		Animator animator;
		[HideInInspector]
		public int currentHealth, currentAmmo, currentShield;
	#endregion

	void Start()
	{
		// Initializing the controller component:
		controller = GetComponent<ControllerBehaviour>();
		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2); // setting the gravity value
		jumpVelocity = Mathf.Abs((gravity * timeToJumpApex)); // setting the jump velocity
		playerRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		if (controller != null)
			controller.Init();
	}

	void Update()
	{
		Shooting();
		Animating();
		// Reseting the velocity when the player is in the ground:
		if (controller.colInfo.above || controller.colInfo.below)
			velocity.y = 0;
		// Getting the player's input:
		Vector2 input = GetInput();
		float targetVelocity = (input.x * moveSpeed);
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity, ref velocityXSmoothing, (controller.colInfo.below) ? accelTimeGrounded : accelTimeAirborne);
		// Making the player jump:
		if (Input.GetButtonDown("Jump") && controller.colInfo.below)
		{
			velocity.y = jumpVelocity;
		}
		// Flipping the sprite:
		SpriteFlip();
		// Adding the gravity to the player:
		float delta = Time.deltaTime;
		velocity.y += (gravity * delta);
		controller.Move((velocity * delta));
	}

	Vector2 GetInput()
	{
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
		return new Vector2(x, y);
	}

	void SpriteFlip()
	{
		if (velocity.x > 0)
			playerRenderer.flipX = false;
		if (velocity.x < 0)
			playerRenderer.flipX = true;
	}

	void Animating()
	{
		float x = Input.GetAxis("Horizontal");
		bool j = Input.GetButtonDown("Jump");
        animator.SetBool("isGrounded", controller.colInfo.below);
		animator.SetBool("isWalking", (x != 0));
		animator.SetBool("isJumping", j);
    }

	void Shooting()
	{
		bool canShoot = (controller.colInfo.below && Input.GetButtonDown("Fire1") && currentAmmo > 0);
		GameObject b = manager.BulletPooling();
		if (canShoot)
		{
			if (!playerRenderer.flipX)
			{
				b.transform.position = turretRight.position;
				b.transform.rotation = turretRight.rotation;
				Bullet bullet = b.GetComponent<Bullet>();
				bullet.Init();
                currentAmmo--;
				Debug.Log(currentAmmo);
            }
			else
			{
				b.transform.position = turretLeft.position;
                b.transform.rotation = turretLeft.rotation;
                Bullet bullet = b.GetComponent<Bullet>();
                bullet.Init();
                currentAmmo--;
				Debug.Log(currentAmmo);
            }
		}
	}
}
