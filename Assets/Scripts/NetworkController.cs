using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour {

	private const string typeName = "WerewolfProject";
	private const string gameName = "RoomName";

	public int playerCount=0;

	public Vector3[] playerPositions = new [] { 
		new Vector3(0f,0.5f,3f), 
		new Vector3(2f,0.5f,2f), 
		new Vector3(3f,0.5f,0f),
		new Vector3(2f,0.5f,-2f),
		new Vector3(0f,0.5f,-3f),
		new Vector3(-2f,0.5f,-2f),
		new Vector3(-3f,0.5f,0f),
		new Vector3(-2f,0.5f,2f)
	};

	private void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer) {
			if (GUI.Button (new Rect (100, 100, 250, 100), "Start Server"))
				StartServer ();
			if (GUI.Button (new Rect (100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList ();
			
			if (hostList != null) {
				for (int i = 0; i < hostList.Length; i++) {
					if (GUI.Button (new Rect (400, 100 + (110 * i), 300, 100), hostList [i].gameName))
						JoinServer (hostList [i]);
				}
			}
		} else {
			GUI.TextArea(new Rect (20,20,67,23), "Players: "+(Network.connections.Length + 1));
			if (GUI.Button (new Rect(920,410,150,60), "Disconnect")) Network.Disconnect ();
		} 
	}

	private HostData[] hostList;
	
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	public GameObject playerPrefab;
	
	void OnServerInitialized()
	{
		SpawnPlayer();
	}
	
	void OnConnectedToServer()
	{
		SpawnPlayer();
	}
	
	private void SpawnPlayer()
	{
		Network.Instantiate(playerPrefab, playerPositions[Network.connections.Length], Quaternion.identity, 0);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Players")){
			Destroy (player);
		}
	}

	void OnPlayerDisconnected(NetworkPlayer player){
		Network.DestroyPlayerObjects (player);
	}

	void OnApplicationQuit(){
		Network.DestroyPlayerObjects (Network.player);
	}

}
