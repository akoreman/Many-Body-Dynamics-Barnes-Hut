using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;

namespace BarnesHut
{
    public class vec2D 
    {
        public float xCoord;
        public float yCoord;

        public vec2D(float xCoord, float yCoord)
        {
            this.xCoord = xCoord;
            this.yCoord = yCoord;
        }

        public static vec2D operator +(vec2D a, vec2D b)
        {
            return new vec2D((float)(a.xCoord + b.xCoord), (float)(a.yCoord + b.yCoord));
        }

        public static vec2D operator -(vec2D a, vec2D b)
        {
            return new vec2D((float)(a.xCoord - b.xCoord), (float)(a.yCoord - b.yCoord));
        }

        public static vec2D operator *(vec2D a, float d)
        {
            return new vec2D((float)(a.xCoord * d), (float)(a.yCoord * d));
        }

        public static vec2D operator *(float d, vec2D a)
        {
            return new vec2D((float)(a.xCoord * d), (float)(a.yCoord * d));
        }

        public float magnitude()
        {
            if (xCoord == 0 & xCoord == 0)
                return 0;
            else
                return (float)Math.Sqrt(xCoord * xCoord + yCoord * yCoord);
        }
    }

    public class vec3D
    {
        public float xCoord;
        public float yCoord;
        public float zCoord;

        public vec3D(float xCoord, float yCoord, float zCoord)
        {
            this.xCoord = xCoord;
            this.yCoord = yCoord;
            this.zCoord = zCoord;
        }

        public static vec3D operator +(vec3D a, vec3D b)
        {
            return new vec3D((float)(a.xCoord + b.xCoord), (float)(a.yCoord + b.yCoord), a.zCoord + b.zCoord);
        }

        public static vec3D operator -(vec3D a, vec3D b)
        {
            return new vec3D((float)(a.xCoord - b.xCoord), (float)(a.yCoord - b.yCoord), a.zCoord - b.zCoord);
        }

        public static vec3D operator *(vec3D a, float d)
        {
            return new vec3D((float)(a.xCoord * d), (float)(a.yCoord * d), (float)(a.zCoord * d));
        }

        public static vec3D operator *(float d, vec3D a)
        {
            return new vec3D((float)(a.xCoord * d), (float)(a.yCoord * d), (float)(a.zCoord * d));
        }

        public float squared()
        {
            return xCoord * xCoord + yCoord * yCoord + zCoord * zCoord;
        }
    }

    public class quad
    {
        public vec2D LowLeftCorner;
        public float edgeLength;

        public quad(vec2D LowLeftCorner, float edgeLength)
        {
            this.LowLeftCorner = LowLeftCorner;
            this.edgeLength = edgeLength;
        }

        public bool contains(Particle part)
        {
            if (part.pos.xCoord > LowLeftCorner.xCoord & part.pos.xCoord < LowLeftCorner.xCoord + edgeLength & part.pos.yCoord > LowLeftCorner.yCoord & part.pos.yCoord < LowLeftCorner.yCoord + edgeLength)
                return true;
            
            return false;
        }

        public quad NW()
        {
            return new quad(LowLeftCorner + new vec2D(0.5f * edgeLength,0), edgeLength * 0.5f);
        }

        public quad NE()
        {
            return new quad(LowLeftCorner + new vec2D(0.5f * edgeLength, 0.5f * edgeLength), edgeLength * 0.5f);
        }

        public quad SW()
        {
            return new quad(LowLeftCorner + new vec2D(0, 0), edgeLength * 0.5f);
        }

        public quad SE()
        {
            return new quad(LowLeftCorner + new vec2D(0, 0.5f * edgeLength), edgeLength * 0.5f);
        }
    }

    public class oct
    {
        public vec3D LowLeftCorner;
        public float edgeLength;

        public oct(vec3D LowLeftCorner, float edgeLength)
        {
            this.LowLeftCorner = LowLeftCorner;
            this.edgeLength = edgeLength;
        }

        public Boolean contains(Particle part)
        {
            if (part.pos.xCoord >= LowLeftCorner.xCoord & part.pos.xCoord < LowLeftCorner.xCoord + edgeLength & part.pos.yCoord >= LowLeftCorner.yCoord & part.pos.yCoord < LowLeftCorner.yCoord + edgeLength & part.pos.zCoord >= LowLeftCorner.zCoord & part.pos.zCoord < LowLeftCorner.zCoord + edgeLength)
                return true;
            
            return false;
        }

        public oct NWH()
        {
            return new oct(LowLeftCorner + new vec3D(0.5f * edgeLength, 0, edgeLength * 0.5f), edgeLength * 0.5f);
        }

        public oct NEH()
        {
            return new oct(LowLeftCorner + new vec3D(0.5f * edgeLength, 0.5f * edgeLength, edgeLength * 0.5f), edgeLength * 0.5f);
        }

        public oct SWH()
        {
            return new oct(LowLeftCorner + new vec3D(0, 0, edgeLength * 0.5f), edgeLength * 0.5f);
        }

        public oct SEH()
        {
            return new oct(LowLeftCorner + new vec3D(0, 0.5f * edgeLength, edgeLength * 0.5f), edgeLength * 0.5f);
        }

        public oct NWL()
        {
            return new oct(LowLeftCorner + new vec3D(0.5f * edgeLength, 0, 0), edgeLength * 0.5f);
        }

        public oct NEL()
        {
            return new oct(LowLeftCorner + new vec3D(0.5f * edgeLength, 0.5f * edgeLength, 0), edgeLength * 0.5f);
        }

        public oct SWL()
        {
            return new oct(LowLeftCorner + new vec3D(0, 0, 0), edgeLength * 0.5f);
        }

        public oct SEL()
        {
            return new oct(LowLeftCorner + new vec3D(0, 0.5f * edgeLength, 0), edgeLength * 0.5f);
        }
    }

}