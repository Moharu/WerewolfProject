using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ListServersController : MonoBehaviour {

	public GameObject serverButton;
	private GameObject[] servers;
	public int itemCount;
	public int columnCount;

	private void OnEnable(){
		int childCount = gameObject.transform.childCount;
		for (int i=0; i<childCount; i++) {
			Destroy (gameObject.transform.GetChild (0).gameObject);
		}
		findHosts ();
	}

	public void findHosts(){
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<NetworkController> ().RefreshHostList (); //Solicita ao NetworkController a lista de servers
	}

	public void listServers(HostData[] hosts){
		for (int i = 0; i < hosts.Length; i++) {
			GameObject newItem = Instantiate(serverButton,new Vector3(540,310 - 110 * i,0), Quaternion.identity) as GameObject;
			newItem.transform.SetParent (gameObject.transform);
			newItem.GetComponent<ServerScript>().setServer(hosts[i]);
			newItem.GetComponentInChildren<Text>().text = hosts[i].gameName;
		}
	}
}
