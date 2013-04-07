using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.ManageMove
{
    public abstract class Element
    {
        public bool canPushRight = true;
        public bool canPushLeft = true;
        public bool canOverRight = true;
        public bool canOverLeft = true;

        private double _posX;

        private bool _toBeMove = false;

        public bool ToBeMove
        {
            get { return _toBeMove; }
            set { _toBeMove = value; }
        }

        public double PosX
        {
            get
            {
                return _posX;
            }
            set { _posX = value; }
        }

        private double _posY;

        public double PosY
        {
            get
            {
                return _posY;
            }
            set { _posY = value; }
        }

        private double _height;

        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        private double _width;

        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public delegate void EventKinectMovement();
    }
}