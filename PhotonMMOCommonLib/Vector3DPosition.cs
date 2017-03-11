using System;
using System.Collections.Generic;
using System.Text;

namespace PhotonMMO.Common
{
    public class Vector3DPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3DPosition()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public Vector3DPosition(float xpos, float ypos, float zpos)
        {
            X = xpos;
            Y = ypos;
            Z = zpos;
        }

    }
}
