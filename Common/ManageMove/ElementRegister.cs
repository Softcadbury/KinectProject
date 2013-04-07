using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.ManageMove
{
    public class ElementRegister
    {
        private static ParentElement parent = new ParentElement();
        private static List<BasicElement> _elements = new List<BasicElement>();
        private static ElementRegister instance;

        private bool needReset = false;

        public bool NeedReset {
            get { return needReset; }
            set { needReset = value; }
        }
        


        public void reset()
        {
            parent = new ParentElement();
            _elements = new List<BasicElement>();
            needReset = true;
        }

        public ParentElement Parent
        {
            get
            {
                return parent;
            }
        }

        public List<BasicElement> Element
        {
            get
            {
                return _elements;
            }
        }

        private List<BasicElement> _constElement = new List<BasicElement>();

        public List<BasicElement> ConstElement
        {
            get { return _constElement; }
            set { _constElement = value; }
        }

        private ElementRegister()
        {
        }

        public static ElementRegister getInstance()
        {
            if (null == instance)
            {
                instance = new ElementRegister();
            }
            return instance;
        }

        public ParentElement registerParent(int x, int y)
        {
            ParentElement res = new ParentElement()
            {
                PosX = x,
                PosY = y
            };
            parent = res;
            return res;
        }

        public Element registerParent(ParentElement parentToAdd)
        {
            parent = parentToAdd;
            return parent;
        }

        public Element registerElement(BasicElement elt)
        {
            _elements.Add(elt);
            return elt;
        }

        public Element registerElement(int x, int y, int height, int width)
        {
            BasicElement res = new BasicElement()
            {
                PosX = x,
                PosY = y,
                Height = height,
                Width = width,
            };
            _elements.Add(res);
            return res;
        }

        public void registerConstElement(BasicElement elt)
        {
            _constElement.Add(elt);
        }

        public Element isOnElement(int x, int y)
        {
            Element res = null;
            foreach (Element elem in _elements)
            {
                if (x >= elem.PosX && x <= elem.PosX + elem.Width && y >= elem.PosY && y <= elem.PosY + elem.Height)
                {
                    res = elem;
                }
            }
            return res;
        }
    }
}