using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MvvmHelpers;
using OWCE.Extensions;
using Xamarin.Forms;

namespace OWCE.Views
{
    public partial class SettingsSwitch : ContentView
    {
        public static readonly BindableProperty OnColorProperty = BindableProperty.Create(
            nameof(OnColor),
            typeof(Color),
            typeof(SettingsSwitch),
            Color.Blue);

        public Color OnColor
        {
            get { return (Color)GetValue(OnColorProperty); }
            set { SetValue(OnColorProperty, value); }
        }


        public static readonly BindableProperty OffColorProperty = BindableProperty.Create(
            nameof(OffColor),
            typeof(Color),
            typeof(SettingsSwitch),
            Color.Black);

        public Color OffColor
        {
            get { return (Color)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }


        public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
            nameof(IsToggled),
            typeof(bool),
            typeof(SettingsSwitch),
            false);

        public bool IsToggled
        {
            get { return (bool)GetValue(IsToggledProperty); }
            set { SetValue(IsToggledProperty, value); }
        }

        // This is a bit of a gross hack, but I want the app out.
        public static readonly BindableProperty CurrentColorProperty = BindableProperty.Create(
            nameof(CurrentColor),
            typeof(Color),
            typeof(SettingsSwitch),
            Color.Black);

        public Color CurrentColor
        {
            get { return (Color)GetValue(CurrentColorProperty); }
            set { SetValue(CurrentColorProperty, value); }
        }

        bool _isToggling = false;

        public event EventHandler<bool> IsToggledChanged = null;

        public SettingsSwitch()
        {
            InitializeComponent();

            var tapGestureRecognizer = new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if (_isToggling == false)
                    {
                        IsToggled = !IsToggled;
                        IsToggledChanged?.Invoke(this, IsToggled);
                    }
                }),
            };
            GestureRecognizers.Add(tapGestureRecognizer);

            UpdateTogglePosition(false).SafeFireAndForget();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            UpdateTogglePosition(false).SafeFireAndForget();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (IsToggledProperty.PropertyName.Equals(propertyName))
            {
                UpdateTogglePosition().SafeFireAndForget();
            }
        }

        async Task UpdateTogglePosition(bool animated = true)
        {
            _isToggling = true;

            uint duration = 100;
            var easing = IsToggled ? Easing.CubicIn : Easing.CubicOut;
            if (animated == false)
            {
                duration = 0;
            }


            var xPosition = IsToggled ? (RoundedRectangle.WidthRequest - 2 - 2 - CircleDot.Width) : 0;
            var animationTasks = new List<Task>();

            animationTasks.Add(CircleDot.TranslateTo(xPosition, 0, duration, easing));

            var fromColor = RoundedRectangle.BackgroundColor;
            var toColor = IsToggled ? OnColor : OffColor;
            animationTasks.Add(RoundedRectangle.ColorTo(fromColor, toColor, (color) =>
            {
                RoundedRectangle.BackgroundColor = color;
            }, duration, easing));

            await Task.WhenAll(animationTasks);

            CurrentColor = toColor;

            _isToggling = false;
        }
    }
}
