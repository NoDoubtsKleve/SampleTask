using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input.Custom;
using Rhino;
using Command = Rhino.Commands.Command;

namespace SampleTask
{
    public class CreateFrameCommand : Command
    {
        public CreateFrameCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a reference in a static property.
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static CreateFrameCommand Instance { get; private set; }

        public override string EnglishName => "CreateProfile";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Get the first corner point
            var gp = new GetPoint();
            gp.SetCommandPrompt("Select the first corner of the rectangle");
            gp.Get();
            if (gp.CommandResult() != Result.Success)
                return gp.CommandResult();
            var pt0 = gp.Point();

            // Get the second corner point
            gp.SetCommandPrompt("Select the second corner of the rectangle");
            gp.SetBasePoint(pt0, true);
            gp.Get();
            if (gp.CommandResult() != Result.Success)
                return gp.CommandResult();
            var pt1 = gp.Point();

            // Create the rectangle
            var plane = new Plane(pt0, Vector3d.ZAxis);
            var rectangle = new Rectangle3d(plane, pt0, pt1);

            var guid = doc.Objects.AddRectangle(rectangle);

            // Show the FrameProfileWindow to get the user inputs
            var viewModel = new ProfileViewModel(pt0,pt1);
            var window = new FrameProfileWindow() { DataContext = viewModel };
            bool? result = window.ShowDialog();
            if (result != true)
            {
                return Result.Cancel;
            }





            // generate I-beam profile
            var profile = new IBeamProfile(viewModel.Height, viewModel.Width, viewModel.Thickness);
            profile.Generate();
            // generate frame geometry
            var sweep = new ProfileSweep(profile);
            var rail = new Polyline { pt0, new Point3d(pt1.X, pt0.Y, 0), pt1, new Point3d(pt0.X, pt1.Y, 0), pt0 };
            sweep.BakeSweep(rail);


            doc.Views.Redraw();

            return Result.Success;
        }
    }
}
