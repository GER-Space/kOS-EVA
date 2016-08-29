using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using kOS.Suffixed;

using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using System.Reflection;
using RemoteTech;


namespace KerbalBot
{
    // [KSPAddon(KSPAddon.Startup.Flight, false)]
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    class KerbalBot : MonoBehaviour 
    {

        internal float maxec = 50;

        public void Awake ()
        {

            // Check for techtree here

            KerbalEVAUtility.AddPartModule("kOSProcessor");
            AddEc("kerbalEVA");
            AddEc("kerbalEVAfemale");


            // check for Remotetech
            if (IsModInstalled("remotetech") ) {
                KerbalEVAUtility.AddPartModule("ModuleRTAntennaPassive");
                KerbalEVAUtility.AddPartModule("ModuleSPUPassive");

            } else
            {
                KerbalEVAUtility.AddPartModule("ModuleDataTransmitter");
            }
            GameEvents.onCrewOnEva.Add(this.OnEvaStart);
            GameEvents.onCrewBoardVessel.Add(this.OnEvaEnd);
        }


        public void OnEvaStart(GameEvents.FromToAction<Part, Part> vessel)
        {
            //vessel.to.RequestResource("ElectricCharge", -maxec);
            kOS.Module.kOSProcessor myproc = null;
            vessel.to.GetComponentCached<kOS.Module.kOSProcessor>(ref myproc);
            myproc.bootFile = "/boot/eva";
            myproc.diskSpace = 5000;
            myproc.ECPerBytePerSecond = 0f;
            myproc.ECPerInstruction = 0f;


            if (IsModInstalled("remotetech"))
            {
                RemoteTech.Modules.ModuleRTAntennaPassive antennapassive = null;
                vessel.to.GetComponentCached<RemoteTech.Modules.ModuleRTAntennaPassive>(ref antennapassive);
                antennapassive.OmniRange = (float)2000;
                antennapassive.RTPacketSize = 1;
                antennapassive.RTPacketInterval = 0.6f;
                antennapassive.RTPacketResourceCost = (float)0.0f;
            }
            else
            {
                ModuleDataTransmitter mytrans = null;
                vessel.to.parent.GetComponentCached<ModuleDataTransmitter>(ref mytrans);
                mytrans.packetInterval = 0.6f;
                mytrans.packetSize = 1;
                mytrans.packetResourceCost = 0;
                mytrans.requiredResource = "ElectricCharge";
            }
        }

        public void OnEvaEnd(GameEvents.FromToAction<Part, Part> vessel)
        {
     //       double FuelLeft = data.from.RequestResource("EVA Propellant", EVATankFuelMax);
     //       data.to.RequestResource("MonoPropellant", -FuelLeft);
     //       ScreenMessages.PostScreenMessage("Returned " + Math.Round(FuelLeft, 2).ToString() + " units of MonoPropellant to ship.", ScreenMessageLife, ScreenMessageStyle.UPPER_CENTER);
        }




        internal void AddEc(string kerbal)
        {
            Part part = PartLoader.getPartInfoByName(kerbal).partPrefab;
            PartResource RES_EC = part.gameObject.AddComponent<PartResource>();
            RES_EC.SetInfo(PartResourceLibrary.Instance.resourceDefinitions["ElectricCharge"]);
            RES_EC.amount = maxec;
            RES_EC.maxAmount = maxec;
            part.Resources.list.Add(RES_EC);
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
