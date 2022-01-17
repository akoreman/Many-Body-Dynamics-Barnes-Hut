using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Globalization;

namespace BarnesHut
{
    public class Program
    {
        static void Main()
        {
            RunSim(1000, "Plummer", "BarnesHut", "Beeman", 0.00005f, 500);
        
        }

        static void RunSim(int numBodies, string initMethod, string forceMethod, string intMethod, float deltaT, int numFrames)
        {
            Frame firstFrame = new Frame(100);

            float velocity = (float)Math.Sqrt((Math.Pow(10, 10) * 2 + 0) / (2f * 50));
            vec3D velvec = new vec3D(0f, (float)(velocity), 0f);

            firstFrame.AddBodies(numBodies, initMethod, new vec3D(0, 0, 0), new vec3D(0, 0, 0));

            //firstFrame.AddBodies(125, initMethod, new vec3D(-50, 0, 0), velvec * 0.6f);
            //firstFrame.AddBodies(1, initMethod, new vec3D(0, 0, 0), new vec3D(0, 0, 0));
            //firstFrame.AddBodies(125, initMethod, new vec3D(50, 0, 0), velvec * -0.6f);


            //for (int i = 0; i < firstFrame.bodyList.Count; i++)
            //    firstFrame.bodyList[i].vel *= 1f;

            Simulation simulation = new Simulation(firstFrame);
            simulation.WriteToFile();

            int frameCounter = 0;

            if (intMethod == "Beeman")
            {
                simulation.NextStep(deltaT, forceMethod, "Euler");
                simulation.WriteToFile();

                simulation.NextStep(deltaT, forceMethod, "Euler");
                simulation.WriteToFile();
            }

            for (int i = 0; i < numFrames; i++)
            {
                Console.WriteLine(frameCounter);
                frameCounter++;
                simulation.NextStep(deltaT, forceMethod, intMethod);
                simulation.WriteToFile();
            }

            simulation.writer.Close();
            
        }
    }

}