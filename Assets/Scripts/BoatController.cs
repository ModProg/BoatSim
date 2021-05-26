using UnityEngine;
using System.Collections;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Valve.VR.InteractionSystem.Sample
{
    public class BoatController : MonoBehaviour
    {
        public Transform Joystick;
        public float joyMove = 0.1f;

        public SteamVR_Action_Vector2 moveAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("platformer", "Move");

        public Transform throttleLever;
        public Transform steeringWheel;

        public float SteeringSpeed = 1;
        public float ThrottleSpeed = 1;

        public bool absolute;

        private Vector3 movement;
        private SteamVR_Input_Sources hand;
        private Interactable interactable;

        private void Start()
        {
            interactable = GetComponent<Interactable>();
        }

        private void Update()
        {
            if (interactable.attachedToHand)
            {
                hand = interactable.attachedToHand.handType;
                Vector2 m = moveAction[hand].axis;
                movement = new Vector3(m.x, 0, m.y);
            }
            else
            {
                return;
            }

            Joystick.localPosition = movement * joyMove;

            if (absolute)
            {
                steeringWheel.localEulerAngles = Vector3.forward * Movement.MAXSTEERING * movement.x;
                throttleLever.localEulerAngles = Vector3.right * Movement.MAXTHROTTLE * movement.z;
            }
            else
            {
                var old_s = steeringWheel.rotation;
                var old_t = throttleLever.rotation;

                float d = Time.deltaTime;
                float h = movement.x;
                steeringWheel.Rotate(new Vector3(0, 0, h * d * SteeringSpeed));
                float v = movement.z;
                throttleLever.Rotate(new Vector3(v * d * ThrottleSpeed, 0, 0));


                // -50 to 50
                var throttle = (throttleLever.localEulerAngles.x > 180 ? throttleLever.localEulerAngles.x - 360 : throttleLever.localEulerAngles.x);
                if (Mathf.Abs(throttle) > Movement.MAXTHROTTLE)
                    throttleLever.rotation = old_t;
                // -120 to 120 Neg is left, Pos is right
                var steering = (steeringWheel.localEulerAngles.z > 180 ? steeringWheel.localEulerAngles.z - 360 : steeringWheel.localEulerAngles.z);
                if (Mathf.Abs(steering) > Movement.MAXSTEERING)
                    steeringWheel.rotation = old_s;
            }
        }
    }
}