using System.Collections.Generic;

namespace BarnesHut
{
    public class Particle
    {
        public float mass;
        public Vec3D position, velocity;

        public Particle(Vec3D position, Vec3D velocity, float mass)
        {
            this.position = position;
            this.velocity = velocity;
            this.mass = mass;
        }
    }

    public class TreeNode
    {
        public Particle body;
        public TreeNode parent;
        public Oct oct;
        public List<TreeNode> children;

        public TreeNode(Oct q, TreeNode parent)
        {
            this.oct = q;
            this.parent = parent;

            body = new Particle(new Vec3D(0, 0, 0), new Vec3D(0, 0, 0), 0);

            children = new List<TreeNode>();
        }
    }

}