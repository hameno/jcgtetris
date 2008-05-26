using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Tetris
{
    class Tetromino
    {
        private TetronType _tetronType;
        private short _objectRotation;
        private Pen _pen;
        private Brush _brush;
        private short[,,] _points;

        public Tetromino()
        {
            _pen = new Pen(Color.Black, 1);
        }

        public void ChangeType(TetronType tetronType)
        {
            _tetronType = tetronType;
            switch (_tetronType)
            {
                case TetronType.I:
                    _brush = Brushes.Cyan;
                    _points = new short[4, 4, 2]
                    {{{0,0}, {1,0}, {-1,0}, {-2,0}},
                    {{0,0}, {0,1}, {0,-1}, {0,-2}},
                    {{0,0}, {1,0}, {-1,0}, {-2,0}},
                    {{0,0}, {0,1}, {0,-1}, {0,-2}}};
                    _objectRotation = 0;
                    break;
                case TetronType.T:
                    _brush = Brushes.Purple;
                    _points = new short[4, 4, 2]
                    {{{0,0}, {1,0}, {0,1}, {-1,0}},
                    {{0,0}, {0,1}, {-1,0}, {0,-1}},
                    {{0,0}, {1,0}, {-1,0}, {0,-1}},
                    {{0,0}, {1,0}, {0,1}, {0,-1}}};
                    _objectRotation = 0;
                    break;
            }
        }

        public void Rotate()
        {
            if (_objectRotation < 3)
                _objectRotation += 1;
            else
                _objectRotation = 0;
        }

        public void RotateBack()
        {
            if (_objectRotation > 0)
                _objectRotation -= 1;
            else
                _objectRotation = 3;
        }

        public TetronType TetronType
        {
            get { return _tetronType; }
        }

        public Pen Pen
        {
            get { return _pen; }
        }

        public Brush Brush
        {
            get { return _brush; }
        }

        public short ObjectRotation
        {
            get { return _objectRotation; }
        }

        public short[,,] Points
        {
            get { return _points; }
        }
    }

    public enum TetronType : int
    {
        I = 1,
        J = 2,
        L = 3,
        O = 4,
        S = 5,
        T = 6,
        Z = 7
    }
}
