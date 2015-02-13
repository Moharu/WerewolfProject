using UnityEngine;
using System.Collections;

public class ListServersController : MonoBehaviour {

	GameObject serverButton;
	HostData[] serverList;

	private void OnEnable(){
		findHosts ();
	}

	public void findHosts(){
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<NetworkController> ().RefreshHostList (); //Solicita ao NetworkController a lista de servers
	}

	public void listServers(HostData[] hosts){
		serverList = hosts;
	}

	public void OnGUI(){
		if (serverList != null) {
			for (int i = 0; i < serverList.Length; i++) {
				if (GUI.Button (new Rect (200, 200 + (110 * i), 100, 50), serverList [i].gameName)){
					GameObject.FindGameObjectWithTag ("Canvas").SetActive (false);
					GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<NetworkController> ().JoinServer (serverList [i]);
				}
			}
		}
	}
}
