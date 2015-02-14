using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ListServersController : MonoBehaviour {

	public GameObject serverButton;
	private GameObject[] servers;
	public int itemCount;
	public int columnCount;

	public int gambiarra=0;

	private void OnEnable(){
		gambiarra++;
		if (gambiarra > 0) {
			int childCount = gameObject.transform.childCount;
			for (int i=0; i<childCount; i++) {
				Destroy (gameObject.transform.GetChild (0).gameObject);
			}
			findHosts ();
		}
	}

	public void findHosts(){
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<NetworkController> ().RefreshHostList (); //Solicita ao NetworkController a lista de servers
	}

	public void listServers(HostData[] hosts){
		itemCount = hosts.Length;
		RectTransform rowRectTransform = serverButton.GetComponent<RectTransform> ();
		RectTransform containerRectTransform = gameObject.GetComponent<RectTransform> ();
		float width = containerRectTransform.rect.width;
		float height = rowRectTransform.rect.height;
		
		for (int i=0; i < itemCount; i++) {
			//Create new button
			GameObject newItem = Instantiate (serverButton) as GameObject;
			newItem.name = i.ToString();
			newItem.transform.SetParent(gameObject.transform);
			
			//Move and Size Button
			RectTransform rectTransform = newItem.GetComponent<RectTransform> ();
			
			float x = -containerRectTransform.rect.width / 2;
			float y = containerRectTransform.rect.height / 2 - height * (i + 1);
			rectTransform.offsetMin = new Vector2 (x, y);
			
			x = rectTransform.offsetMin.x + width;
			y = rectTransform.offsetMin.y + height;
			rectTransform.offsetMax = new Vector2 (x, y);

			newItem.GetComponentInChildren<ServerScript>().server = hosts[i];
			newItem.GetComponentInChildren<Text>().text = hosts[i].gameName;
		}
	}
}
