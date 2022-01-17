using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;

namespace BarnesHut
{
    public class Particle
    {
        public float mass;
        public vec3D pos, vel;

        public Particle(vec3D pos, vec3D vel, float mass)
        {
            this.pos = pos;
            this.vel = vel;
            this.mass = mass;
        }
    }

    public class TreeNode
    {
        public Particle body;

        public TreeNode parent;

        public oct q;

        public List<TreeNode> children;

        public TreeNode(oct q, TreeNode parent)
        {
            this.q = q;
            this.parent = parent;

            body = new Particle(new vec3D(0, 0, 0), new vec3D(0, 0, 0), 0);

            children = new List<TreeNode>(); 
        }
    }

}