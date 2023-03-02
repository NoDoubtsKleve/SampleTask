using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

namespace SampleTask
{
    public class CreateLineCommand : Command
    {
        public CreateLineCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static CreateLineCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "CreateLine";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // select start of the line
            Point3d pt0;
            using (GetPoint getPointAction = new GetPoint())
            {
                getPointAction.SetCommandPrompt("Please select the start point");
                if (getPointAction.Get() != GetResult.Point)
                {
                    RhinoApp.WriteLine("No start point was selected.");
                    return getPointAction.CommandResult();
                }
                pt0 = getPointAction.Point();
            }

            // select end of the line
            Point3d pt1;
            using (GetPoint getPointAction = new GetPoint())
            {
                getPointAction.SetCommandPrompt("Please select the end point");
                getPointAction.SetBasePoint(pt0, true);
                getPointAction.DynamicDraw +=
                  (sender, e) => e.Display.DrawLine(pt0, e.CurrentPoint, System.Drawing.Color.DarkRed);
                if (getPointAction.Get() != GetResult.Point)
                {
                    RhinoApp.WriteLine("No end point was selected.");
                    return getPointAction.CommandResult();
                }
                pt1 = getPointAction.Point();
            }

            // create a 100x100 profile on the line
            RectangularProfile profile = new RectangularProfile(100, 100);
            ProfileSweep sweep = new ProfileSweep(profile);
            Polyline axis = new Polyline() { pt0, pt1 };
            sweep.BakeSweep(axis);

            doc.Views.Redraw();

            return Result.Success;
        }

        internal object CreateProfile(double height, double breadth, double thickness)
        {
            throw new NotImplementedException();
        }
    }
}
