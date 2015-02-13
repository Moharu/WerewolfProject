using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputName : MonoBehaviour {

	public Text text;

	// Update is called once per frame
	public void FixedUpdate () {
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<NetworkController>().setName(text.text);
	}
}
