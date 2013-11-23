using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using EyeLens.Resources;
using Windows.Phone.Media.Capture;
using System.Threading;
using System.Windows.Media;
using Windows.Phone.Speech.Synthesis;
using System.Windows.Input;
using EyeLens.ViewModel;

namespace EyeLens
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MediaElement _mediaElement = null;
        private PhotoCaptureDevice _photoCaptureDevice = null;
        private NokiaImagingSDKEffects _cameraEffect = null;
        private CameraStreamSource _cameraStreamSource = null;
        private Semaphore _cameraSemaphore = new Semaphore(1, 1);

        public MainPage()
        {
            InitializeComponent();

            ApplicationBar = new ApplicationBar();

            var previousButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/previous.png", UriKind.Relative));
            previousButton.Text = AppResources.PreviousEffectButtonText;
            //previousButton.Text = LocalizedStrings["PreviousEffectButtonText"];
            previousButton.Click += PreviousButton_Click;

            ApplicationBar.Buttons.Add(previousButton);

            var nextButton = new ApplicationBarIconButton(new Uri("/Assets/Icons/next.png", UriKind.Relative));
            nextButton.Text = AppResources.NextEffectButtonText;
            nextButton.Click += NextButton_Click;

            ApplicationBar.Buttons.Add(nextButton);

            var zoomInButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/zoomin.png", UriKind.Relative));
            zoomInButton.Text = AppResources.ZoomInButtonText;
            zoomInButton.Click+=zoomInButton_Click;
            ApplicationBar.Buttons.Add(zoomInButton);

            var zoomOutButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/zoomout.png", UriKind.Relative));
            zoomOutButton.Text = AppResources.ZoomOutButtonText;
            zoomOutButton.Click += zoomOutButton_Click;
            ApplicationBar.Buttons.Add(zoomOutButton);

            var aboutMenuItem = new ApplicationBarMenuItem();
            aboutMenuItem.Text = AppResources.AboutPageButtonText;
            aboutMenuItem.Click += AboutMenuItem_Click;

            ApplicationBar.MenuItems.Add(aboutMenuItem);

            var settingsMenuItem = new ApplicationBarMenuItem();
            settingsMenuItem.Text = AppResources.SettingsPageButtonText;
            settingsMenuItem.Click+=settingsMenuItem_Click;

            ApplicationBar.MenuItems.Add(settingsMenuItem);
        }

        void zoomOutButton_Click(object sender, EventArgs e)
        {
            ViewModelLocator.MainStatic.SayText(AppResources.ZoomingOut);
            ViewModelLocator.MainStatic.ZoomLevel--;
            //this.viewCompositeTransform.ScaleX = ViewModelLocator.MainStatic.ZoomLevel;
            //this.viewCompositeTransform.ScaleY = ViewModelLocator.MainStatic.ZoomLevel;
        }

        private void zoomInButton_Click(object sender, EventArgs e)
        {
            ViewModelLocator.MainStatic.SayText(AppResources.ZoomingIn);
            ViewModelLocator.MainStatic.ZoomLevel++;
            //this.viewCompositeTransform.ScaleX = ViewModelLocator.MainStatic.ZoomLevel;
           // this.viewCompositeTransform.ScaleY = ViewModelLocator.MainStatic.ZoomLevel;
        }

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/Pages/AboutPage.xaml", UriKind.Relative));
            }
            catch{ };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Initialize();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            while (!_cameraSemaphore.WaitOne(100)) ;

            Uninitialize();

            _cameraSemaphore.Release();
        }

        private async void Initialize()
        {
            //Dispatcher.BeginInvoke(async () =>
            //{
                StatusTextBlock.Text = AppResources.MainPage_StatusTextBlock_StartingCamera;

                var resolutionsList = PhotoCaptureDevice.GetAvailablePreviewResolutions(CameraSensorLocation.Back);
                var resolution = resolutionsList.Last();

                _photoCaptureDevice = await PhotoCaptureDevice.OpenAsync(CameraSensorLocation.Back, resolution);

                await _photoCaptureDevice.SetPreviewResolutionAsync(resolution);

                _cameraEffect = new NokiaImagingSDKEffects();
                _cameraEffect.PhotoCaptureDevice = _photoCaptureDevice;

                _cameraStreamSource = new CameraStreamSource(_cameraEffect, resolution);
                _cameraStreamSource.FrameRateChanged += CameraStreamSource_FPSChanged;

                _mediaElement = new MediaElement();
                _mediaElement.Stretch = Stretch.UniformToFill;
                _mediaElement.BufferingTime = new TimeSpan(0);
                _mediaElement.SetSource(_cameraStreamSource);

                // Using VideoBrush in XAML instead of MediaElement, because otherwise
                // CameraStreamSource.CloseMedia() does not seem to be called by the framework:/

                BackgroundVideoBrush.SetSource(_mediaElement);

                StatusTextBlock.Text = _cameraEffect.EffectName;
            //});
        }

        private void Uninitialize()
        {
            StatusTextBlock.Text = "";

            if (_mediaElement != null)
            {
                _mediaElement.Source = null;
                _mediaElement = null;
            }

            if (_cameraStreamSource != null)
            {
                _cameraStreamSource.FrameRateChanged -= CameraStreamSource_FPSChanged;
                _cameraStreamSource = null;
            }

            _cameraEffect = null;

            if (_photoCaptureDevice != null)
            {
                _photoCaptureDevice.Dispose();
                _photoCaptureDevice = null;
            }
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AboutPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// switch to the next filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NextButton_Click(object sender, EventArgs e)
        {
            _cameraEffect.NextEffect();

            StatusTextBlock.Text = _cameraEffect.EffectName;

            await ViewModelLocator.MainStatic.SayText(AppResources.Filter + _cameraEffect.ShortFilterName);
        }

        /// <summary>
        /// switch to the previous filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PreviousButton_Click(object sender, EventArgs e)
        {
            _cameraEffect.PreviousEffect();

            StatusTextBlock.Text = _cameraEffect.EffectName;
            await ViewModelLocator.MainStatic.SayText(AppResources.Filter + _cameraEffect.ShortFilterName);
        }

        private void CameraStreamSource_FPSChanged(object sender, int e)
        {
            FrameRateTextBlock.Text = String.Format(AppResources.MainPage_FrameRateTextBlock_Format, e);
        }

        /// <summary>
        /// focus camera on tap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ViewModelLocator.MainStatic.SayText(AppResources.MakingFocus);
            if (_cameraSemaphore.WaitOne(100))
            {
                await _photoCaptureDevice.FocusAsync();
                _cameraSemaphore.Release();
            }
        }


        private void LayoutRoot_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
        }

        private void LayoutRoot_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
        }

        private void LayoutRoot_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (e.IsInertial)
            {
                this.OnFlick(sender, e);
            }
        }

        private void OnFlick(object sender, ManipulationCompletedEventArgs e)
        {
            // All of the properties on FlickGestureEventArgs have been replaced by the single property
            // FinalVelocities.LinearVelocity.  This method shows how to retrieve from FinalVelocities.LinearVelocity
            // the properties that used to be in FlickGestureEventArgs.
            double horizontalVelocity = e.FinalVelocities.LinearVelocity.X;
            double verticalVelocity = e.FinalVelocities.LinearVelocity.Y;

            //MessageBox.Show(string.Format("{0} Flick: Angle {1} Velocity {2},{3}",
            //     this.GetDirection(horizontalVelocity, verticalVelocity), Math.Round(this.GetAngle(horizontalVelocity, verticalVelocity)), horizontalVelocity, verticalVelocity));
            if (this.GetDirection(horizontalVelocity, verticalVelocity) == System.Windows.Controls.Orientation.Horizontal)
            {
                if (Math.Round(this.GetAngle(horizontalVelocity, verticalVelocity)) == 180)
                {        
                    //next filter
                    NextButton_Click(this, EventArgs.Empty);
                }
                else
                {
                    //previous filter
                    PreviousButton_Click(this, EventArgs.Empty);
                };
            };

            if (this.GetDirection(horizontalVelocity, verticalVelocity) == System.Windows.Controls.Orientation.Vertical)
            {
                if (Math.Round(this.GetAngle(horizontalVelocity, verticalVelocity)) == 90)
                {
                    zoomOutButton_Click(this, EventArgs.Empty);
                }
                else
                {
                    zoomInButton_Click(this, EventArgs.Empty);
                };
            };
        }

        private Orientation GetDirection(double x, double y)
        {
            return Math.Abs(x) >= Math.Abs(y) ? System.Windows.Controls.Orientation.Horizontal : System.Windows.Controls.Orientation.Vertical;
        }

        private double GetAngle(double x, double y)
        {
            // Note that this function works in xaml coordinates, where positive y is down, and the
            // angle is computed clockwise from the x-axis. 
            double angle = Math.Atan2(y, x);

            // Atan2() returns values between pi and -pi.  We want a value between
            // 0 and 2 pi.  In order to compensate for this, we'll add 2 pi to the angle
            // if it's less than 0, and then multiply by 180 / pi to get the angle
            // in degrees rather than radians, which are the expected units in XAML.
            if (angle < 0)
            {
                angle += 2 * Math.PI;
            }

            return angle * 180 / Math.PI;
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}