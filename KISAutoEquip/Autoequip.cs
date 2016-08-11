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
            GameEvents.onVesselChange.Add(EquipPart);
        }
        void OnDestroy()
        {
            // not working in that State
            // GameEvents.onCrewOnEva.Remove(EquipPart);
            GameEvents.onVesselChange.Remove(EquipPart);
        }

        public void EquipPart(Vessel arg)
        {
            // for onCrewOnEva
            // arg.from = ship kerbal is getting out of
            // arg.to = ship being switched to (the eva kerbal)

            var kerbal = arg;

            //            if (kerbal.isEVA && IsModInstalled("KIS")) // should be true, better safe though
            if (!IsModInstalled("KIS")) { Debug.LogWarning("KIS not loaded"); return; }
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
                            Debug.LogWarning("No needed item");
                        }
                    } else { Debug.LogWarning("No KIS found"); }
                }

       //     if (kerbal.FindPartModulesImplementing<YourPartModule>().Count == 0)
       //            kerbal.rootPart.AddModule("YourPartModule");
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
