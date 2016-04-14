using UnityEngine;
using System.Collections;

public class ScaleWithParent : MonoBehaviour {
   
	// Update is called once per frame
	void Update () {
        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
	}
}
