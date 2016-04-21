using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerSelectScript : MonoBehaviour {


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Retry") || Input.GetKeyDown(KeyCode.A))
        {
            PlayerPrefs.SetInt("numPlayers", 1);
            SceneManager.LoadScene("cj_test");
        }
        else if (Input.GetButtonDown("Quit") || Input.GetKeyDown(KeyCode.B))
        {
            PlayerPrefs.SetInt("numPlayers", 2);
            SceneManager.LoadScene("cj_test");
        }
    }
}