using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
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
            if (playerScript.currentHealth < manager.maxHealth)
                gameObject.SetActive(false);
			if (playerScript.currentHealth + regenRatio < manager.maxHealth)
				playerScript.currentHealth += 10;
			else if (playerScript.currentHealth < manager.maxHealth)
				playerScript.currentHealth = manager.maxHealth;
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
