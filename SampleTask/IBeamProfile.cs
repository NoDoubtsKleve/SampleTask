using Rhino;
using Rhino.Geometry;

namespace SampleTask
{
    internal class IBeamProfile : IProfileGenerator
    {
        public IBeamProfile(double height, double width, double thickness)
        {
            Height = height;
            Width = width;
            Thickness = thickness;
        }
        public double Height { get; }
        public double Width { get; }
        public double Thickness { get; }
        public Polyline Generate()
        {
            Point3d p0 = new Point3d(0,0,0);
            Point3d p1 = new Point3d(Width, 0, 0);
            Point3d p2 = new Point3d(Width, Thickness, 0);
            Point3d p3 = new Point3d((Width / 2.0) +(Thickness / 2.0),Thickness, 0);
            Point3d p4 = new Point3d((Width / 2.0) + (Thickness / 2.0), Height - Thickness, 0);
            Point3d p5 = new Point3d(Width, Height - Thickness,0);
            Point3d p6 = new Point3d(Width, Height, 0);
            Point3d p7 = new Point3d(0, Height, 0);
            Point3d p8 = new Point3d(0, Height - Thickness, 0);
            Point3d p9 = new Point3d((Width / 2.0) - (Thickness / 2.0), Height - Thickness, 0);
            Point3d p10 = new Point3d((Width / 2.0) - (Thickness / 2.0), Thickness, 0);
            Point3d p11 = new Point3d(0,Thickness,0);

            Polyline profile = new Polyline() { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p0 };

            return profile;
            
        }
        
    }
}
