﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Controls
{
    /// <summary>
    /// Defines a control for providing a header for read-only text.
    /// </summary>
    [TemplatePart(Name = "HeaderContentPresenter", Type = typeof(ContentPresenter))]
    [ContentProperty(Name = nameof(Inlines))]
    public class HeaderedTextBlock : Control
    {
        private ContentPresenter _headerContentPresenter;
        private TextBlock _textContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedTextBlock"/> class.
        /// </summary>
        public HeaderedTextBlock()
        {
            DefaultStyleKey = typeof(HeaderedTextBlock);
        }

        /// <summary>
        /// Called when applying the control template.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _headerContentPresenter = GetTemplateChild("HeaderContentPresenter") as ContentPresenter;
            _textContent = GetTemplateChild("TextContent") as TextBlock;

            UpdateVisibility();
            Inlines.AddItemsToTextBlock(_textContent);
            UpdateForOrientation(Orientation);
        }

        private void UpdateVisibility()
        {
            if (_headerContentPresenter != null)
            {
                _headerContentPresenter.Visibility = _headerContentPresenter.Content == null
                                                     ? Visibility.Collapsed
                                                     : Visibility.Visible;
            }

            if (_textContent != null)
            {
                _textContent.Visibility = string.IsNullOrWhiteSpace(_textContent.Text) && HideTextIfEmpty
                                                    ? Visibility.Collapsed
                                                    : Visibility.Visible;
            }
        }

        private void UpdateForOrientation(Orientation orientationValue)
        {
            switch (orientationValue)
            {
                case Orientation.Vertical:
                    VisualStateManager.GoToState(this, "Vertical", true);
                    break;
                case Orientation.Horizontal:
                    VisualStateManager.GoToState(this, "Horizontal", true);
                    break;
            }
        }

        /// <summary>
        /// Defines the <see cref="HeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            nameof(HeaderTemplate),
            typeof(DataTemplate),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(null));

        /// <summary>
        /// Defines the <see cref="TextStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextStyleProperty = DependencyProperty.Register(
            nameof(TextStyle),
            typeof(Style),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(null));

        /// <summary>
        /// Defines the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(null, (d, e) => { ((HeaderedTextBlock)d).UpdateVisibility(); }));

        /// <summary>
        /// Defines the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(null, (d, e) => { ((HeaderedTextBlock)d).UpdateVisibility(); }));

        /// <summary>
        /// Defines the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(Orientation.Vertical, (d, e) => { ((HeaderedTextBlock)d).UpdateForOrientation((Orientation)e.NewValue); }));

        /// <summary>
        /// Defines the <see cref="HideTextIfEmpty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HideTextIfEmptyProperty = DependencyProperty.Register(
            nameof(HideTextIfEmpty),
            typeof(bool),
            typeof(HeaderedTextBlock),
            new PropertyMetadata(false, (d, e) => { ((HeaderedTextBlock)d).UpdateVisibility(); }));

        /// <summary>
        /// Gets or sets the header style.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderTemplateProperty);
            }

            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text style.
        /// </summary>
        public Style TextStyle
        {
            get
            {
                return (Style)GetValue(TextStyleProperty);
            }

            set
            {
                SetValue(TextStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header
        {
            get
            {
                return (string)GetValue(HeaderProperty);
            }

            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets the collection of inline text elements within a Windows.UI.Xaml.Controls.TextBlock.
        /// </summary>
        /// <returns>
        /// A collection that holds all inline text elements from the Windows.UI.Xaml.Controls.TextBlock. The default is an empty collection.</returns>
        public InlineCollectionWrapper Inlines { get; } = new InlineCollectionWrapper();

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Text TextBlock is hidden if its value is empty
        /// </summary>
        public bool HideTextIfEmpty
        {
            get
            {
                return (bool)GetValue(HideTextIfEmptyProperty);
            }

            set
            {
                SetValue(HideTextIfEmptyProperty, value);
            }
        }
    }
}
