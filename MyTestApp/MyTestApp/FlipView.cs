using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsSilverEduMemoryGame
{
    /// <summary>
    /// Mostly copied from https://github.com/UdaraAlwis/XFFlipViewControl
    /// </summary>
    public class FlipView : ContentView
    {
        /// <summary>
        /// Unveil = shows its contents.
        /// Veil = hides its contents.
        /// Content change = when the flip reaches half.
        /// Fade out = animation at removal.
        /// </summary>
        internal event EventHandler BeforeUnveil, AfterUnveil,
            BeforeVeil, AfterVeil,
            BeforeContentChange, AfterContentChange;



        private readonly RelativeLayout _contentHolder;

        public FlipView()
        {
            _contentHolder = new RelativeLayout();
            Content = _contentHolder;
        }

        public static readonly BindableProperty FrontViewProperty =
            BindableProperty.Create(
                nameof(FrontView),
                typeof(View),
                typeof(FlipView),
                null,
                BindingMode.Default,
                null,
                FrontViewPropertyChanged);
        private static void FrontViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != null)
            {
                ((FlipView)bindable)
                    ._contentHolder
                    .Children
                    .Add(((FlipView)bindable).FrontView,
                        Constraint.Constant(0),
                        Constraint.Constant(0),
                        Constraint.RelativeToParent((parent) => parent.Width),
                        Constraint.RelativeToParent((parent) => parent.Height)
                    );
            }
        }
        /// <summary>
        /// Gets or Sets the front view (contents)
        /// </summary>
        public View FrontView
        {
            get { return (View)this.GetValue(FrontViewProperty); }
            set { this.SetValue(FrontViewProperty, value); }
        }

        public static readonly BindableProperty BackViewProperty =
            BindableProperty.Create(
                nameof(BackView),
                typeof(View),
                typeof(FlipView),
                null,
                BindingMode.Default,
                null,
                BackViewPropertyChanged);
        private static void BackViewPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            //Set BackView Rotation before rotating
            if (newvalue != null)
            {
                ((FlipView)bindable)
                    ._contentHolder
                    .Children
                    .Add(((FlipView)bindable).BackView,
                        Constraint.Constant(0),
                        Constraint.Constant(0),
                        Constraint.RelativeToParent((parent) => parent.Width),
                        Constraint.RelativeToParent((parent) => parent.Height)
                        );
                ((FlipView)bindable).BackView.IsVisible = false;
            }
        }
        /// <summary>
        /// Gets or Sets the back view ("?")
        /// </summary>
        public View BackView
        {
            get { return (View)this.GetValue(BackViewProperty); }
            set { this.SetValue(BackViewProperty, value); }
        }


        /// <summary>
        /// True: "?" is shown.
        /// </summary>
        internal bool InternalIsFlipped { get; set; } = false;

        public async Task Toggle()
        {
            if (InternalIsFlipped) // if "?" is shown
            {
                await FlipFromBackToFront(); // show contents
            }
            else
            {
                await FlipFromFrontToBack(); // show "?"
            }
        }


        /// <summary>
        /// Performs the flip: from contents to "?"
        /// </summary>
        public async Task FlipFromFrontToBack()
        {
            BeforeVeil?.Invoke(this, EventArgs.Empty);

            await FrontToBackRotate();

            BeforeContentChange?.Invoke(this, EventArgs.Empty);

            // Change the visible content
            FrontView.IsVisible = false;

            BackView.IsVisible = true;

            InternalIsFlipped = true;

            AfterContentChange?.Invoke(this, EventArgs.Empty);

            await BackToFrontRotate();

            AfterVeil?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Performs the flip: from "?" to contents
        /// </summary>
        public async Task FlipFromBackToFront()
        {
            BeforeUnveil?.Invoke(this, EventArgs.Empty);

            await FrontToBackRotate();

            BeforeContentChange?.Invoke(this, EventArgs.Empty);

            FrontView.IsVisible = true;

            BackView.IsVisible = false;

            InternalIsFlipped = false;

            AfterContentChange?.Invoke(this, EventArgs.Empty);

            await BackToFrontRotate();

            AfterUnveil?.Invoke(this, EventArgs.Empty);
        }

        #region Animation Stuff

        private async Task<bool> FrontToBackRotate()
        {
            ViewExtensions.CancelAnimations(this);
            this.RotationY = 360;
            await this.RotateYTo(270, 500, Easing.Linear);
            return true;
        }

        private async Task<bool> BackToFrontRotate()
        {
            ViewExtensions.CancelAnimations(this);
            this.RotationY = 90;
            await this.RotateYTo(0, 500, Easing.Linear);
            return true;
        }

        #endregion
    }
}