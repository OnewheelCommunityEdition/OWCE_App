using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OWCE.Extensions
{
    public static class ViewExtensions
    {
        public static Task<bool> ColorTo(this VisualElement self, Color fromColor, Color toColor, Action<Color> callback, uint length = 250, Easing easing = null)
        {
            Func<double, Color> transform = (t) =>
              Color.FromRgba(fromColor.R + t * (toColor.R - fromColor.R),
                             fromColor.G + t * (toColor.G - fromColor.G),
                             fromColor.B + t * (toColor.B - fromColor.B),
                             fromColor.A + t * (toColor.A - fromColor.A));
            return ColorAnimation(self, "ColorTo", transform, callback, length, easing);
        }

        static Task<bool> ColorAnimation(VisualElement element, string name, Func<double, Color> transform, Action<Color> callback, uint length, Easing easing)
        {
            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            element.Animate<Color>(name, transform, callback, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));
            return taskCompletionSource.Task;
        }


        public static void AnimateWidthPercent(this VisualElement self, double toPercent) //,Action<Color> callback, uint length = 250, Easing easing = null)
        {
            if (self.Parent is VisualElement parent)
            {
                self.AbortAnimation("WidthPercent");
                var newWidth = parent.Width * toPercent;
                var animation = new Animation(width => self.WidthRequest = width, self.WidthRequest, newWidth);
                animation.Commit(self, "WidthPercent");
            }


            /*


            Func<double, Color> transform = (t) =>
              Color.FromRgba(fromColor.R + t * (toColor.R - fromColor.R),
                             fromColor.G + t * (toColor.G - fromColor.G),
                             fromColor.B + t * (toColor.B - fromColor.B),
                             fromColor.A + t * (toColor.A - fromColor.A));
            return ColorAnimation(self, "WidthPercent", transform, callback, length, easing);
            */
        }

        /*
        public static void CancelAnimation(this VisualElement self)
        {
            self.AbortAnimation("WidthPercent");
        }

        static Task<bool> ColorAnimation(VisualElement element, string name, Func<double, Color> transform, Action<Color> callback, uint length, Easing easing)
        {
            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            element.Animate<Color>(name, transform, callback, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));
            return taskCompletionSource.Task;
        }
        */
    }
}
