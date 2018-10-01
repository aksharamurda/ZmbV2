using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AksharaMurda {

    public class SettingManager : MonoBehaviour
    {
        public static SettingManager instance;

        [Header("Control")]
        public bool useMobileConsole;

        [Header("UI")]
        public bool hideHealthBar;

        void Awake()
        {
            instance = this;
        }
    }
}
