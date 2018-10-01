using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AksharaMurda
{
    [RequireComponent(typeof(Collider))]
    public class Actor : MonoBehaviour
    {
        #region Properties

        public bool IsAlive
        {
            get { return _isAlive; }
        }

        public bool IsArmed
        {
            get { return _motor != null && _motor.CurrentWeapon > 0 && _motor.Weapons[_motor.CurrentWeapon - 1].Type != WeaponType.Tool; }
        }

        public Cover Cover
        {
            get { return _cover; }
        }

        public Vector3 RelativeStandingTopPosition
        {
            get
            {
                if (_hasStandingHeight)
                    return Vector3.up * _standingHeight;
                else
                    return Vector3.up * _height;
            }
        }

        public Vector3 RelativeTopPosition
        {
            get { return Vector3.up * _height; }
        }

        public Vector3 StandingTopPosition
        {
            get
            {
                if (_hasStandingHeight)
                    return transform.position + Vector3.up * _standingHeight;
                else
                    return transform.position + Vector3.up * _height;
            }
        }

        public Vector3 TopPosition
        {
            get { return transform.position + Vector3.up * _height; }
        }

        public Collider Collider
        {
            get { return _collider; }
        }
        
        public Vector3 HeadDirection
        {
            get
            {
                if (_motor == null)
                    return transform.forward;
                else
                    return _motor.HeadForward;
            }
        }

        public bool IsAlerted
        {
            get { return _isAlerted; }
        }

        #endregion

        #region Public fields

        [Tooltip("Team number used by the AI.")]
        public int Side = 0;

        [Tooltip("Is the actor aggresive. Value used by the AI. Owning AI usually overwrites the value if present.")]
        public bool IsAggressive = true;

        #endregion

        #region Private fields

        private bool _isAlive = true;
        private Cover _cover;
        private bool _hasStandingHeight;
        private float _standingHeight;
        private float _height;
        private Collider _collider;
        private PlayerMotor _motor;
        private bool _isAlerted;

        #endregion

        #region Events
        
        public void OnStandingHeight(float value)
        {
            _hasStandingHeight = true;
            _standingHeight = value;
        }

        public void OnDead()
        {
            _isAlive = false;
            Actors.Unregister(this);
        }

        public void OnEnterCover(Cover cover)
        {
            _cover = cover;
        }

        public void OnLeaveCover()
        {
            _cover = null;
        }

        public void OnAlerted()
        {
            _isAlerted = true;
        }

        #endregion

        #region Behaviour

        private void Update()
        {
            _height = _collider.bounds.extents.y * 2;
        }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _motor = GetComponent<PlayerMotor>();

            _height = _collider.bounds.extents.y * 2;
        }

        private void OnEnable()
        {
            Actors.Register(this);
        }

        private void OnDisable()
        {
            Actors.Unregister(this);
        }

        private void OnDestroy()
        {
            Actors.Unregister(this);
        }

        #endregion
    }

    public static class Actors
    {
        public static IEnumerable<Actor> All
        {
            get { return _list; }
        }

        private static List<Actor> _list = new List<Actor>();
        private static Dictionary<GameObject, Actor> _map = new Dictionary<GameObject, Actor>();

        public static Actor Get(GameObject gameObject)
        {
            if (_map.ContainsKey(gameObject))
                return _map[gameObject];
            else
                return null;
        }

        public static void Register(Actor actor)
        {
            if (!_list.Contains(actor))
                _list.Add(actor);

            _map[actor.gameObject] = actor;
        }

        public static void Unregister(Actor actor)
        {
            if (_list.Contains(actor))
                _list.Remove(actor);

            if (_map.ContainsKey(actor.gameObject))
                _map.Remove(actor.gameObject);
        }
    }
}
