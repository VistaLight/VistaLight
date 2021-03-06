﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CreateDockTool : IMapEditorTool {
	private IndustryType type;
	private MapController mapController;

	public CreateDockTool(MapController mapController, IndustryType type) {
		this.type = type;
		this.mapController = mapController;
	}

	public IndustryType Type {
		get { return type; }
		set { type = value; }
	}

	public void Destory() {
	}

	public void RespondMouseLeftClick() {
		RaycastHit2D ray = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (ray.collider != null && ray.collider.tag == "Node") {
			GameObject dockGO = mapController.AddDock(ray.collider.gameObject, type);
			GameObject.Find("MapEditorController").GetComponent<MapEditorController>().SelectOne(dockGO.GetComponent<DockVO>());
		}
	}

	public void RespondMouseLeftUp() {
	}

	public void RespondMouseMove(float x, float y) {
	}

	public void RespondMouseRightClick() {
	}

	public bool CanDestroy() {
		return true;
	}
}

