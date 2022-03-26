using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ammoCountUI : MonoBehaviour
{
    [SerializeField] GameObject player;


    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = player.GetComponent<SnowballGather>().ballNum.ToString();
    }
}
