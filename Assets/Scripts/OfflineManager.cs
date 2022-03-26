using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OfflineManager : MonoBehaviour
{

    [SerializeField] private GameObject death;
    [SerializeField] private GameObject offline;

    private void Awake()
    {

        PlayerManager gm = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();

        Debug.Log(gm.isDead);

        if(gm.isDead)
        {
            gm.DeadFalse();
            death.SetActive(true);

        } else
        {
            offline.SetActive(true);
        }
    }



}
