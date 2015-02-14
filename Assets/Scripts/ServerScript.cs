using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ServerScript : MonoBehaviour {

	public HostData server;

	// Use this for initialization
	public void Connect () {
		Network.Connect(server);
	}

	public void setServer(HostData sv){
		server = sv;
	}

	public HostData getServer(){
		return server;
	}
}
