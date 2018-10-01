using System;
using UnityEngine;

namespace AksharaMurda
{
    [Serializable]
    public struct WeaponDescription
    {
        [Tooltip("Link to the weapon object containg a Gun component.")]
        public GameObject Item;

        [Tooltip("Link to the holstered weapon object which is made visible when the weapon is not used.")]
        public GameObject Holster;

        [Tooltip("Defines character animations used with this weapon.")]
        public WeaponType Type;

        [Tooltip("Animations to use for a tool. Relevant when weapon type is set to 'tool'.")]
        public Tool Tool;

        public Flashlight Flashlight
        {
            get
            {
                if (_cacheItem == Item)
                    return _cachedFlashlight;
                else
                {
                    cache();
                    return _cachedFlashlight;
                }
            }
        }

        public Gun Gun
        {
            get
            {
                if (_cacheItem == Item)
                    return _cachedGun;
                else
                {
                    cache();
                    return _cachedGun;
                }
            }
        }

        public bool IsAnAimableTool(bool useAlternate)
        {
            return Type == WeaponType.Tool && ToolDescription.Defaults[(int)Tool].HasAiming(useAlternate);
        }

        public bool IsAContinuousTool(bool useAlternate)
        {
            return Type == WeaponType.Tool && ToolDescription.Defaults[(int)Tool].IsContinuous(useAlternate);
        }

        private Gun _cachedGun;
        private Flashlight _cachedFlashlight;

        private GameObject _cacheItem;

        private void cache()
        {
            _cacheItem = Item;
            _cachedGun = Item == null ? null : Item.GetComponent<Gun>();

            _cachedFlashlight = Item == null ? null : Item.GetComponent<Flashlight>();

            if (_cachedFlashlight == null && Item != null)
                _cachedFlashlight = Item.GetComponentInChildren<Flashlight>();
        }
    }

    public enum WeaponType
    {
        Pistol,
        Rifle,
        Tool,
    }

    public struct WeaponAnimationStates
    {
        public string Reload;

        public string Hit;

        public string[] Common;

        public string Equip;

        public string Use;

        public string AlternateUse;

        public static WeaponAnimationStates Default()
        {
            var states = new WeaponAnimationStates();
            states.Reload = "Reload";
            states.Hit = "Hit";
            states.Common = new string[] { "Idle", "Aim", "Cover", "Empty state", "Jump", "Use", "Alternate Use", "Reload" };
            states.Equip = "Equip";
            states.Use = "Use";
            states.AlternateUse = "Alternate Use";

            return states;
        }
    }
}