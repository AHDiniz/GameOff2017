using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform player;
    public float followSpeed;

    void Update()
    {
        Vector3 target = new Vector3(player.position.x, player.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followSpeed);
    }
}
