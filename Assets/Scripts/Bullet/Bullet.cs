using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
	public float speed, lifeTime;
	public int damage;
	public float explosionDuration;

	Animator bulletAnimator;
	Rigidbody2D bulletBody;
	bool canBlow;

	public void Init()
	{
		StopAllCoroutines();
		gameObject.SetActive(true);
		bulletAnimator = GetComponent<Animator>();
		bulletBody = GetComponent<Rigidbody2D>();
		bulletBody.velocity = Vector2.zero;
		bulletBody.AddRelativeForce((Vector2.right * speed), ForceMode2D.Impulse);
		StartCoroutine(LifeTime());
	}

	IEnumerator LifeTime()
	{
		yield return new WaitForSeconds(lifeTime);
		gameObject.SetActive(false);
	}
}
