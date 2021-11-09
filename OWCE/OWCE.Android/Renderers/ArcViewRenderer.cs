using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using OWCE.Views;
using OWCE.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ArcView), typeof(ArcViewRenderer))]

namespace OWCE.Droid.Renderers
{
    public class ArcViewRenderer : ViewRenderer
    {
        ArcView _arcView;
        float _range;
        float _centerPoint;
        float _centerX;
        float _centerY;
        float _radius;
        //static float PiOnOneEighty = (float)Math.PI / 180f;
        float _startAngle;
        float _endAngle = 43;
        float _currentPercent;


        Paint _arcPaint;
        Paint _circlePaint;
        RectF _rectangle;
        float _density;

        public ArcViewRenderer(Context context) : base(context)
        {
            SetWillNotDraw(false);

            _density = Resources.DisplayMetrics.Density;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                _arcView = null;
            }

            if (e.NewElement != null)
            {
                _arcView = e.NewElement as ArcView;

                CalculateSize();
                CalculateRange();
                CalculateArc();
                SetArcColor();
                SetCircleColor();
                Invalidate();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (ArcView.WidthProperty.PropertyName.Equals(e.PropertyName) || ArcView.HeightProperty.PropertyName.Equals(e.PropertyName))
            {
                if (_arcView.Width > 0 && _arcView.Height > 0)
                {
                    CalculateSize();
                    Invalidate();
                }
            }
            else if (ArcView.MinimumProperty.PropertyName.Equals(e.PropertyName) || ArcView.MaximumProperty.PropertyName.Equals(e.PropertyName))
            {
                CalculateRange();
                CalculateArc();
                Invalidate();
            }
            else if (ArcView.CurrentProperty.PropertyName.Equals(e.PropertyName))
            {
                CalculateArc();
                Invalidate();
            }
            else if (ArcView.ArcColorProperty.PropertyName.Equals(e.PropertyName))
            {
                SetArcColor();
                Invalidate();
            }
            else if (ArcView.CircleColorProperty.PropertyName.Equals(e.PropertyName))
            {
                SetCircleColor();
                Invalidate();
            }
        }


        void CalculateSize()
        {
            var width = (float)_arcView.Width * _density;
            _rectangle = new RectF(0, 0, width, width);

            _centerPoint = (float)(0.5f * width);
            _centerX = _centerPoint;
            _centerY = _centerPoint;
            _radius = _centerPoint;
        }

        void CalculateRange()
        {
            _range = _arcView.Maximum - _arcView.Minimum;
        }

        void CalculateArc()
        {
            _currentPercent = (_arcView.Current - _arcView.Minimum) / _range;
            if (_currentPercent < 0f)
            {
                _currentPercent = 0f;
            }
            else if (_currentPercent > 1.215f) // Allows line to go all the way up to end point
            {
                _currentPercent = 1.215f;
            }

            //var startAngle = 177 * piOnOneEighty;
            _startAngle = (177 + (186f * _currentPercent));
        }

        void SetArcColor()
        {
            if (_arcPaint == null)
            {
                _arcPaint = new Paint();
            }
            _arcPaint.Color = _arcView.ArcColor.ToAndroid();
        }

        void SetCircleColor()
        {
            if (_circlePaint == null)
            {
                _circlePaint = new Paint();
            }
            _circlePaint.Color = _arcView.CircleColor.ToAndroid();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            canvas.DrawCircle(_centerX, _centerY, _radius, _circlePaint);


            var path = new Path();
            path.AddArc(_rectangle, _startAngle, _endAngle - _startAngle);
            path.LineTo(_centerX, _centerY);
            canvas.DrawPath(path, _arcPaint);
            //canvas.DrawArc(_rectangle, _startAngle, _endAngle - _startAngle, false, _arcPaint); 
        }
    }
}
