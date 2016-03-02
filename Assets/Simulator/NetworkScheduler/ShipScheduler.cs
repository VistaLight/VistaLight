﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShipScheduler {

	private ShipController ship;
	public ShipController Ship { 
		set { ship = value; }
	}

	private List<Path> paths = new List<Path>();
	private List<ShipSchedule> schedules = new List<ShipSchedule>();
	

	public void Schedule() {
		FindAllPaths();

		if (paths.Count == 0) {
			ship.status = ShipStatus.NoRoute;
			return;
		}

		PathsToSchedules();
		ResolveConflict();
		ApplyBestSchedule();
	}

	private void ApplyBestSchedule() {
		DateTime earliestEta = DateTime.MaxValue;
		ShipSchedule bestSchedule = null;
		foreach(ShipSchedule schedule in schedules) {
			if (schedule.ETA.CompareTo(earliestEta) < 0) {
				earliestEta = schedule.ETA;
				bestSchedule = schedule;
			}
		}

		ship.schedule = bestSchedule;
		ReservationManager reservationManager = GameObject.Find("MapUtil").GetComponent<ReservationManager>();
		reservationManager.Reserve(bestSchedule);

		ship.status = ShipStatus.Waiting;
	}

	private void ResolveConflict() {
		ReservationManager reservationManager = GameObject.Find("MapUtil").GetComponent<ReservationManager>();
		foreach (ShipSchedule schedule in schedules) {
			reservationManager.PostponeScheduleToResolveConflict(schedule);
		}
	}

	private void PathsToSchedules() {
		foreach (Path path in paths) {
			schedules.Add(PathToSchedule(path));
		}
	}

	private ShipSchedule PathToSchedule(Path path) {
		double shipSpeed = 3.0;
		ShipSchedule schedule = new ShipSchedule();
		MapUtil mapUtil = GameObject.Find("MapUtil").GetComponent<MapUtil>();

		DateTime currentTime = GameObject.Find("Timer").GetComponent<Timer>().VirtualTime;
		Vector2 currentPosition = new Vector2((float)ship.ship.X, (float)ship.ship.Y);
		bool unloadingScheduled = false;

		Node previousNode = null;
		foreach (Node node in path.path) {
			ShipMoveTask moveTask = new ShipMoveTask();
			moveTask.Position = new Vector2((float)node.X, (float)node.Y);

			double distance = Math.Pow(Math.Pow(node.X - currentPosition.x, 2) + Math.Pow(node.Y - currentPosition.y, 2), 0.5);
			TimeSpan duration = new TimeSpan(0, 0, (int)Math.Round(distance/shipSpeed));
			moveTask.StartTime = currentTime;
			moveTask.EndTime = currentTime.Add(duration);

			if (previousNode != null) {
				moveTask.connection = mapUtil.GetConnection(previousNode, node);
            }
			previousNode = node;

			currentTime = currentTime.Add(duration);
			currentPosition = new Vector2((float)node.X, (float)node.Y);

			schedule.AppendTask(moveTask);

			// Unloading task
			double unloadingSpeed = 0.2;
			Dock dock = mapUtil.GetDockByNode(node);
            if (!unloadingScheduled && dock != null) {
				TimeSpan unloadingDuration = new TimeSpan(0, 0, (int)Math.Round(ship.ship.cargo / unloadingSpeed));
				UnloadingTask unloadingTask = new UnloadingTask();
				unloadingTask.Position = currentPosition;
				unloadingTask.StartTime = currentTime;
				unloadingTask.EndTime = currentTime.Add(unloadingDuration);
				unloadingTask.dock = dock;
				currentTime = currentTime.Add(unloadingDuration);
				schedule.AppendTask(unloadingTask);
				unloadingScheduled = true;
			}

			// Task
			List<Node> exits = mapUtil.ExitNodes();
			if (exits.Contains(node)) {
				VanishTask vanishTask = new VanishTask();
				vanishTask.StartTime = currentTime;
				vanishTask.EndTime = currentTime;
				schedule.AppendTask(vanishTask);
			}
		}
		return schedule;
	}

	private void FindAllPaths() {
		if (ship.ship.cargo == 0) {
			FindAllPathsToLeaveMap();
        } else {
			FindAllPathsToUnloadAndLeaveMap();
		}

	}

	private void FindAllPathsToLeaveMap() {
		MapUtil mapUtil = GameObject.Find("MapUtil").GetComponent<MapUtil>();

		Node startNode = mapUtil.FindNearestNode(ship.ship.X, ship.ship.Y);

		List<Node> exitNodes = mapUtil.ExitNodes();

		foreach (Node exitNode in exitNodes) {
			List<Path> pathsToExit = mapUtil.FindPath(startNode, exitNode);
			paths.AddRange(pathsToExit);
		}
	}

	private void FindAllPathsToUnloadAndLeaveMap() {
		MapUtil mapUtil = GameObject.Find("MapUtil").GetComponent<MapUtil>();

		Node startNode = mapUtil.FindNearestNode(ship.ship.X, ship.ship.Y);
		List<Node> exitNodes = mapUtil.ExitNodes();
		List<Dock> docksToUnload = mapUtil.GetAllDocksOfType(ship.ship.Industry);
		if (docksToUnload.Count == 0) {
			Debug.LogError("No dock found!");
		}

		foreach (Dock dock in docksToUnload) {
			List<Path> pathsToDock;
			List<Path> pathsToExit;

			pathsToDock = mapUtil.FindPath(startNode, dock.node);
			if (pathsToDock.Count == 0) {
				return;
			}
			foreach (Node exitNode in exitNodes) {
				pathsToExit = mapUtil.FindPath(dock.node, exitNode);
				if (pathsToExit.Count == 0) {
					foreach (Path pathToDock in pathsToDock) {
						paths.Add(pathToDock);	
					}
				}
				foreach (Path pathToDock in pathsToDock) {
					foreach (Path pathToExit in pathsToExit) {
						paths.Add(pathToDock.ConcatenatePath(pathToExit));
					}
				}
			}
		}
	}

}
