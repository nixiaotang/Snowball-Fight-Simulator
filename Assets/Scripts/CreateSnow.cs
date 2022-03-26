using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSnow : MonoBehaviour
{
    [SerializeField] GameObject MainCam;
    [SerializeField] GameObject Snow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!MainCam.activeInHierarchy)
        {
            Snow.SetActive(true);
        }
    }
}
