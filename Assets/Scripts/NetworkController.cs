using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkController : MonoBehaviour {

	private const string typeName = "WerewolfProject";
	private string gameName = "Game Room";

	public int playerCount;

	public string myName;

	void Update(){
		if (Network.isServer){
			playerCount = Network.connections.Length + 1;
			networkView.RPC ("receivePlayerCount", RPCMode.Others, playerCount);
		}
	}

	[RPC]
	void sendPlayerCount(){
		playerCount = Network.connections.Length + 1;
		networkView.RPC ("receivePlayerCount", RPCMode.Others, playerCount);
	}


	[RPC]
	void receivePlayerCount(int count){
		playerCount = count;
	}

	[RPC]
	void enablePlayerCreation(NetworkPlayer player){
		networkView.RPC ("createPlayerAfterConnected", player);
	}

	[RPC]
	void createPlayerAfterConnected(){
		SpawnPlayer ();
	}

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

	void OnGUI()
	{
		if (Network.isClient || Network.isServer) {
			GUI.TextArea(new Rect (20,20,67,23), "Players: "+ playerCount);
			if (GUI.Button (new Rect(0,50,150,60), "Disconnect")) Network.Disconnect ();
		}
	} 

	public void exitGame(){
		Application.Quit ();
	}

	public void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

	public HostData[] hostList;
	
	public void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
		GameObject.FindGameObjectWithTag ("ServerList").GetComponent<ListServersController> ().listServers (hostList);	//Envia para o ListServers a lista de servidores recebeidas do MasterServer
	}

	public void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	public GameObject playerPrefab;
	
	void OnServerInitialized()
	{
		SpawnPlayer ();
	}
	
	void OnConnectedToServer()
	{
		networkView.RPC ("sendPlayerCount", RPCMode.Server);
		networkView.RPC ("enablePlayerCreation", RPCMode.Server, Network.player);
	}
	
	private void SpawnPlayer()
	{
		Update ();
		Debug.Log (playerCount);
		Network.Instantiate(playerPrefab, playerPositions[playerCount-1], Quaternion.identity, 0);	//Instancia na rede um novo jogado, na posicao playersCount-1

	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Network.DestroyPlayerObjects (Network.player);
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Players")) {
			Destroy (player);
		}
	}

	void OnPlayerDisconnected(NetworkPlayer player){
		if(Network.isServer) Network.DestroyPlayerObjects (player);
		Network.RemoveRPCs(player);	//FUNCIONA
	}

	void OnApplicationQuit(){
		Network.DestroyPlayerObjects (Network.player);
	}

	public void setName(string name){
		myName = name;
		gameName = myName + "'s Room";
	}
}
