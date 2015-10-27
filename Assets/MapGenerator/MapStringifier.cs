//------------------------------------------------------------------------------// <auto-generated>//     This code was generated by a tool.//     Runtime Version:4.0.30319.42000////     Changes to this file may cause incorrect behavior and will be lost if//     the code is regenerated.// </auto-generated>//------------------------------------------------------------------------------using System;using System.Text;public class MapStringifier{	private Map map;	public MapStringifier (Map map)	{		this.map = map;	}	private void AddNodeInformation(StringBuilder sb) {		// Append each node		int count = 0;		sb.Append("\"nodes:\":[\n");		foreach (Node node in map.nodes) {			sb.Append(String.Format("{{ \"id\":{0}, \"position\": {{ \"x\":{1}, \"y\":{2}, \"z\":{3} }} }}",					  node.Id,					  node.gameObject.transform.position.x,					  node.gameObject.transform.position.y,					  node.gameObject.transform.position.z));			count++;			if (count != map.nodes.Count) {				sb.Append(",\n");			}		}		sb.Append("\n],\n");	}	private void AddConnectionInformation(StringBuilder sb) {		int count = 0;		sb.Append("\"segments\": [\n");		foreach (Connection connection in map.connections) {			sb.Append(String.Format("{{\"src\":{0}, \"dst\":{1}, \"bidirectional\":{2} }}",									connection.StartNode.Id,									connection.EndNode.Id,									connection.Bidirectional.ToString().ToLower()));			count++;			if (count != map.connections.Count) {				sb.Append(",\n");			}		}		sb.Append("\n]\n");	}	private void AddNetworkInformation(StringBuilder sb) {		sb.Append("\"network\":{\n");		AddNodeInformation(sb);		AddConnectionInformation(sb);		sb.Append("},\n");	}	private void AddDockInformation(StringBuilder sb) {		int count = 0;		sb.Append("\"docks\":[\n");		foreach (Dock dock in map.docks) {			sb.Append(String.Format("{{\"industry\": \"{0}\",\"position\": " +							"{{\"x\":{1}, \"y\":{2}, \"z\":{3}}}}}", 							dock.Type.ToString(), 							dock.Node.Position.x,							dock.Node.Position.y,							dock.Node.Position.z));			count++;			if (count != map.docks.Count) { 				sb.Append(",\n");			}		}		sb.Append("\n]\n");	}	public string Stringify() {		StringBuilder sb = new StringBuilder ();		sb.Append("{\n");		sb.Append ("\"map_name\": \"basemap\",\n");		sb.Append("\"start_time\": \"2015-01-01 12:00:00\",\n");		// Network		AddNetworkInformation(sb);		// Docks		AddDockInformation(sb);		// Finish the object		sb.Append("}");		// Return the built string		return sb.ToString ();	}}