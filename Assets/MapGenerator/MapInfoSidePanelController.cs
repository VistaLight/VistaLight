﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using System.Xml.Serialization;

public class MapInfoSidePanelController : MonoBehaviour {

	public GameObject mapInformationSetting;
	public MapController mapController;
	public ShipPanelController shipPannelController;
	public Map map;
	public bool hasModification = false;
	public string path = "";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void updateMapInformationDisplay() {
		if (map != null) {
			GameObject.Find("MapNameInput").GetComponent<InputField>().text = map.Name;
			GameObject.Find("StartTimeInput").GetComponent<InputField>().text = 
				map.StartTime.ToString(Map.DateTimeFormat);
		}
	}

	public void NewMap() {
		if (mapController.Map != null) {
			CloseMap();
		}
		map = new Map();
		mapController.RegenerateMap(map);
		hasModification = false;

		mapInformationSetting.SetActive(true);
		updateMapInformationDisplay();
	}

	public void LoadMap() {

		CloseMap();

		string[] mapTypes = {
			"VistaLights Map Files", "vlmap",
			"All Files", "*"
		};
		path = EditorUtility.OpenFilePanel("Load map", "", "vlmap");

		MapSerializer mapSerializer = new MapSerializer();
		this.map = mapSerializer.LoadMap(path);

		mapController.RegenerateMap(map);
		shipPannelController.RegenerateShip(map.ships);

		mapInformationSetting.SetActive(true);
		this.updateMapInformationDisplay();
	} 

	public void SaveMap() {
		if (path == "") {
			path = EditorUtility.SaveFilePanel("Select file location", "", "map", "vlmap");
		}

		MapSerializer mapSerializer = new MapSerializer();
		mapSerializer.SaveMap(map, path);
	}

	public void SaveMapAs() {
	}

	public void CloseMap() {
		map = null;
		mapController.CloseMap();
		shipPannelController.ClearShips();

		mapInformationSetting.SetActive(false);

		path = "";
		hasModification = false;
	}
}