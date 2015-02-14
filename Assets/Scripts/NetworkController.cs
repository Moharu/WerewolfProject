using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkController : MonoBehaviour {

	public GameObject lobbycanvas;
	public GameObject menucanvas;

	void Start(){
		lobbycanvas = GameObject.FindGameObjectWithTag("LobbyCanvas");
		menucanvas = GameObject.FindGameObjectWithTag("MenuCanvas");
		lobbycanvas.SetActive (false);
	}
	
	private const string typeName = "WerewolfProject";
	private string gameName = "Game Room";
	public HostData[] hostList;
	public string myName;
	public GameObject userOnListPrefab;

	public void refreshList(){
		networkView.RPC ("getPlayerListForAll", RPCMode.All);
	}

//RPC METHODS
	[RPC]
	private void getPlayerListForAll(){
		if (Network.isServer) {
			networkView.RPC ("refreshPlayerList", RPCMode.All, (Network.connections.Length + 1));
		}
	}

	[RPC]
	void refreshPlayerList(int itemCount){
		int childCount = GameObject.FindGameObjectWithTag ("PlayerContainer").transform.childCount;
		for (int i=0; i<childCount; i++) {
			Destroy (GameObject.FindGameObjectWithTag ("PlayerContainer").transform.GetChild (0).gameObject);
		}
		RectTransform rowRectTransform = userOnListPrefab.GetComponent<RectTransform> ();
		RectTransform containerRectTransform = GameObject.FindGameObjectWithTag ("PlayerContainer").GetComponent<RectTransform> ();
		float width = containerRectTransform.rect.width;
		float height = rowRectTransform.rect.height;

		for (int i=0; i < itemCount; i++) {
			//Create new button
			GameObject newItem = Instantiate (userOnListPrefab) as GameObject;
			newItem.name = GameObject.FindGameObjectWithTag ("PlayerContainer").name + " player at position " + i;
			newItem.transform.parent = GameObject.FindGameObjectWithTag ("PlayerContainer").transform;

			//Move and Size Button
			RectTransform rectTransform = newItem.GetComponent<RectTransform>();

			float x = -containerRectTransform.rect.width / 2;
			float y = containerRectTransform.rect.height/ 2 - height * (i+1);
			rectTransform.offsetMin = new Vector2(x, y);

			x = rectTransform.offsetMin.x + width;
			y = rectTransform.offsetMin.y + height;
			rectTransform.offsetMax = new Vector2(x, y);
		}
	}


//PUBLIC METHODS

	public void setName(string name){
		myName = name;
		gameName = myName + "'s Room";
	}

	public string getName(){
		return myName;
	} 

	public void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}

	public void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}

	public void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	public void exitGame(){
		Application.Quit ();
	}

	public void createUserOnList(string name){
		Network.Instantiate(userOnListPrefab, new Vector3(0,0,0),Quaternion.identity,5);
	}


//PRIVATE METHODS

	void Awake(){
		DontDestroyOnLoad (this);
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
		GameObject.FindGameObjectWithTag ("ServerList").GetComponent<ListServersController> ().listServers (hostList);	//Envia para o ListServers a lista de servidores recebeidas do MasterServer
	}

	void OnServerInitialized()
	{
		//
	}
	
	void OnConnectedToServer()
	{
		menucanvas.SetActive (false);
		lobbycanvas.SetActive (true);
		refreshList ();
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

}
