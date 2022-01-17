using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;

namespace BarnesHut
{
    public class Simulation
    {
        //public List<Frame> frameList;
        public StreamWriter writer = new StreamWriter("animation.csv");

        public Frame lastFrame;
        vec3D[] lastForces;

        public Frame prevLastFrame;
        vec3D[] prevLastForces;

        public Simulation(Frame firstFrame)
        {
            //frameList = new List<Frame>();
            this.lastFrame = new Frame(firstFrame);
            lastForces = new vec3D[firstFrame.bodyList.Count];
            prevLastForces = new vec3D[firstFrame.bodyList.Count];
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
            vec3D[] newForces = new vec3D[lastFrame.bodyList.Count()];

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
                            newFrame.bodyList[i].pos += deltaT * oldFrame.bodyList[i].vel;
                            newFrame.bodyList[i].vel += (deltaT/oldFrame.bodyList[i].mass) * newForces[i];
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
                            newFrame.bodyList[i].pos += deltaT * oldFrame.bodyList[i].vel + (deltaT * deltaT / oldFrame.bodyList[i].mass) * ((2f / 3f) * lastForces[i] - (1f / 6f) * prevLastForces[i]);
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
                            newFrame.bodyList[i].vel += (deltaT / oldFrame.bodyList[i].mass) * ((5f / 12f) * newForces[i] + (2f/3f) * lastForces[i] - (1f/12f) * prevLastForces[i]);  
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
            Random randomGenerator;
            TreeNode rootNode;

            float length;

            public Frame(float length)
            {
                bodyList = new List<Particle>();
                randomGenerator = new Random();
                this.length = length;
            }

            public Frame(Frame frame)
            {
                randomGenerator = new Random();

                this.bodyList = new List<Particle>();
                this.length = frame.length;

                for (int i = 0; i < frame.bodyList.Count(); i++)
                {
                    this.bodyList.Add(new Particle(frame.bodyList[i].pos, frame.bodyList[i].vel, frame.bodyList[i].mass));
                }
            }

            public void AddBodies(int nBodies, string method, vec3D CenterOfMass = null, vec3D CenterVelocity = null)
            {
                switch (method)
                {
                    case ("Random"):
                        for (int i = 0; i < nBodies; i++)
                            bodyList.Add(new Particle(new vec3D((float)randomGenerator.NextDouble() * 2f * length - length, (float)randomGenerator.NextDouble() * 2f * length - length, (float)randomGenerator.NextDouble() * 2f * length - length), new vec3D(0, 0, 0), 1));
                        break;

                    case ("UniformSphere"):
                        double rad;
                        double r;

                        for (int i = 0; i < nBodies; i++)
                        {
                            r = randomGenerator.NextDouble() * length;
                            rad = randomGenerator.NextDouble() * 2 * Math.PI;

                            vec3D posvec = new vec3D((float)(r * Math.Cos(rad)), (float)(r * Math.Sin(rad)), 0f);
                            vec3D velvec = new vec3D((float)(1 * r * Math.Sin(rad)), (float)(-1 * r * Math.Cos(rad)), 0f);

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

                            vec3D posvec = new vec3D((float)(radius * Math.Sin(theta) * Math.Cos(phi)), (float)(radius * Math.Sin(theta) * Math.Sin(phi)), (float)(radius * Math.Cos(theta)));
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

                            vec3D velvec = new vec3D((float)(velocity * Math.Sin(theta) * Math.Cos(phi)), (float)(velocity * Math.Sin(theta) * Math.Sin(phi)), (float)(velocity * Math.Cos(theta)));

                            velvec += CenterVelocity;

                            bodyList.Add(new Particle(posvec, velvec, 1 / (float)nBodies));
                        }

                        break;
                    }

                case ("Kuzmin"):
                    {
                        bodyList.Add(new Particle(new vec3D(0, 0, 0), new vec3D(0, 0, 0), 10));

                        for (int i = 0; i < nBodies; i++)
                        {
                            float rand = (float)randomGenerator.NextDouble();
                            float a = 0.01f;

                            float radius = (float)Math.Sqrt((a * a) / (Math.Pow(1 - rand, 2)) + a*a);
                            float phi = (float)(randomGenerator.NextDouble() * 2.0 * Math.PI);

                            vec3D posvec = new vec3D((float)(radius * Math.Cos(phi)), (float)(radius * Math.Sin(phi)), (float)randomGenerator.NextDouble());

                            //float velocity = (float)Math.Sqrt((1 - (a) / (Math.Sqrt(a * a + radius * radius))) / (radius * radius));
                            //float velocity = radius * (float)Math.Pow(radius * radius + a * a, 3/4);
                            float velocity = (float)Math.Sqrt(10 / radius);
                            //Console.WriteLine(velocity);
                            vec3D velvec = new vec3D((float)( velocity * Math.Sin(phi)), (float)(-1 * velocity * Math.Cos(phi)), 0);


                            //bodyList.Add(new Particle(posvec, velvec, 1 / (float)nBodies));
                            //bodyList.Add(new Particle(posvec, new vec3D(0,0,0), 1 / (float)nBodies));
                            bodyList.Add(new Particle(posvec, velvec, 1f));
                            //bodyList.Add(new Particle(posvec, 0.5f * velvec , 0.00025f));
                        }

                        break;
                  
                    }
                case ("Kepler"):
                    {
                        float blackHoleMass = (float)Math.Pow(10,10);

                        bodyList.Add(new Particle(CenterOfMass, CenterVelocity, blackHoleMass));

                        for (int i = 0; i < nBodies; i++)
                        {
                            float radius = (float)(randomGenerator.NextDouble() * 35) + 10f;
                            float phi = (float)(randomGenerator.NextDouble() * 2.0 * Math.PI);

                            vec3D posvec = new vec3D((float)(radius * Math.Cos(phi)), (float)(radius * Math.Sin(phi)), 0f);

                            float velocity = (float)Math.Sqrt(blackHoleMass / radius);
                            vec3D velvec = new vec3D((float)(velocity * Math.Sin(phi)), (float)(-1f * velocity * Math.Cos(phi)), 0f);

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
                rootNode = new TreeNode(new oct(new vec3D(-length, -length, -length), 2f * length), null);

                rootNode.children.Add(new TreeNode(rootNode.q.NWH(), rootNode));
                rootNode.children.Add(new TreeNode(rootNode.q.NEH(), rootNode));
                rootNode.children.Add(new TreeNode(rootNode.q.SWH(), rootNode));
                rootNode.children.Add(new TreeNode(rootNode.q.SEH(), rootNode));
                rootNode.children.Add(new TreeNode(rootNode.q.NWL(), rootNode));
                rootNode.children.Add(new TreeNode(rootNode.q.NEL(), rootNode));
                rootNode.children.Add(new TreeNode(rootNode.q.SWL(), rootNode));
                rootNode.children.Add(new TreeNode(rootNode.q.SEL(), rootNode));

                for (int i = 0; i < bodyList.Count(); i++)
                    AddParticle(bodyList[i], rootNode);
            }

            public void AddParticle(Particle part, TreeNode node)
            {
                if (node.q.contains(part))
                {
                    //Console.WriteLine(part.mass);

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
                            node.children.Add(new TreeNode(node.q.NWH(), node));
                            node.children.Add(new TreeNode(node.q.NEH(), node));
                            node.children.Add(new TreeNode(node.q.SWH(), node));
                            node.children.Add(new TreeNode(node.q.SEH(), node));
                            node.children.Add(new TreeNode(node.q.NWL(), node));
                            node.children.Add(new TreeNode(node.q.NEL(), node));
                            node.children.Add(new TreeNode(node.q.SWL(), node));
                            node.children.Add(new TreeNode(node.q.SEL(), node));

                            for (int i = 0; i < 8; i++)
                            {
                                //if (node.children[i].q.contains(part))
                                AddParticle(part, node.children[i]);

                                //if (node.children[i].q.contains(node.body))
                                AddParticle(node.body, node.children[i]);
                            }
                        }
                    }

                    node.body.pos *= node.body.mass;
                    node.body.pos += part.pos *  part.mass;

                    node.body.mass += part.mass;
                    node.body.pos *= (1f / node.body.mass);
                }
            }

            public vec3D[] ForcesBrute()
            {
                vec3D[] forces = new vec3D[bodyList.Count()];

                for (int i = 0; i < forces.Length; i++)
                    forces[i] = new vec3D(0f, 0f, 0f);

                for (int i = 0; i < bodyList.Count(); i++)
                    for (int j = 0; j < bodyList.Count(); j++)
                    {
                        if (i != j)
                        {
                            vec3D temp = forcePair(bodyList[i], bodyList[j]);

                            forces[i] += temp;
                            //forces[j] -= temp;
                        }
                    }

                return forces;
            }

            public vec3D[] forcesBHTree()
            {
                vec3D[] forces = new vec3D[bodyList.Count()];

                for (int i = 0; i < bodyList.Count(); i++)
                {
                    //Console.WriteLine("body nr: " + i);
                    forces[i] = forceTreeNode(rootNode, bodyList[i]);
                    //Console.WriteLine("force: " + forces[i].xCoord + " " + forces[i].yCoord);
                }

                return forces;
            }

            public vec3D forceTreeNode(TreeNode node, Particle part)
            {
                float theta = node.q.edgeLength / (float)Math.Sqrt((node.body.pos - part.pos).squared());

                //Console.WriteLine("theta: " + theta);

                if (!((node.body.pos - part.pos).squared() == 0))
                {
                    if (theta < 1f)
                    {
                        return forcePair(part, node.body);
                    }
                    else
                    {
                        if (node.children.Count() > 0)
                        {
                            //Console.WriteLine("int node");
                            vec3D force = new vec3D(0, 0, 0);

                            for (int i = 0; i < 8; i++)
                            {
                                force += forceTreeNode(node.children[i], part);
                            }

                            return force;
                        }
                        else
                        {
                            //Console.WriteLine("ext node");
                            return forcePair(part, node.body);
                        }
                    }

                }
                else
                {
                    //Console.WriteLine("zero vec");
                    return new vec3D(0, 0, 0);
                }
            }

        /*
        public vec3D forcePair(Particle a, Particle b)
        {
            vec3D temp = new vec3D(0, 0, 0);
            vec3D r = a.pos - b.pos;
            float off = 0.01f;

            if (r.squared() > 0)
                temp = -1.0f * ((a.mass * b.mass) / (float)Math.Pow(r.squared() + off * off, 3 / 2)) * (r * (float)(1.0f / Math.Pow(r.squared(), 1.0f / 2.0f)) );

            //temp = -1 * (((a.mass * b.mass) / (Math.Pow((r.magnitude() + 1, 2)) * (a.pos - b.pos).magnitude()) * r);
            //Console.WriteLine("temp: " + temp.xCoord + " " + temp.yCoord + " " + temp.zCoord);

            return temp;
        }
        */

        public vec3D forcePair(Particle a, Particle b)
        {
            vec3D temp = new vec3D(0, 0, 0);
            vec3D r = a.pos - b.pos;

            float eps = 1f;

            if (Math.Sqrt(r.squared()) > 1f)
                temp = -1.0f * ((a.mass * b.mass) / (float)Math.Pow(r.squared() + eps*eps, 2 / 2)) * (r * (float)(1.0f / Math.Pow(r.squared(), 1.0 / 2.0)));

            //temp = -1 * (((a.mass * b.mass) / (Math.Pow((r.magnitude() + 1, 2)) * (a.pos - b.pos).magnitude()) * r);
            //Console.WriteLine("temp: " + temp.xCoord + " " + temp.yCoord + " " + temp.zCoord);

            return temp;
        }

        public void WriteToFile(StreamWriter writer)
            {
                List<double> xList = new List<double>();
                List<double> yList = new List<double>();
                List<double> zList = new List<double>();

                foreach (Particle body in bodyList)
                {
                    xList.Add(body.pos.xCoord);
                    yList.Add(body.pos.yCoord);
                    zList.Add(body.pos.zCoord);
                }

                writer.WriteLine(String.Join(",", xList.Select(i => i.ToString()).ToArray()));
                writer.WriteLine(String.Join(",", yList.Select(i => i.ToString()).ToArray()));
                writer.WriteLine(String.Join(",", zList.Select(i => i.ToString()).ToArray()));
            }
        }
    
}
