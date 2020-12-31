using System;
using Xamarin.Forms;

namespace OWCE.Views
{
    public class ArcView : View
    {
        public static readonly BindableProperty ArcColorProperty = BindableProperty.Create(
            propertyName: "ArcColor",
            returnType: typeof(Color),
            declaringType: typeof(ArcView),
            defaultValue: Color.FromHex("#F3F300"));

        public Color ArcColor
        {
            get { return (Color)GetValue(ArcColorProperty); }
            set { SetValue(ArcColorProperty, value); }
        }

        public static readonly BindableProperty CircleColorProperty = BindableProperty.Create(
            propertyName: "CircleColor",
            returnType: typeof(Color),
            declaringType: typeof(ArcView),
            defaultValue: Color.FromHex("#D6D600"));

        public Color CircleColor
        {
            get { return (Color)GetValue(CircleColorProperty); }
            set { SetValue(CircleColorProperty, value); }
        }

        public static readonly BindableProperty MinimumProperty = BindableProperty.Create(
            propertyName: "Minimum",
            returnType: typeof(float),
            declaringType: typeof(ArcView),
            defaultValue: 0f);

        public float Minimum
        {
            get { return (float)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly BindableProperty MaximumProperty = BindableProperty.Create(
            propertyName: "Maximum",
            returnType: typeof(float),
            declaringType: typeof(ArcView),
            defaultValue: 100f);

        public float Maximum
        {
            get { return (float)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly BindableProperty CurrentProperty = BindableProperty.Create(
            propertyName: "Current",
            returnType: typeof(float),
            declaringType: typeof(ArcView),
            defaultValue: 0f);

        public float Current
        {
            get { return (float)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }



        public ArcView()
        {
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > 0 && height > 0)
            {

            }
        }
    }
}
