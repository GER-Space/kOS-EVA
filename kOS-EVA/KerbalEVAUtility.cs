using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Utility methods for manipulating kerbals
/// </summary>
public static class KerbalEVAUtility
{
	/// <summary>
	/// Adds a part module to the kerbalEVA and kerbalEVAfemale parts
	/// </summary>
	public static void AddPartModule(string moduleName)
	{
		AddModule ("kerbalEVA", moduleName);
		AddModule ("kerbalEVAfemale", moduleName);
	}
	private static void AddModule(string partName, string moduleName)
	{
		Debug.Log ("Adding " + moduleName + " to part " + partName);

		foreach (var aPart in PartLoader.LoadedPartsList)
		{
			if (aPart.name != partName)
				continue;

			try
			{
				aPart.partPrefab.AddModule (moduleName);
			}
			catch {}
			Debug.Log ("Added " + moduleName + " to part " + partName + " successfully");
		}
	}

	public static List<KFSMState> GetEVAStates(KerbalEVA eva)
	{
		var fsm = eva.fsm;

		var type = fsm.GetType();
		var statesF = type.GetField ("States", BindingFlags.NonPublic | BindingFlags.Instance);
		List<KFSMState> states = (List<KFSMState>)statesF.GetValue (fsm);
		return states;
	}
	public static List<KFSMEvent> GetEVAEvents(KerbalEVA eva, List<KFSMState> states)
	{
		List<KFSMEvent> events = new List<KFSMEvent> ();
		foreach (var state in states)
		{
			events.AddRange ( state.StateEvents.Where(e => !events.Contains(e)) );
		}
		return events;
	}
	public static void RunEvent(this KerbalEVA eva, string name)
	{
        var kfsme = eva.fsm.CurrentState.StateEvents.Where(f => f.name.ToLower() == name.ToLower()).First();
		if (kfsme != null)
			eva.fsm.RunEvent (kfsme);
		else
			Debug.LogError ("[assembly: " + Assembly.GetExecutingAssembly ().GetName().Name + "]:" + "Event " + name + " not found");
		
	}
	public static void RunEvent(this KerbalFSM fsm, string name)
	{
		foreach (var kfsme in fsm.CurrentState.StateEvents)
		{
			if (kfsme.name == name)
				fsm.RunEvent (kfsme);
			else
				Debug.LogError ("[assembly: " + Assembly.GetExecutingAssembly ().GetName().Name + "]:" + "Event " + name + " not found");
		}
	}
}
