﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor (typeof (Level))]
public class LevelEditor : Editor {

	int selectedBrush = 0;
	GameObject selectedBrushPrefab
	{
		get
		{
			if (selectedBrush < 1)
				return null;
			return (target as Level).brushPrefabs[selectedBrush-1];
		}
	}
	
	Vector2 lastMousePos = Vector2.zero;
	Level level;
	
	public void OnEnable () {
		level = target as Level;
		//SceneView.onSceneGUIDelegate += OnSceneGUICustom;
	}
	
	public void OnDisable () {
		//SceneView.onSceneGUIDelegate -= OnSceneGUICustom;
	}
	
	public override void OnInspectorGUI ()
	{
		// Brush toolbar
		string[] brushNames = new string[level.brushPrefabs.Count + 1];
		brushNames[0] = "Empty";
		for (int i=0; i<level.brushPrefabs.Count; i++)
			brushNames[i+1] = level.brushPrefabs[i].name;
		selectedBrush = GUILayout.Toolbar (selectedBrush, brushNames);
		
		DrawDefaultInspector ();
	}
	
	public void OnSceneGUI () {
		MySceneGUI ();
	}
	
	public void OnPreSceneGUI () {
		MySceneGUI ();
	}
	
	public void MySceneGUI () {
		Tools.current = Tool.None;
		
		Vector2 mousePos = GUIToGridPoint (Event.current.mousePosition);
		DrawCell (mousePos);
		
		int id = GUIUtility.GetControlID (FocusType.Passive);
		
		if (Event.current.type == EventType.MouseDown) {
			SetTile (mousePos, selectedBrushPrefab);
			EditorGUIUtility.hotControl = id;
		}
		if (Event.current.type == EventType.MouseUp) {
			if (EditorGUIUtility.hotControl == id)
				EditorGUIUtility.hotControl = 0;
		}
		if (Event.current.type == EventType.MouseDrag) {
			if (EditorGUIUtility.hotControl == id) {
				if (mousePos != lastMousePos) {
					lastMousePos = mousePos;
					SetTile (mousePos, selectedBrushPrefab);
				}
			}
		}
		
		if (Event.current.type == EventType.MouseMove)
			Event.current.Use ();
	}
	
	void SetTile (Vector2 pos, GameObject brushPrefab) {
		for (int i=level.tiles.Count-1; i>=0; i--) {
			GameObject go = level.tiles[i];
			if (go == null) {
				level.tiles.RemoveAt (i);
				continue;
			}
			if (go.transform.position == (Vector3)pos) {
				level.tiles.RemoveAt (i);
				DestroyImmediate (go);
				continue;
			}
		}	
		
		if (brushPrefab != null) {
			GameObject instance = PrefabUtility.InstantiatePrefab (brushPrefab) as GameObject;
			instance.transform.position = pos;
			instance.transform.parent = level.transform;
			level.tiles.Add (instance);
		}
		Event.current.Use ();
	}
	
	void DrawCell (Vector3 pos) {
		Handles.DrawLine (pos + new Vector3 (1,1,0) * 0.5f, pos + new Vector3 (-1,1,0) * 0.5f);
		Handles.DrawLine (pos + new Vector3 (-1,1,0) * 0.5f, pos + new Vector3 (-1,-1,0) * 0.5f);
		Handles.DrawLine (pos + new Vector3 (-1,-1,0) * 0.5f, pos + new Vector3 (1,-1,0) * 0.5f);
		Handles.DrawLine (pos + new Vector3 (1,-1,0) * 0.5f, pos + new Vector3 (1,1,0) * 0.5f);
	}
	
	Vector2 GUIToGridPoint (Vector2 guiPoint) {
		Plane plane = new Plane (-Vector3.forward, Vector3.zero);
		Ray ray = HandleUtility.GUIPointToWorldRay (guiPoint);
		float dist = 0;
		plane.Raycast (ray, out dist);
		Vector2 point = ray.GetPoint (dist);
		point.x = Mathf.Round (point.x);
		point.y = Mathf.Round (point.y);
		return point;
	}
}
