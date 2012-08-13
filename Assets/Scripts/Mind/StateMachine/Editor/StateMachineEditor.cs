using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public enum Direction {
	Forwards,
	Backwards,
	None
}

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
	public Direction myDir;
	public Line(Vector2 newStart, Vector2 newEnd, Color colour) {
		start = newStart;
		end = newEnd;
		drawColour = colour;
		myDir = Direction.Forwards;
	}
	public Line(Vector2 newStart, Vector2 newEnd, Color colour, Direction newDir) {
		start = newStart;
		end = newEnd;
		drawColour = colour;
		myDir = newDir;
	}
}

public class StateMachineEditor : EditorWindow {
	
	////--- REGISTER NEW STATES HERE!
	
	StateMenuItem[] menuItems = new StateMenuItem[] {
		new StateMenuItem("Fuzzy Transition", typeof(FuzzyTransition)),
		new StateMenuItem("Idle", typeof(Idle)),
		new StateMenuItem("Sheep Idle", typeof(SheepIdle))
	};
	
	////---- END
	
	Dictionary<State, Rect> stateInspectorPositions = new Dictionary<State, Rect>();
	Dictionary<TriggerManager, Rect> triggerInspectorPositions = new Dictionary<TriggerManager, Rect>();
	
	List<Line> linesToDraw = new List<Line>();
	List<State> statesToRemove = new List<State>();
	List<TriggerManager> triggersToRemove = new List<TriggerManager>();
	
	Vector2 globalViewOffset = Vector2.one * 10;
	
	
	Machine stMachine;
	
	Vector2 currentScrollPos;
	Vector2 curMousePos;
	
	bool pickingStartState = false;
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
		newState.inspectorCorner = curMousePos - globalViewOffset;
		stMachine.controlledStates.Add (newState);
		
	}
	void AddSubMachine() {
		GameObject newObj = new GameObject("Machine");
		State newState = newObj.AddComponent<Machine>();
		newState.inspectorCorner = curMousePos - globalViewOffset;
		stMachine.controlledStates.Add(newState);
		newObj.transform.parent = stMachine.transform;
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
			if(curState.GetType() == typeof(Machine))
			{
				DestroyImmediate(curState.gameObject);
			} else {
				DestroyImmediate (curState);
			}
		}
		foreach(TriggerManager trig in triggersToRemove) {
			stMachine.controlledTriggers.Remove(trig);
			triggerInspectorPositions.Remove(trig);
		}
		bool shouldRebuildStates = false;
		foreach(State state in stateInspectorPositions.Keys) {
			if(!stMachine.controlledStates.Contains(state)) {
				shouldRebuildStates = true;
			}
		}
		if(shouldRebuildStates) {
			stateInspectorPositions = new Dictionary<State, Rect>();
			triggerInspectorPositions = new Dictionary<TriggerManager, Rect>();
		}
		statesToRemove = new List<State>();
		
		if(stMachine.startingState == null && stMachine.controlledStates.Count > 0) {
			stMachine.startingState = stMachine.controlledStates[0];
		}
	}
	
	void AddTrigger() {
		TriggerManager newTrig = new TriggerManager();
		stMachine.controlledTriggers.Add(newTrig);
		newTrig.inspectorCorner = curMousePos - globalViewOffset;
	}
	
	void DrawContextMenu() {
		GenericMenu menu = new GenericMenu();
		foreach(StateMenuItem it in menuItems) {
			menu.AddItem(new GUIContent("Add State/" + it.name), false, AddStateToMachine, it);
		}
		menu.AddItem(new GUIContent("Add State/Sub Machine"), false, AddSubMachine);
		menu.AddItem(new GUIContent("Add Trigger"), false, AddTrigger);
		menu.ShowAsContext ();
	}
	
	public void OnGUI() {
		if(stMachine == null) {
			GUILayout.Label("Please select a valid state machine to edit!");
			return;
		}
		if(stMachine.transform.parent.GetComponent<Machine>() != null) {
			if(GUILayout.Button ("Pop")) {
				Selection.activeGameObject = stMachine.transform.parent.gameObject;
				stMachine = null;
				stateInspectorPositions = new Dictionary<State, Rect>();
				triggerInspectorPositions = new Dictionary<TriggerManager, Rect>();
				return;
			}
		}
		pickingStartState = GUILayout.Toggle(pickingStartState, "Pick Initial State", GUI.skin.button);
		if(pickingStartState) {
			if(Event.current.type == EventType.MouseDown) {
				foreach(State state in stateInspectorPositions.Keys) {
					if(stateInspectorPositions[state].Contains(Event.current.mousePosition - globalViewOffset)) {
						Debug.Log (state);
						stMachine.startingState = state;
						break;
					}
				}
				pickingStartState = false;
				Event.current.Use();
			}
		}
		if(stMachine.controlledStates.Count == 0) {
			GUILayout.Label("You must add at least one state to this machine!");
		}
		curMousePos = Event.current.mousePosition;
		foreach(State state in stMachine.controlledStates) {
			if(!stateInspectorPositions.ContainsKey(state)) {
				stateInspectorPositions.Add(state, new Rect(state.inspectorCorner.x, state.inspectorCorner.y, 50, 50));
			}
		}
		foreach(TriggerManager manager in stMachine.controlledTriggers) {
			if(!triggerInspectorPositions.ContainsKey(manager)) {
				triggerInspectorPositions.Add(manager, new Rect(manager.inspectorCorner.x, manager.inspectorCorner.y, 50, 50));
			}
		}
		if(Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == 0) {
			globalViewOffset += Event.current.delta;
			EditorGUIUtility.AddCursorRect(new Rect(Event.current.mousePosition.x - 10, Event.current.mousePosition.y - 10, 20, 20), MouseCursor.MoveArrow);
			Event.current.Use();
		}
		
		State[] tempStates = new State[stateInspectorPositions.Count];
		stateInspectorPositions.Keys.CopyTo(tempStates, 0);
		Rect[] tempRects = new Rect[stateInspectorPositions.Count];
		stateInspectorPositions.Values.CopyTo(tempRects, 0);
		currentScrollPos = GUILayout.BeginScrollView(currentScrollPos);
		BeginWindows();
		Vector2 bottomCorner = Vector2.zero;
		for(int i = 0; i < tempStates.Length; ++i) {
			stateInspectorPositions[tempStates[i]] = GUILayout.Window(i, tempRects[i].OffsetBy(globalViewOffset), DrawStateInspector, tempStates[i].GetNiceName()).OffsetBy(-globalViewOffset);
			bottomCorner.x = Mathf.Max(bottomCorner.x, tempRects[i].OffsetBy(globalViewOffset).x);
			bottomCorner.y = Mathf.Max(bottomCorner.y, tempRects[i].OffsetBy(globalViewOffset).y);
		}
		TriggerManager[] tempTriggers = new TriggerManager[triggerInspectorPositions.Count];
		triggerInspectorPositions.Keys.CopyTo(tempTriggers, 0);
		tempRects = new Rect[triggerInspectorPositions.Count];
		triggerInspectorPositions.Values.CopyTo(tempRects, 0);
		for(int i = 0; i < tempTriggers.Length; ++i) {
			triggerInspectorPositions[tempTriggers[i]] = GUILayout.Window(i + tempStates.Length, tempRects[i].OffsetBy(globalViewOffset), DrawTriggerInspector, "Trigger").OffsetBy(-globalViewOffset);
			bottomCorner.x = Mathf.Max(bottomCorner.x, tempRects[i].OffsetBy(globalViewOffset).x);
			bottomCorner.y = Mathf.Max(bottomCorner.y, tempRects[i].OffsetBy(globalViewOffset).y);
		}
		
		GUILayoutUtility.GetRect(bottomCorner.x, bottomCorner.y);
		EndWindows();
		//GUI.Box(GUILayoutUtility.GetLastRect(), "BABABHAHHA");
		foreach(Line curLine in linesToDraw) {
			Handles.color = curLine.drawColour;
			Handles.DrawLine (curLine.start + globalViewOffset, curLine.end + globalViewOffset);
			switch(curLine.myDir)
			{
			case Direction.Forwards:
				Handles.ArrowCap(0, (Vector3)(curLine.start + globalViewOffset) - new Vector3(0, 0, 10), Quaternion.LookRotation(curLine.end - curLine.start), 70);
				break;
			case Direction.Backwards:
				Handles.ArrowCap(0, (Vector3)(curLine.end + globalViewOffset) - new Vector3(0, 0, 10), Quaternion.LookRotation(curLine.start - curLine.end), 70);
				break;
			}
			
		}
		if(stMachine.startingState != null && stateInspectorPositions.ContainsKey(stMachine.startingState)) {
			Handles.color = Color.white;
			Handles.DrawSolidRectangleWithOutline(stateInspectorPositions[stMachine.startingState].OffsetBy(globalViewOffset).GetCorners(), Color.clear, Color.green);
		}
		
		//foreach(
		//GUILayout.Label ("BLUH BLAH");
		
		if(Event.current.type == EventType.ContextClick) {
			DrawContextMenu();
			Event.current.Use();
		}
		//GUI.matrix = Matrix4x4.identity;
		GUILayout.EndScrollView();
	}
	
	void DeleteState(object objToDelete)
	{
		statesToRemove.Add((State)objToDelete);
	}
	
	void DeleteTrigger(object objToDelete)
	{
		triggersToRemove.Add((TriggerManager)objToDelete);
	}
	
	void DrawTriggerInspector(int windowID) {
		TriggerManager[] tempKeys = new TriggerManager[triggerInspectorPositions.Count];
		triggerInspectorPositions.Keys.CopyTo(tempKeys, 0);
		TriggerManager curTrigger = tempKeys[windowID - stateInspectorPositions.Count];
		
		if(Event.current.type == EventType.MouseDown && Event.current.button == 1) {
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Delete"), false, DeleteTrigger, curTrigger);
			menu.ShowAsContext ();
			Event.current.Use();
		}
		
		GUILayout.Label("From --> Obs --> To");
		GUILayout.BeginHorizontal();
		curTrigger.owner = LineStateSelector(curTrigger.owner, Color.cyan, windowID, curTrigger.inspectorCorner, null, Direction.Backwards);
		GUILayout.FlexibleSpace();
		curTrigger.watched = LineStateSelector((State)curTrigger.watched, Color.green, windowID, curTrigger.inspectorCorner, null, Direction.None);
		GUILayout.FlexibleSpace();
		curTrigger.target = LineStateSelector(curTrigger.target, Color.blue, windowID, curTrigger.inspectorCorner, null, Direction.Forwards);
		GUILayout.EndHorizontal();
		if(curTrigger.watched != null && curTrigger.watched.GetExposedVariables().Length > 0)
		{
			curTrigger.observedIndex = curTrigger.watched.DrawObservableSelector(curTrigger.observedIndex);
			GUI.contentColor = Color.black;
			EditorGUIUtility.LookLikeControls();
			
			curTrigger.mode = (TriggerMode)EditorGUILayout.EnumPopup(curTrigger.mode);
			switch(curTrigger.obsType) {
			case ObservedType.integer:
				curTrigger.intTarget = EditorGUILayout.IntField(curTrigger.intTarget, GUILayout.MaxWidth(triggerInspectorPositions[curTrigger].width));
				break;
			case ObservedType.floatingPoint:
				curTrigger.floatTarget = EditorGUILayout.FloatField(curTrigger.floatTarget, GUILayout.MaxWidth(triggerInspectorPositions[curTrigger].width));
				break;
			case ObservedType.boolean:
				curTrigger.boolTarget = EditorGUILayout.Toggle(curTrigger.boolTarget, GUILayout.MaxWidth(triggerInspectorPositions[curTrigger].width));
				break;
			}
			GUI.contentColor = Color.white;
			EditorGUIUtility.LookLikeInspector();
		}
		if(curTrigger.watched == null) {
			GUILayout.Label("Observing Memory");
			EditorGUIUtility.LookLikeControls();
			curTrigger.memoryKey = EditorGUILayout.TextField(curTrigger.memoryKey);
			curTrigger.mode = (TriggerMode)EditorGUILayout.EnumPopup(curTrigger.mode);
			curTrigger.floatTarget = EditorGUILayout.FloatField(curTrigger.floatTarget, GUILayout.MaxWidth(triggerInspectorPositions[curTrigger].width));
			EditorGUIUtility.LookLikeInspector();
		}
		GUI.DragWindow();
		curTrigger.inspectorCorner = new Vector2(triggerInspectorPositions[curTrigger].x, triggerInspectorPositions[curTrigger].y);
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
			
			stRef.SetState(LineStateSelector (stRef.GetState(), Color.blue, windowID, curState.inspectorCorner, curState, Direction.Forwards));
			
			GUILayout.EndHorizontal();
		}
		
		if(curState.GetType() == typeof(Machine)) {
			if(GUILayout.Button("Push"))
			{
				Selection.activeGameObject = curState.gameObject;
				stMachine = (Machine)curState;
				stateInspectorPositions = new Dictionary<State, Rect>();
				triggerInspectorPositions = new Dictionary<TriggerManager, Rect>();
				return;
			}
		}
		
		curState.DrawInspector();
		
		GUI.DragWindow();
		curState.inspectorCorner = new Vector2(stateInspectorPositions[curState].x, stateInspectorPositions[curState].y);
	}
	
	State LineStateSelector(State currentlySelected, Color defaultColour, int idHint, Vector2 mouseOffset, State ignoreThis, Direction lineDirection, params GUILayoutOption[] options) {
		Rect circleRect = GUILayoutUtility.GetRect(18, 18, options);
		int linkID = GUIUtility.GetControlID (idHint, FocusType.Native);
		EditorGUIUtility.AddCursorRect(circleRect, MouseCursor.Link);
		State retV = currentlySelected;
		switch (Event.current.GetTypeForControl(linkID))
        {
            case EventType.MouseDown:
			if (circleRect.Contains (Event.current.mousePosition) && GUIUtility.hotControl == 0)
			{
				Undo.RegisterUndo(stMachine, "Modify state transition");
				GUIUtility.hotControl = linkID;
                Event.current.Use ();
			}
			break;
			case EventType.MouseUp:
            if (GUIUtility.hotControl == linkID)
            {
				// Check for having dragged over a different state here!
				retV = null;
				foreach(State curst in stateInspectorPositions.Keys) {
					if(stateInspectorPositions[curst].Contains(Event.current.mousePosition + mouseOffset)) {
						if(curst == ignoreThis) {
							continue;
						}
						//Event.current.Use();
						retV = curst;
						break;
					}
				}
				GUIUtility.hotControl = 0;
				Event.current.Use ();
				return retV;
            }
            break;
			case EventType.MouseDrag:
			if(GUIUtility.hotControl == linkID)
			{
				Event.current.Use ();
			}
			break;
	        case EventType.Repaint:
	            Handles.color = GUIUtility.hotControl == linkID ? Color.red : defaultColour;
				Vector2 circlePos = new Vector2(circleRect.xMax - 5, circleRect.yMin + (circleRect.height / 2));
				Handles.DrawSolidDisc(circlePos, Vector3.forward, 5);
				if(GUIUtility.hotControl == linkID) {
					linesToDraw.Add(new Line(circlePos + mouseOffset, Event.current.mousePosition + mouseOffset, Handles.color, lineDirection));
					//Vector2 outPos;	
					//stateInspectorPositions[curState].IntersectLine(circlePos + curState.inspectorCorner, Event.current.mousePosition + curState.inspectorCorner, out outPos);
				} else if (currentlySelected != null) {
					Vector2 outPos;
					if(!stateInspectorPositions.ContainsKey(currentlySelected)) {
						return retV;
					}
					stateInspectorPositions[currentlySelected].IntersectLine(circlePos + mouseOffset, stateInspectorPositions[currentlySelected].center, out outPos);
					linesToDraw.Add(new Line(circlePos + mouseOffset, outPos, Handles.color, lineDirection));
				}
	        break;
        }
		return retV;
	}
}
