using Common;
using Common.ManageMove;
using Microsoft.Kinect;
using ModuleStart.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ModuleStart.Views
{
    /// <summary>
    /// Interaction logic for ModuleStartView.xaml
    /// </summary>
    public partial class ModuleStartView : UserControl
    {
        private const int TimerResolution = 2;  // ms
        private const int NumIntraFrames = 3;
        private const int MaxShapes = 80;
        private const double MaxFramerate = 70;
        private const double MinFramerate = 15;
        private const double MinShapeSize = 12;
        private const double MaxShapeSize = 90;
        private const double DefaultDropRate = 2.8;
        private const double DefaultDropSize = 32.0;
        private const double DefaultDropGravity = 1.0;
        private double dropRate = DefaultDropRate;
        private double dropSize = DefaultDropSize;
        private double dropGravity = DefaultDropGravity;
        private DateTime lastFrameDrawn = DateTime.MinValue;
        private DateTime predNextFrame = DateTime.MinValue;
        private double actualFrameTime;
        private FallingThings myFallingThings;
        private readonly Dictionary<int, Player> players = new Dictionary<int, Player>();
        private double targetFramerate = MaxFramerate;
        private int frameCount;
        private bool runningGameThread;
        private const int skeletonCount = 6;
        private Rect playerBounds;
        private Rect screenRect;
        private Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        private Skeleton[] skeletonData;
        private int playersAlive;
        private MovementManagement movementManager;
        private System.Timers.Timer changeShape;
        private static int counter = 0;

        public ModuleStartView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ModuleStartViewModel();
            KinectSensorChooserSub.KinectSensorChanged += new DependencyPropertyChangedEventHandler(KinectSensorChooserSub_KinectSensorChanged);
            movementManager = new MovementManagement(this.KinectSensorChooserSub);
            Playfield.ClipToBounds = true;

            changeShape = new System.Timers.Timer(13500);
            changeShape.Elapsed += new ElapsedEventHandler(changeShape_Elapsed);
            changeShape.Start();
            /* Rect rect = new Rect(
                         0.4 * System.Windows.SystemParameters.PrimaryScreenWidth,
                         0.25 * System.Windows.SystemParameters.PrimaryScreenHeight,
                         0.1 * System.Windows.SystemParameters.PrimaryScreenWidth,
                         0.72 * System.Windows.SystemParameters.PrimaryScreenHeight);
             */

            //BannerText.NewBanner("Images", rect, true, System.Windows.Media.Color.FromRgb(255, 255, 255));
            this.myFallingThings = new FallingThings(MaxShapes, this.targetFramerate, NumIntraFrames);

            this.UpdatePlayfieldSize();

            this.myFallingThings.SetGravity(this.dropGravity);
            this.myFallingThings.SetDropRate(this.dropRate);
            this.myFallingThings.SetSize(this.dropSize);
            this.myFallingThings.SetPolies(PolyType.Square);
            this.myFallingThings.SetGameMode(GameMode.Off);

            TimeBeginPeriod(TimerResolution);
            var myGameThread = new Thread(this.GameThread);
            myGameThread.SetApartmentState(ApartmentState.STA);
            myGameThread.Start();

            this.Dispatcher.Invoke((Action)(() =>
            {
                ImageToBreak.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\Ressources\Document.png"));
            }));
            FallingThings.destroyType = Model.FallingThings.TypeOfShape.Texte;
        }

        private void changeShape_Elapsed(object sender, ElapsedEventArgs e)
        {
            counter++;

            //string toDisplay = "";
            if (counter % 4 == 0)
            {
                //toDisplay = "Musique";
                this.Dispatcher.Invoke((Action)(() =>
                {
                    ImageToBreak.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\Ressources\Head Phone.png"));
                }));

                FallingThings.destroyType = Model.FallingThings.TypeOfShape.Music;
            }
            else if (counter % 4 == 1)
            {
                //toDisplay = "Vidéo";
                this.Dispatcher.Invoke((Action)(() =>
                {
                    ImageToBreak.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\Ressources\Camera.png"));
                }));
                FallingThings.destroyType = Model.FallingThings.TypeOfShape.Video;
            }
            else if (counter % 4 == 2)
            {
                //toDisplay = "Documents";
                this.Dispatcher.Invoke((Action)(() =>
                {
                    ImageToBreak.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\Ressources\Document.png"));
                }));
                FallingThings.destroyType = Model.FallingThings.TypeOfShape.Texte;
            }
            else
            {
                //toDisplay = "Images";
                this.Dispatcher.Invoke((Action)(() =>
                {
                    ImageToBreak.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\Ressources\Video Camera.png"));
                }));
                FallingThings.destroyType = Model.FallingThings.TypeOfShape.Image;
            }
            /* Rect rect = new Rect(
                         0.4 * System.Windows.SystemParameters.PrimaryScreenWidth,
                         0.25 * System.Windows.SystemParameters.PrimaryScreenHeight,
                         0.1 * System.Windows.SystemParameters.PrimaryScreenWidth,
                         0.72 * System.Windows.SystemParameters.PrimaryScreenHeight);
             BannerText.NewBanner(toDisplay, rect, true, System.Windows.Media.Color.FromRgb(255, 255, 255));*/
        }

        private void KinectSensorChooserSub_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        [DllImport("Winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern int TimeBeginPeriod(uint period);

        private void UpdatePlayfieldSize()
        {
            // Size of player wrt size of Playfield, putting ourselves low on the screen.
            this.screenRect.X = 0;
            this.screenRect.Y = 0;
            this.screenRect.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.screenRect.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            Playfield.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            Playfield.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            BannerText.UpdateBounds(this.screenRect);

            this.playerBounds.X = 0;
            this.playerBounds.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.playerBounds.Y = 0;
            this.playerBounds.Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            foreach (var player in this.players)
            {
                player.Value.SetBounds(this.playerBounds);
            }

            Rect fallingBounds = this.playerBounds;
            fallingBounds.Y = 0;
            fallingBounds.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            fallingBounds.X = 0;
            fallingBounds.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            if (this.myFallingThings != null)
            {
                this.myFallingThings.SetBoundaries(fallingBounds);
            }
        }

        #region falling Shape

        private void GameThread()
        {
            this.runningGameThread = true;
            this.predNextFrame = DateTime.Now;
            this.actualFrameTime = 1000.0 / this.targetFramerate;

            // Try to dispatch at as constant of a framerate as possible by sleeping just enough since
            // the last time we dispatched.
            while (this.runningGameThread)
            {
                // Calculate average framerate.
                DateTime now = DateTime.Now;
                if (this.lastFrameDrawn == DateTime.MinValue)
                {
                    this.lastFrameDrawn = now;
                }

                double ms = now.Subtract(this.lastFrameDrawn).TotalMilliseconds;
                this.actualFrameTime = (this.actualFrameTime * 0.95) + (0.05 * ms);
                this.lastFrameDrawn = now;

                // Adjust target framerate down if we're not achieving that rate
                this.frameCount++;
                if ((this.frameCount % 100 == 0) && (1000.0 / this.actualFrameTime < this.targetFramerate * 0.92))
                {
                    this.targetFramerate = Math.Max(MinFramerate, (this.targetFramerate + (1000.0 / this.actualFrameTime)) / 2);
                }

                if (now > this.predNextFrame)
                {
                    this.predNextFrame = now;
                }
                else
                {
                    double milliseconds = this.predNextFrame.Subtract(now).TotalMilliseconds;
                    if (milliseconds >= TimerResolution)
                    {
                        Thread.Sleep((int)(milliseconds + 0.5));
                    }
                }

                this.predNextFrame += TimeSpan.FromMilliseconds(1000.0 / this.targetFramerate);

                this.Dispatcher.Invoke(DispatcherPriority.Send, new Action<int>(this.HandleGameTimer), 0);
            }
        }

        private void CheckPlayers()
        {
            foreach (var player in this.players)
            {
                if (!player.Value.IsAlive)
                {
                    // Player left scene since we aren't tracking it anymore, so remove from dictionary
                    this.players.Remove(player.Value.GetId());
                    break;
                }
            }

            // Count alive players
            int alive = this.players.Count(player => player.Value.IsAlive);

            if (alive != this.playersAlive)
            {
                if (alive == 2)
                {
                    this.myFallingThings.SetGameMode(GameMode.TwoPlayer);
                }
                else if (alive == 1)
                {
                    this.myFallingThings.SetGameMode(GameMode.Solo);
                }
                else if (alive == 0)
                {
                    this.myFallingThings.SetGameMode(GameMode.Off);
                }

                this.playersAlive = alive;
            }
        }

        private void HandleGameTimer(int param)
        {
            // Every so often, notify what our actual framerate is
            if ((this.frameCount % 100) == 0)
            {
                this.myFallingThings.SetFramerate(1000.0 / this.actualFrameTime);
            }

            // Advance animations, and do hit testing.
            for (int i = 0; i < NumIntraFrames; ++i)
            {
                foreach (var pair in this.players)
                {
                    HitType hit = this.myFallingThings.LookForHits(pair.Value.Segments, pair.Value.GetId());
                }
                this.myFallingThings.AdvanceFrame();
            }

            // Draw new Wpf scene by adding all objects to canvas
            Playfield.Children.Clear();
            this.myFallingThings.DrawFrame(this.Playfield.Children);

            BannerText.Draw(this.Playfield.Children);
            foreach (var player in this.players)
            {
                player.Value.Draw(Playfield.Children);
            }
            this.CheckPlayers();
        }

        #endregion falling Shape

        private KinectSensor InitializeKinect(KinectSensor sensor)
        {
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
                KinectSensorChooserSub.AppConflictOccurred();
                return null;
            }

            return sensor;
        }

        private void skeletonsReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            try
            {
                using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                {
                    if (null != skeletonFrame)
                    {
                        if ((this.skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                        {
                            this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                        }
                        int skeletonSlot = 0;
                        skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                        foreach (Skeleton skeleton in this.skeletonData)
                        {
                            if (SkeletonTrackingState.Tracked == skeleton.TrackingState)
                            {
                                Player player;
                                if (this.players.ContainsKey(skeletonSlot))
                                {
                                    player = this.players[skeletonSlot];
                                }
                                else
                                {
                                    player = new Player(skeletonSlot);
                                    player.SetBounds(this.playerBounds);
                                    this.players.Add(skeletonSlot, player);
                                }

                                player.LastUpdated = DateTime.Now;

                                // Update player's bone and joint positions
                                if (skeleton.Joints.Count > 0)
                                {
                                    player.IsAlive = true;

                                    // Head, hands, feet (hit testing happens in order here)
                                    player.UpdateJointPosition(skeleton.Joints, JointType.Head);
                                    player.UpdateJointPosition(skeleton.Joints, JointType.HandLeft);
                                    player.UpdateJointPosition(skeleton.Joints, JointType.HandRight);
                                    player.UpdateJointPosition(skeleton.Joints, JointType.FootLeft);
                                    player.UpdateJointPosition(skeleton.Joints, JointType.FootRight);

                                    // Hands and arms
                                    player.UpdateBonePosition(skeleton.Joints, JointType.HandRight, JointType.WristRight);
                                    player.UpdateBonePosition(skeleton.Joints, JointType.WristRight, JointType.ElbowRight);
                                    player.UpdateBonePosition(skeleton.Joints, JointType.ElbowRight, JointType.ShoulderRight);

                                    player.UpdateBonePosition(skeleton.Joints, JointType.HandLeft, JointType.WristLeft);
                                    player.UpdateBonePosition(skeleton.Joints, JointType.WristLeft, JointType.ElbowLeft);
                                    player.UpdateBonePosition(skeleton.Joints, JointType.ElbowLeft, JointType.ShoulderLeft);

                                    // Head and Shoulders
                                    player.UpdateBonePosition(skeleton.Joints, JointType.ShoulderCenter, JointType.Head);
                                    player.UpdateBonePosition(skeleton.Joints, JointType.ShoulderLeft, JointType.ShoulderCenter);
                                    player.UpdateBonePosition(skeleton.Joints, JointType.ShoulderCenter, JointType.ShoulderRight);

                                    // Legs
                                    player.UpdateBonePosition(skeleton.Joints, JointType.HipLeft, JointType.KneeLeft);
                                    player.UpdateBonePosition(skeleton.Joints, JointType.KneeLeft, JointType.AnkleLeft);
                                    player.UpdateBonePosition(skeleton.Joints, JointType.AnkleLeft, JointType.FootLeft);

                                    player.UpdateBonePosition(skeleton.Joints, JointType.HipRight, JointType.KneeRight);
                                    player.UpdateBonePosition(skeleton.Joints, JointType.KneeRight, JointType.AnkleRight);
                                    player.UpdateBonePosition(skeleton.Joints, JointType.AnkleRight, JointType.FootRight);

                                    player.UpdateBonePosition(skeleton.Joints, JointType.HipLeft, JointType.HipCenter);
                                    player.UpdateBonePosition(skeleton.Joints, JointType.HipCenter, JointType.HipRight);

                                    // Spine
                                    player.UpdateBonePosition(skeleton.Joints, JointType.HipCenter, JointType.ShoulderCenter);
                                }
                            }

                            skeletonSlot++;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    sensor.Stop();
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // ((StartModule.ViewModels.StartModuleViewModel)DataContext).GoButton = this.ButtonGo;
            // ((StartModule.ViewModels.StartModuleViewModel)DataContext).InitView();
        }
    }
}