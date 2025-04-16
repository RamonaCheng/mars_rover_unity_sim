# Mars Rover Unity Simulation

## ROS2 Integration

This Unity project is designed to work with [mars_rover_ros2_ws](https://github.com/Maxwell44772029/mars_rover_ros2_ws), which handles all mapping, planning, and control.

Unity publishes:

- `/pointcloud` – Simulated LiDAR
- `/tf` – Rover pose
- `/clock` – Simulated time

ROS2 responds by:

- Subscribing to the topics above
- Planning paths with OctoMap + A*
- Sending velocity commands to `/cmd_vel`

