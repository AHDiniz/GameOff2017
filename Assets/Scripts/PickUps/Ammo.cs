using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
	[Header("Regen Ratio")]
	public int regenRatio = 10;
	[Header("Important Objects")]
	public GameManager manager;
	public GameObject player;
	[Header("Bounding Box")]
	public Vector2 halfSize;

	bool colSides = false, colVert = false;

    void Awake()
    {
        manager = GameObject.FindObjectOfType<GameManager>();
		player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        DetectCollisionWith(player);
        PlayerBehaviour playerScript = player.GetComponent<PlayerBehaviour>();
        if (colSides && colVert)
        {
            if (playerScript.currentAmmo < manager.maxAmmo)
                gameObject.SetActive(false);
            if (playerScript.currentAmmo + regenRatio < manager.maxAmmo)
                playerScript.currentAmmo += 10;
            else if (playerScript.currentAmmo < manager.maxAmmo)
                playerScript.currentAmmo = manager.maxAmmo;
        }
    }

    void DetectCollisionWith(GameObject g)
    {
        Transform t = g.transform;
        float dist = Vector2.Distance(t.position, transform.position);
        if (dist < 2 * halfSize.x)
            colSides = colVert = true;
        else
            colSides = colVert = false;
    }
}
