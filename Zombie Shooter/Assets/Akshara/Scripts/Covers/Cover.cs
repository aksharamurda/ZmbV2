using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AksharaMurda
{
    public struct CoverUser
    {
        public Actor Actor;
        public Vector3 Position;
    }

    public enum CoverClimb
    {
        No,
        Climb,
        Vault
    }

    [RequireComponent(typeof(BoxCollider))]
    public class Cover : MonoBehaviour
    {
        public const float TallThreshold = 1.2f;

        public IEnumerable<CoverUser> Users
        {
            get { return _users.Values; }
        }

        public float Top
        {
            get
            {
                if (!Application.isPlaying)
                    return GetComponent<BoxCollider>().bounds.max.y;
                else
                    return _collider.bounds.max.y;
            }
        }

        public float Bottom
        {
            get
            {
                if (!Application.isPlaying)
                    return GetComponent<BoxCollider>().bounds.min.y;
                else
                    return _collider.bounds.min.y;
            }
        }

        public Vector3 Forward
        {
            get { return transform.forward; }
        }

        public Vector3 Right
        {
            get { return transform.right; }
        }

        public Vector3 Left
        {
            get { return -transform.right; }
        }

        public float Angle
        {
            get { return transform.eulerAngles.y; }
        }

        public float Width
        {
            get
            {
                checkOrientationAndSize();
                return _size.x;
            }
        }

        public float Height
        {
            get
            {
                checkOrientationAndSize();
                return _size.y;
            }
        }

        public float Depth
        {
            get
            {
                checkOrientationAndSize();
                return _size.z;
            }
        }

        public Cover LeftAdjacent
        {
            get { return _leftAdjacent; }
        }

        public Cover RightAdjacent
        {
            get { return _rightAdjacent; }
        }

        [Tooltip("Can the character use the left corner of the cover.")]
        public bool OpenLeft = true;

        [Tooltip("Can the character use the rgiht corner of the cover.")]
        public bool OpenRight = true;

        [Tooltip("Maximum allowed distance to adjacent covers.")]
        public float AdjacentDistance = 1;

        private Vector3 _size;
        private Cover _leftAdjacent;
        private Cover _rightAdjacent;
        private BoxCollider _collider;
        private bool _hasLeftCorner;
        private bool _hasRightCorner;
        private bool _hasOrientationAndSize;
        private Vector3 _leftCorner;
        private Vector3 _rightCorner;
        private Quaternion _orientation;
        private Quaternion _negativeOrientation;
        private Dictionary<Actor, CoverUser> _users = new Dictionary<Actor, CoverUser>();

        private void Awake()
        {
            if (_collider == null)
                _collider = GetComponent<BoxCollider>();

            if (_leftAdjacent == null)
            {
                _leftAdjacent = findAdjacentTo(LeftCorner(Bottom), -120, 60, false);

                if (_leftAdjacent != null)
                    _leftAdjacent._rightAdjacent = this;
            }

            if (_rightAdjacent == null)
            {
                _rightAdjacent = findAdjacentTo(RightCorner(Bottom), -60, 120, true);

                if (_rightAdjacent != null)
                    _rightAdjacent._leftAdjacent = this;
            }
        }

        private Cover findAdjacentTo(Vector3 point, float minAngle, float maxAngle, bool useLeftCorner)
        {
            float closestDistance = 0f;
            Cover closestCover = null;

            foreach (var other in GameObject.FindObjectsOfType<Cover>())
                if (other != this)
                {
                    var closest = useLeftCorner ? other.LeftCorner(point.y) : other.RightCorner(point.y);
                    var distance = Vector3.Distance(point, closest);

                    if (distance > AdjacentDistance)
                        continue;

                    var closestAngle = other.Angle;
                    var deltaAngle = Mathf.DeltaAngle(Angle, closestAngle);

                    if (deltaAngle >= minAngle && deltaAngle <= maxAngle)
                        if (closestCover == null || distance < closestDistance)
                        {
                            closestCover = other;
                            closestDistance = distance;
                        }
                }

            return closestCover;
        }

        public CoverClimb GetClimbAt(Vector3 position, float radius, float maxClimbHeight, float maxVaultHeight, float maxVaultDistance)
        {
            var left = LeftCorner(position.y);
            var right = RightCorner(position.y);
            var x = Vector3.Dot(Right, position - left) / Width;

            if ((x < 0 && _leftAdjacent == null) ||
                (x > 1 && _rightAdjacent == null))
                return CoverClimb.No;

            var space = radius / Width;

            if ((Height > maxClimbHeight && Height > maxVaultHeight) ||
                checkForward(x - space) ||
                checkForward(x) ||
                checkForward(x + space) ||
                checkUp(x - space) ||
                checkUp(x) ||
                checkUp(x + space))
                return CoverClimb.No;
            else if (Height < maxVaultHeight &&
                     !checkDown(x - space, maxVaultDistance) &&
                     !checkDown(x, maxVaultDistance) &&
                     !checkDown(x + space, maxVaultDistance))
                return CoverClimb.Vault;
            else
                return CoverClimb.Climb;
        }

        public void RegisterUser(Actor actor, Vector3 position)
        {
            CoverUser user;
            user.Actor = actor;
            user.Position = position;

            _users[actor] = user;
        }

        public void UnregisterUser(Actor actor)
        {
            if (_users.ContainsKey(actor))
                _users.Remove(actor);
        }

        public bool IsTall
        {
            get
            {
                return (Top - Bottom) > TallThreshold;
            }
        }

        public bool CheckTall(float observer)
        {
            return (Top - observer) > TallThreshold;
        }

        public bool IsInFront(Vector3 observer, bool isOld)
        {
            var closest = ClosestPointTo(observer, 0, 0);
            var vector = (closest - observer).normalized;
            var dot = Vector3.Dot(vector, Forward);

            if (isOld)
                return dot >= 0.85f;
            else
                return dot >= 0.95f;
        }

        public Vector3 LeftCorner(float y, float offset = 0)
        {
            var point = _leftCorner;

            if (!_hasLeftCorner)
            {
                _leftCorner = point = ClosestPointTo(transform.position + Left * 999, 0, 0);
                _hasLeftCorner = Application.isPlaying;
            }

            point += Left * offset;
            point.y = y;
            return point;
        }

        public Vector3 RightCorner(float y, float offset = 0)
        {
            var point = _rightCorner;

            if (!_hasRightCorner)
            {
                _rightCorner = point = ClosestPointTo(transform.position + Right * 999, 0, 0);
                _hasRightCorner = Application.isPlaying;
            }

            point += Right * offset;
            point.y = y;
            return point;
        }

        public bool IsByLeftCorner(Vector3 position, float distance)
        {
            return Vector3.Distance(LeftCorner(position.y), position) <= distance;
        }

        public bool IsByRightCorner(Vector3 position, float distance)
        {
            return Vector3.Distance(RightCorner(position.y), position) <= distance;
        }

        public int ClosestCornerToSegment(Vector3 a, Vector3 b, float radius, out Vector3 position)
        {
            var left = LeftCorner(0, -radius);
            var right = RightCorner(0, -radius);

            var distLeft = Util.DistanceToSegment(left, a, b);
            var distRight = Util.DistanceToSegment(right, a, b);

            if (distLeft < distRight)
            {
                position = left;
                return -1;
            }
            else
            {
                position = right;
                return 1;
            }
        }

        public int ClosestCornerTo(Vector3 point, float radius, out Vector3 position)
        {
            var left = LeftCorner(0, -radius);
            var right = RightCorner(0, -radius);

            var distLeft = Vector3.Distance(left, point);
            var distRight = Vector3.Distance(right, point);

            if (distLeft < distRight)
            {
                position = left;
                return -1;
            }
            else
            {
                position = right;
                return 1;
            }
        }

        private void checkOrientationAndSize()
        {
            if (Application.isPlaying && _collider == null)
                _collider = GetComponent<BoxCollider>();

            var collider = Application.isPlaying ? _collider : GetComponent<BoxCollider>();

            if (!_hasOrientationAndSize)
            {
                _orientation = Quaternion.Euler(0, -transform.eulerAngles.y, 0);
                _negativeOrientation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

                _size.x = 1.0f / transform.InverseTransformVector(_negativeOrientation * Vector3.right).magnitude;
                _size.y = collider.bounds.size.y;
                _size.z = 1.0f / transform.InverseTransformVector(_negativeOrientation * Vector3.forward).magnitude;

                _hasOrientationAndSize = Application.isPlaying;
            }
        }

        public Vector3 ClosestPointTo(Vector3 point, float sideRadius, float frontRadius)
        {
            checkOrientationAndSize();

            var hw = _size.x * 0.5f;
            var hd = _size.z * 0.5f;

            var local = _orientation * (point - transform.position);
            var left = new Vector3(-hw, local.y, -hd);
            var right = new Vector3(hw, local.y, -hd);
            var leftToRight = (right - left).normalized;
            left += leftToRight * sideRadius;
            right -= leftToRight * sideRadius;

            local = Util.FindClosestToPath(left, right, local);

            var result = _negativeOrientation * local + transform.position - Forward * frontRadius;

            return result;
        }

        public bool IsFront(float angle, SideAngles angles)
        {
            if (IsLeft(angle))
                return IsFront(angle, angles.Left);
            else
                return IsFront(angle, angles.Right);
        }

        public bool IsFront(float angle, TriggerAngles angles, bool state)
        {
            if (state)
                return IsFront(angle, angles.Exit);
            else
                return IsFront(angle, angles.Enter);
        }

        public bool IsFront(float angle, FaceAngles angles, int direction)
        {
            if (direction < 0)
            {
                if (IsLeft(angle))
                    return IsFront(angle, angles.Face);
                else
                    return IsFront(angle, angles.Opposite);
            }
            else
            {
                if (IsRight(angle))
                    return IsFront(angle, angles.Face);
                else
                    return IsFront(angle, angles.Opposite);
            }
        }

        public bool IsFrontField(float angle, float field)
        {
            return IsFront(angle, (180 - field) / 2);
        }

        public bool IsFront(float angle, float margin = 0)
        {
            float delta = Mathf.DeltaAngle(angle, Angle);

            return delta >= (-90 + margin) && delta <= (90 - margin);
        }

        public bool IsBack(float angle, float margin = 0)
        {
            float delta = Mathf.DeltaAngle(angle, Angle);

            return delta <= (-90 - margin) || delta >= (90 + margin);
        }

        public bool IsLeft(float angle, float margin = 0)
        {
            float delta = Mathf.DeltaAngle(angle, Angle);

            return delta >= margin && delta <= (180 - margin);
        }

        public bool IsLeft(float angle, TriggerAngles angles, bool state)
        {
            return IsLeft(angle, state ? angles.Exit : angles.Enter);
        }

        public bool IsLeftField(float angle, float field)
        {
            return IsLeft(angle, (180 - field) / 2);
        }

        public bool IsRight(float angle, float margin = 0)
        {
            float delta = Mathf.DeltaAngle(angle, Angle);

            return delta >= (-180 + margin) && delta <= -margin;
        }

        public bool IsRight(float angle, TriggerAngles angles, bool state)
        {
            return IsRight(angle, state ? angles.Exit : angles.Enter);
        }

        public bool IsRightField(float angle, float field)
        {
            return IsRight(angle, (180 - field) / 2);
        }

        private bool checkForward(float x)
        {
            return checkRay(LeftCorner(Top + 0.1f, -Width * x), Forward, 0.5f);
        }

        private bool checkDown(float x, float distance)
        {
            return checkRay(LeftCorner(Top + 0.1f, -Width * x) + Forward * distance, Vector3.down, 0.5f);
        }

        private bool checkUp(float x)
        {
            return checkRay(LeftCorner(Top + 0.1f, -Width * x) + Forward * 0.3f, Vector3.up, 2.0f);
        }

        private bool checkRay(Vector3 position, Vector3 direction, float distance)
        {
            return checkLine(position, position + direction * distance);
        }

        private bool checkLine(Vector3 position, Vector3 end)
        {
            if (Physics.Raycast(position, (end - position).normalized, Vector3.Distance(end, position), 1, QueryTriggerInteraction.Ignore))
            {
                Debug.DrawLine(position, end, Color.red, 3);
                return true;
            }

            Debug.DrawLine(position, end, Color.green, 3);
            return false;
        }
    }
}
