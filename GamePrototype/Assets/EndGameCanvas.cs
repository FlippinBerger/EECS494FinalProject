using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndGameCanvas : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetButtonDown("Retry") || Input.GetKeyDown(KeyCode.A))
        {
            // reload scene
            SceneManager.LoadScene("cj_test");
        }
        else if (Input.GetButtonDown("Quit") || Input.GetKeyDown(KeyCode.B))
        {
            Application.Quit();
        }
	}
}
