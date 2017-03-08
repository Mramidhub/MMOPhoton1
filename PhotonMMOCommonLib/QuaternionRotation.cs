using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonMMO.Common
{
    public class QuaternionRotation
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public QuaternionRotation()
        {

        }

        public QuaternionRotation(float xrot, float yrot, float zrot, float wrot)
        {
            X = xrot;
            Y = yrot;
            Z = zrot;
            W = wrot;
        }

    }
}
