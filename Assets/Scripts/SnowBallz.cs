using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SnowBallz : NetworkBehaviour
{

    [SerializeField] float lifeTime;
    [SerializeField] GameObject player;
    [SerializeField] GameObject particleSmall;
    [SerializeField] GameObject particleMedium;
    [SerializeField] GameObject particleLarge;
    public string shooterID;
    public PlayerMovement shooterScript;
    public Vector3 force;
    public float size;

    [Server]
    private void Awake()
    {
        Destroy(gameObject, lifeTime);
        
    }

    [Server]
    private void Start()
    {

        GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {

        SnowballGather SGScript = other.gameObject.GetComponent<SnowballGather>();
        if (SGScript != null) Debug.Log("COLLIDE: " + other + " " + other.gameObject.ToString());

        Debug.Log((int)Math.Round(size * 10));

        if (SGScript == null || other.gameObject.ToString() != shooterID)
        {
            Debug.Log(other);

            if (SGScript != null)
            {
                other.gameObject.GetComponent<PlayerMovement>().decreaseHealth((int)Math.Round(size*10));

            }

            /*GameObject particle = null;
            switch (transform.localScale.x)
            {
                case < 2f:
                    particle = Instantiate(particleSmall, transform.position, Quaternion.identity);
                    break;
                case >= 2f and < 4f:
                    particle = Instantiate(particleMedium, transform.position, Quaternion.identity);
                    break;
                case >= 4f:
                    particle = Instantiate(particleLarge, transform.position, Quaternion.identity);
                    break;
            }
            Destroy(particle, 1f);
            
            Destroy(gameObject);*/

            shooterScript.RpcParticle(transform.position, transform.localScale.x);

            Destroy(gameObject);
        }


    }

}
