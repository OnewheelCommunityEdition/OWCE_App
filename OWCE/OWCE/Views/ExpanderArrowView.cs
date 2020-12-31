using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace OWCE.Views
{
    public class ExpanderArrowView : ContentView
    {

        public static readonly BindableProperty ArrowColorProperty = BindableProperty.Create(
          "ArrowColor",
          typeof(Color),
          typeof(ExpanderArrowView),
          Color.White);

        public Color ArrowColor
        {
            get
            {
                return (Color)GetValue(ArrowColorProperty);
            }
            set
            {
                SetValue(ArrowColorProperty, value);
            }
        }

        public ExpanderArrowView()
        {
            InputTransparent = true;
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
                Stroke = new SolidColorBrush(ArrowColor),
                StrokeThickness = 2,
                StrokeLineJoin = PenLineJoin.Round,
                Points = new PointCollection(),
            };
            polyLine.Points.Add(new Point(2.5, 4.5));
            polyLine.Points.Add(new Point(7.5, 9.5));
            polyLine.Points.Add(new Point(12.5, 4.5));

            Content = polyLine;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (ArrowColorProperty.PropertyName.Equals(propertyName))
            {
                if (Content is Polyline polyline)
                {
                    polyline.Stroke = new SolidColorBrush(ArrowColor);
                    ForceLayout();
                }
            }
        }
    }
}
