using Common;
using Common.ManageMove;
using System;
using System.Windows;

namespace KinectProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();

            BasicElement rightHand = new BasicElement()
            {
                ToBeMove = true
            };
            rightHand.OnFollow += new Element.EventKinectMovement(OnFollowRightHand);

            BasicElement leftHand = new BasicElement()
            {
                ToBeMove = true
            };
            leftHand.OnFollow += new Element.EventKinectMovement(OnFollowLeftHand);

            ElementRegister.getInstance().registerConstElement(rightHand);
            ElementRegister.getInstance().registerConstElement(leftHand);

            _movementManager = new MovementManagement(this.KinectSensorChooser);
        }

        public MovementManagement _movementManager;

        private void OnFollowRightHand()
        {
            RightHand.Margin = MovementManagement.getRightMargin();
            RightHand.Visibility = System.Windows.Visibility.Visible;
        }

        private void OnFollowLeftHand()
        {
            LeftHand.Margin = MovementManagement.getLeftMargin();
            LeftHand.Visibility = System.Windows.Visibility.Visible;
        }
    }
}