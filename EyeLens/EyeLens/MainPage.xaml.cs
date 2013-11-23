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

            var aboutMenuItem = new ApplicationBarMenuItem();
            aboutMenuItem.Text = AppResources.AboutPageButtonText;
            aboutMenuItem.Click += AboutMenuItem_Click;

            ApplicationBar.MenuItems.Add(aboutMenuItem);
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

                var resolution = PhotoCaptureDevice.GetAvailablePreviewResolutions(CameraSensorLocation.Back).Last();

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
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private async void NextButton_Click(object sender, EventArgs e)
        {
            _cameraEffect.NextEffect();

            StatusTextBlock.Text = _cameraEffect.EffectName;

            SpeechSynthesizer synth = new SpeechSynthesizer();
            await synth.SpeakTextAsync(AppResources.NextEffectButtonText + _cameraEffect.ShortFilterName);

        }

        private async void PreviousButton_Click(object sender, EventArgs e)
        {
            _cameraEffect.PreviousEffect();

            StatusTextBlock.Text = _cameraEffect.EffectName;
            SpeechSynthesizer synth = new SpeechSynthesizer();
            await synth.SpeakTextAsync(AppResources.PreviousEffectButtonText + _cameraEffect.ShortFilterName);
        }

        private void CameraStreamSource_FPSChanged(object sender, int e)
        {
            FrameRateTextBlock.Text = String.Format(AppResources.MainPage_FrameRateTextBlock_Format, e);
        }

        private async void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_cameraSemaphore.WaitOne(100))
            {
                await _photoCaptureDevice.FocusAsync();

                _cameraSemaphore.Release();
            }
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