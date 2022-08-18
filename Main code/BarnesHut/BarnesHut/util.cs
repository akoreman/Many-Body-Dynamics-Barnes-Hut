using System;

namespace BarnesHut
{
    public class Vec2D
    {
        public float xCoord;
        public float yCoord;

        public Vec2D(float xCoord, float yCoord)
        {
            this.xCoord = xCoord;
            this.yCoord = yCoord;
        }

        public static Vec2D operator +(Vec2D a, Vec2D b)
        {
            return new Vec2D((float)(a.xCoord + b.xCoord), (float)(a.yCoord + b.yCoord));
        }

        public static Vec2D operator -(Vec2D a, Vec2D b)
        {
            return new Vec2D((float)(a.xCoord - b.xCoord), (float)(a.yCoord - b.yCoord));
        }

        public static Vec2D operator *(Vec2D a, float d)
        {
            return new Vec2D((float)(a.xCoord * d), (float)(a.yCoord * d));
        }

        public static Vec2D operator *(float d, Vec2D a)
        {
            return new Vec2D((float)(a.xCoord * d), (float)(a.yCoord * d));
        }

        public float Magnitude()
        {
            if (xCoord == 0 & xCoord == 0)
                return 0;

            return (float)Math.Sqrt(xCoord * xCoord + yCoord * yCoord);
        }
    }

    public class Vec3D
    {
        public float xCoord;
        public float yCoord;
        public float zCoord;

        public Vec3D(float xCoord, float yCoord, float zCoord)
        {
            this.xCoord = xCoord;
            this.yCoord = yCoord;
            this.zCoord = zCoord;
        }

        public static Vec3D operator +(Vec3D a, Vec3D b)
        {
            return new Vec3D((float)(a.xCoord + b.xCoord), (float)(a.yCoord + b.yCoord), a.zCoord + b.zCoord);
        }

        public static Vec3D operator -(Vec3D a, Vec3D b)
        {
            return new Vec3D((float)(a.xCoord - b.xCoord), (float)(a.yCoord - b.yCoord), a.zCoord - b.zCoord);
        }

        public static Vec3D operator *(Vec3D a, float d)
        {
            return new Vec3D((float)(a.xCoord * d), (float)(a.yCoord * d), (float)(a.zCoord * d));
        }

        public static Vec3D operator *(float d, Vec3D a)
        {
            return new Vec3D((float)(a.xCoord * d), (float)(a.yCoord * d), (float)(a.zCoord * d));
        }

        public float MagnitudeSquard()
        {
            return xCoord * xCoord + yCoord * yCoord + zCoord * zCoord;
        }
    }

    public class Quad
    {
        public Vec2D lowLeftCorner;
        public float edgeLength;

        public Quad(Vec2D lowLeftCorner, float edgeLength)
        {
            this.lowLeftCorner = lowLeftCorner;
            this.edgeLength = edgeLength;
        }

        public bool Contains(Particle particle)
        {
            if (particle.position.xCoord > lowLeftCorner.xCoord & particle.position.xCoord < lowLeftCorner.xCoord + edgeLength & particle.position.yCoord > lowLeftCorner.yCoord & particle.position.yCoord < lowLeftCorner.yCoord + edgeLength)
                return true;

            return false;
        }

        public Quad GetNW()
        {
            return new Quad(lowLeftCorner + new Vec2D(0.5f * edgeLength, 0), edgeLength * 0.5f);
        }

        public Quad GetNE()
        {
            return new Quad(lowLeftCorner + new Vec2D(0.5f * edgeLength, 0.5f * edgeLength), edgeLength * 0.5f);
        }

        public Quad GetSW()
        {
            return new Quad(lowLeftCorner + new Vec2D(0, 0), edgeLength * 0.5f);
        }

        public Quad GetSE()
        {
            return new Quad(lowLeftCorner + new Vec2D(0, 0.5f * edgeLength), edgeLength * 0.5f);
        }
    }

    public class Oct
    {
        public Vec3D lowLeftCorner;
        public float edgeLength;

        public Oct(Vec3D lowLeftCorner, float edgeLength)
        {
            this.lowLeftCorner = lowLeftCorner;
            this.edgeLength = edgeLength;
        }

        public bool Contains(Particle particle)
        {
            if (particle.position.xCoord >= lowLeftCorner.xCoord & particle.position.xCoord < lowLeftCorner.xCoord + edgeLength & particle.position.yCoord >= lowLeftCorner.yCoord & particle.position.yCoord < lowLeftCorner.yCoord + edgeLength & particle.position.zCoord >= lowLeftCorner.zCoord & particle.position.zCoord < lowLeftCorner.zCoord + edgeLength)
                return true;

            return false;
        }

        public Oct GetHighNW()
        {
            return new Oct(lowLeftCorner + new Vec3D(0.5f * edgeLength, 0, edgeLength * 0.5f), edgeLength * 0.5f);
        }

        public Oct GetHighNE()
        {
            return new Oct(lowLeftCorner + new Vec3D(0.5f * edgeLength, 0.5f * edgeLength, edgeLength * 0.5f), edgeLength * 0.5f);
        }

        public Oct GetHighSW()
        {
            return new Oct(lowLeftCorner + new Vec3D(0, 0, edgeLength * 0.5f), edgeLength * 0.5f);
        }

        public Oct GetHighSE()
        {
            return new Oct(lowLeftCorner + new Vec3D(0, 0.5f * edgeLength, edgeLength * 0.5f), edgeLength * 0.5f);
        }

        public Oct GetLowNW()
        {
            return new Oct(lowLeftCorner + new Vec3D(0.5f * edgeLength, 0, 0), edgeLength * 0.5f);
        }

        public Oct GetLowNE()
        {
            return new Oct(lowLeftCorner + new Vec3D(0.5f * edgeLength, 0.5f * edgeLength, 0), edgeLength * 0.5f);
        }

        public Oct GetLowSW()
        {
            return new Oct(lowLeftCorner + new Vec3D(0, 0, 0), edgeLength * 0.5f);
        }

        public Oct GetLowSE()
        {
            return new Oct(lowLeftCorner + new Vec3D(0, 0.5f * edgeLength, 0), edgeLength * 0.5f);
        }
    }

}