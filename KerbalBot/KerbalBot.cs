using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;


namespace KerbalBot
{
    // [KSPAddon(KSPAddon.Startup.Flight, false)]
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    class KerbalBot : MonoBehaviour 
    {

        internal double maxec = 50;

        public void Awake ()
        {

            // Check for techtree here

            KerbalEVAUtility.AddPartModule("kOSProcessor");

            // check for Remotetech
            if (IsModInstalled("RemoteTech") ) {
                Debug.Log("KerbalBot: Enable Remotetech modules");
                KerbalEVAUtility.AddPartModule("ModuleRTAntennaPassive");
                KerbalEVAUtility.AddPartModule("ModuleRTDataTransmitter");
                KerbalEVAUtility.AddPartModule("ModuleSPUPassive");

            } else {
                Debug.Log("KerbalBot: Enable std antenna modules");
                KerbalEVAUtility.AddPartModule("ModuleDataTransmitter");
            }
            GameEvents.onCrewOnEva.Add(this.OnEvaStart);
            GameEvents.onCrewBoardVessel.Add(this.OnEvaEnd);
            AddEc("kerbalEVA");
            AddEc("kerbalEVAfemale");
        }


        public void OnEvaStart(GameEvents.FromToAction<Part, Part> vessel)
        {
            Debug.Log("KerbalBot: settings, kOS modules");
            //vessel.to.RequestResource("ElectricCharge", -maxec);
            kOS.Module.kOSProcessor myproc = null;
            vessel.to.GetComponentCached<kOS.Module.kOSProcessor>(ref myproc);
            myproc.bootFile = "/boot/eva";
            myproc.diskSpace = 5000;
            myproc.ECPerBytePerSecond = 0f;
            myproc.ECPerInstruction = 0f;


            if (IsModInstalled("RemoteTech"))
            {
                Debug.Log("KerbalBot: settings, Remotetech modules");
                RemoteTech.Modules.ModuleRTAntennaPassive antennapassive = null;
                vessel.to.GetComponentCached<RemoteTech.Modules.ModuleRTAntennaPassive>(ref antennapassive);
                antennapassive.OmniRange = (float)2000;
                antennapassive.RTPacketSize = 1;
                antennapassive.RTPacketInterval = 0.6f;
                antennapassive.RTPacketResourceCost = (float)0.0f;

                RemoteTech.Modules.ModuleRTDataTransmitter datatrans = null;
                vessel.to.GetComponentCached<RemoteTech.Modules.ModuleRTDataTransmitter>(ref datatrans);
                datatrans.PacketInterval = 0.6f;
                datatrans.PacketSize = 1;
                datatrans.PacketResourceCost = (float)0.0f;

            } else {
                Debug.Log("KerbalBot: settings, std modules");
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
     //     
     //      maybe recovering of unused EC.
     //     
        }


        internal void AddEc(string kerbal)
        {
            Debug.Log("KerbalBot: adding EC");
            Part part = PartLoader.getPartInfoByName(kerbal).partPrefab;
            PartResource resource = part.gameObject.AddComponent<PartResource>();
            resource.SetInfo(PartResourceLibrary.Instance.resourceDefinitions["ElectricCharge"]);
            resource.maxAmount = maxec;
            resource.amount = maxec;
            resource.flowState = true;
            resource.flowMode = PartResource.FlowMode.Both;
            Debug.Log("KerbalBot: EC added");
        }


        internal bool IsModInstalled(string assemblyName)
        {
            Assembly assembly = (from a in AssemblyLoader.loadedAssemblies
                                 where a.name.ToLower().Equals(assemblyName.ToLower())
                                 select a).FirstOrDefault().assembly;
            return assembly != null;
        }

    }
}
