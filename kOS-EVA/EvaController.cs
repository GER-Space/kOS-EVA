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


        public static EvaController Instance
        {
            get
            {
                return instance;
            }
        }
        public EvaController() { Debug.LogWarning("EvaController Created"); instance = this; }

        public Command order = Command.Stop;
        public KerbalEVA eva = null;
        public Vector3d lookdirection = Vector.Zero;
        public float rotationdeg;
        internal string currentanimation = null;
        internal string tgtanimation = null;

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


        }


        void Update()
        {
            if (tgtanimation != currentanimation)
            {
                StopAllAnimations();
                PlayAnimation(tgtanimation);
            }


        }


        void OnDestroy()
        {
            instance = null;
        }



        #region internal functions
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
