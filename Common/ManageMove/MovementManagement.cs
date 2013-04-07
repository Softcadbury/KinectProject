using Microsoft.Kinect;
using Microsoft.Samples.Kinect.WpfViewers;
using System;
using System.IO;
using System.Timers;
using System.Windows;

namespace Common.ManageMove
{
    public class MovementManagement
    {
        #region attributes

        private KinectSensor sensor;
        private const int skeletonCount = 6;
        private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        private Skeleton[] skeletonData;
        public KinectSensorChooser kinectSensorChooser1;
        private bool skeletonPresent = false;
        private Timer timer;
        private double ratioZRight = -1;
        private double ratioZLeft = -1;
        private double kinectZRight = 0;
        private double kinectZLeft = 0;
        private Timer pushManagement = new Timer(1000);
        private Timer pushManagementrHand = new Timer(1000);
        private Timer pushManagementlHand = new Timer(1000);
        private static Timer afterPushRight = null;
        private static Timer afterPushLeft = null;
        private static bool afterPushRightOk = true;
        private static bool afterPushLeftOk = true;

        private Timer slideRefresh = new Timer(2500);
        private bool canPush = true;
        private bool needUpdateRight = true;
        private bool needUpdateLeft = true;
        private Position histoPosRhand = null;
        private Position histoPosLhand = null;
        private Position decalRight = null;
        private Position decalLeft = null;
        private Position maxDecalRight = null;
        private Position maxDecalLeft = null;
        private double diff = 0;
        private double maxDiff = 0;
        private double originDiff = 0;
        private bool canSlideRight = true;
        private bool canSlideLeft = true;
        private bool canSlideUp = true;
        private bool canSlideDown = true;
        private bool canRotateRight = true;
        private bool canRotateLeft = true;
        private bool canExtend = true;
        private bool canNarrow = true;
        public Timer slideTimer = new Timer();
        private Timer extendManagement;
        private Timer narrowManagement;

        private static double lHandX = 0;
        private static double lHandY = 0;
        private static double rHandX = 0;
        private static double rHandY = 0;

        private double screenHeigh = 1024;

        private double screenWidth = 768;

        #endregion attributes

        #region Tools

        public static Thickness getLeftMargin()
        {
            return new Thickness(lHandX, 0, 0, lHandY);
        }

        public static Thickness getRightMargin()
        {
            return new Thickness(rHandX, 0, 0, rHandY);
        }

        #endregion Tools

        #region Constantructors

        public MovementManagement(KinectSensorChooser sensorChooser)
        {
            timer = new Timer(500);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            kinectSensorChooser1 = sensorChooser;
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
            pushManagement.Elapsed += new ElapsedEventHandler(pushManagement_Elapsed);
            pushManagementlHand.Elapsed += new ElapsedEventHandler(pushManagementlHand_Elapsed);
            pushManagementrHand.Elapsed += new ElapsedEventHandler(pushManagementrHand_Elapsed);
            slideRefresh.Elapsed += new ElapsedEventHandler(slideRefresh_Elapsed);

            slideTimer.Elapsed += new ElapsedEventHandler(slideTimer_Elapsed);
            screenHeigh = System.Windows.SystemParameters.PrimaryScreenHeight;
            screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        }

        /*  public MovementManagement(KinectSensorChooser sensorChooser, double screenHeigh, double screenWidth)
          {
              timer = new Timer(500);
              timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
              kinectSensorChooser1 = sensorChooser;
              kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
              pushManagement.Elapsed += new ElapsedEventHandler(pushManagement_Elapsed);
              pushManagementlHand.Elapsed += new ElapsedEventHandler(pushManagementlHand_Elapsed);
              pushManagementrHand.Elapsed += new ElapsedEventHandler(pushManagementrHand_Elapsed);
              slideRefresh.Elapsed += new ElapsedEventHandler(slideRefresh_Elapsed);
              slideTimer.Elapsed += new ElapsedEventHandler(slideTimer_Elapsed);
              this.screenHeigh = screenHeigh;
              this.screenWidth = screenWidth;
          }*/

        #endregion Constantructors

        #region Timers events

        private void afterPushRight_Elapsed(object sender, ElapsedEventArgs e)
        {
            afterPushRightOk = true;
        }

        private void afterPushLeft_Elapsed(object sender, ElapsedEventArgs e)
        {
            afterPushLeftOk = true;
        }

        private void slideTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            canSlideRight = true;
            canSlideLeft = true;
            canSlideUp = true;
            canSlideDown = true;
            canRotateRight = true;
            canRotateLeft = true;
        }

        private void slideRefresh_Elapsed(object sender, ElapsedEventArgs e)
        {
            histoPosLhand = null;
            histoPosRhand = null;
            decalLeft = null;
            decalRight = null;
        }

        private void pushManagementrHand_Elapsed(object sender, ElapsedEventArgs e)
        {
            needUpdateRight = true;
        }

        private void pushManagementlHand_Elapsed(object sender, ElapsedEventArgs e)
        {
            needUpdateLeft = true;
        }

        private void pushManagement_Elapsed(object sender, ElapsedEventArgs e)
        {
            canPush = true;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            skeletonPresent = false;
        }

        #endregion Timers events

        #region Kinect Init

        private void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor old = (KinectSensor)e.OldValue;

            StopKinect(old);

            KinectSensor sensor = (KinectSensor)e.NewValue;

            if (sensor == null)
            {
                return;
            }

            if (e.NewValue != null)
            {
                this.InitializeKinect((KinectSensor)e.NewValue);
            }
        }

        private KinectSensor InitializeKinect(KinectSensor sensor)
        {
            //sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            sensor.SkeletonFrameReady += this.skeletonsReady;

            sensor.SkeletonStream.Enable(new TransformSmoothParameters()
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            });

            try
            {
                sensor.Start();
            }
            catch (IOException)
            {
                kinectSensorChooser1.AppConflictOccurred();
                return null;
            }
            return sensor;
        }

        private void initSlide(double lHandX, double lHandY, double rHandX, double rHandY)
        {
            histoPosLhand = new Position()
            {
                X = lHandX,
                Y = lHandY
            };

            histoPosRhand = new Position()
            {
                X = rHandX,
                Y = rHandY
            };

            decalLeft = new Position()
            {
                X = 0,
                Y = 0
            };

            decalRight = new Position()
            {
                X = 0,
                Y = 0
            };

            diff = 0;
            maxDiff = 0;
            originDiff = rHandX - lHandX;

            maxDecalLeft = decalLeft;
            maxDecalRight = decalRight;
            slideRefresh.Stop();
            slideRefresh.Start();
        }

        #endregion Kinect Init

        private void skeletonsReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (null != skeletonFrame)
                {
                    if ((this.skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                    int indexSkeleton = -1;
                    int i = 0;

                    // Select the closest person
                    foreach (Skeleton skeleton in this.skeletonData)
                    {
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            if (indexSkeleton != -1)
                            {
                                if (this.skeletonData[indexSkeleton].Position.Z > skeleton.Position.Z)
                                {
                                    indexSkeleton = i;
                                }
                            }
                            else
                            {
                                indexSkeleton = i;
                            }
                        }
                        ++i;
                    }

                    // if a skeleton is find use it
                    if (-1 == indexSkeleton)
                    {
                        skeletonPresent = false;
                        foreach (BasicElement elt in ElementRegister.getInstance().Element)
                        {
                            elt.callOnSkeltonAbsent();
                        }
                    }
                    else
                    {
                        skeletonPresent = true;
                        if (ElementRegister.getInstance().NeedReset)
                        {
                            needUpdateLeft = true;
                            needUpdateRight = true;
                            ElementRegister.getInstance().NeedReset = false;
                        }
                        Skeleton skeleton = this.skeletonData[indexSkeleton];
                        Joint scaleLeft = scaleJoint(skeleton.Joints[JointType.HandLeft], screenWidth, screenHeigh, 1f, 1f);
                        Joint scaleRight = scaleJoint(skeleton.Joints[JointType.HandRight], screenWidth, screenHeigh, 1f, 1f);
                        lHandX = scaleLeft.Position.X;
                        lHandY = scaleLeft.Position.Y;
                        rHandX = scaleRight.Position.X;
                        rHandY = scaleRight.Position.Y;
                        if (null == histoPosLhand || null == histoPosRhand)
                        {
                            initSlide(lHandX, lHandY, rHandX, rHandY);
                        }

                        // generate the new decal
                        Position newDecalRight = new Position()
                        {
                            X = histoPosRhand.X - rHandX,
                            Y = histoPosRhand.Y - rHandY
                        };
                        Position newDecalLeft = new Position()
                        {
                            X = histoPosLhand.X - lHandX,
                            Y = histoPosLhand.Y - lHandY
                        };

                        #region slide Management

                        // Test si retour position
                        if ((Math.Abs(histoPosRhand.X - rHandX) < 50 && (Math.Abs(histoPosRhand.Y - rHandY) < 50)) ||
                            (Math.Abs(histoPosLhand.X - lHandX) < 50 && (Math.Abs(histoPosLhand.Y - lHandY) < 50)))
                        {
                            canSlideRight = true;
                            canSlideLeft = true;
                            canSlideUp = true;
                            canSlideDown = true;
                            canRotateRight = true;
                            canRotateLeft = true;
                        }
                        if ((Math.Abs(decalLeft.X - newDecalLeft.X) > 50 && Math.Abs(decalLeft.Y - newDecalLeft.Y) > 50) ||
                            (Math.Abs(decalRight.X - newDecalRight.X) > 50 && Math.Abs(decalRight.Y - newDecalRight.Y) > 50))
                        {
                            // not a slide movement
                            //initSlide(lHandX, lHandY, rHandX, rHandY);
                        }
                        else
                        {
                            double diffRightX = decalRight.X - newDecalRight.X;
                            double diffRightY = decalRight.Y - newDecalRight.Y;
                            double diffLeftX = decalLeft.X - newDecalLeft.X;
                            double diffLeftY = decalLeft.Y - newDecalLeft.Y;

                            diff = rHandX - lHandX;
                            if (diff > maxDiff)
                            {
                                maxDiff = diff;
                            }

                            if (newDecalLeft.bigger(maxDecalLeft))
                            {
                                maxDecalLeft = newDecalLeft;
                            }
                            if (newDecalRight.bigger(maxDecalRight))
                            {
                                maxDecalRight = newDecalRight;
                            }

                            ParentElement parent = ElementRegister.getInstance().Parent;
                            if (originDiff > 300 && diff < Constant.SlideRatio * 2 && canNarrow && canExtend)
                            {
                                parent.callNarrow();
                                canExtend = false;
                                canNarrow = false;
                                narrowManagement = new Timer(2000);
                                narrowManagement.Elapsed += new ElapsedEventHandler(narrowManagement_Elapsed);
                                narrowManagement.AutoReset = false;
                                narrowManagement.Start();
                                initSlide(lHandX, lHandY, rHandX, rHandY);
                            }
                            else if (originDiff < Constant.SlideRatio * 2 && diff > 300 && canExtend && canNarrow)
                            {
                                parent.callExtend();
                                canExtend = false;
                                canNarrow = false;
                                narrowManagement = new Timer(2000);
                                narrowManagement.Elapsed += new ElapsedEventHandler(narrowManagement_Elapsed);
                                narrowManagement.AutoReset = false;
                                narrowManagement.Start();
                                initSlide(lHandX, lHandY, rHandX, rHandY);
                            }
                            else if (diffLeftY > Constant.SlideRatio && diffRightY < -Constant.SlideRatio && canRotateRight)
                            {
                                parent.callOnRotateRight();
                                canRotateLeft = false;
                                initSlide(lHandX, lHandY, rHandX, rHandY);
                            }
                            else if (diffLeftY < -Constant.SlideRatio && diffRightY > Constant.SlideRatio && canRotateLeft)
                            {
                                parent.callOnRotateLeft();
                                canRotateRight = false;
                                initSlide(lHandX, lHandY, rHandX, rHandY);
                            }
                            else
                            {
                                if (diffLeftX < -Constant.SlideRatio && canSlideLeft)
                                {
                                    parent.callOnLeftHandSlideLeft();
                                    canSlideRight = false;
                                    initSlide(lHandX, lHandY, rHandX, rHandY);
                                }
                                else if (diffLeftX > Constant.SlideRatio && canSlideRight)
                                {
                                    parent.callOnLeftHandSlideRight();
                                    canSlideLeft = false;
                                    initSlide(lHandX, lHandY, rHandX, rHandY);
                                }
                                else if (diffLeftY < -Constant.SlideRatio && canSlideDown)
                                {
                                    parent.callOnLeftHandSlideDown();
                                    canSlideUp = false;
                                    initSlide(lHandX, lHandY, rHandX, rHandY);
                                }
                                else if (diffLeftY > Constant.SlideRatio && canSlideUp)
                                {
                                    parent.callOnLeftHandSlideUp();
                                    canSlideDown = false;
                                    initSlide(lHandX, lHandY, rHandX, rHandY);
                                }

                                if (diffRightX < -Constant.SlideRatio && canSlideLeft)
                                {
                                    parent.callOnRightHandSlideLeft();
                                    canSlideRight = false;
                                    initSlide(lHandX, lHandY, rHandX, rHandY);
                                }
                                else if (diffRightX > Constant.SlideRatio && canSlideRight)
                                {
                                    parent.callOnRightHandSlideRight();
                                    canSlideLeft = false;
                                    initSlide(lHandX, lHandY, rHandX, rHandY);
                                }
                                else if (diffRightY < -Constant.SlideRatio && canSlideDown)
                                {
                                    parent.callOnRightHandSlideDown();
                                    canSlideUp = false;
                                    initSlide(lHandX, lHandY, rHandX, rHandY);
                                }
                                else if (diffRightY > Constant.SlideRatio && canSlideUp)
                                {
                                    parent.callOnRightHandSlideUp();
                                    canSlideDown = false;
                                    initSlide(lHandX, lHandY, rHandX, rHandY);
                                }
                            }
                        }

                        #endregion slide Management

                        kinectZLeft = skeleton.Joints[JointType.HandLeft].Position.Z;
                        kinectZRight = skeleton.Joints[JointType.HandRight].Position.Z;

                        #region loop for the movements

                        if (skeleton.Joints.Count > 0 && skeleton.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked)
                        {
                            timer.Stop();
                            timer.Start();

                            // etude du contact avec les éléments

                            foreach (BasicElement elt in ElementRegister.getInstance().ConstElement)
                            {
                                elt.callOnFollow();
                            }
                            foreach (BasicElement elt in ElementRegister.getInstance().Element)
                            {
                                bool rightHandOver = false;
                                bool rightHandPush = false;
                                bool leftHandOver = false;
                                bool leftHandPush = false;
                                if (lHandX >= elt.PosX && lHandX <= elt.PosX + elt.Width
                                    && lHandY >= elt.PosY && lHandY <= elt.PosY + elt.Height)
                                {
                                    leftHandOver = true;

                                    if (needUpdateLeft)
                                    {
                                        ratioZLeft = kinectZLeft;
                                        needUpdateLeft = false;
                                        pushManagementlHand.Start();
                                        leftHandPush = false;
                                    }
                                    else if (afterPushLeftOk && kinectZLeft <= ratioZLeft - Constant.DistMoveToPush)
                                    {
                                        leftHandPush = true;
                                    }
                                }
                                if (rHandX >= elt.PosX && rHandX <= elt.PosX + elt.Width
                                    && rHandY >= elt.PosY && rHandY <= elt.PosY + elt.Height)
                                {
                                    rightHandOver = true;

                                    if (needUpdateRight)
                                    {
                                        ratioZRight = kinectZRight;
                                        needUpdateRight = false;
                                        pushManagementrHand.Start();
                                        rightHandPush = false;
                                    }
                                    else if (afterPushRightOk && kinectZRight <= ratioZRight - Constant.DistMoveToPush)
                                    {
                                        rightHandPush = true;
                                    }
                                }

                                if (leftHandOver && rightHandOver)
                                {
                                    elt.callOnTwoHandsOverDesign();
                                }
                                else
                                {
                                    elt.callOnTwoHandsNotOverDesign();
                                    if (leftHandOver)
                                    {
                                        elt.callOnRightHandNotOverDesign();
                                        elt.callOnLeftHandOverDesign();
                                        if (elt.canOverLeft)
                                        {
                                            elt.callOnLeftHandOver();
                                            elt.canOverLeft = false;
                                        }
                                        if (!elt.canOverRight)
                                        {
                                            elt.callOnTwoHandsUnOver();
                                            elt.callOnRightHandUnOver();
                                            elt.canOverRight = true;
                                        }
                                    }
                                    else if (rightHandOver)
                                    {
                                        elt.callOnLeftHandNotOverDesign();
                                        elt.callOnRightHandOverDesign();
                                        if (!elt.canOverLeft)
                                        {
                                            elt.callOnTwoHandsUnOver();
                                            elt.callOnLeftHandUnOver();
                                            elt.canOverLeft = true;
                                        }
                                        if (elt.canOverRight)
                                        {
                                            elt.callOnRightHandOver();
                                            elt.canOverRight = false;
                                        }
                                    }
                                    else
                                    {
                                        elt.callOnLeftHandNotOverDesign();
                                        elt.callOnRightHandNotOverDesign();
                                        if (!elt.canOverLeft)
                                        {
                                            elt.callOnTwoHandsUnOver();
                                            elt.callOnLeftHandUnOver();
                                            elt.canOverLeft = true;
                                        }
                                        if (!elt.canOverRight)
                                        {
                                            elt.callOnTwoHandsUnOver();
                                            elt.callOnRightHandUnOver();
                                            elt.canOverRight = true;
                                        }
                                    }
                                }
                                if (afterPushRightOk && afterPushLeftOk)
                                {
                                    if (rightHandPush && leftHandPush)
                                    {
                                        elt.callOnTwoHandsPushDesign();
                                    }
                                    else if (rightHandPush)
                                    {
                                        elt.callOnRightHandPushDesign();
                                        if (elt.canPushRight && afterPushRightOk)
                                        {
                                            elt.canPushRight = false;
                                            elt.callOnRightHandPush();
                                            afterPushRightOk = false;
                                            if (null != afterPushRight)
                                                afterPushRight.Stop();
                                            afterPushRight = new Timer(1500);
                                            afterPushRight.Elapsed += new ElapsedEventHandler(afterPushRight_Elapsed);
                                            afterPushRight.Start();
                                            break;
                                        }
                                        if (!elt.canPushLeft)
                                        {
                                            elt.callOnTwoHandsUnOver();
                                            elt.canPushLeft = true;
                                            elt.callOnLeftHandUnPush();
                                        }
                                    }
                                    else if (leftHandPush)
                                    {
                                        elt.callOnLeftHandPushDesign();
                                        if (!elt.canPushRight)
                                        {
                                            elt.callOnTwoHandsUnOver();
                                            elt.canPushRight = true;
                                            elt.callOnRightHandUnPush();
                                        }
                                        if (elt.canPushLeft && afterPushLeftOk)
                                        {
                                            elt.canPushLeft = false;
                                            elt.callOnLeftHandPush();
                                            afterPushLeftOk = false;
                                            if (null != afterPushLeft)
                                                afterPushLeft.Stop();
                                            afterPushLeft = new Timer(1500);
                                            afterPushLeft.Elapsed += new ElapsedEventHandler(afterPushLeft_Elapsed);
                                            afterPushLeft.Start();
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (!elt.canPushLeft)
                                        {
                                            elt.callOnTwoHandsUnOver();
                                            elt.canPushLeft = true;
                                            elt.callOnLeftHandUnPush();
                                            break;
                                        }
                                        if (!elt.canPushRight)
                                        {
                                            elt.callOnTwoHandsUnOver();
                                            elt.canPushRight = true;
                                            elt.callOnRightHandUnPush();
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        // skelette prsent ou pas
                        if (skeletonPresent)
                        {
                            foreach (BasicElement elt in ElementRegister.getInstance().Element)
                            {
                                elt.callOnSkeltonPresent();
                            }
                        }
                        else
                        {
                            foreach (BasicElement elt in ElementRegister.getInstance().Element)
                            {
                                elt.callOnSkeltonAbsent();
                            }
                        }
                    }
                }

                        #endregion loop for the movements
            }
        }

        private Joint scaleJoint(Joint joint, double width, double height, float skeletonMaxX, float skeletonMaxY)
        {
            SkeletonPoint pos = new SkeletonPoint()
            {
                X = Scale(Convert.ToInt32(width), skeletonMaxX, joint.Position.X),
                Y = Scale(Convert.ToInt32(height), skeletonMaxY, joint.Position.Y),
                Z = joint.Position.Z
            };
            Joint j = new Joint();
            j.TrackingState = joint.TrackingState;
            j.Position = pos;
            return j;
        }

        private float Scale(int maxPixel, float maxSkeleton, float position)
        {
            float scaleFactor = (float)Math.Min(screenWidth, screenHeigh / 2);
            float value = ((scaleFactor * position) + (maxPixel / 2));

            if (value > maxPixel)
                return maxPixel;
            if (value < 0)
                return 0;
            return value;
        }

        private void narrowManagement_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.canNarrow = true;
            this.canExtend = true;
        }

        private void extendManagement_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.canExtend = true;
        }

        private double _ratioX = 01;

        public double RatioX
        {
            get { return screenWidth * _ratioX; }
            set { _ratioX = value; }
        }

        private double _ratioY = 1;

        public double RatioY
        {
            get { return screenHeigh * _ratioY; }
            set { _ratioY = value; }
        }

        public void setRatioRight()
        {
        }

        public void setRatioLeft()
        {
        }

        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    //stop sensor
                    sensor.Stop();

                    //stop audio if not null
                    if (sensor.AudioSource != null)
                    {
                        sensor.AudioSource.Stop();
                    }
                }
            }
        }
    }
}