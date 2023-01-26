using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace OWCE.Views
{
	public enum CustomToolbarItemPosition
	{
		Left,
		Right,
	}

	public class CustomToolbarItem : ContentView
    {
		public static readonly BindableProperty PositionProperty = BindableProperty.Create(
			propertyName: nameof(Position),
			returnType: typeof(CustomToolbarItemPosition),
			declaringType: typeof(CustomToolbarItem),
			defaultValue: CustomToolbarItemPosition.Right);

		public CustomToolbarItemPosition Position
		{
			get { return (CustomToolbarItemPosition)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}


		public static readonly BindableProperty PriorityProperty = BindableProperty.Create(
			propertyName: nameof(Priority),
			returnType: typeof(int),
			declaringType: typeof(CustomToolbarItem),
			defaultValue: 1);

		public int Priority
		{
			get { return (int)GetValue(PriorityProperty); }
			set { SetValue(PriorityProperty, value); }
		}


		public static readonly BindableProperty IconImageSourceProperty = BindableProperty.Create(
			propertyName: nameof(IconImageSource),
			returnType: typeof(ImageSource),
			declaringType: typeof(CustomToolbarItem));

		public ImageSource IconImageSource
		{
			get { return (ImageSource)GetValue(IconImageSourceProperty); }
			set { SetValue(IconImageSourceProperty, value); }
		}

        
        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(CustomToolbarItem));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }



        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            propertyName: nameof(Command),
            returnType: typeof(ICommand),
            declaringType: typeof(CustomToolbarItem));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        Label _label;
        Image _image;

        public CustomToolbarItem()
		{
			MinimumWidthRequest = 30;
			MinimumHeightRequest = 30;
            VerticalOptions = LayoutOptions.FillAndExpand;

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if (this.Command != null && this.Command.CanExecute(null))
                    {
                        // Simulate click visual
                        this.Opacity = 0.6;
                        this.FadeTo(1.0);

                        this.Command.Execute(null);
                    }
                }),
            };

            GestureRecognizers.Add(tapGestureRecognizer);
		}

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (IsEnabledProperty.PropertyName.Equals(propertyName))
            {
                UpdateIsEnabled();
            }
            else if (IconImageSourceProperty.PropertyName.Equals(propertyName))
            {
                UpdateIconImage();
            }
            else if (TextProperty.PropertyName.Equals(propertyName))
            {
                UpdateText();
            }
        }

        void UpdateIsEnabled()
        {
            Opacity = IsEnabled ? 0.5 : 1.0;
        }

        void UpdateIconImage()
        {
            // If label exists lets nuke it.
            if (_label != null)
            {
                _label = null;
            }

            if (_image == null)
            {
                _image = new Image()
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                };
                Content = _image;
            }
            _image.Source = IconImageSource;
        }

        void UpdateText()
        {
            // If image exists lets nuke it.
            if (_image != null)
            {
                _image = null;
            }

            if (_label == null)
            {
                _label = new Label()
                {
                    FontSize = 18,
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                };
                Content = _label;
            }
            _label.Text = Text;
        }
    }
}
