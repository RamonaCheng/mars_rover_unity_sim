using UnityEngine;
using System.IO;
using Unity.Robotics.Core; 

public class PoseLogger : MonoBehaviour
{
    public string fileName = "pose_log.csv";
    public float logInterval = 0.1f; // secs

    private float timeSinceLastLog = 0f;
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.dataPath, fileName);

        // Write CSV header
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine("sec,nsec,x,y,z,roll,pitch,yaw");
        }
    }

    void Update()
    {
        timeSinceLastLog += Time.deltaTime;
        if (timeSinceLastLog >= logInterval)
        {
            LogPose();
            timeSinceLastLog = 0f;
        }
    }

    void LogPose()
    {
        // Get pose
        Vector3 pos = transform.position;
        Vector3 euler = transform.rotation.eulerAngles;

        double now = Clock.Now;
        TimeStamp timestamp = new TimeStamp(now);

        // Format roll/pitch/yaw
        float roll = euler.z;
        float pitch = euler.x;
        float yaw = euler.y;

        string line = string.Format("{0},{1},{2:F3},{3:F3},{4:F3},{5:F3},{6:F3},{7:F3}",
            timestamp.Seconds,
            timestamp.NanoSeconds,
            pos.x, pos.y, pos.z,
            roll, pitch, yaw
        );

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(line);
        }
    }
}
