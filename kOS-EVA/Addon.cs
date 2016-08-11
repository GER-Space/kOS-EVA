using System;
using System.Collections;
using System.Linq;
using System.Text;
using kOS;
using kOS.Safe;
using UnityEngine;
using kOS.Suffixed;

using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using EVAMove;


namespace kOS.AddOns.kOSEVA
{
    [kOSAddon("EVA")]
    [kOS.Safe.Utilities.KOSNomenclature("EVAAddon")]
    public class Addon : Suffixed.Addon
    {
        public Addon(SharedObjects shared) : base(shared)
        {
            InitializeSuffixes();
        }

        private void InitializeSuffixes()
        {
            
        //    AddSuffix("DOEVENT", new TwoArgsSuffix<Suffixed.Part.PartValue, StringValue>(DoEvent, ""));
            AddSuffix("LADDER_RELEASE", new NoArgsVoidSuffix(LadderRelease, "Release a grabbed ladder"));
            AddSuffix("LADDER_GRAB", new NoArgsVoidSuffix(LadderGrab, "Grab a nearby ladder"));
            AddSuffix("TURN_LEFT", new OneArgsSuffix<ScalarValue>(TurnLeft, "make the kerbal turn by <deg>"));
            AddSuffix("TURN_RIGHT", new OneArgsSuffix<ScalarValue>(TurnRight, "make the kerbal turn by <deg>"));
            AddSuffix("TURN_TO", new OneArgsSuffix<Vector>(TurnTo, "make the kerbal turn to <vector>"));
            AddSuffix("MOVE", new OneArgsSuffix<StringValue>(MoveKerbal, "make the kerbal move"));
            AddSuffix("BOARDPART", new OneArgsSuffix<Suffixed.Part.PartValue>(BoardPart, "Enters the Part"));
            AddSuffix("BOARD", new NoArgsVoidSuffix(DoBoard, "Boad a Nearby Vessel or Part"));
            AddSuffix("PLANTFLAG", new NoArgsVoidSuffix(DoPlantFlag, "Plants a Flag"));
            AddSuffix("RUNACTION", new OneArgsSuffix<StringValue>(DoRunEvent, "Runs a Event by its name"));
            AddSuffix("ACTIONLIST", new NoArgsSuffix<ListValue>(ListEvents, "List of all event names"));
            AddSuffix("ANIMATIONLIST", new NoArgsSuffix<ListValue>(ListAnimations, "List of all animation names"));
            AddSuffix("PLAYANIMATION", new OneArgsSuffix<StringValue>(PlayAnimation, "Runs a build-in animation by its internal name"));
            AddSuffix("LOADANIMATION", new OneArgsSuffix<StringValue>(LoadAnimation, "Runs a custom animation by its relative pathname"));
            AddSuffix("STOPANIMATION", new OneArgsSuffix<StringValue>(StopAnimation, "Stops the Animation"));
            AddSuffix("STOPALLANIMATIONS", new NoArgsVoidSuffix(StopAllAnimations, "Stops all Animations"));
            AddSuffix(new[] { "GOEVA", "EVA" }, new OneArgsSuffix<CrewMember>(GoEVA, "Compliments a Kerbal to the Outside"));
            

            // Set a default bootfilename, when no other has been set.
            if (shared.Vessel.isEVA && shared.KSPPart.GetComponentCached<Module.kOSProcessor>(ref _myprocessor).bootFile.ToLower() == "none" )
            {
                Module.kOSProcessor  myproc = null;
                shared.KSPPart.GetComponentCached<Module.kOSProcessor>(ref myproc);
                myproc.bootFile = "/boot/eva";
            }
            if (shared.Vessel.isEVA)
            {
             //   Debug.Log("EVA Initialisierung abgeschlossen0");
                this.kerbaleva = shared.Vessel.GetComponentCached<KerbalEVA>(ref kerbaleva);
            }


        }
        internal Module.kOSProcessor _myprocessor = null;
        public KerbalEVA kerbaleva = null;
        internal EvaController evacontrol = null;

        public override BooleanValue Available()
        {
            return true;
        }



        #region Suffix functions

        private void DoEvent(Suffixed.Part.PartValue part , StringValue eventname)
        {
            var mypart = part.Part;
            if   (Vector3d.Magnitude(mypart.transform.position - kerbaleva.transform.position) < 3)
            {
           //     mypart.g

            }

        }


        private void BoardPart(Suffixed.Part.PartValue toboard)
        {
            kerbaleva.BoardPart(toboard.Part);

        }

        private void DoBoard()
        {
            try
            {
                    KerbalEVAUtility.RunEvent(kerbaleva, "Boarding Part");
            }
            catch { }

        }

        private void LadderGrab()
        {
            try
            {
                KerbalEVAUtility.RunEvent(kerbaleva, "Ladder Grab Start");
            }
            catch { }

        }
        private void LadderRelease()
        {
            try
            {
                KerbalEVAUtility.RunEvent(kerbaleva, "Ladder Let Go");
            }
            catch { }

        }


        private void DoRunEvent(StringValue eventname)
        {
            try
            {
                KerbalEVAUtility.RunEvent(kerbaleva, eventname.ToString());
            }
            catch { }

        }

        private ListValue ListEvents()
        {
            ListValue events = new ListValue();
            foreach (var evaevent in KerbalEVAUtility.GetEVAEvents(kerbaleva, KerbalEVAUtility.GetEVAStates(kerbaleva) ))
            {
                events.Add(new StringValue(evaevent.name));
            }
            return events;

        }



        // Code from Flightcontroller
        private void GoEVA (CrewMember kerbal)
        {
            foreach (var crewMember in shared.Vessel.GetVesselCrew())
            {
                if (crewMember.name.ToLower() == kerbal.Name.ToLower())
                {
                    FlightEVA.fetch.StartCoroutine(GoEVADelayed(crewMember.KerbalRef));
                    return;
                }
            }

        }

        private IEnumerator GoEVADelayed(Kerbal kerbal)
        {
            yield return new WaitForFixedUpdate();
            FlightEVA.SpawnEVA(kerbal);
        }

        public void DoPlantFlag()
        {
            if (!shared.Vessel.isEVA)
            {
                return;
            }
            if (kerbaleva.part.GroundContact)
            {
                kerbaleva.PlantFlag();
            }
        }

        private ListValue ListAnimations ()
        {
            ListValue animations = new ListValue();
            foreach (AnimationState state in kerbaleva.GetComponent<Animation>() )
            {
                animations.Add(new StringValue(state.name));
            }
            return animations;
        }

        private void PlayAnimation(StringValue name)
        {
            Animation _kerbalanimation = null;
            shared.Vessel.GetComponentCached<Animation>(ref _kerbalanimation);
            _kerbalanimation.CrossFade(name.ToString());
        }

        private void StopAnimation(StringValue name)
        {
            Animation _kerbalanimation = null;
            shared.Vessel.GetComponentCached<Animation>(ref _kerbalanimation);
            _kerbalanimation.Stop(name);
            _kerbalanimation.CrossFade("idle");
        }

        private void StopAllAnimations()
        {
            Animation _kerbalanimation = null;
            shared.Vessel.GetComponentCached<Animation>(ref _kerbalanimation);
            _kerbalanimation.Stop();
            _kerbalanimation.CrossFade("idle");
        }

        private void LoadAnimation (StringValue path)
        {
            Animation _kerbalanimation = null;
            shared.Vessel.GetComponentCached<Animation>(ref _kerbalanimation);
            var kerbaltransform = shared.Vessel.transform;
            KerbalAnimationClip myclip = new KerbalAnimationClip();
            myclip.LoadFromURL(path.ToString());
            myclip.Initialize(_kerbalanimation, kerbaltransform);
        }

        private void CheckEvaController()
        {
            if (evacontrol == null)
            {
                Debug.LogWarning("kOSEVA: Start init EvaController");
                this.kerbaleva = shared.Vessel.GetComponentCached<KerbalEVA>(ref kerbaleva);
                evacontrol = evacontrol = shared.Vessel.GetComponentCached<EvaController>(ref evacontrol);
                evacontrol.eva = kerbaleva;
                Debug.LogWarning("kOSEVA: Stop init EvaController");
            }

        }

        private void MoveKerbal(StringValue direction)
        {
            if (!shared.Vessel.isEVA) { return; }
            CheckEvaController();

            Command command = (Command)Enum.Parse(typeof(Command), direction, true);
            Debug.Log("EVA Command: " + command.ToString());
            this.evacontrol.order = command;
        }

        private void TurnLeft(ScalarValue degrees)
        {
            if (!shared.Vessel.isEVA) { return; }
            CheckEvaController();
            this.evacontrol.lookdirection = v_rotate(kerbaleva.vessel.transform.forward, kerbaleva.vessel.transform.right, -degrees.GetDoubleValue());
            this.evacontrol.order = Command.LookAt;
        }
        private void TurnRight(ScalarValue degrees)
        {
            if (!shared.Vessel.isEVA) { return; }
            CheckEvaController();
            this.evacontrol.lookdirection = v_rotate(kerbaleva.vessel.transform.forward, kerbaleva.vessel.transform.right, degrees.GetDoubleValue());
            this.evacontrol.order = Command.LookAt;
        }

        private void TurnTo(Vector direction)
        {
            if (!shared.Vessel.isEVA) { return; }
            CheckEvaController();
            this.evacontrol.lookdirection = direction.ToVector3D();
            this.evacontrol.order = Command.LookAt;
        }
        #endregion

        #region internal functions
        internal Vector3d v_rotate(Vector3d vec_from, Vector3d vec_to, double deg)
        {
            double deginrad = Mathf.Deg2Rad * deg;
            return ((Math.Cos(deginrad) * vec_from) + (Math.Sin(deginrad) * vec_to));
        }
        #endregion

    }
}