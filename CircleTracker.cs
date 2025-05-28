using OpenCvSharp.Demo;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;
using System.Xml.Serialization;
//using NUnit.Framework.Constraints;
//using Unity.VisualScripting;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CircleTracker : WebCamera
{
    [SerializeField] private FlipMode ImageFlip;
    [SerializeField] private float Threshold = 240f;
    [SerializeField] private bool ShowProcessedImage = true;
    [SerializeField] private float CurveAccuracy = 10.0f;
    [SerializeField] private float MinArea = 5000f;

    private Mat image;
    private Mat processImage = new Mat();

    private Point[][] contours;
    private HierarchyIndex[] hierarchy;

    private CircleSegment[] circles;
    private CircleSegment chosen = new CircleSegment();
    private CircleSegment prevCircle = new CircleSegment();

    private double param1 = 100;
    private double param2 = 50;
    private int minRadius = 25;
    private int maxRadius = 75;

    private Vector2 newMousePos = new Vector2();

    private Point2f boundingCenter = new Point2f();
    private float boundingRadius = new float();
    private CircleSegment boundingCircle = new CircleSegment();

    private float upperBoundX = 1429f;
    private float upperBoundY = 786f;
    private float lowerBoundX = 618f;
    private float lowerBoundY = 228f;

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        boundingCenter.X = 9f;
        boundingCenter.Y = 10f;
        boundingRadius = 10f;

        boundingCircle.Radius = boundingRadius;
        boundingCircle.Center = boundingCenter;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(Mouse.current.position.ReadValue());
        }

        image = OpenCvSharp.Unity.TextureToMat(input);

        // read and write to same image
        Cv2.Flip(image, image, ImageFlip);
        Cv2.CvtColor(image, processImage, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(processImage, processImage, Threshold, 255, ThresholdTypes.BinaryInv);
        Cv2.MedianBlur(processImage, processImage, 5);

        circles = Cv2.HoughCircles(processImage, HoughMethods.Gradient, 1.2,
                    700, param1, param2, minRadius, maxRadius);

        if (circles != null)
        {
            chosen.Radius = 0;
            prevCircle.Radius = 0;

            foreach (CircleSegment circle in circles)
            {
                // check here if circle is within the boundaries
                // before we assign chosen to anything
                if (withinBounds(circle.Center.X, circle.Center.Y))
                {
                    if (chosen.Radius == 0 || prevCircle.Radius != 0)
                    {
                        chosen = circle;
                    }

                    // if the new circle detected isn't in the exact same 
                    // position as the previous circle (ie it's not detecting
                    // the same circle again), draw circle and move mouse to
                    // center of the circle, then assign prevCircle to keep
                    // track of the circle that was just drawn.
                    if (chosen.Center != prevCircle.Center && chosen.Radius != 0)
                    {
                        Cv2.Circle(processImage, chosen.Center, (int)chosen.Radius, new Scalar(127, 127, 127), 1);
                        newMousePos.x = chosen.Center.X;
                        newMousePos.y = chosen.Center.Y;
                        Mouse.current.WarpCursorPosition(newMousePos);
                        prevCircle = chosen;
                    }
                }
            }
        }

        //Cv2.FindContours(processImage, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);

        //foreach (Point[] contour in contours)
        //{
        //    Point[] points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true);
        //    var area = Cv2.ContourArea(contour);

        //    if (area > MinArea)
        //    {
        //        drawContour(processImage, new Scalar(127, 127, 127), 2, points);
        //    }
        //}

        //only doing once, if output is null
        //only once as it will take up too much memory
        //to do this every time, otherwise if output is
        //not null, override the object that already exists
        if (output == null)
        {
            // format for statement in ()
            // (if var is true ? then do var : else do var)
            output = OpenCvSharp.Unity.MatToTexture(ShowProcessedImage ? processImage : image);
        }
        else
        {
            OpenCvSharp.Unity.MatToTexture(ShowProcessedImage ? processImage : image, output);
        }
        return true;
    }

    //private void drawContour(Mat Image, Scalar Color, int Thickness, Point[] Points)
    //{
    //    for (int i = 1; i < Points.Length; i++)
    //    {
    //        // Connect all the points
    //        Cv2.Line(Image, Points[i - 1], Points[i], Color, Thickness);
    //    }
    //    // Close the shape, connect first and last points
    //    Cv2.Line(Image, Points[Points.Length - 1], Points[0], Color, Thickness);
    //}

    private bool withinBounds(float checkX, float checkY)
    {
        if (lowerBoundX < checkX && checkX < upperBoundX &&
            lowerBoundY < checkY && checkY < upperBoundY)
        {
            return true;
        }
        return false;
    }
}
