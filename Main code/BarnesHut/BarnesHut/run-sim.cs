using System;
using System.Collections.Generic;

namespace BarnesHut
{
    public class Program
    {
        // Entry point from where the simulation starts and where parameters can be set.
        static void Main()
        {
            // Add groups of particles to initialise here.
            List<BodyGroupProps3D> bodyPropList = new List<BodyGroupProps3D>
            {
                new BodyGroupProps3D() { numBodies = 1000, initialisationMethod = "Plummer", centerOfMass = new Vec3D(0f, 0f, 0f), centerVelocity = new Vec3D(0f, 0f, 0f) }
            };

            RunSim(bodyPropList, "BarnesHut", "Beeman", 0.00005f, 500);
        }

        static void RunSim(List<BodyGroupProps3D> bodyProps, string forceMethod, string integrationMethod, float deltaT, int numFrames)
        {
            Frame firstFrame = new Frame(100);

            foreach (BodyGroupProps3D props in bodyProps)
            {
                firstFrame.AddBodies(props.numBodies, props.initialisationMethod, props.centerOfMass, props.centerVelocity);
            }

            Simulation simulation = new Simulation(firstFrame);
            simulation.WriteToFile();

            int frameCounter = 0;

            if (integrationMethod == "Beeman")
            {
                simulation.NextStep(deltaT, forceMethod, "Euler");
                simulation.WriteToFile();
                frameCounter++;

                simulation.NextStep(deltaT, forceMethod, "Euler");
                simulation.WriteToFile();
                frameCounter++;
            }

            for (int i = frameCounter; i < numFrames; i++)
            {
                Console.WriteLine(frameCounter);

                frameCounter++;
                simulation.NextStep(deltaT, forceMethod, integrationMethod);
                simulation.WriteToFile();
            }

            simulation.writer.Close();
        }
    }

    public struct BodyGroupProps3D
    {
        public int numBodies;
        public string initialisationMethod;
        public Vec3D centerOfMass;
        public Vec3D centerVelocity;
    }
}