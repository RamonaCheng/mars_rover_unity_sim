using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;

public class PointCloudPublisher : MonoBehaviour
{
    [Header("LiDAR Settings")]
    public GameObject lidarOrigin;
    public float rangeMin = 0.5f;
    public float rangeMax = 5f;
    public float horizontalFOV = 360f;
    public float verticalFOV = 10f;
    public float horizontalResolution = 2.0f;
    public float verticalResolution = 1.0f;

    [Header("ROS2 Settings")]
    public string topicName = "/pointcloud";
    public string frameId = "lidar_link";
    public float publishHz = 4.0f;

    private ROSConnection ros;
    private LaserSensor3D laser;
    private float timeSinceLastScan = 0.0f;

    void Start()
    {
        if (lidarOrigin == null)
        {
            Debug.LogError("[PointCloudPublisher] Lidar origin not assigned.");
            enabled = false;
            return;
        }

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PointCloud2Msg>(topicName);

        laser = new LaserSensor3D(
            lidarOrigin,
            rangeMin,
            rangeMax,
            horizontalFOV,
            verticalFOV,
            verticalResolution,
            horizontalResolution
        );
    }

    void Update()
    {
        timeSinceLastScan += Time.deltaTime;

        if (timeSinceLastScan >= 1.0f / publishHz)
        {
            PointCloud2Msg msg = laser.GetScanMsg();
            msg.header.frame_id = frameId;
            ros.Publish(topicName, msg);
            timeSinceLastScan = 0f;
        }
    }
}
