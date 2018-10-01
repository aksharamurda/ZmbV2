using System;
using UnityEngine;

namespace AksharaMurda
{
    [Serializable]
    public struct CoverSettings
    {
        [Tooltip("Can the character peek from corners to aim.")]
        public bool CanUseCorners;

        [Tooltip("Height of character's collision capsule when idle in low cover.")]
        [Range(0, 10)]
        public float LowCapsuleHeight;

        [Tooltip("Height of character's collision capsule when aiming back in low cover.")]
        [Range(0, 10)]
        public float LowAimCapsuleHeight;

        [Tooltip("How quickly the character is orientated towards a direction.")]
        [Range(0, 50)]
        public float RotationSpeed;

        [Tooltip("Character enter cover if it is closer than this value. Defined as a distance between a cover and an edge of players capsule.")]
        [Range(0, 10)]
        public float EnterDistance;

        [Tooltip("Character exit cover if it is furhter away than this value. Defined as a distance between a cover and an edge of players capsule.")]
        [Range(0, 10)]
        public float LeaveDistance;

        [Tooltip("Controls the location of camera pivot position when in cover. Pivot point does not move beyond this margin.")]
        [Range(-10, 10)]
        public float PivotSideMargin;

        [Tooltip("Distance from a side of a cover at which player can enter aiming from a corner.")]
        [Range(0, 10)]
        public float CornerAimTriggerDistance;

        [Tooltip("Capsule radius used when determining if the character is in front of a tall cover when entering it.")]
        [Range(0, 10)]
        public float TallSideEnterRadius;

        [Tooltip("Capsule radius used when determining if the character is in front of a tall cover when leaving it.")]
        [Range(0, 10)]
        public float TallSideLeaveRadius;

        [Tooltip("Point relative to a tall corner for the motor to go back to after corner aiming.")]
        [Range(0, 10)]
        public float TallCornerOffset;

        [Tooltip("Capsule radius used when determining if the character is in front of a low cover when entering it.")]
        [Range(0, 10)]
        public float LowSideEnterRadius;

        [Tooltip("Capsule radius used when determining if the character is in front of a low cover when leaving it.")]
        [Range(0, 10)]
        public float LowSideLeaveRadius;

        [Tooltip("Point relative to a low corner for the motor to go back to after corner aiming.")]
        [Range(0, 10)]
        public float LowCornerOffset;

        [Tooltip("Time in seconds for player to start moving again after changing direction.")]
        [Range(0, 5)]
        public float DirectionChangeDelay;

        [Tooltip("Time in seconds for the character to take cover automatically when not facing the front of that cover. ")]
        [Range(0, 5)]
        public float BackDelay;

        [Tooltip("Approximate position shift done by the corner peek animation. Inverted when peeking left.")]
        public Vector3 CornerOffset;

        [Tooltip("Settings for cover update delays.")]
        public CoverUpdateSettings Update;

        [Tooltip("Defines angles for various gameplay situations. ")]
        public CoverAngleSettings Angles;

        public static CoverSettings Default()
        {
            var settings = new CoverSettings();
            settings.CanUseCorners = true;
            settings.LowCapsuleHeight = 0.75f;
            settings.LowAimCapsuleHeight = 1.25f;
            settings.RotationSpeed = 20.0f;
            settings.EnterDistance = 0.15f;
            settings.LeaveDistance = 0.25f;
            settings.PivotSideMargin = 0.5f;
            settings.CornerAimTriggerDistance = 0.6f;
            settings.TallSideEnterRadius = 0.15f;
            settings.TallSideLeaveRadius = 0.05f;
            settings.TallCornerOffset = 0.25f;
            settings.LowSideEnterRadius = 0.3f;
            settings.LowSideLeaveRadius = 0.2f;
            settings.LowCornerOffset = 0.4f;
            settings.DirectionChangeDelay = 0.25f;
            settings.BackDelay = 0.5f;
            settings.CornerOffset = new Vector3(0.8f, 0, 0);
            settings.Update = CoverUpdateSettings.Default();
            settings.Angles = CoverAngleSettings.Default();

            return settings;
        }
    }

    [Serializable]
    public struct CoverUpdateSettings
    {
        [Tooltip("Cover check delay when idle and not in cover.")]
        public float IdleNonCover;

        [Tooltip("Cover check delay when idle and in cover.")]
        public float IdleCover;

        [Tooltip("Cover check delay when moving outside of cover.")]
        public float MovingNonCover;

        [Tooltip("Cover check delay when moving in cover.")]
        public float MovingCover;

        public static CoverUpdateSettings Default()
        {
            var settings = new CoverUpdateSettings();
            settings.IdleNonCover = 10;
            settings.IdleCover = 2;
            settings.MovingNonCover = 0.5f;
            settings.MovingCover = 0.1f;

            return settings;
        }
    }

    [Serializable]
    public struct CoverAngleSettings
    {
        [Tooltip("Front area of a cover in angles, defined as a circle.")]
        [Range(0, 360)]
        public float Front;

        [Tooltip("Front area of a cover in angles, defined as a circle. Used to determine if can peek over a corner under low cover.")]
        [Range(0, 360)]
        public float LowCornerFront;

        [Tooltip("Front area of a cover in angles, defined as a circle. Used to determine if can peek over a corner under tall cover.")]
        [Range(0, 360)]
        public float TallLeftCornerFront;

        [Tooltip("Front area of a cover in angles, defined as a circle. Used to determine if can peek over a corner under tall cover.")]
        [Range(0, 360)]
        public float TallRightCornerFront;

        [Tooltip("Back area of a cover in angles, aiming in that direction results in character throwing grenade behind their back.")]
        [Range(0, 180)]
        public float BackThrow;

        [Tooltip("Angle from side axis opposite from a facing direction that sustains previous facing direction even if player is moving opposite of it.")]
        [Range(-90, 90)]
        public float LowWalkFaceChange;

        [Tooltip("Angle from side axis to trigger aiming away from a tall cover.")]
        public FieldAnglesSustain TallBack;

        [Tooltip("Angle from side axis that lowers the character, used when aiming from a lower cover.")]
        public SideAngles LowerAim;

        [Tooltip("Angle from front axis that manages left corner aiming state.")]
        public TriggerAngles LeftCorner;

        [Tooltip("Angle from front axis that manages right corner aiming state.")]
        public TriggerAngles RightCorner;

        [Tooltip("Angle from front axis used to maintain a facing direction when aiming.")]
        public SideAngles LowAimFaceChange;

        [Tooltip("Angle from front axis used to maintain a facing direction when aiming a grenade.")]
        public SideAngles LowGrenadeFaceChange;

        [Tooltip("Angle from side axis that allows player to fire at a tall cover wall.")]
        public FaceAngles TallWallAim;

        public static CoverAngleSettings Default()
        {
            var settings = new CoverAngleSettings();
            settings.Front = 140;
            settings.LowCornerFront = 90;
            settings.TallLeftCornerFront = 180;
            settings.TallRightCornerFront = 120;
            settings.BackThrow = 120;
            settings.LowWalkFaceChange = 60;
            settings.TallBack = new FieldAnglesSustain(20, 30, 1.0f);
            settings.LowerAim = new SideAngles(-5, 10);
            settings.LeftCorner = new TriggerAngles(-15, -17);
            settings.RightCorner = new TriggerAngles(-25, -27);
            settings.LowAimFaceChange = new SideAngles(0, 20);
            settings.LowGrenadeFaceChange = new SideAngles(0, 20);
            settings.TallWallAim = new FaceAngles(40, 20);

            return settings;
        }
    }

    [Serializable]
    public struct TriggerAngles
    {
        [Tooltip("Degrees from the front axis to trigger a state.")]
        [Range(-90, 90)]
        public float Enter;

        [Tooltip("Degrees from the front axis to trigger an exit from a state.")]
        [Range(-90, 90)]
        public float Exit;

        public TriggerAngles(float enter, float exit)
        {
            Enter = enter;
            Exit = exit;
        }
    }

    [Serializable]
    public struct FaceAngles
    {
        [Tooltip("Degrees from a side axis when facing the same direction as the player.")]
        [Range(-90, 90)]
        public float Face;

        [Tooltip("Degrees from a side axis when facing the opposite direction as the player.")]
        [Range(-90, 90)]
        public float Opposite;

        public FaceAngles(float face, float opposite)
        {
            Face = face;
            Opposite = opposite;
        }
    }

    [Serializable]
    public struct FieldAnglesSustain
    {
        [Tooltip("Degrees from a side axis when facing the same direction as the player.")]
        [Range(-90, 90)]
        public float Face;

        [Tooltip("Degrees from a side axis when facing the opposite direction as the player.")]
        [Range(-90, 90)]
        public float Opposite;

        [Tooltip("Time in seconds to sustain a change to the opposite direction.")]
        [Range(0, 10)]
        public float OppositeSustainTime;

        public FieldAnglesSustain(float face, float opposite, float sustain)
        {
            Face = face;
            Opposite = opposite;
            OppositeSustainTime = sustain;
        }
    }

    [Serializable]
    public struct SideAngles
    {
        [Tooltip("Degrees from a side axis when facing left of the cover.")]
        [Range(0, 90)]
        public float Left;

        [Tooltip("Degrees from a side axis when facing right of the cover.")]
        [Range(0, 90)]
        public float Right;

        public SideAngles(float left, float right)
        {
            Left = left;
            Right = right;
        }
    }
}