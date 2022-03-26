using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadGameScene : MonoBehaviour
{

    public void SceneLoad()
    {
        SceneManager.LoadScene("02 - Worlds");
    }
}
