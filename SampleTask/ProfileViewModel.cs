using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Rhino;
using Rhino.Geometry;

namespace SampleTask
{
    internal class ProfileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string name ="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private Point3d _bottomLeft;
        private Point3d _topRight;
        private double _height;
        private double _width;
        private double _thickness;

        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        public double Thickness
        {
            get => _thickness;
            set
            {
                _thickness = value;
                OnPropertyChanged(nameof(Thickness));
            }
        }
        public Point3d BottomLeft
        {
            get { return _bottomLeft; }
            set
            {
                _bottomLeft = value;
                OnPropertyChanged(nameof(BottomLeft));
            }
        }

        public Point3d TopRight
        {
            get { return _topRight; }
            set
            {
                _topRight = value;
                OnPropertyChanged(nameof(TopRight));
            }
        }
        public ICommand OkCommand { get; set; }
        public ProfileViewModel(Point3d pt0,Point3d pt1)
        {
            BottomLeft = pt0;
            TopRight = pt1;
            OkCommand = new RelayCommand(OkCommand_Execute, CanExecuteOKCommand);
        }

        private bool CanExecuteOKCommand()
        {
            return Height > 0 && Width > 0 && Thickness > 0;
        }

        private void OkCommand_Execute()
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            // Generate the rectangular frame geometry using the input values
            IBeamProfile profileGenerator = new IBeamProfile(Height, Width, Thickness);
            profileGenerator.Generate();

            // Generate the sweep geometry and add it to the Rhino document
            ProfileSweep sweep = new ProfileSweep(profileGenerator);
            Polyline rail = new Polyline() { BottomLeft, new Point3d(TopRight.X, BottomLeft.Y, 0), TopRight, new Point3d(BottomLeft.X, TopRight.Y, 0), BottomLeft };
            sweep.BakeSweep(rail);
           
            doc.Views.Redraw();
        }
    }

}
