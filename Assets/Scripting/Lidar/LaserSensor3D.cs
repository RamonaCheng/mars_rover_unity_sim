using System;
using RosMessageTypes.Sensor;
using RosMessageTypes.BuiltinInterfaces;
using UnityEngine;
using Unity.Robotics.Core;

public class LaserSensor3D
{
    const int PointStep = 16;
    const int OffsetX = 0;
    const int OffsetY = 4;
    const int OffsetZ = 8;
    const int OffsetI = 12;

    float rangeMin;
    float rangeMax;
    float fovH;
    float fovV;
    float resolutionH;
    float resolutionV;

    int numH;
    int numV;

    float[] anglesH;
    float[] anglesV;

    uint numPoints;
    uint rawDataLength;
    byte[] rawData;

    GameObject sensorObject;
    string frameId;

    public LaserSensor3D(GameObject sensorObject, float rangeMin, float rangeMax, float fovH, float fovV, float resolutionV, float resolutionH)
    {
        this.rangeMin = rangeMin;
        this.rangeMax = rangeMax;
        this.fovH = Mathf.Clamp(fovH, 0, 360);
        this.fovV = Mathf.Clamp(fovV, 0, 360);
        this.resolutionH = resolutionH;
        this.resolutionV = resolutionV;
        this.sensorObject = sensorObject;
        this.frameId = sensorObject.name;

        float hStart = -fovH / 2;
        float vStart = -fovV / 2;

        numH = Mathf.FloorToInt(fovH / resolutionH) + 1;
        numV = Mathf.FloorToInt(fovV / resolutionV) + 1;

        if (fovH == 360) numH--;
        if (fovV == 360) numV--;

        anglesH = new float[numH];
        anglesV = new float[numV];

        for (int i = 0; i < numH; i++)
            anglesH[i] = hStart + i * resolutionH;

        for (int j = 0; j < numV; j++)
            anglesV[j] = vStart + j * resolutionV;

        numPoints = (uint)(numH * numV);
        rawDataLength = PointStep * numPoints;
        rawData = new byte[(int)rawDataLength];
    }

    public PointCloud2Msg GetScanMsg()
    {
        Transform tf = sensorObject.transform;
        Vector3 sensorPos = tf.position;
        Quaternion sensorRot = tf.rotation;

        int index = 0;

        for (int i = 0; i < numH; i++)
        {
            float theta = Mathf.Deg2Rad * anglesH[i];

            for (int j = 0; j < numV; j++)
            {
                float phi = Mathf.Deg2Rad * anglesV[j];

                Vector3 localDir = new Vector3(
                    Mathf.Cos(phi) * Mathf.Sin(theta),
                    -Mathf.Sin(phi),
                    Mathf.Cos(phi) * Mathf.Cos(theta)
                );

                Vector3 worldDir = sensorRot * localDir;
                Vector3 rayStart = sensorPos + worldDir * rangeMin;


                if (Physics.Raycast(rayStart, worldDir, out RaycastHit hit, rangeMax))
                {
                    float dist = Vector3.Distance(sensorPos, hit.point);

                    // FIX: You need this line!
                    Vector3 hitLocal = tf.InverseTransformPoint(hit.point);

                    BitConverter.GetBytes(hitLocal.z).CopyTo(rawData, index * PointStep + OffsetX);   // x (forward)
                    BitConverter.GetBytes(-hitLocal.x).CopyTo(rawData, index * PointStep + OffsetY);  // y (left)
                    BitConverter.GetBytes(hitLocal.y).CopyTo(rawData, index * PointStep + OffsetZ);   // z (up)
                    BitConverter.GetBytes(0.0f).CopyTo(rawData, index * PointStep + OffsetI);         // intensity

                }
                else
                    {
                        float invalid = float.NaN;
                        BitConverter.GetBytes(invalid).CopyTo(rawData, index * PointStep + OffsetX);
                        BitConverter.GetBytes(invalid).CopyTo(rawData, index * PointStep + OffsetY);
                        BitConverter.GetBytes(invalid).CopyTo(rawData, index * PointStep + OffsetZ);
                        BitConverter.GetBytes(0.0f).CopyTo(rawData, index * PointStep + OffsetI);
                    }

                index++;
            }
        }

        var timestamp = new TimeStamp(Clock.time);



        return new PointCloud2Msg
        {
            header = new RosMessageTypes.Std.HeaderMsg
            {
                frame_id = frameId,
                stamp = new TimeMsg
                {
                    sec = (int)timestamp.Seconds,
                    nanosec = timestamp.NanoSeconds
                }
            },
            height = 1,
            width = numPoints,                      // ✅ Pass uint directly
            fields = new PointFieldMsg[]
            {
                new PointFieldMsg("x", OffsetX, PointFieldMsg.FLOAT32, 1),
                new PointFieldMsg("y", OffsetY, PointFieldMsg.FLOAT32, 1),
                new PointFieldMsg("z", OffsetZ, PointFieldMsg.FLOAT32, 1),
                new PointFieldMsg("i", OffsetI, PointFieldMsg.FLOAT32, 1)
            },
            is_bigendian = false,
            point_step = PointStep,
            row_step = rawDataLength,               // ✅ Pass uint directly
            data = rawData,
            is_dense = false
        };


    }
}
