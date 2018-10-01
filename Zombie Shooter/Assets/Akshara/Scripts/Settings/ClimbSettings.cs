using System;
using UnityEngine;

namespace AksharaMurda
{
    [Serializable]
    public struct ClimbSettings
    {
        [Tooltip("Capsule height to set during a climb.")]
        [Range(0, 10)]
        public float CapsuleHeight;

        [Tooltip("Scale of movement in Y axis. The animation usually assumed a 1 meter high obstacle.")]
        [Range(0, 3)]
        public float VerticalScale;

        [Tooltip("Scale of movement done by the climbing animation in X and Z axes.")]
        [Range(0, 3)]
        public float HorizontalScale;

        [Tooltip("Additional velocity added to the character in the direction of cover.")]
        [Range(0, 5)]
        public float Push;

        [Tooltip("Moment in the climbing animation to turn on the push force.")]
        [Range(0, 1)]
        public float PushOn;

        [Tooltip("Moment in the climbing animation to turn off the push force.")]
        [Range(0, 1)]
        public float PushOff;

        [Tooltip("Moment in the climbing animation to turn off the capsule collider.")]
        [Range(0, 1)]
        public float CollisionOff;

        [Tooltip("Moment in the climbing animation to turn back on the capsule collider.")]
        [Range(0, 1)]
        public float CollisionOn;

        public static ClimbSettings Default()
        {
            var settings = new ClimbSettings();
            settings.CapsuleHeight = 1.5f;
            settings.VerticalScale = 1.0f;
            settings.HorizontalScale = 1.05f;
            settings.Push = 0.5f;
            settings.PushOn = 0.6f;
            settings.PushOff = 0.9f;
            settings.CollisionOff = 0.3f;
            settings.CollisionOn = 0.7f;

            return settings;
        }
    }

    [Serializable]
    public struct VaultSettings
    {
        [Tooltip("Capsule height to set during a vault.")]
        [Range(0, 10)]
        public float CapsuleHeight;

        [Tooltip("Moment in vault animation to turn the character gravity on.")]
        [Range(0, 1)]
        public float FallTime;

        [Tooltip("Scale of movement in Y axis. The animation usually assumed a 1 meter high obstacle.")]
        [Range(0, 3)]
        public float VerticalScale;

        [Tooltip("Scale of movement done by the climbing animation in X and Z axes.")]
        [Range(0, 3)]
        public float HorizontalScale;

        [Tooltip("Additional velocity added to the character in the direction of cover.")]
        [Range(0, 5)]
        public float Push;

        [Tooltip("Moment in the vault animation to turn on the push force.")]
        [Range(0, 1)]
        public float PushOn;

        [Tooltip("Moment in the vault animation to turn off the push force.")]
        [Range(0, 1)]
        public float PushOff;

        [Tooltip("Moment in the climbing animation to turn off the capsule collider.")]
        [Range(0, 1)]
        public float CollisionOff;

        [Tooltip("Moment in the climbing animation to turn back on the capsule collider.")]
        [Range(0, 1)]
        public float CollisionOn;

        public static VaultSettings Default()
        {
            var settings = new VaultSettings();
            settings.CapsuleHeight = 1.5f;
            settings.FallTime = 0.7f;
            settings.VerticalScale = 1.2f;
            settings.HorizontalScale = 0.8f;
            settings.Push = 0.0f;
            settings.PushOn = 0.0f;
            settings.PushOff = 1.0f;
            settings.CollisionOff = 0.0f;
            settings.CollisionOn = 0.7f;

            return settings;
        }
    }
}