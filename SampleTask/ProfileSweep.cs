using System;
using System.Collections.Generic;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace SampleTask
{
    /// <summary>
    /// Bas class for different types
    /// </summary>
    internal class ProfileSweep
    {
        private readonly IProfileGenerator _profileGenerator;

        public ProfileSweep(IProfileGenerator generator)
        {
            _profileGenerator = generator;
        }

        public RhinoObject BakeSweep(Polyline rail)
        {
            // check rail
            if ( rail == null)
            {
                throw new ArgumentNullException("Rail is null!");
            }

            // check rail planarity
            Plane.FitPlaneToPoints(rail, out Plane railPlane);
            if (railPlane == null || railPlane.IsValid == false)
            {
                throw new InvalidOperationException("Rail must be planer!!!");
            }

            // check rail points
            if (rail.Count < 2)
            {
                throw new InvalidOperationException("Rail can not have less then 2 points!!!");
            }

            // generate profile section
            Polyline profile = _profileGenerator.Generate();

            // check planarity of profile
            Plane.FitPlaneToPoints(profile, out Plane plane);
            if(plane == null || plane.IsValid == false)
            {
                throw new InvalidOperationException("Generated profile is not planer!!!");
            }

            // orient profile to the rail
            plane.Origin = profile[0];
            Vector3d yVector = rail[0] - rail[1];
            Vector3d xVector = Vector3d.CrossProduct(railPlane.Normal, yVector);
            Plane targetPlane = new Plane(rail[0], xVector, railPlane.Normal);
            Transform orient = Transform.PlaneToPlane(plane, targetPlane);
            profile.Transform(orient);

            // create sections for sweep
            IEnumerable<Curve> sections = new List<Curve>() { profile.ToNurbsCurve() };

            var sweep = new SweepOneRail();
            sweep.AngleToleranceRadians = RhinoDoc.ActiveDoc.ModelAngleToleranceRadians;
            sweep.ClosedSweep = false;
            sweep.SweepTolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            sweep.SetToRoadlikeTop();

            // generate sweep geometry
            Brep[] breps = sweep.PerformSweep(rail.ToNurbsCurve(), sections);
            Guid id = RhinoDoc.ActiveDoc.Objects.AddBrep(breps[0]);
            
            return RhinoDoc.ActiveDoc.Objects.Find(id);
        }

        internal static object Create(object profile, LineCurve lineCurve)
        {
            throw new NotImplementedException();
        }
    }
}
