using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    [SerializeField]
    public bool isDead = false;

    private void Awake()
    {
        isDead = false;
        DontDestroyOnLoad(this.gameObject);
    }

    public void DeadTrue()
    {
        isDead = true;
        Debug.Log("DEAD TRUE");
    }

    public void DeadFalse()
    {
        isDead = false;
        Debug.Log("DEAD FALSE");
    }

    public bool GetDeath()
    {
        Debug.Log("DEAD " + isDead);
        return isDead;
    }
}
