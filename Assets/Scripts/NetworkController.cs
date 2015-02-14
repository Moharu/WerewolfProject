using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkController : MonoBehaviour {

	public GameObject lobbycanvas;
	public GameObject menucanvas;
	public GameObject selectservercanvas;
	private string[] playersOnServer = {"-","-","-","-","-","-"};

	void Start(){
		selectservercanvas = GameObject.FindGameObjectWithTag ("SelectServerCanvas");
		lobbycanvas = GameObject.FindGameObjectWithTag("LobbyCanvas");
		menucanvas = GameObject.FindGameObjectWithTag("MenuCanvas");
		lobbycanvas.SetActive (false);
		selectservercanvas.SetActive (false);
	}
	
	private const string typeName = "WerewolfProject";
	private string gameName = "Game Room";
	public HostData[] hostList;
	public string myName;
	public GameObject userOnListPrefab;

//RPC METHODS
	[RPC]
	void addPlayerToList(string name){
		for (int i=0; i<4; i++) {
			if(playersOnServer[5-i] != "-"){
				if(playersOnServer[4-i] != "-")
					playersOnServer[4-i] = playersOnServer[5-i];
				playersOnServer[5-i] = "-";
			}
		}
		for (int i=0; i<6; i++) {
			if(playersOnServer[i] == "-"){
				playersOnServer[i] = name;
				break;
			}
		}
		networkView.RPC ("publishPlayerList", RPCMode.Others, playersOnServer);
	}

	[RPC]
	void publishPlayerList(string[] players){
		Debug.Log ("aqui");
		for (int i=0; i<6; i++) {
			playersOnServer[i] = players[i];
		}
	}

	[RPC]
	private void getPlayerListForAll(){
		if (Network.isServer) {
			networkView.RPC ("refreshPlayerList", RPCMode.All, (Network.connections.Length + 1));
		}
	}

	[RPC]
	void refreshPlayerList(int itemCount){
		destroyPlayerList ();
		RectTransform rowRectTransform = userOnListPrefab.GetComponent<RectTransform> ();
		RectTransform containerRectTransform = GameObject.FindGameObjectWithTag ("PlayerContainer").GetComponent<RectTransform> ();
		float width = containerRectTransform.rect.width;
		float height = rowRectTransform.rect.height;

		for (int i=0; i < itemCount; i++) {
			//Create new button
			GameObject newItem = Instantiate (userOnListPrefab) as GameObject;
			newItem.name = GameObject.FindGameObjectWithTag ("PlayerContainer").name + " player at position " + i;
			newItem.transform.parent = GameObject.FindGameObjectWithTag ("PlayerContainer").transform;
			newItem.GetComponentInChildren<Text>().text = playersOnServer[i];
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

	public void refreshList2(){
		networkView.RPC ("publishPlayerList", RPCMode.Others, playersOnServer);
	}

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

	public void refreshList(){
		networkView.RPC ("getPlayerListForAll", RPCMode.All);
	}

	public void disconnect(){
		Network.Disconnect ();
	}

	public void destroyPlayerList(){
		int childCount = GameObject.FindGameObjectWithTag ("PlayerContainer").transform.childCount;
		for (int i=0; i<childCount; i++) {
			Destroy (GameObject.FindGameObjectWithTag ("PlayerContainer").transform.GetChild (0).gameObject);
		}
	}

//PRIVATE METHODS

	void Awake(){
		DontDestroyOnLoad (this);
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived) {
			hostList = MasterServer.PollHostList ();
			GameObject.FindGameObjectWithTag ("ServerList").GetComponent<ListServersController> ().listServers (hostList);	//Envia para o ListServers a lista de servidores recebeidas do MasterServer
		}
	}

	void OnServerInitialized()
	{
		playersOnServer [0] = myName;
	}
	
	void OnConnectedToServer()
	{
		selectservercanvas.SetActive (false);
		menucanvas.SetActive (false);
		lobbycanvas.SetActive (true);
		networkView.RPC ("addPlayerToList", RPCMode.Server, myName);
		refreshList ();
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		destroyPlayerList ();
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Players")) {
			Destroy (player);
		}
		lobbycanvas.SetActive (false);
		menucanvas.SetActive (true);
	}

	void OnPlayerDisconnected(NetworkPlayer player){
		destroyPlayerList ();
		if(Network.isServer) Network.DestroyPlayerObjects (player);
		Network.RemoveRPCs(player);	//FUNCIONA
		refreshList ();
	}

	void OnApplicationQuit(){
		Network.DestroyPlayerObjects (Network.player);
	}

}
