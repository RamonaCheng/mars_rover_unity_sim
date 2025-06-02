// using UnityEngine;
// using Unity.Robotics.ROSTCPConnector;
// using RosMessageTypes.Geometry;

// public class Nav2CmdVelBridge : MonoBehaviour
// {
//     public SCC_InputProcessor inputProcessor;

//     public float maxLinearSpeed = 3f;   // meters per second
//     public float maxAngularSpeed = 1f;  // radians per second

//     void Start()
//     {
//         ROSConnection.GetOrCreateInstance().Subscribe<TwistMsg>("/cmd_vel", OnCmdVelReceived);
//     }

//     void OnCmdVelReceived(TwistMsg msg)
//     {
//         if (inputProcessor == null)
//             return;

//         var inputs = new SCC_Inputs();

//         // Normalize velocities to SCC input range (-1 to 1)
//         inputs.throttleInput = Mathf.Clamp((float)(msg.linear.x / maxLinearSpeed), -1f, 1f);
//         inputs.brakeInput = inputs.throttleInput < 0 ? -inputs.throttleInput : 0f;
//         inputs.throttleInput = Mathf.Max(inputs.throttleInput, 0f);

//         inputs.steerInput = -Mathf.Clamp((float)(msg.angular.z / maxAngularSpeed), -1f, 1f);
//         inputs.handbrakeInput = 0f;
//         Debug.Log($"Received /cmd_vel: linear.x = {msg.linear.x}, angular.z = {msg.angular.z}");

//         inputProcessor.OverrideInputs(inputs);
//     }
// }
