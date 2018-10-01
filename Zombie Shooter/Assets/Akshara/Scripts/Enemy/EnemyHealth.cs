using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AksharaMurda
{
    public class EnemyHealth : CharacterHealth
    {
        public override void OnValidate()
        {
            base.OnValidate();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
        }

        public override void OnDead()
        {
            base.OnDead();
        }

        public override void OnHit(Hit hit)
        {
            base.OnHit(hit);
        }

        public override void Deal(float amount)
        {
            base.Deal(amount);
        }
    }
}
