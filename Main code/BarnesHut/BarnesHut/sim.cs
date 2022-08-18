using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace BarnesHut
{
    public class Simulation
    {
        //public List<Frame> frameList;
        public StreamWriter writer = new StreamWriter("animation.csv");

        public Frame lastFrame;
        Vec3D[] lastForces;

        public Frame prevLastFrame;
        Vec3D[] prevLastForces;

        public Simulation(Frame firstFrame)
        {
            //frameList = new List<Frame>();
            this.lastFrame = new Frame(firstFrame);
            lastForces = new Vec3D[firstFrame.bodyList.Count];
            prevLastForces = new Vec3D[firstFrame.bodyList.Count];
        }

        /*
        public void AddFrame(Frame frame)
        {
            //frameList.Add(frame);
        }
        */

        public void WriteToFile()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
            lastFrame.WriteToFile(writer);
        }

        public void NextStep(float deltaT, string forceMethod, string integrationMethod)
        {
            Vec3D[] newForces = new Vec3D[lastFrame.bodyList.Count()];

            Frame newFrame = new Frame(lastFrame);
            Frame oldFrame = new Frame(lastFrame);

            switch (forceMethod)
            {
                case ("Brute"):
                    {
                        if (integrationMethod != "Beeman")
                        {
                            newForces = lastFrame.ForcesBrute();

                            for (int i = 0; i < oldFrame.bodyList.Count; i++)
                            {
                                prevLastForces[i] = lastForces[i];
                                lastForces[i] = newForces[i];
                            }
                        }

                        break;
                    }

                case ("BarnesHut"):
                    {
                        lastFrame.BuildTree();
                        newForces = lastFrame.forcesBHTree();


                        for (int i = 0; i < oldFrame.bodyList.Count; i++)
                        {
                            prevLastForces[i] = lastForces[i];
                            lastForces[i] = newForces[i];
                        }

                        break;
                    }
            }

            switch (integrationMethod)
            {
                case ("Euler"):
                    {
                        for (int i = 0; i < oldFrame.bodyList.Count; i++)
                        {
                            newFrame.bodyList[i].position += deltaT * oldFrame.bodyList[i].velocity;
                            newFrame.bodyList[i].velocity += (deltaT / oldFrame.bodyList[i].mass) * newForces[i];
                        }

                        //frameList.Add(newFrame);
                        lastFrame = new Frame(newFrame);
                        prevLastFrame = new Frame(oldFrame);

                        break;
                    }

                case ("Beeman"):
                    {
                        //vec3D[] newForces = new vec3D[lastFrame.bodyList.Count()];

                        for (int i = 0; i < oldFrame.bodyList.Count; i++)
                        {
                            newFrame.bodyList[i].position += deltaT * oldFrame.bodyList[i].velocity + (deltaT * deltaT / oldFrame.bodyList[i].mass) * ((2f / 3f) * lastForces[i] - (1f / 6f) * prevLastForces[i]);
                        }

                        newFrame.BuildTree();
                        newForces = newFrame.forcesBHTree();
                        //newForces = newFrame.ForcesBrute();

                        for (int i = 0; i < oldFrame.bodyList.Count; i++)
                        {
                            prevLastForces[i] = lastForces[i];
                            lastForces[i] = newForces[i];
                        }



                        for (int i = 0; i < oldFrame.bodyList.Count; i++)
                        {
                            //vec3D diff = (5f / 12f) * newForces[i] + (2f / 3f) * lastForces[i] - (1 / 12) * prevLastForces[i];
                            //Console.WriteLine("diff: " + diff.xCoord + " " + diff.yCoord + " " + diff.zCoord);
                            newFrame.bodyList[i].velocity += (deltaT / oldFrame.bodyList[i].mass) * ((5f / 12f) * newForces[i] + (2f / 3f) * lastForces[i] - (1f / 12f) * prevLastForces[i]);
                        }

                        lastFrame = new Frame(newFrame);
                        prevLastFrame = new Frame(oldFrame);

                        break;
                    }
            }
        }
    }

    public class Frame
    {
        public List<Particle> bodyList;
        readonly Random randomGenerator;
        TreeNode rootNode;

        readonly float boundingBoxWidth;

        public Frame(float boundingboxwidth)
        {
            bodyList = new List<Particle>();
            randomGenerator = new Random();
            this.boundingBoxWidth = boundingboxwidth;
        }

        public Frame(Frame frame)
        {
            randomGenerator = new Random();

            this.bodyList = new List<Particle>();
            this.boundingBoxWidth = frame.boundingBoxWidth;

            for (int i = 0; i < frame.bodyList.Count(); i++)
            {
                this.bodyList.Add(new Particle(frame.bodyList[i].position, frame.bodyList[i].velocity, frame.bodyList[i].mass));
            }
        }

        public void AddBodies(int nBodies, string method, Vec3D CenterOfMass = null, Vec3D CenterVelocity = null)
        {
            switch (method)
            {
                case ("Random"):
                    for (int i = 0; i < nBodies; i++)
                        bodyList.Add(new Particle(new Vec3D((float)randomGenerator.NextDouble() * 2f * boundingBoxWidth - boundingBoxWidth, (float)randomGenerator.NextDouble() * 2f * boundingBoxWidth - boundingBoxWidth, (float)randomGenerator.NextDouble() * 2f * boundingBoxWidth - boundingBoxWidth), new Vec3D(0, 0, 0), 1));
                    break;

                case ("UniformSphere"):
                    double rad;
                    double r;

                    for (int i = 0; i < nBodies; i++)
                    {
                        r = randomGenerator.NextDouble() * boundingBoxWidth;
                        rad = randomGenerator.NextDouble() * 2 * Math.PI;

                        Vec3D posvec = new Vec3D((float)(r * Math.Cos(rad)), (float)(r * Math.Sin(rad)), 0f);
                        Vec3D velvec = new Vec3D((float)(1 * r * Math.Sin(rad)), (float)(-1 * r * Math.Cos(rad)), 0f);

                        bodyList.Add(new Particle(posvec, velvec, 1));
                    }
                    break;

                case ("Plummer"):
                    {
                        for (int i = 0; i < nBodies; i++)
                        {
                            float radius = (float)(1.0 / Math.Sqrt(Math.Pow(randomGenerator.NextDouble(), (-2.0 / 3.0)) - 1.0));

                            float theta = (float)(Math.Acos(randomGenerator.NextDouble() * 2.0 - 1.0));
                            float phi = (float)(randomGenerator.NextDouble() * 2.0 * Math.PI);

                            Vec3D posvec = new Vec3D((float)(radius * Math.Sin(theta) * Math.Cos(phi)), (float)(radius * Math.Sin(theta) * Math.Sin(phi)), (float)(radius * Math.Cos(theta)));
                            posvec += CenterOfMass;

                            float x = 0f;
                            float y = 0.1f;

                            while (y > x * x * (float)Math.Pow((1 - x * x), 3.5))
                            {
                                x = (float)randomGenerator.NextDouble();
                                y = (float)randomGenerator.NextDouble() * 0.1f;
                            }

                            double velocity = x * Math.Sqrt(2) * Math.Pow((1 + radius * radius), -0.25);
                            //Console.WriteLine(velocity);
                            theta = (float)(Math.Acos(randomGenerator.NextDouble() * 2 - 1));
                            phi = (float)(randomGenerator.NextDouble() * 2 * Math.PI);

                            Vec3D velvec = new Vec3D((float)(velocity * Math.Sin(theta) * Math.Cos(phi)), (float)(velocity * Math.Sin(theta) * Math.Sin(phi)), (float)(velocity * Math.Cos(theta)));

                            velvec += CenterVelocity;

                            bodyList.Add(new Particle(posvec, velvec, 1 / (float)nBodies));
                        }

                        break;
                    }

                case ("Kuzmin"):
                    {
                        bodyList.Add(new Particle(new Vec3D(0, 0, 0), new Vec3D(0, 0, 0), 10));

                        for (int i = 0; i < nBodies; i++)
                        {
                            float rand = (float)randomGenerator.NextDouble();
                            float a = 0.01f;

                            float radius = (float)Math.Sqrt((a * a) / (Math.Pow(1 - rand, 2)) + a * a);
                            float phi = (float)(randomGenerator.NextDouble() * 2.0 * Math.PI);

                            Vec3D posvec = new Vec3D((float)(radius * Math.Cos(phi)), (float)(radius * Math.Sin(phi)), (float)randomGenerator.NextDouble());

                            //float velocity = (float)Math.Sqrt((1 - (a) / (Math.Sqrt(a * a + radius * radius))) / (radius * radius));
                            //float velocity = radius * (float)Math.Pow(radius * radius + a * a, 3/4);
                            float velocity = (float)Math.Sqrt(10 / radius);
                            //Console.WriteLine(velocity);
                            Vec3D velvec = new Vec3D((float)(velocity * Math.Sin(phi)), (float)(-1 * velocity * Math.Cos(phi)), 0);


                            //bodyList.Add(new Particle(posvec, velvec, 1 / (float)nBodies));
                            //bodyList.Add(new Particle(posvec, new vec3D(0,0,0), 1 / (float)nBodies));
                            bodyList.Add(new Particle(posvec, velvec, 1f));
                            //bodyList.Add(new Particle(posvec, 0.5f * velvec , 0.00025f));
                        }

                        break;

                    }
                case ("Kepler"):
                    {
                        float blackHoleMass = (float)Math.Pow(10, 10);

                        bodyList.Add(new Particle(CenterOfMass, CenterVelocity, blackHoleMass));

                        for (int i = 0; i < nBodies; i++)
                        {
                            float radius = (float)(randomGenerator.NextDouble() * 35) + 10f;
                            float phi = (float)(randomGenerator.NextDouble() * 2.0 * Math.PI);

                            Vec3D posvec = new Vec3D((float)(radius * Math.Cos(phi)), (float)(radius * Math.Sin(phi)), 0f);

                            float velocity = (float)Math.Sqrt(blackHoleMass / radius);
                            Vec3D velvec = new Vec3D((float)(velocity * Math.Sin(phi)), (float)(-1f * velocity * Math.Cos(phi)), 0f);

                            posvec += CenterOfMass;
                            velvec += CenterVelocity;

                            bodyList.Add(new Particle(posvec, velvec, 1f));
                        }

                        break;
                    }
            }

        }

        public void BuildTree()
        {
            rootNode = new TreeNode(new Oct(new Vec3D(-boundingBoxWidth, -boundingBoxWidth, -boundingBoxWidth), 2f * boundingBoxWidth), null);

            rootNode.children.Add(new TreeNode(rootNode.oct.GetHighNW(), rootNode));
            rootNode.children.Add(new TreeNode(rootNode.oct.GetHighNE(), rootNode));
            rootNode.children.Add(new TreeNode(rootNode.oct.GetHighSW(), rootNode));
            rootNode.children.Add(new TreeNode(rootNode.oct.GetHighSE(), rootNode));
            rootNode.children.Add(new TreeNode(rootNode.oct.GetLowNW(), rootNode));
            rootNode.children.Add(new TreeNode(rootNode.oct.GetLowNE(), rootNode));
            rootNode.children.Add(new TreeNode(rootNode.oct.GetLowSW(), rootNode));
            rootNode.children.Add(new TreeNode(rootNode.oct.GetLowSE(), rootNode));

            for (int i = 0; i < bodyList.Count(); i++)
                AddParticle(bodyList[i], rootNode);
        }

        public void AddParticle(Particle part, TreeNode node)
        {
            if (!node.oct.Contains(part))
                return;

            if (node.children.Count > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    //if (node.children[i].q.contains(part))
                    AddParticle(part, node.children[i]);
                }
            }
            else
            {
                if (node.body.mass > 0)
                {
                    node.children.Add(new TreeNode(node.oct.GetHighNW(), node));
                    node.children.Add(new TreeNode(node.oct.GetHighNE(), node));
                    node.children.Add(new TreeNode(node.oct.GetHighSW(), node));
                    node.children.Add(new TreeNode(node.oct.GetHighSE(), node));
                    node.children.Add(new TreeNode(node.oct.GetLowNW(), node));
                    node.children.Add(new TreeNode(node.oct.GetLowNE(), node));
                    node.children.Add(new TreeNode(node.oct.GetLowSW(), node));
                    node.children.Add(new TreeNode(node.oct.GetLowSE(), node));

                    for (int i = 0; i < 8; i++)
                    {
                        //if (node.children[i].q.contains(part))
                        AddParticle(part, node.children[i]);

                        //if (node.children[i].q.contains(node.body))
                        AddParticle(node.body, node.children[i]);
                    }
                }
            }

            node.body.position *= node.body.mass;
            node.body.position += part.position * part.mass;

            node.body.mass += part.mass;
            node.body.position *= (1f / node.body.mass);
        }

        public Vec3D[] ForcesBrute()
        {
            Vec3D[] forces = new Vec3D[bodyList.Count()];

            for (int i = 0; i < forces.Length; i++)
                forces[i] = new Vec3D(0f, 0f, 0f);

            for (int i = 0; i < bodyList.Count(); i++)
                for (int j = 0; j < bodyList.Count(); j++)
                {
                    if (i == j) { continue; }

                    Vec3D force = forcePair(bodyList[i], bodyList[j]);

                    forces[i] += force;
                }

            return forces;
        }

        public Vec3D[] forcesBHTree()
        {
            Vec3D[] forces = new Vec3D[bodyList.Count()];

            for (int i = 0; i < bodyList.Count(); i++)
            {
                forces[i] = forceTreeNode(rootNode, bodyList[i]);
            }

            return forces;
        }

        public Vec3D forceTreeNode(TreeNode node, Particle part)
        {
            float theta = node.oct.edgeLength / (float)Math.Sqrt((node.body.position - part.position).MagnitudeSquard());

            if ((node.body.position - part.position).MagnitudeSquard() == 0)
                return new Vec3D(0, 0, 0);

            if (theta < 1f) { return forcePair(part, node.body); }

            if (node.children.Count() == 0)
                return forcePair(part, node.body);

            Vec3D force = new Vec3D(0, 0, 0);

            for (int i = 0; i < 8; i++)
            {
                force += forceTreeNode(node.children[i], part);
            }

            return force;
        }


        /// <summary>
        /// Function to calculate the force between 2 particles.
        /// </summary>
        /// <param name="a">First Particle to calculate the force for.</param>
        /// <param name="b">Second Particle to calculate the force for.</param>
        /// <returns></returns>
        public Vec3D forcePair(Particle a, Particle b)
        {
            Vec3D force = new Vec3D(0, 0, 0);
            Vec3D r = a.position - b.position;

            float eps = 1f;

            if (Math.Sqrt(r.MagnitudeSquard()) > 1f)
                force = -1.0f * ((a.mass * b.mass) / (float)Math.Pow(r.MagnitudeSquard() + eps * eps, 2 / 2)) * (r * (float)(1.0f / Math.Pow(r.MagnitudeSquard(), 1.0 / 2.0)));

            return force;
        }

        public void WriteToFile(StreamWriter writer)
        {
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            List<double> zList = new List<double>();

            foreach (Particle body in bodyList)
            {
                xList.Add(body.position.xCoord);
                yList.Add(body.position.yCoord);
                zList.Add(body.position.zCoord);
            }

            writer.WriteLine(String.Join(",", xList.Select(i => i.ToString()).ToArray()));
            writer.WriteLine(String.Join(",", yList.Select(i => i.ToString()).ToArray()));
            writer.WriteLine(String.Join(",", zList.Select(i => i.ToString()).ToArray()));
        }
    }

}
