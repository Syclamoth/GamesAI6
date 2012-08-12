using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public struct StateMenuItem{
	public string name;
	public Type curType;
	
	public Component AddComponentToGameObject(GameObject obj) {
		return obj.AddComponent(curType);
	}
	
	public StateMenuItem(string newName, Type newType) {
		name = newName;
		curType = newType;
	}
}

public struct Line {
	public Vector2 start;
	public Vector2 end;
	public Color drawColour;
	public Line(Vector2 newStart, Vector2 newEnd, Color colour) {
		start = newStart;
		end = newEnd;
		drawColour = colour;
	}
}

public class StateMachineEditor : EditorWindow {
	
	////--- REGISTER NEW STATES HERE!
	
	StateMenuItem[] menuItems = new StateMenuItem[] {
		new StateMenuItem("Fuzzy Transition", typeof(FuzzyTransition))
	};
	
	////---- END
	
	Dictionary<State, Rect> stateInspectorPositions = new Dictionary<State, Rect>();
	
	List<Line> linesToDraw = new List<Line>();
	List<State> statesToRemove = new List<State>();
	
	
	Machine stMachine;
	
	Vector2 currentScrollPos;
	Vector2 curMousePos;
	State GetStateFromPosition(Vector2 guiPos) {
		foreach(KeyValuePair<State, Rect> pair in stateInspectorPositions) {
			if(pair.Value.Contains(guiPos)) {
				return pair.Key;
			}
		}
		return null;
	}
	
	[MenuItem ("Window/FSM Logic Editor")]
	static void Init () {
		StateMachineEditor window = (StateMachineEditor)EditorWindow.GetWindow(typeof(StateMachineEditor));
		window.wantsMouseMove = false;
	}
	
	void AddStateToMachine(object data) {
		StateMenuItem controlItem = (StateMenuItem)data;
		State newState = (State)controlItem.AddComponentToGameObject(stMachine.gameObject);
		newState.inspectorCorner = curMousePos;
		stMachine.controlledStates.Add (newState);
		
	}
	void AddSubMachine() {
		GameObject newObj = new GameObject("Machine");
		State newState = newObj.AddComponent<Machine>();
		newState.inspectorCorner = curMousePos;
		stMachine.controlledStates.Add(newState);
	}
	
	void Update() {
		linesToDraw = new List<Line>();
		UnityEngine.Object[] actives = Selection.GetFiltered(typeof(Machine), SelectionMode.Unfiltered);
		if(actives.Length > 0) {
			stMachine = (Machine)actives[0];
		}
		foreach(State curState in statesToRemove) {
			stMachine.controlledStates.Remove(curState);
			stateInspectorPositions.Remove(curState);
			DestroyImmediate (curState);
		}
		statesToRemove = new List<State>();
	}
	
	void DrawContextMenu() {
		GenericMenu menu = new GenericMenu();
		foreach(StateMenuItem it in menuItems) {
			menu.AddItem(new GUIContent("Add State/" + it.name), false, AddStateToMachine, it);
		}
		menu.AddItem(new GUIContent("Add State/Sub Machine"), false, AddSubMachine);
		menu.ShowAsContext ();
	}
	
	public void OnGUI() {
		if(stMachine == null) {
			GUILayout.Label("Please select a valid state machine to edit!");
			return;
		}
		curMousePos = Event.current.mousePosition;
		foreach(State state in stMachine.controlledStates) {
			if(!stateInspectorPositions.ContainsKey(state)) {
				stateInspectorPositions.Add(state, new Rect(state.inspectorCorner.x, state.inspectorCorner.y, 50, 50));
			}
		}
		//Debug.Log(GUI.GetNameOfFocusedControl());
		
		State[] tempKeys = new State[stateInspectorPositions.Count];
		stateInspectorPositions.Keys.CopyTo(tempKeys, 0);
		Rect[] tempValues = new Rect[stateInspectorPositions.Count];
		stateInspectorPositions.Values.CopyTo(tempValues, 0);
		currentScrollPos = GUILayout.BeginScrollView(currentScrollPos);
		BeginWindows();
		for(int i = 0; i < tempKeys.Length; ++i) {
			stateInspectorPositions[tempKeys[i]] = GUILayout.Window(i, tempValues[i], DrawStateInspector, tempKeys[i].ToString());
		}
		
		EndWindows();
		
		foreach(Line curLine in linesToDraw) {
			Handles.color = curLine.drawColour;
			Handles.DrawLine (curLine.start, curLine.end);
			Handles.ArrowCap(0, (Vector3)curLine.start - new Vector3(0, 0, 10), Quaternion.LookRotation(curLine.end - curLine.start), 70);
		}
		
		
		//GUILayout.Label ("BLUH BLAH");
		
		if(Event.current.type == EventType.ContextClick) {
			DrawContextMenu();
			Event.current.Use();
		}
		GUILayout.EndScrollView();
	}
	
	void DeleteState(object objToDelete)
	{
		statesToRemove.Add((State)objToDelete);
	}
	
	void DrawStateInspector(int windowID) {
		State[] tempKeys = new State[stateInspectorPositions.Count];
		stateInspectorPositions.Keys.CopyTo(tempKeys, 0);
		State curState = tempKeys[windowID];
		
		if(Event.current.type == EventType.MouseDown && Event.current.button == 1) {
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Delete"), false, DeleteState, curState);
			menu.ShowAsContext ();
			Event.current.Use();
		}
		
		foreach(LinkedStateReference stRef in curState.GetStateTransitions()) {
			GUILayout.BeginHorizontal();
			GUILayout.Label(stRef.GetLabel());
			int linkID = GUIUtility.GetControlID (windowID, FocusType.Native);
			Rect circleRect = GUILayoutUtility.GetRect(18, 18);
			
			EditorGUIUtility.AddCursorRect(circleRect, MouseCursor.Link);
			switch (Event.current.GetTypeForControl(linkID))
	        {
	            case EventType.MouseDown:
				if (circleRect.Contains (Event.current.mousePosition) && GUIUtility.hotControl == 0)
				{
					GUIUtility.hotControl = linkID;
                    Event.current.Use ();
				}
				break;
				case EventType.MouseUp:
	            if (GUIUtility.hotControl == linkID)
	            {
					// Check for having dragged over a different state here!
					bool newStatePicked = false;
					foreach(State curst in tempKeys) {
						if(stateInspectorPositions[curst].Contains(Event.current.mousePosition + curState.inspectorCorner)) {
							if(curst == curState) {
								continue;
							}
							stRef.SetState(curst);
							newStatePicked = true;
							break;
						}
					}
					if(!newStatePicked) {
						stRef.SetState(null);	
					}
					GUIUtility.hotControl = 0;
					Event.current.Use ();
	            }
	            break;
				case EventType.MouseDrag:
				if(GUIUtility.hotControl == linkID)
				{
					Event.current.Use ();
				}
				break;
		        case EventType.Repaint:
		            Handles.color = GUIUtility.hotControl == linkID ? Color.red : Color.blue;
					Vector2 circlePos = new Vector2(circleRect.xMax - 5, circleRect.yMin + (circleRect.height / 2));
					Handles.DrawSolidDisc(circlePos, Vector3.forward, 5);
					if(GUIUtility.hotControl == linkID) {
						linesToDraw.Add(new Line(circlePos + curState.inspectorCorner, Event.current.mousePosition + curState.inspectorCorner, Handles.color));
						//Vector2 outPos;	
						//stateInspectorPositions[curState].IntersectLine(circlePos + curState.inspectorCorner, Event.current.mousePosition + curState.inspectorCorner, out outPos);
					} else if (stRef.GetState() != null) {
						Vector2 outPos;
						if(!stateInspectorPositions.ContainsKey(stRef.GetState())) {
							stRef.SetState(null);
							break;
						}
						stateInspectorPositions[stRef.GetState()].IntersectLine(circlePos + curState.inspectorCorner, stateInspectorPositions[stRef.GetState()].center, out outPos);
						linesToDraw.Add(new Line(circlePos + curState.inspectorCorner, outPos, Handles.color));
					}
		        break;
	        }
			
			
			GUILayout.EndHorizontal();
		}
		
		GUILayout.Label(curState.ToString());
		GUI.DragWindow();
		curState.inspectorCorner = new Vector2(stateInspectorPositions[curState].x, stateInspectorPositions[curState].y);
	}
}
