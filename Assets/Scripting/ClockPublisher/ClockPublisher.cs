using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.Core;
using RosMessageTypes.Rosgraph; // ✅ NOTE: this is different
using RosMessageTypes.BuiltinInterfaces;

public class ClockPublisher : MonoBehaviour
{
    public float publishHz = 20f;
    private ROSConnection ros;
    private float timeSinceLastPublish = 0f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ClockMsg>("/clock"); // ✅ CHANGED TYPE
    }

    void Update()
    {
        timeSinceLastPublish += Time.deltaTime;
        if (timeSinceLastPublish >= 1f / publishHz)
        {
            double now = Clock.Now;
            int sec = (int)now;
            uint nsec = (uint)((now - sec) * 1e9);

            TimeMsg time = new TimeMsg(sec, nsec);
            ClockMsg clockMsg = new ClockMsg { clock = time };

            ros.Publish("/clock", clockMsg); // ✅ CHANGED TYPE
            timeSinceLastPublish = 0f;
        }
    }
}
