using System;
using System.Runtime.CompilerServices;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace OWCE.Views
{
    public class SpeedArcView : SKCanvasView
    {
        SKPaint _backgroundPaint;
        SKPaint _circlePaint;
        SKPaint _arcPaint;
        float _displayScale;
        float _circleRadius;
        float _circleCenterX;
        float _circleCenterY;

        float _arcEndX;
        float _arcEndY;
        SKRect _circleRect;
        float _endAngle;
        float _endAngleForArc;


        float _currentSweepAngle = 0;
        float _minSweepAngle = 5;
        int _animationNumber = 0;

        float _totalArc;

        public static readonly BindableProperty CurrentRPMProperty = BindableProperty.Create(
            propertyName: nameof(CurrentRPM),
            returnType: typeof(int),
            declaringType: typeof(SpeedArcView),
            defaultValue: 0);

        public int CurrentRPM
        {
            get { return (int)GetValue(CurrentRPMProperty); }
            set { SetValue(CurrentRPMProperty, value); }
        }


        public static readonly BindableProperty MinRPMProperty = BindableProperty.Create(
            propertyName: nameof(MinRPM),
            returnType: typeof(int),
            declaringType: typeof(SpeedArcView),
            defaultValue: 0);

        public int MinRPM
        {
            get { return (int)GetValue(MinRPMProperty); }
            set { SetValue(MinRPMProperty, value); }
        }


        public static readonly BindableProperty MaxRPMProperty = BindableProperty.Create(
            propertyName: nameof(MaxRPM),
            returnType: typeof(int),
            declaringType: typeof(SpeedArcView),
            defaultValue: 0);

        public int MaxRPM
        {
            get { return (int)GetValue(MaxRPMProperty); }
            set { SetValue(MaxRPMProperty, value); }
        }


        public SpeedArcView()
        {
            _backgroundPaint = new SKPaint();
            _backgroundPaint.Color = new SKColor(255, 255, 0); // Bright yellow

            _circlePaint = new SKPaint();
            _circlePaint.Color = new SKColor(0, 0, 0, 20); // Black with 8% alpha

            _arcPaint = new SKPaint();
            _arcPaint.Color = new SKColor(0, 0, 0, 31); // Black with 12% alpha

            _displayScale = (float)Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
            //_circleRadius = (_displayScale * 278) * 0.5f;
            //_startAngle = (float)(Math.PI / 180) * 270;
            //_endAngle = (float)(Math.PI / 180) * 13;

            // Total arc is from 3.318 degrees below the center all the way to 45 degrees past the other center.
            _totalArc = (3.318f + 180f + 45f);


            _endAngle = 45;
            _endAngleForArc = 90 - _endAngle;

            _currentSweepAngle = _minSweepAngle;
            //_maxSweepAngle = 270 - _endAngle;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            // Scale up for units we use to draw with SkiaSharp.
            var scaledWidth = width * _displayScale;

            // padding magic value of 0.1924 = 66 / 343, to match the designs
            var padding = scaledWidth * 0.24f;

            // circle radius is half of (width minus padding)
            _circleRadius = (float)((scaledWidth - padding) * 0.5f);

            // Circle is centered horizontally but touches the top vertically
            _circleCenterX = (float)(scaledWidth * 0.5);
            _circleCenterY = _circleRadius;
            _circleRect = new SKRect(_circleCenterX - _circleRadius, 0, _circleCenterX + _circleRadius, _circleRadius * 2);

            _arcEndX = _circleCenterX + (_circleRadius * (float)Math.Sin(DegreesToRadians(45))); // 13?
            _arcEndY = _circleCenterY + (_circleRadius * (float)Math.Cos(DegreesToRadians(45))); // 13?

            // Set the height of the widget to the 2.888% increase of the circle radius
            var screenCircleRadius = (_circleRadius / _displayScale);
            HeightRequest = (screenCircleRadius + (screenCircleRadius * 0.0288f)); // * 2;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == CurrentRPMProperty.PropertyName)
            {
                UpdateTargetArcAngle();
            }
            else if(propertyName == MinRPMProperty.PropertyName)
            {
                UpdateTargetArcAngle();
            }
            else if(propertyName == MaxRPMProperty.PropertyName)
            {
                UpdateTargetArcAngle();
            }
        }

        float DegreesToRadians(float degrees)
        {
            return (float)(Math.PI / 180.0) * degrees;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("OnPaintSurface");
            base.OnPaintSurface(e);

            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.Yellow);

            canvas.DrawCircle(_circleCenterX, _circleCenterY, _circleRadius, _circlePaint);

            SKPath path = new SKPath();
            path.MoveTo(_circleCenterX, _circleCenterY);
            path.LineTo(_arcEndX, _arcEndY);

            path.ArcTo(_circleRect, _endAngleForArc, _currentSweepAngle * -1, false);
            //path.ArcTo(_arcEndX, _arcEndY, _arcStartX, _arcStartY, _circleRadius);
            path.Close();
            canvas.DrawPath(path, _arcPaint);
        }

        void UpdateTargetArcAngle()
        {
            float oldSweepAngle = _currentSweepAngle;

            float targetSweepAngle = _totalArc * ((MaxRPM - CurrentRPM) / (float)MaxRPM);

            if (targetSweepAngle < _minSweepAngle)
            {
                targetSweepAngle = _minSweepAngle;
            }
            else if (targetSweepAngle > _totalArc)
            {
                targetSweepAngle = _totalArc;
            }

            ++_animationNumber;
            var animationNumber = _animationNumber;

            var animation = new Animation(v =>
            {
                _currentSweepAngle = (float)v;
                InvalidateSurface();
                //Device.BeginInvokeOnMainThread(InvalidateSurface);
            }, oldSweepAngle, targetSweepAngle);

            animation.Commit(this, "SimpleAnimation", 16, 250, Easing.Linear, (a, b) =>
            {
                //System.Diagnostics.Debug.WriteLine($"Finished: {animationNumber}");
            });

        }
    }
}

