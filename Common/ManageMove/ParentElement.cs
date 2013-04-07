using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.ManageMove {
    public class ParentElement : Element {
        // Slide event
        public event EventKinectMovement OnRightHandSlideRight;
        public event EventKinectMovement OnRightHandSlideLeft;
        public event EventKinectMovement OnRightHandSlideUp;
        public event EventKinectMovement OnRightHandSlideDown;

        public event EventKinectMovement OnLeftHandSlideRight;
        public event EventKinectMovement OnLeftHandSlideLeft;
        public event EventKinectMovement OnLeftHandSlideUp;
        public event EventKinectMovement OnLeftHandSlideDown;

        public event EventKinectMovement OnRotateLeft;
        public event EventKinectMovement OnRotateRight;
        public event EventKinectMovement OnNarrow;
        public event EventKinectMovement OnExtend;

        public void callOnRightHandSlideRight() {
            if (null != OnRightHandSlideRight) {
                OnRightHandSlideRight();
            }
        }
        public void callOnRightHandSlideLeft() {
            if (null != OnRightHandSlideLeft) {
                OnRightHandSlideLeft();
            }
        }
        public void callOnRightHandSlideUp() {
            if (null != OnRightHandSlideUp) {
                OnRightHandSlideUp();
            }
        }
        public void callOnRightHandSlideDown() {
            if (null != OnRightHandSlideDown) {
                OnRightHandSlideDown();
            }
        }
        public void callOnLeftHandSlideRight() {
            if (null != OnLeftHandSlideRight) {
                OnLeftHandSlideRight();
            }
        }
        public void callOnLeftHandSlideLeft() {
            if (null != OnLeftHandSlideLeft) {
                OnLeftHandSlideLeft();
            }
        }
        public void callOnLeftHandSlideUp() {
            if (null != OnLeftHandSlideUp) {
                OnLeftHandSlideUp();
            }
        }
        public void callOnLeftHandSlideDown() {
            if (null != OnLeftHandSlideDown) {
                OnLeftHandSlideDown();
            }
        }

        public void callOnRotateRight() {
            if (null != OnRotateRight) {
                OnRotateRight();
            }
        }

        public void callOnRotateLeft() {
            if (null != OnRotateLeft) {
                OnRotateLeft();
            }
        }

        public void callNarrow() {
            if (null != OnNarrow) {
                OnNarrow();
            }
        }

        public void callExtend() {
            if (null != OnExtend) {
                OnExtend();
            }
        }
    }
}
