using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using kOS.Suffixed;

using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;

namespace EVAMove
{
    public enum Command
    {
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down,
        LookAt,
        Stop
    }
    public class EvaController : VesselModule
    {

        public static EvaController instance = null;

 //       public EvaController() { if (FlightGlobals.ActiveVessel.isEVA) {  Debug.LogWarning("EvaController Created"); instance = this; } }

        public Command order = Command.Stop;
        public KerbalEVA eva = null;
        public Vector3d lookdirection = Vector.Zero;
        public float rotationdeg;
        internal string currentanimation = null;
        internal string tgtanimation = null;
        public FieldInfo eva_tgtFwd = null;
        public FieldInfo eva_tgtUp = null;
        public FieldInfo eva_tgtRpos = null;
        public FieldInfo eva_packTgtRPos = null;
        public FieldInfo eva_packLinear = null;
        internal bool once = true;
        internal Vessel parentVessel = null;
        internal float lastkeypressed = 0.0f;


        /// <summary>
        /// OnAwake is called once when instatiating a new VesselModule.  This is the first method called
        /// by KSP after the VesselModule has been attached to the parent Vessel.  We use it to store
        /// the parent Vessel and track the kOSVesselModule instances.
        /// </summary>
        public override void OnAwake()
        {
            Debug.LogWarning("EvaController Awake()!");
            parentVessel = GetComponent<Vessel>();
            if (parentVessel != null)
            {
                if (parentVessel.isEVA)
                {
                    instance = this;
                    Debug.LogWarning("EvaController Awake() finished on " + parentVessel.vesselName);
                }
                else
                {
                    Debug.LogWarning("EvaController destroyed on " + parentVessel.vesselName + " not EVA" );
                    Destroy(this);
                }
            } else
            {
                Debug.LogWarning("EvaController destroyed: No Vessel");
                Destroy(this);
            }
        }

        /// <summary>
        /// Start is called after OnEnable activates the module.  This is the second method called by
        /// KSP after Awake.  All parts should be added to the vessel now, so it is safe to walk the
        /// parts tree to find the attached kOSProcessor modules.
        /// </summary>
        public void Start()
        {
            eva = parentVessel.GetComponentCached<KerbalEVA>(ref eva);

            eva_tgtRpos = typeof(KerbalEVA).GetField("tgtRpos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            eva_packTgtRPos = typeof(KerbalEVA).GetField("packTgtRPos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            eva_tgtFwd = typeof(KerbalEVA).GetField("tgtFwd", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            eva_tgtUp = typeof(KerbalEVA).GetField("tgtUp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            eva_packLinear = typeof(KerbalEVA).GetField("packLinear", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        }


        public void FixedUpdate()
        {

            if (eva == null || !eva.vessel.isEVA )
            {
                return;
            }
            // priority: 0. Ragdoll recover 1. onladder 2. in water 3. on land 4. flying around
            if (eva.isRagdoll)
            {
                TryRecoverFromRagdoll();
                return;
            }

            if (eva.OnALadder)
            {
                DoMoveOnLadder();
                return;
            }

            if (eva.part.WaterContact)
            {
                DoMoveInWater();
                return;
            }

            if (eva.vessel.situation == Vessel.Situations.LANDED || eva.vessel.situation == Vessel.Situations.SPLASHED)
            {
                DoMoveOnLand();
                return;
            }

            if (eva.JetpackDeployed)
            {
                DoMoveInSpace();
                return;
            }

        }


        void Update()
        {

            if (tgtanimation != currentanimation)
            {
                StopAllAnimations();
                PlayAnimation(tgtanimation);
            }
            CheckKeys();
        }


        void OnDestroy()
        {
            instance = null;
        }



        #region internal functions

        internal void CheckKeys()
        {
            if (!parentVessel.isEVA || parentVessel.id != FlightGlobals.ActiveVessel.id ) { return;  }


            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                parentVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom01);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                parentVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom02);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                parentVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom03);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                parentVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom04);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                parentVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom05);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                parentVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom06);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                parentVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom07);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                parentVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom08);
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                parentVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom09);
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                parentVessel.ActionGroups.ToggleGroup(KSPActionGroup.Custom10);
            }
        }


        internal void TryRecoverFromRagdoll()
        {
            Debug.Log("KOSEVA: Trying to recover kerbal.");
            if (eva.canRecover && eva.fsm.TimeAtCurrentState > 1.21f && eva.part.GroundContact)
            {
                KerbalEVAUtility.RunEvent(eva, "Recover Start");
            }
        }

        // only up and down allowed
        internal void DoMoveOnLadder()
        {
            float dtime = Time.deltaTime;
            switch (order)
            { 
                case Command.Up:
                    eva.vessel.transform.position += eva.ladderClimbSpeed * dtime * eva.vessel.transform.up;
                    tgtanimation = "ladder_up";
                    break;
                case Command.Down:
                    eva.vessel.transform.position -= eva.ladderClimbSpeed * dtime * eva.vessel.transform.up;
                    tgtanimation = "ladder_down";
                    break;
                case Command.Stop:
                    tgtanimation = "ladder_idle";
                    break;
                default:
                    tgtanimation = "ladder_idle";
                    break;
            }

        }
        // we only allow turning and forward in water
        internal void DoMoveInWater()
        {
            float dtime = Time.deltaTime;
            switch (order)
            {
                case Command.Forward:
                    eva.vessel.transform.position += eva.swimSpeed * dtime * eva.vessel.transform.forward;
                    tgtanimation = "swim_forward";
                    break;
                case Command.LookAt:
                    tgtanimation = "swim_idle";
                    if (Vector3d.Angle(eva.vessel.transform.forward, Vector3d.Exclude(eva.vessel.transform.up, lookdirection)) < 0.2)
                    {
                        order = Command.Stop;
                        break;
                    }
                    var step = eva.turnRate;
                    Quaternion from = eva.vessel.transform.rotation;
                    Quaternion to = Quaternion.LookRotation(lookdirection, eva.vessel.transform.up);
                    Quaternion result = Quaternion.RotateTowards(from, to, step);
                    eva.vessel.SetRotation(result);
                    break;
                case Command.Stop:
                    tgtanimation = "swim_idle";
                    break;
                default:
                    tgtanimation = "swim_idle";
                    break;
            }

        }



        internal void DoMoveOnLand()
        {
            if (eva.CharacterFrameMode && once) { Debug.LogWarning("Framemode active"); once = false; }
            float dtime = Time.deltaTime;
            switch (order)
            {
                case Command.Forward:
                    eva.vessel.transform.position += eva.walkSpeed * dtime * eva.vessel.transform.forward;
                    tgtanimation = eva.vessel.geeForce > eva.minWalkingGee ? "wkC_forward" : "wkC_loG_forward";
                    break;
                case Command.Backward:
                    eva.vessel.transform.position -= eva.strafeSpeed * dtime * eva.vessel.transform.forward;
                    // couldn't find a low-g backward animation
                    tgtanimation = eva.vessel.geeForce > eva.minWalkingGee ? "wkC_backwards" : "wkC_backwards";
                    break;
                case Command.Left:
                    eva.vessel.transform.position -= eva.strafeSpeed * dtime * eva.vessel.transform.right;
                    tgtanimation = eva.vessel.geeForce > eva.minWalkingGee ? "wkC_sideLeft" : "wkC_loG_sideLeft";
                    break;
                case Command.Right:
                    eva.vessel.transform.position += eva.strafeSpeed * dtime * eva.vessel.transform.right;
                    tgtanimation = eva.vessel.geeForce > eva.minWalkingGee ? "wkC_sideRight" : "wkC_loG_sideRight";
                    break;
                case Command.Up:
                    break;
                case Command.Down:
                    break;
                case Command.LookAt:
                    if (Vector3d.Angle(eva.vessel.transform.forward, Vector3d.Exclude(eva.vessel.transform.up, lookdirection)) < 0.2)
                    {
                        order = Command.Stop;
                        tgtanimation = "idle";
                        break;
                    }
                    //var step = eva.turnRate * dtime;
                    var step = eva.turnRate;
                    Quaternion from = eva.vessel.transform.rotation;
                    Quaternion to = Quaternion.LookRotation(lookdirection, eva.vessel.transform.up);
                    Quaternion result = Quaternion.RotateTowards(from, to, step);
                    eva.vessel.SetRotation(result);
                    tgtanimation = Vector3d.Angle(eva.vessel.transform.right, Vector3d.Exclude(eva.vessel.transform.up, lookdirection)) < 90 ? "leftTurn" : "rightTurn";
                    break;
                case Command.Stop:
                    tgtanimation = "idle";
                    break;
                default:
                    break;
            }
        }

        internal void DoMoveInSpace()
        {
            float dtime = Time.deltaTime;
            if  (once) { Debug.LogWarning("linPower: " + eva.linPower.ToString() + "   rotation Power:  " + eva.rotPower.ToString()  ); once = false; }
            switch (order)
            {
                case Command.Forward:
                    //this.eva_packTgtRPos.SetValue(eva, eva.transform.forward);
                    //this.eva_Vtgt.SetValue(eva, eva.transform.forward );
                    eva.part.Rigidbody.AddForce(eva.transform.forward * dtime * 2f, ForceMode.Force);
                    //this.eva_packLinear.SetValue(eva, eva.transform.forward);
                    break;
                case Command.Backward:
                    //this.eva_tgtRpos.SetValue(eva, -eva.transform.forward);
                    //this.eva_packTgtRPos.SetValue(eva, -eva.transform.forward);
                    //this.eva_Vtgt.SetValue(eva, -eva.transform.forward);
                    eva.part.Rigidbody.AddForce(-eva.transform.forward * dtime * 2f, ForceMode.Force);
                    //FlightInputHandler.state.mainThrottle = 1.0f;
                    break;
                case Command.Left:
                    // this.eva_packTgtRPos.SetValue(eva, -eva.transform.right);
                    eva.part.Rigidbody.AddForce(-eva.transform.right * dtime * 2f, ForceMode.Force);
                    break;
                case Command.Right:
                    this.eva_packTgtRPos.SetValue(eva, eva.transform.right);
                    eva.part.Rigidbody.AddForce(eva.transform.right * dtime * 2f, ForceMode.Force);
                    break;
                case Command.Up:
                    this.eva_packTgtRPos.SetValue(eva, eva.transform.up);
                    eva.part.Rigidbody.AddForce(eva.transform.up * dtime * 2f, ForceMode.Force);
                    break;
                case Command.Down:
                    this.eva_packTgtRPos.SetValue(eva, -eva.transform.up);
                    eva.part.Rigidbody.AddForce(-eva.transform.up * dtime * 2f, ForceMode.Force);
                    break;
                case Command.LookAt:
                    if (Vector3d.Angle(eva.vessel.transform.forward,  lookdirection) < 3)
                    {
                        order = Command.Stop;
                        break;
                    }
                   // var step = eva.turnRate * dtime;
                    var step = eva.rotPower * dtime;
                   // Quaternion from = eva.vessel.transform.rotation;
                   // Quaternion to = Quaternion.LookRotation((Vector3)lookdirection.normalized, eva.vessel.transform.up);
                   // Quaternion result = Quaternion.RotateTowards(from, to, step);
                  //  this.eva_tgtFwd.SetValue(eva, result * (Vector3)this.eva_tgtFwd.GetValue(eva));
                  //  this.eva_tgtUp.SetValue(eva, result * (Vector3)this.eva_tgtUp.GetValue(eva));
                       this.eva_tgtFwd.SetValue(eva, (Vector3)lookdirection.normalized);
                    break;
                case Command.Stop:
                    break;
                default:
                    break;
            }
        }

        private void PlayAnimation(string name)
        {
            Animation _kerbalanimation = null;
            eva.vessel.GetComponentCached<Animation>(ref _kerbalanimation);
            _kerbalanimation.CrossFade(name);
            currentanimation = name;
        }

        private void StopAllAnimations()
        {
            Animation _kerbalanimation = null;
            eva.vessel.GetComponentCached<Animation>(ref _kerbalanimation);
            _kerbalanimation.Stop();
            _kerbalanimation.CrossFade("idle");

        }
        #endregion



    }
}
