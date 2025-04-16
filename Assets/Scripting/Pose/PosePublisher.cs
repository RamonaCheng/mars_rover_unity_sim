using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.Core;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

public class PosePublisher : MonoBehaviour
{
    // ROS connection
    ROSConnection ros;

    [Header("ROS Settings")]
    public string topicName = "/pose";
    public string frameId = "base_link";
    public float publishRateHz = 4.0f;

    [Header("Conversion Settings")]
    [Tooltip("Scale Unity units to ROS meters. Example: 1/30 = large Unity terrain becomes realistic in RViz.")]
    [SerializeField] float unityToRosScale = 1f / 30f;

    private float timeSinceLastPublish = 0.0f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(topicName);
    }

    void Update()
    {
        timeSinceLastPublish += Time.deltaTime;
        if (timeSinceLastPublish >= 1.0f / publishRateHz)
        {
            PublishPose();
            timeSinceLastPublish = 0.0f;
        }
    }

    void PublishPose()
    {
        // Apply scale to position, keep rotation as-is
        Vector3<FLU> rosPosition = (transform.position * unityToRosScale).To<FLU>();
        Quaternion<FLU> rosRotation = transform.rotation.To<FLU>();

        var timestamp = new TimeStamp(Clock.time);

        PoseStampedMsg poseMsg = new PoseStampedMsg
        {
            header = new HeaderMsg
            {
                frame_id = frameId,
                stamp = new TimeMsg
                {
                    sec = (int)timestamp.Seconds,
                    nanosec = timestamp.NanoSeconds
                }
            },
            pose = new PoseMsg
            {
                position = new PointMsg(rosPosition.x, rosPosition.y, rosPosition.z),
                orientation = new QuaternionMsg(rosRotation.x, rosRotation.y, rosRotation.z, rosRotation.w)
            }
        };

        ros.Publish(topicName, poseMsg);
    }
}
