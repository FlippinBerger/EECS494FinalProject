using UnityEngine;
using System.Collections;

public class BuyZone : MonoBehaviour {

    public GameObject itemPrefab;

	// Use this for initialization
	void Start () {
        // spawn random item instead of itemprefab (maybe)
        GameObject itemGO = (GameObject)Instantiate(itemPrefab, transform.position, Quaternion.identity);
        itemGO.transform.parent = this.transform;
        // spawn price tag
        GameObject pricetagGO = (GameObject)Instantiate(GameManager.S.pricetagPrefab, transform.position + Vector3.right * .5f, Quaternion.identity);
        pricetagGO.transform.parent = itemGO.transform;
        TextMesh tm = pricetagGO.transform.FindChild("Price").GetComponent<TextMesh>();
        tm.text = itemGO.GetComponent<Item>().cost.ToString();
        print(tm.text);
	}
}
