using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.ManageMove {
    public class Position {
        private double _x;

        public double X {
            get { return _x; }
            set { _x = value; }
        }

        private double _y;

        public double Y {
            get { return _y; }
            set { _y = value; }
        }

        internal bool bigger(Position maxDecalLeft) {
            return (Math.Abs(X) > Math.Abs(maxDecalLeft.X) ||
                    Math.Abs(Y) > Math.Abs(maxDecalLeft.Y));
        }
    }
}
