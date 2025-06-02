using System;
using UnityEngine;
// using RosMessageTypes.BuiltinInterfaces;

namespace Unity.Robotics.Core
{
    public readonly struct TimeStamp
    {
        public const double k_NanosecondsInSecond = 1e9;

        public readonly int Seconds;
        public readonly uint NanoSeconds;

        // Constructor from Unity time (double seconds)
        public TimeStamp(double timeInSeconds)
        {
            var sec = Math.Floor(timeInSeconds);
            var nsec = (timeInSeconds - sec) * k_NanosecondsInSecond;

            Seconds = (int)sec;
            NanoSeconds = nsec < 0 ? 0u : (uint)nsec;
        }

        // Constructor from ROS2 TimeMsg (int, uint)
        public TimeStamp(int sec, uint nsec)
        {
            Seconds = sec;
            NanoSeconds = nsec;
        }

        /*
        // Implicit conversion to ROS2 TimeMsg
        public static implicit operator TimeMsg(TimeStamp stamp)
        {
            return new TimeMsg(stamp.Seconds, stamp.NanoSeconds);
        }

        // Implicit conversion from ROS2 TimeMsg
        public static implicit operator TimeStamp(TimeMsg stamp)
        {
            return new TimeStamp(stamp.sec, stamp.nanosec);
        }
        */
    }
}
