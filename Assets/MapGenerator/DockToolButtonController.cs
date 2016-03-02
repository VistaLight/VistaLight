﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class DockToolButtonController : ToolButtonController {

	public MapController mapController;
	public ToolSelector toolSelector;

	public override void SelectTool(string setting) {
		CreateDockTool tool = null;
        switch (setting) {
		case "Petro":
			tool = new CreateDockTool(mapController, IndustryType.Petro);
			break;			

		case "Breakbulk":
			tool = new CreateDockTool(mapController, IndustryType.BreakBulk);
			break;

		case "Bulk":
			tool = new CreateDockTool(mapController, IndustryType.Bulk);
			break;

		case "Port":
			tool = new CreateDockTool(mapController, IndustryType.Port);
			break;

		default:
			return;
		}
		toolSelector.SelectTool(tool);
		gameObject.GetComponent<Button>().image.color = Color.white;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
