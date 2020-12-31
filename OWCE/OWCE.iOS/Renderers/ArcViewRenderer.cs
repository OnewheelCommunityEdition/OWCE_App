using System;
using System.Collections.Generic;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using OWCE.Views;
using OWCE.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ArcView), typeof(ArcViewRenderer))]

namespace OWCE.iOS.Renderers
{
    public class ArcViewRenderer : ViewRenderer
    {
        ArcView _arcView;
        float _range;
        nfloat _centerPoint;
        nfloat _centerX;
        nfloat _centerY;
        nfloat _radius;
        static nfloat PiOnOneEighty = (nfloat)Math.PI / 180f;
        nfloat _startAngle;
        nfloat _endAngle = 43 * PiOnOneEighty;
        float _currentPercent;
        UIColor _arcColor;
        UIColor _circleColor;
        CGRect _circleRect;

        public ArcViewRenderer()
        {
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
                SetNeedsDisplay();
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
                    SetNeedsDisplay();
                }
            }
            else if (ArcView.MinimumProperty.PropertyName.Equals(e.PropertyName) || ArcView.MaximumProperty.PropertyName.Equals(e.PropertyName))
            {
                CalculateRange();
                CalculateArc();
                SetNeedsDisplay();
            }
            else if (ArcView.CurrentProperty.PropertyName.Equals(e.PropertyName))
            {
                CalculateArc();
                SetNeedsDisplay();
            }
            else if (ArcView.ArcColorProperty.PropertyName.Equals(e.PropertyName))
            {
                SetArcColor();
                SetNeedsDisplay();
            }
            else if (ArcView.CircleColorProperty.PropertyName.Equals(e.PropertyName))
            {
                SetCircleColor();
                SetNeedsDisplay();
            }
        }

        void CalculateSize()
        {
            _circleRect = new CGRect(0, 0, _arcView.Width, _arcView.Width);
            _centerPoint = (nfloat)(0.5f * _arcView.Width);
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
            _startAngle = (177 + (186f * _currentPercent)) * PiOnOneEighty;
        }

        void SetArcColor()
        {
            _arcColor = _arcView.ArcColor.ToUIColor();
        }

        void SetCircleColor()
        {
            _circleColor = _arcView.CircleColor.ToUIColor();
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            using (CGContext context = UIGraphics.GetCurrentContext())
            {
                _circleColor.SetFill();
                context.FillEllipseInRect(_circleRect);

                _arcColor.SetFill();
                context.AddArc(_centerX, _centerY, _radius, _startAngle, _endAngle, true);
                context.AddLineToPoint(_centerX, _centerY);
                context.FillPath();
            }
        }
    }
}
