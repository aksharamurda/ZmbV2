using UnityEngine;

namespace AksharaMurda
{
    public enum AimStep
    {
        None,
        Enter,
        Aiming
    }

    public struct CoverAimState
    {
        public const float TimeEnterToAim = 0.2f;

        public const float TimeAimToLeave = 0.15f;

        public bool IsAiming
        {
            get { return Step == AimStep.Enter || Step == AimStep.Aiming; }
        }

        public AimStep Step;

        public float Angle;

        public bool IsZoomed;

        public float TimeLeftForNextStep;

        public bool LeaveAfterAiming;

        public void Update()
        {
            if (TimeLeftForNextStep >= -float.Epsilon)
                TimeLeftForNextStep -= Time.deltaTime;
            else
                switch (Step)
                {
                    case AimStep.Enter:
                        Step = AimStep.Aiming;
                        TimeLeftForNextStep = TimeAimToLeave;
                        break;

                    case AimStep.Aiming:
                        if (LeaveAfterAiming)
                        {
                            Step = AimStep.None;
                            LeaveAfterAiming = false;
                        }
                        break;
                }
        }

        public void ImmediateLeave()
        {
            LeaveAfterAiming = false;
            TimeLeftForNextStep = 0;
            Step = AimStep.None;
        }

        public void Leave()
        {
            switch (Step)
            {
                case AimStep.Enter:
                    LeaveAfterAiming = true;
                    break;

                case AimStep.Aiming:
                    if (!LeaveAfterAiming)
                    {
                        LeaveAfterAiming = true;
                        TimeLeftForNextStep = TimeAimToLeave;
                    }
                    break;
            }
        }

        public void FreeAim(float angle)
        {
            Angle = angle;
            Step = AimStep.Aiming;
        }

        public void CoverAim(float angle)
        {
            Angle = angle;

            if (Step == AimStep.Aiming)
                LeaveAfterAiming = false;
            else if (Step != AimStep.Enter)
            {
                Step = AimStep.Enter;
                TimeLeftForNextStep = TimeEnterToAim;
            }
        }
    }
}