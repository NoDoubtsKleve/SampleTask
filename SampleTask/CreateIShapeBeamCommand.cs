using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using Microsoft.Build.Framework.XamlTypes;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using SampleTask;
using SendGrid.Helpers.Mail;
using Rhino.DocObjects;

namespace MyRhinoPlugin
{
    public class CreateIShapeBeamCommand : Command
    {
        public override string EnglishName => throw new NotImplementedException();

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Get the input from the user using a WPF window
            var viewModel = new CreateIShapeBeamViewModel();
            var dialog = new CreateIShapeBeamWindow(viewModel);
            var result = dialog.ShowDialog();

            // If the user clicked OK, create the I-Shape beam
            if ((bool)(result = true))
            {
                var length = viewModel.Length;
                var breadth = viewModel.Breadth;
                var height = viewModel.Height;

                // Create the I-Shape beam using Rhino's geometry classes
                var profile = new Rectangle3d(Plane.WorldXY, length / 2.0, breadth / 2.0);
                var topLine = new Line(new Point3d(0, 0, height), new Point3d(0, breadth / 2.0, height));
                var bottomLine = new Line(new Point3d(0, 0, height), new Point3d(0, -breadth / 2.0, height));
                var topProfile = profile.Center;
                topProfile.Transform(Transform.Translation(new Vector3d(0, breadth / 2.0, height)));
                var bottomProfile = profile.Center;
                bottomProfile.Transform(Transform.Translation(new Vector3d(0, -breadth / 2.0, height)));
                var iShape = new Brep();
                iShape.CapPlanarHoles(doc.ModelAbsoluteTolerance);

                // Add the I-Shape beam to the document
                var attributes = doc.CreateDefaultAttributes();
                var guid = doc.Objects.AddBrep(iShape, attributes);

                // Display the I-Shape beam in the viewport
                doc.Views.Redraw();

                // Display a message to the user
                RhinoApp.WriteLine("I-Shape beam created with dimensions {0} x {1} x {2}", length, breadth, height);

                // Return success
                return Result.Success;
            }

            // If the user clicked Cancel, return failure
            return Result.Cancel;
        }
    }


    public class RectangularProfile : IProfileGenerator
    {
        public Curve CreateProfile(double height, double width, double thickness)
        {
            // Create a rectangular profile using Rhino's geometry classes
            var profile = new Rectangle3d(Plane.WorldXY, width / 2.0, height / 2.0);

            // Convert the rectangular profile to a curve and return it
            return profile.ToNurbsCurve();
        }

        Polyline IProfileGenerator.Generate()
        {
            throw new NotImplementedException();
        }
    }

    public class CreateIShapeBeamViewModel : RectangularProfile
    {
        private double _length;
        private double _breadth;
        private double _height;
        private double _thickness;

        public double Length
        {
            get { return _length; }

        }

        public double Breadth
        {
            get { return _breadth; }

        }

        public double Height
        {
            get { return _height; }

        }

        public double Thickness
        {
            get { return _thickness; }

        }
    }

    public class CreateIShapeBeamWindow : CreateLineCommand
    {
        private CreateIShapeBeamViewModel DataContext;
        private string Title;

        public ResizeMode WindowStartupLocation { get; }

        public CreateIShapeBeamWindow(CreateIShapeBeamViewModel viewModel)
        {
            // Set the dialog properties
            // This will define the dialog in Rhino
            // The dialog can be opened using the "MyRhinoPluginCreateIShapeBeam" command
            // The dialog will display a WPF window with the XAML code defined in the resource file
            // The view model will be used as the data context for the window
            // The window will be displayed as a modal dialog
            Title = "Create I-Shape Beam";
            ResizeMode ResizeMode;
            WindowStartupLocation =
            ResizeMode = ResizeMode.NoResize;
            bool ShowInTaskbar = false;
            SizeToContent SizeToContent = SizeToContent.WidthAndHeight;
            CreateIShapeBeamView Content = new CreateIShapeBeamView { DataContext = viewModel };
            int Width = 300;
            int Height = 200;
            Eto.Forms.Window Owner = Rhino.UI.RhinoEtoApp.MainWindow;
            WindowStyle WindowStyle = WindowStyle.SingleBorderWindow;
        }

        public double Length => ((CreateIShapeBeamViewModel)DataContext).Length;
        public double Breadth => ((CreateIShapeBeamViewModel)DataContext).Breadth;
        public double Height => ((CreateIShapeBeamViewModel)DataContext).Height;
        public double Thickness => ((CreateIShapeBeamViewModel)DataContext).Thickness;

        internal object ShowDialog()
        {
            throw new NotImplementedException();
        }

        internal bool ShowModal()
        {
            throw new NotImplementedException();
        }
    }

    public class CreateIShapeBeamView : SampleTaskPlugin
    {
        internal CreateIShapeBeamViewModel DataContext;

        public CreateIShapeBeamView()
        {
            // Load the XAML code from the resource file and set it as the content of the view
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }
    }

    public class CreateIShapeBeam : Command
    {
        public override string EnglishName => "MyRhinoPluginCreateIShapeBeam";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Prompt the user for the bottom-left and top-right points of the rectangle
            var gp = new GetPoint();
            gp.SetCommandPrompt("Select bottom-left point of rectangle");
            gp.Get();
            if (gp.CommandResult() != Result.Success)
                return gp.CommandResult();
            var pt1 = gp.Point();

            gp.SetCommandPrompt("Select top-right point of rectangle");
            gp.SetBasePoint(pt1, true);
            gp.DrawLineFromPoint(pt1, true);
            gp.Get();
            if (gp.CommandResult() != Result.Success)
                return gp.CommandResult();
            var pt2 = gp.Point();

            // Create the I-beam profile generator
            var profileGenerator = new CreateLineCommand();

            // Prompt the user for the height, width, and thickness of the I-beam
            var viewModel = new CreateIShapeBeamViewModel();
            var window = new CreateIShapeBeamWindow(viewModel);
            if (window.ShowModal() != true)
                return Result.Cancel;

            // Create the rectangular profile sweep using the input from the user
            var profile = profileGenerator.CreateProfile(viewModel.Height, viewModel.Breadth, viewModel.Thickness);
            var sweep = ProfileSweep.Create(profile, new LineCurve(pt1, pt2));

            // Return success
            return Result.Success;
        }
    }
}