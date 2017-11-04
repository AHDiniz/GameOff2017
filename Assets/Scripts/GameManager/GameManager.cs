using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[Header("Bullets")]
	public GameObject bulletPrefab;
	[Header("Player Stats")]
	public PlayerBehaviour player;
	public int maxHealth = 100;
	public int maxShield = 100;
	public int maxAmmo = 50;
	[Header("Player Stats Bars")]
	public Image healthBar;
	public Image ammoBar;
	public Image shieldBar;

	[HideInInspector]
	public List<GameObject> bulletsToPool;

	void Awake()
	{
		player.currentAmmo = maxAmmo;
		player.currentHealth = maxHealth;
		player.currentShield = maxShield;
		bulletsToPool = new List<GameObject>();
	}

	void Update()
	{
		healthBar.fillAmount = (float)player.currentHealth / (float)maxHealth;
		shieldBar.fillAmount = (float)player.currentShield / (float)maxShield;
		ammoBar.fillAmount = (float)player.currentAmmo / (float)maxAmmo;
	}

	public GameObject BulletPooling()
	{
		GameObject g;
		for (int i = 0; i < bulletsToPool.Count; i++)
		{
			g = bulletsToPool[i];
			if (!g.activeInHierarchy)
				return g;
		}
		g = Instantiate(bulletPrefab);
		bulletsToPool.Add(g);
		return g;
	}

	public void DecreaseHealth(int d)
	{
		player.currentHealth -= d;
	}

	public void DeacreaseShield(int d)
	{
		player.currentShield -= d;
	}
}
