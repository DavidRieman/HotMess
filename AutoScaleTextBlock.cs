using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HotMess
{
    public class AutoScaleTextBlock : TextBlock
    {
        public double MaxFontSize
        {
            get { return (double)GetValue(MaxFontSizeProperty); }
            set { SetValue(MaxFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxFontSizeProperty =
            DependencyProperty.Register("MaxFontSize", typeof(double), typeof(AutoScaleTextBlock), new UIPropertyMetadata(500d));

        public double MinFontSize
        {
            get { return (double)GetValue(MinFontSizeProperty); }
            set { SetValue(MinFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinFontSizeProperty =
            DependencyProperty.Register("MinFontSize", typeof(double), typeof(AutoScaleTextBlock), new UIPropertyMetadata(5d));

        private bool justUpdated = false;
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!justUpdated)
            {
                Update();
            }
            else
            {
                justUpdated = false;
            }
        }

        private void Update()
        {
            double availableWidth = this.Width;
            double availableHeight = this.Height;

            // TODO: Binary search.
            var lastFitSize = this.MinFontSize;
            for (double fontSize = this.MinFontSize; fontSize < this.MaxFontSize; fontSize += 0.5)
            {
                this.FontSize = fontSize;
                this.UpdateLayout();

                if (this.ActualWidth > this.Width || this.ActualHeight > this.Height)
                {
                    break;
                }

                lastFitSize = fontSize;
            }

            this.FontSize = lastFitSize;
            this.UpdateLayout();
            justUpdated = true;
        }
    }
}