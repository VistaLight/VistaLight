﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

	public GameObject nodePreFab;
	public GameObject connectionPreFab;

	private int nextNodeId = 1;
	public List<Node> nodes = new List<Node>();
	public List<Connection> connections = new List<Connection>();
	//public List<Vector3> ports;

	public Map() {}

	public string ToString() { return "1234"; }

	public Node AddNode(Vector3 position){
		GameObject node = Instantiate (nodePreFab, 
									   new Vector3(position.x, position.y, -1), 
			                           transform.rotation) as GameObject;
		nodes.Add (node.GetComponent<Node>());
		node.GetComponent<Node> ().Id = nextNodeId;
		nextNodeId ++;
		return node.GetComponent<Node>();
	}
 
	public Connection AddConnection(Node startNode, Node endNode, bool isBidirectional){
		// Instantiate the connection
		GameObject connectionObject = Instantiate (connectionPreFab, Vector3.zero, Quaternion.identity) as GameObject;
		Connection connection = connectionObject.GetComponent<Connection> ();
		connections.Add (connection);

		// Set the parameters of the connection
		connection.StartNode = startNode;
		connection.EndNode = endNode;
		connection.Bidirectional = isBidirectional;

		// Update the position of a connection
		connection.UpdatePosition();

		// Let both startNode and endNode have information about the connection
		startNode.AddConnection(connection);
		endNode.AddConnection(connection);

		// Return the newly created connection
		return connection;
	}
	
	/*
	public void addPort(Vector3 port, Vector3 replacednode){

		ports.Add(port);
		foreach (Vector3 element in nodes) {
			if(element = replacednode){
				nodes.Remove(element);
			}
		}
	}
	*/


}
