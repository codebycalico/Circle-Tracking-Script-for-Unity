# Circle Tracking Script for Unity

This is a C# script for Unity that incorporates OpenCV for Unity (the free assets package in the asset store).

It uses a webcam and OpenCV to search for circles and track them, moving the mouse's position to the center of the circle's coordinates. There is also a boundary to check within, to only track the circle and move the mouse if the center coordinates of the detected circle lie within the set boundaries.

To utilize this script, create a new project (or incorporate it into an existing project) and create (or use) a Raw Image UI object in the scene. Attach the script to the Raw Image, and assign the "Surface" GameObject to the Raw Image that the script is attached to.