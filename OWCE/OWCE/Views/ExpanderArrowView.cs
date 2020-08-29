using System;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace OWCE.Views
{
    public class ExpanderArrowView : ContentView
    {
        public ExpanderArrowView()
        {
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;
            WidthRequest = 15;
            HeightRequest = 15;

            var polyLine = new Polyline()
            {
                WidthRequest = 15,
                HeightRequest = 15,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Stroke = Brush.White,
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                Points = new PointCollection(),
            };
            polyLine.Points.Add(new Point(2.5, 4.5));
            polyLine.Points.Add(new Point(7.5, 9.5));
            polyLine.Points.Add(new Point(12.5, 4.5));

            Content = polyLine;
        }
    }
}
