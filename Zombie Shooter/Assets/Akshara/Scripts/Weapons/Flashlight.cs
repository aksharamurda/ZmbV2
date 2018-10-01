using UnityEngine;

namespace AksharaMurda
{
    [RequireComponent(typeof(Light))]
    public class Flashlight : MonoBehaviour
    {
        public bool IsTurnedOn
        {
            get { return _isOn; }
        }

        private bool _isOn;
        private Light _light;

        private void Awake()
        {
            _light = GetComponent<Light>();
        }

        public void OnStartUsing()
        {
            _isOn = true;
        }

        public void OnUsed()
        {
            _isOn = false;
        }

        private void Update()
        {
            _light.enabled = _isOn;
        }
    }
}
