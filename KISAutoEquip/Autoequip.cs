using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KIS;
using System.Reflection;

namespace KISAutoEquipt
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KISAutoEquipt : MonoBehaviour
    {
        void Start()
        {
            // not working in that State
            //  GameEvents.onCrewOnEva.Add(EquipPart);
            if (IsModInstalled("KIS"))
            {
                GameEvents.onVesselChange.Add(EquipPart);
            }
        }
        void OnDestroy()
        {
            // not working in that State
            // GameEvents.onCrewOnEva.Remove(EquipPart);
            if (IsModInstalled("KIS"))
            {
                GameEvents.onVesselChange.Remove(EquipPart);
            }
        }


        /// <summary>
        /// Scans the local KIS inventory of a kerbal and attaches the needed part.
        /// </summary>
        /// <param name="kerbal"></param>
        public void EquipPart(Vessel kerbal)
        {
            if (!IsModInstalled("KIS")) { Debug.Log("KIS not loaded"); return; }
            if (kerbal.isEVA)
            {
                var kis = FlightGlobals.ActiveVessel.rootPart.FindModuleImplementing<ModuleKISInventory>();
                if (kis != null)
                {
                    var neededItem = kis.items.FirstOrDefault(x => x.Value.availablePart.name == "kOSPad3").Value;
                    if (neededItem != null && !neededItem.equipped)
                    {
                        neededItem.Equip();
                    }
                    else
                    {
                        Debug.Log("No needed item");
                    }
                }
                else { Debug.LogWarning("No KIS found"); }
            }
        }

        internal static bool IsModInstalled(string assemblyName)
        {
            Assembly assembly = (from a in AssemblyLoader.loadedAssemblies
                                 where a.name.ToLower().Equals(assemblyName.ToLower())
                                 select a).FirstOrDefault().assembly;
            return assembly != null;
        }

    }

}
