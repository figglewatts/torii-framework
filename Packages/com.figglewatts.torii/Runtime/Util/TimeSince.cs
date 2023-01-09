using System.Globalization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Torii.Util
{
    // all credit goes to Garry Newman for this one: https://garry.tv/timesince
    public struct TimeSince
    {
        private float time;

        public static implicit operator float(TimeSince ts) { return getTime() - ts.time; }

        public static implicit operator TimeSince(float ts) { return new TimeSince {time = getTime() - ts}; }

        public override string ToString() { return ((float)this).ToString(CultureInfo.InvariantCulture); }

        public void Reset()
        {
            time = getTime();
        }

        private static float getTime()
        {
#if UNITY_EDITOR
            return (float)EditorApplication.timeSinceStartup;
#else
            return Time.time;
#endif
        }
    }
}
