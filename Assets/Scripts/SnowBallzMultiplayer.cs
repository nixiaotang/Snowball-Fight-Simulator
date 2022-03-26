using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SnowBallzMultiplayer : NetworkBehaviour
{

    private float speed = 5f;
    private Collider playerCollider;
    private Vector3 snowballDirection;


    private void Update()
    {
        if (snowballDirection == null) return;

        transform.Translate(snowballDirection * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerCollider == null) return;
        else if (playerCollider != other)
        {
            if(other.gameObject.tag == "Player")
            {
                Debug.Log("called decrease health");
                other.gameObject.GetComponent<PlayerMovementMultiplayer>().decreaseHealth();
            }

            Destroy(gameObject);
        }
        
    }

    /*[ClientRpc]
    private void RpcDamage(GameObject hitPlayer)
    {
        hitPlayer.GetComponent<PlayerMovementMultiplayer>().decreaseHealth();
    }*/

    public void setSnowballSettings(Collider player, Vector3 direct)
    {
        playerCollider = player;
        snowballDirection = direct;
    }

}
