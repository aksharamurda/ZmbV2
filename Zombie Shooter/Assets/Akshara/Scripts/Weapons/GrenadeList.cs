using System.Collections.Generic;

namespace AksharaMurda
{
    public static class GrenadeList
    {
        public static IEnumerable<Grenade> All
        {
            get { return _list; }
        }

        private static List<Grenade> _list = new List<Grenade>();

        public static void Register(Grenade grenade)
        {
            if (!_list.Contains(grenade))
                _list.Add(grenade);
        }

        public static void Unregister(Grenade grenade)
        {
            if (_list.Contains(grenade))
                _list.Remove(grenade);
        }
    }
}
