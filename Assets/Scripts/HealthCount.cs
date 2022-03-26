using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HealthCount : MonoBehaviour
{
    [SerializeField] private PlayerMovement movementScript;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.localScale = new Vector3((float)movementScript.health / 100, 1, 1);

    }

    bool playerDeath => movementScript.health <= 0;
}
