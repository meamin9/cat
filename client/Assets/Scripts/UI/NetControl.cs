using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class NetControl : MonoBehaviour {

	void Awake() {
		Game.G.InitProto ();
	}
	// Use this for initialization
	void Start () {
		
		Net.Instance.P.Start ("127.0.0.1:7200");
	}
	
	// Update is called once per frame
	void Update () {
		Net.Instance.Poll ();
	}
}
