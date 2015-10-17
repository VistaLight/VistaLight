//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;

public class SingleDirectionTool : IMapEditorTool
{
	public Map map;
	public float segmentLength = 2;

	private Node previousNode;

	/**
	 * State machine state
	 *   0 - Not started
	 *   1 - Started
	 */
	private int state = 0;

	public SingleDirectionTool (Map map)
	{
		state = 0;
		this.map = map;
	}

	private void SelectNode(Node node) {
		node.gameObject.transform.FindChild("NodeDot").GetComponent<SpriteRenderer>().enabled = false;
		node.gameObject.transform.FindChild("NodeDotSelected").GetComponent<SpriteRenderer>().enabled = true;
	}

	private void DeselectNode(Node node) {
		node.gameObject.transform.FindChild("NodeDot").GetComponent<SpriteRenderer>().enabled = true;
		node.gameObject.transform.FindChild("NodeDotSelected").GetComponent<SpriteRenderer>().enabled = false;
	}


	private void CreateNewRoad(Vector3 startPosition, Vector3 targetPosition) {
		DeselectNode(previousNode);
		float remainingDistance = Vector3.Distance(startPosition, targetPosition);

		// Get the direction from the start point to the destination
		Vector3 vector = (targetPosition - startPosition).normalized;

		// Create a series of nodes towards the destination
		Vector3 previousPosition = startPosition;
		Node nextNode;
		while (remainingDistance > segmentLength) {
			// Create next node
			Vector3 nextPosition = previousPosition + vector * segmentLength;
			nextNode = map.AddNode(nextPosition);

			// Create connection
			Connection connection = map.AddConnection(previousNode, nextNode, false);

			// Update for the next iteration
			previousNode = nextNode;
			previousPosition = nextPosition;
			remainingDistance = (targetPosition - previousPosition).magnitude;
		}

		// At the end, create a node at the target position
		nextNode = map.AddNode(targetPosition);
		map.AddConnection(previousNode, nextNode, false);
		previousPosition = nextNode.gameObject.transform.position;

		
        previousNode = nextNode;
		SelectNode(nextNode);

	}

	public void RespondMouseClick() {
		RaycastHit2D ray = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		switch (state) {
		case 0:
			if(ray.collider != null ) {
				if (ray.collider.tag == "Background") {
					previousNode = map.AddNode(ray.point);
					this.SelectNode(previousNode);
				} else if (ray.collider.tag == "Node") {
					// Console.WriteLine(ray.collider);
					previousNode = ray.collider.GetComponent<Node>();
					this.SelectNode(previousNode);
				}
			}
			state = 1;
			break;
		case 1:
			if (ray.collider != null && ray.collider.tag == "Background") {
				Vector3 targetPosition = ray.point;
				Vector3 startPosition = previousNode.gameObject.transform.position;
				this.CreateNewRoad(startPosition, targetPosition);
			} else if (ray.collider != null && ray.collider.tag == "Node") {
				DeselectNode(previousNode);
				previousNode = ray.collider.GetComponent<Node>();
				SelectNode(previousNode);
			}
			break;
		default:
			break;
		}
	}

    public void RespondMouseMove(float x, float y) {
		Debug.Log("Move");
    }

	public void Destory() {
		DeselectNode(previousNode);
	}
}


