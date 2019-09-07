using System;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyTestApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        internal Timer initialFullViewTimer = new Timer()
        {
            Interval = 5000,
            AutoReset = true
        };

        public GamePage()
        {
            InitializeComponent();

            initialFullViewTimer.Elapsed += HandleElapsed;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            MyFirstPiece.AfterUnveil += HandleAfterUnveil;

            MyFirstPiece.FlipFromBackToFront();
            MySecondPiece.FlipFromBackToFront();
        }

        public void HandleAfterUnveil(object sender, EventArgs e)
        {
            initialFullViewTimer.Start();
            MyFirstPiece.AfterUnveil -= HandleAfterUnveil;
        }

        private void HandleElapsed(object sender, EventArgs e)
        {
            // stop the timer so it does not ticking again
            initialFullViewTimer.Stop();

            MyFirstPiece.Toggle();
            MySecondPiece.Toggle();
        }
    }
}