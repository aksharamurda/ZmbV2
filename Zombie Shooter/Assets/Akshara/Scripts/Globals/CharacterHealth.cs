﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AksharaMurda
{
    public class CharacterHealth : MonoBehaviour
    {
        [Tooltip("Current health of the character.")]
        public float Health = 100f;

        [Tooltip("Max health to regenerate to.")]
        public float MaxHealth = 100f;

        [Tooltip("Amount of health regenerated per second.")]
        public float Regeneration = 0f;

        [Tooltip("Does the component reduce damage on hits. Usually used for debugging purposes to make immortal characters.")]
        public bool IsTakingDamage = true;

        [Tooltip("Are bullet hits done to the main collider registered as damage.")]
        public bool IsRegisteringHits = true;

        private bool _isDead;

        public virtual void OnValidate()
        {
            Health = Mathf.Max(0, Health);
            MaxHealth = Mathf.Max(0, MaxHealth);
        }

        public virtual void LateUpdate()
        {
            if (_isDead)
                Health = 0;
            else
                Health = Mathf.Clamp(Health + Regeneration * Time.deltaTime, 0, MaxHealth);
        }

        public virtual void OnDead()
        {
            _isDead = true;
        }

        public virtual void OnHit(Hit hit)
        {
            Deal(hit.Damage);
        }

        public virtual void Deal(float amount)
        {
            if (Health <= 0 || !IsTakingDamage)
                return;

            Health -= amount;

            if (Health <= 0)
                SendMessage("OnDead");
        }
    }
}
