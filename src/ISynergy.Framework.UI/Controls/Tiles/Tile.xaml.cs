﻿#if (__UWP__ || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#elif (__WINUI__)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class Tile. This class cannot be inherited.
    /// Implements the <see cref="Button" />
    /// Implements the <see cref="IComponentConnector" />
    /// </summary>
    /// <seealso cref="Button" />
    /// <seealso cref="IComponentConnector" />
    public sealed partial class Tile : Button
    {
        /// <summary>
        /// Enum Modes
        /// </summary>
        public enum Modes
        {
            /// <summary>
            /// The default
            /// </summary>
            Default,
            /// <summary>
            /// The wide
            /// </summary>
            Wide,
            /// <summary>
            /// The small
            /// </summary>
            Small
        }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public Modes Mode
        {
            get { return (Modes)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The mode property
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(Modes), typeof(Tile), new PropertyMetadata(Modes.Default));

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public string Count
        {
            get { return (string)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The count property
        /// </summary>
        public static readonly DependencyProperty CountProperty = DependencyProperty.Register(nameof(Count), typeof(string), typeof(Tile), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the size of the count font.
        /// </summary>
        /// <value>The size of the count font.</value>
        public double CountFontSize
        {
            get { return (double)GetValue(CountFontSizeProperty); }
            set { SetValue(CountFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CountFontSize.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The count font size property
        /// </summary>
        public static readonly DependencyProperty CountFontSizeProperty = DependencyProperty.Register(nameof(CountFontSize), typeof(double), typeof(Tile), new PropertyMetadata(18));

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The title property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(Tile), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the size of the title font.
        /// </summary>
        /// <value>The size of the title font.</value>
        public int TitleFontSize
        {
            get { return (int)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderFontSize.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The title font size property
        /// </summary>
        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register(nameof(TitleFontSize), typeof(int), typeof(Tile), new PropertyMetadata(10));

        /// <summary>
        /// Gets or sets the title vertical alignment.
        /// </summary>
        /// <value>The title vertical alignment.</value>
        public VerticalAlignment TitleVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(TitleVerticalAlignmentProperty); }
            set { SetValue(TitleVerticalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderVerticalAlignment.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The title vertical alignment property
        /// </summary>
        public static readonly DependencyProperty TitleVerticalAlignmentProperty = DependencyProperty.Register(nameof(TitleVerticalAlignment), typeof(VerticalAlignment), typeof(Tile), new PropertyMetadata(VerticalAlignment.Top));

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Details.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The header property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(Tile), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the size of the header font.
        /// </summary>
        /// <value>The size of the header font.</value>
        public int HeaderFontSize
        {
            get { return (int)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DetailsFontSize.  This enables animation, styling, binding, etc...
        /// <summary>
        /// The header font size property
        /// </summary>
        public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.Register(nameof(HeaderFontSize), typeof(int), typeof(Tile), new PropertyMetadata(14));

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class.
        /// </summary>
        public Tile()
        {
            InitializeComponent();
            ResizeTile();
        }

        /// <summary>
        /// Resizes the tile.
        /// </summary>
        private void ResizeTile()
        {
            switch (Mode)
            {
                case Modes.Wide:
                    break;

                case Modes.Small:
                    break;
            }
        }
    }
}
