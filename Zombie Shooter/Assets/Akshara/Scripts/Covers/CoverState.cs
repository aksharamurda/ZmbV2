using UnityEngine;

namespace AksharaMurda
{
    public struct CoverState
    {
        public bool In
        {
            get { return Main != null; }
        }

        public bool IsTall
        {
            get { return Main != null && Main.CheckTall(Observer.y); }
        }

        public float LeftAdjacentAngle
        {
            get
            {
                if (Main == null || LeftAdjacent == null)
                    return 0;

                return Mathf.DeltaAngle(LeftAdjacent.Angle, Main.Angle);
            }
        }

        public float RightAngleDiff
        {
            get
            {
                if (Main == null || RightAdjacent == null)
                    return 0;

                return Mathf.DeltaAngle(Main.Angle, RightAdjacent.Angle);
            }
        }

        public bool IsStandingLeft
        {
            get
            {
                return Mathf.DeltaAngle(MovementAngle, ForwardAngle) > 0;
            }
        }

        public bool IsStandingRight
        {
            get
            {
                return Mathf.DeltaAngle(MovementAngle, ForwardAngle) < 0;
            }
        }

        public float Width
        {
            get
            {
                if (Main == null)
                    return 0;
                else
                    return Main.Width;
            }
        }

        public float ForwardAngle
        {
            get
            {
                if (Main == null)
                    return 0;
                else
                    return Main.Angle;
            }
        }

        public float MovementAngle
        {
            get { return ForwardAngle + 90 * Direction; }
        }

        public float FaceAngle
        {
            get
            {
                if (IsTall)
                    return ForwardAngle + Mathf.DeltaAngle(ForwardAngle, ForwardAngle + 180);
                else
                    return ForwardAngle + 90 * Direction;
            }
        }

        public Vector3 ForwardDirection
        {
            get
            {
                if (Main == null)
                    return Vector3.zero;
                else
                    return Main.Forward;
            }
        }

        public bool HasLeftAdjacent
        {
            get { return LeftAdjacent != null; }
        }

        public bool HasRightAdjacent
        {
            get { return RightAdjacent != null; }
        }

        public bool IsLeftAdjacentTall
        {
            get
            {
                if (LeftAdjacent == null)
                    return false;
                else
                    return (LeftAdjacent.Top - Observer.y) >= TallThreshold;
            }
        }

        public bool IsRightAdjacentTall
        {
            get
            {
                if (RightAdjacent == null)
                    return false;
                else
                    return (RightAdjacent.Top - Observer.y) >= TallThreshold;
            }
        }

        public bool HasLeftCorner
        {
            get
            {
                if (Main == null || !Main.OpenLeft)
                    return false;

                return LeftAdjacent == null || (IsTall && !IsLeftAdjacentTall);
            }
        }

        public bool HasRightCorner
        {
            get
            {
                if (Main == null || !Main.OpenRight)
                    return false;

                return RightAdjacent == null || (IsTall && !IsRightAdjacentTall);
            }
        }

        public const float TallThreshold = 1.1f;

        public Cover Main;

        public Cover LeftAdjacent;

        public Cover RightAdjacent;

        public Vector3 Observer;

        public float MainChangeAge;

        public int Direction;

        public bool Take(CoverSearch search, Vector3 observer)
        {
            Observer = observer;

            var wasIn = In;
            var closest = search.FindClosest();
            var previousMain = Main;

            if (Main == null && closest != null)
            {
                Main = closest;
                LeftAdjacent = Main.LeftAdjacent;
                RightAdjacent = Main.RightAdjacent;
            }
            else
                Clear();

            if (Main != previousMain)
                MainChangeAge = 0;

            if (In && !wasIn)
                return true;
            else
                return false;
        }

        public void Maintain(CoverSearch search, Vector3 observer)
        {
            Observer = observer;

            var closest = search.FindClosest();
            var previousMain = Main;

            if (Main != null && Main != closest)
            {
                if (closest != null)
                {
                    if (closest == LeftAdjacent)
                    {
                        StandLeft();
                        Main = closest;
                    }
                    else if (closest == RightAdjacent)
                    {
                        StandRight();
                        Main = closest;
                    }
                    else
                        Main = null;
                }
                else
                    Main = null;
            }

            if (Main != null)
            {
                LeftAdjacent = Main.LeftAdjacent;
                RightAdjacent = Main.RightAdjacent;
            }
            else
            {
                LeftAdjacent = null;
                RightAdjacent = null;
            }

            if (Main != previousMain)
                MainChangeAge = 0;
        }

        public void Clear()
        {
            Main = null;
            LeftAdjacent = null;
            RightAdjacent = null;
            MainChangeAge = 0;
        }

        public void StandRight()
        {
            Direction = 1;
        }

        public void StandLeft()
        {
            Direction = -1;
        }

        public void Update()
        {
            MainChangeAge += Time.deltaTime;
        }

        public void MoveToLeftAdjacent()
        {
            RightAdjacent = Main;
            Main = LeftAdjacent;
            LeftAdjacent = null;
        }

        public void MoveToRightAdjacent()
        {
            LeftAdjacent = Main;
            Main = RightAdjacent;
            RightAdjacent = null;
        }
    }
}