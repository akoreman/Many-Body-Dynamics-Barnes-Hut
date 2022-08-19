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

        public void InitializeChildren()
        {
            this.children.Add(new TreeNode(this.oct.GetHighNW(), this));
            this.children.Add(new TreeNode(this.oct.GetHighNE(), this));
            this.children.Add(new TreeNode(this.oct.GetHighSW(), this));
            this.children.Add(new TreeNode(this.oct.GetHighSE(), this));
            this.children.Add(new TreeNode(this.oct.GetLowNW(), this));
            this.children.Add(new TreeNode(this.oct.GetLowNE(), this));
            this.children.Add(new TreeNode(this.oct.GetLowSW(), this));
            this.children.Add(new TreeNode(this.oct.GetLowSE(), this));
        }
    }

}