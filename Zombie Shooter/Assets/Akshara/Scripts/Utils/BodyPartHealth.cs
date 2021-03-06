﻿using UnityEngine;

namespace AksharaMurda
{
    public class BodyPartHealth : MonoBehaviour
    {
        [Tooltip("By default target is the first found parent object with CharacterHealth. Setting TargetOverride overrides it.")]
        public CharacterHealth TargetOveride;

        [Tooltip("Multiplies taken damage before applying it to the target CharacterHealth.")]
        public float DamageScale = 1.0f;

        private CharacterHealth _target;

        public void OnHit(Hit hit)
        {
            var target = TargetOveride;

            if (target == null)
                target = _target;

            if (target == null)
            {
                var obj = transform;

                while (obj != null && target == null)
                {
                    target = obj.GetComponent<CharacterHealth>();
                    obj = obj.transform.parent;
                }

                _target = target;
            }

            if (target == null)
                return;

            hit.Damage *= DamageScale;

            target.SendMessage("OnHit",
                               hit,
                               SendMessageOptions.DontRequireReceiver);
        }
    }
}
