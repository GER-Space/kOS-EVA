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
            GameEvents.onCrewOnEva.Add(this.OnEvaStart);
            GameEvents.onCrewBoardVessel.Add(this.OnEvaEnd);
        }

        public void OnDestroy()
        {
            GameEvents.onCrewOnEva.Remove(this.OnEvaStart);
            GameEvents.onCrewBoardVessel.Remove(this.OnEvaEnd);
        }

        public void OnEvaStart(GameEvents.FromToAction<Part, Part> vessel)
        {
            KerbalEVA eva = null;
            vessel.to.GetComponentCached<KerbalEVA>(ref eva);

            var crew = eva.vessel.GetVesselCrew().FirstOrDefault();
            // check for trait here
            //if (crew.trait != "") { return; }

            Part evapart = eva.part;
            AddEc(evapart);

            // EC removed from origin vessel and added to new kerbaleva

            evapart.AddModule("kOSProcessor");

            Debug.Log("KerbalBot: settings, kOS modules");
            kOS.Module.kOSProcessor myproc = null;
            vessel.to.GetComponentCached<kOS.Module.kOSProcessor>(ref myproc);
            myproc.bootFile = "/boot/eva";
            myproc.diskSpace = 5000;
            myproc.ECPerBytePerSecond = 0f;
            myproc.ECPerInstruction = 0f;


            if (IsModInstalled("RemoteTech"))
            {
                Debug.Log("KerbalBot: settings, Remotetech modules");
                evapart.AddModule("ModuleRTAntennaPassive");
                evapart.AddModule("ModuleRTDataTransmitter");
                evapart.AddModule("ModuleSPUPassive");

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
                evapart.AddModule("ModuleDataTransmitter");
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
     //       recovering of unused EC.
     //     
        }



        internal void AddEc(Part topart)
        {
            Debug.Log("KerbalBot: adding EC");

            PartResource resource = topart.gameObject.AddComponent<PartResource>();
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
