using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AksharaMurda {

    public class GlobalManager : MonoBehaviour
    {
        public static GlobalManager instance;

        [Header("Control")]
        public bool useMobileConsole;
        public bool useBodyLookTarget;

        [Header("UI")]
        public bool hideHealthBar;

        void Awake()
        {
            instance = this;
        }
    }
}
