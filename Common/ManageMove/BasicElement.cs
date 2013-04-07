using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.ManageMove
{
    public class BasicElement : Element
    {
        // Action event
        public event EventKinectMovement OnRightHandOver;
        public event EventKinectMovement OnLeftHandOver;
        public event EventKinectMovement OnTwoHandsOver;
        public event EventKinectMovement OnRightHandUnOver;
        public event EventKinectMovement OnLeftHandUnOver;
        public event EventKinectMovement OnTwoHandsUnOver;
        public event EventKinectMovement OnRightHandPush;
        public event EventKinectMovement OnLeftHandPush;
        public event EventKinectMovement OnTwoHandsPush;
        public event EventKinectMovement OnRightHandUnPush;
        public event EventKinectMovement OnLeftHandUnPush;
        public event EventKinectMovement OnTwoHandsUnPush;

        // Design event
        public event EventKinectMovement OnRightHandOverDesign;
        public event EventKinectMovement OnLeftHandOverDesign;
        public event EventKinectMovement OnTwoHandsOverDesign;
        public event EventKinectMovement OnRightHandNotOverDesign;
        public event EventKinectMovement OnLeftHandNotOverDesign;
        public event EventKinectMovement OnTwoHandsNotOverDesign;
        public event EventKinectMovement OnTwoHandsPushDesign;
        public event EventKinectMovement OnRightHandPushDesign;
        public event EventKinectMovement OnLeftHandPushDesign;

        // Common event
        public event EventKinectMovement OnFollow;
        public event EventKinectMovement OnSkeltonPresent;
        public event EventKinectMovement OnSkeltonAbsent;

        #region Event Caller

        public void callOnRightHandOver()
        {
            if (null != OnRightHandOver)
                OnRightHandOver();
        }

        public void callOnLeftHandOver()
        {
            if (null != OnLeftHandOver)
                OnLeftHandOver();
        }

        public void callOnTwoHandsOver()
        {
            if (null != OnTwoHandsOver)
                OnTwoHandsOver();
        }

        public void callOnRightHandUnOver()
        {
            if (null != OnRightHandUnOver)
                OnRightHandUnOver();
        }

        public void callOnLeftHandUnOver()
        {
            if (null != OnLeftHandUnOver)
                OnLeftHandUnOver();
        }

        public void callOnTwoHandsUnOver()
        {
            if (null != OnTwoHandsUnOver)
                OnTwoHandsUnOver();
        }

        public void callOnRightHandPush()
        {
            if (null != OnRightHandPush)
                OnRightHandPush();
        }

        public void callOnLeftHandPush()
        {
            if (null != OnLeftHandPush)
                OnLeftHandPush();
        }

        public void callOnTwoHandsPush()
        {
            if (null != OnTwoHandsPush)
                OnTwoHandsPush();
        }

        public void callOnRightHandUnPush()
        {
            if (null != OnRightHandUnPush)
                OnRightHandUnPush();
        }

        public void callOnLeftHandUnPush()
        {
            if (null != OnLeftHandUnPush)
                OnLeftHandUnPush();
        }

        public void callOnRightHandOverDesign()
        {
            if (null != OnRightHandOverDesign)
                OnRightHandOverDesign();
        }

        public void callOnLeftHandOverDesign()
        {
            if (null != OnLeftHandOverDesign)
                OnLeftHandOverDesign();
        }

        public void callOnTwoHandsOverDesign()
        {
            if (null != OnTwoHandsOverDesign)
                OnTwoHandsOverDesign();
        }

        public void callOnRightHandNotOverDesign()
        {
            if (null != OnRightHandNotOverDesign)
                OnRightHandNotOverDesign();
        }

        public void callOnLeftHandNotOverDesign()
        {
            if (null != OnLeftHandNotOverDesign)
                OnLeftHandNotOverDesign();
        }

        public void callOnTwoHandsNotOverDesign()
        {
            if (null != OnTwoHandsNotOverDesign)
                OnTwoHandsNotOverDesign();
        }

        public void callOnTwoHandsUnPush()
        {
            if (null != OnTwoHandsUnPush)
                OnTwoHandsUnPush();
        }

        public void callOnRightHandPushDesign()
        {
            if (null != OnRightHandPushDesign)
                OnRightHandPushDesign();
        }

        public void callOnLeftHandPushDesign()
        {
            if (null != OnLeftHandPushDesign)
                OnLeftHandPushDesign();
        }

        public void callOnTwoHandsPushDesign()
        {
            if (null != OnTwoHandsPushDesign)
                OnTwoHandsPushDesign();
        }

        public void callOnSkeltonPresent()
        {
            if (null != OnSkeltonPresent)
                OnSkeltonPresent();
        }

        public void callOnSkeltonAbsent()
        {
            if (null != OnSkeltonAbsent)
                OnSkeltonAbsent();
        }

        public void callOnFollow()
        {
            if (null != OnFollow)
                OnFollow();
        }

        #endregion Event Caller
    }
}