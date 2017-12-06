// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Expression = System.Linq.Expressions.Expression;

namespace ConverterSystems.Workstation.Controls
{
    public class Trendline : Control
    {
        public static readonly DependencyProperty ItemsSourceProperty;
        public static readonly DependencyProperty ValuePathProperty;
        public static readonly DependencyProperty TimePathProperty;
        public static readonly DependencyProperty MinValueProperty;
        public static readonly DependencyProperty MaxValueProperty;
        public static readonly DependencyProperty StrokeProperty;
        public static readonly DependencyProperty StrokeThicknessProperty;
        public static readonly DependencyProperty TimeSpanProperty;
        public static readonly DependencyProperty StartTimeProperty;
        public static readonly DependencyProperty EndTimeProperty;
        public static readonly DependencyProperty GeometryProperty;
        public static readonly DependencyProperty ShowAxisProperty;
        public static readonly DependencyProperty AutoRangeProperty;


        private readonly ScaleTransform _renderScale;
        private readonly TranslateTransform _renderTranslate;
        private readonly ScaleTransform _scale;
        private readonly TranslateTransform _translate;
        private FrameworkElement _chartArea;
        private bool _isAnimationRunning;
        private bool _isGeometryInvalid = true;
        private DateTime _previousUpdateTime;
        private Func<object, DateTime> _timeGetter;
        private Func<object, IConvertible> _valueGetter;

        static Trendline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (Trendline), new FrameworkPropertyMetadata(typeof (Trendline)));
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof (IEnumerable), typeof (Trendline), new FrameworkPropertyMetadata(null, OnItemsSourceChanged));
            ValuePathProperty = DependencyProperty.Register("ValuePath", typeof (string), typeof (Trendline), new FrameworkPropertyMetadata("Value"), IsPathValid);
            TimePathProperty = DependencyProperty.Register("TimePath", typeof (string), typeof (Trendline), new FrameworkPropertyMetadata("SourceTimestamp"), IsPathValid);
            MinValueProperty = DependencyProperty.Register("MinValue", typeof (double), typeof (Trendline), new FrameworkPropertyMetadata(0.0, OnMinMaxChanged), IsMinMaxValid);
            MaxValueProperty = DependencyProperty.Register("MaxValue", typeof (double), typeof (Trendline), new FrameworkPropertyMetadata(100.0, OnMinMaxChanged), IsMinMaxValid);
            StartTimeProperty = DependencyProperty.Register("StartTime", typeof (DateTime), typeof (Trendline), new FrameworkPropertyMetadata(DateTime.MinValue));
            EndTimeProperty = DependencyProperty.Register("EndTime", typeof (DateTime), typeof (Trendline), new FrameworkPropertyMetadata(DateTime.MaxValue));
            StrokeProperty = DependencyProperty.Register("Stroke", typeof (Brush), typeof (Trendline), new FrameworkPropertyMetadata(null));
            StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof (double), typeof (Trendline), new FrameworkPropertyMetadata(1.0));
            TimeSpanProperty = DependencyProperty.Register("TimeSpan", typeof (TimeSpan), typeof (Trendline), new FrameworkPropertyMetadata(new TimeSpan(0, 0, 60)));
            GeometryProperty = DependencyProperty.Register("Geometry", typeof (StreamGeometry), typeof (Trendline));
            ShowAxisProperty = DependencyProperty.Register("ShowAxis", typeof (bool), typeof (Trendline), new FrameworkPropertyMetadata(false));
            AutoRangeProperty = DependencyProperty.Register("AutoRange", typeof (bool), typeof (Trendline), new FrameworkPropertyMetadata(true));
        }

        public Trendline()
        {
            _translate = new TranslateTransform();
            _scale = new ScaleTransform();
            _renderTranslate = new TranslateTransform();
            _renderScale = new ScaleTransform();
            Geometry = new StreamGeometry { Transform = new TransformGroup { Children = new TransformCollection { _translate, _scale, _renderScale, _renderTranslate } } };
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        [Category("Common")]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        [Category("Common")]
        public double MinValue
        {
            get { return (double) GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        [Category("Common")]
        public double MaxValue
        {
            get { return (double) GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        [Category("Common")]
        public TimeSpan TimeSpan
        {
            get { return (TimeSpan) GetValue(TimeSpanProperty); }
            set { SetValue(TimeSpanProperty, value); }
        }

        [Category("Common")]
        public string TimePath
        {
            get { return (string) GetValue(TimePathProperty); }
            set { SetValue(TimePathProperty, value); }
        }

        [Category("Common")]
        public string ValuePath
        {
            get { return (string) GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        [Category("Brush")]
        public Brush Stroke
        {
            get { return (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        [Category("Appearance")]
        [TypeConverter(typeof (LengthConverter))]
        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public bool AutoRange
        {
            get { return (bool) GetValue(AutoRangeProperty); }
            set { SetValue(AutoRangeProperty, value); }
        }

        public bool ShowAxis
        {
            get { return (bool) GetValue(ShowAxisProperty); }
            set { SetValue(ShowAxisProperty, value); }
        }

        public DateTime StartTime
        {
            get { return (DateTime) GetValue(StartTimeProperty); }
            private set { SetValue(StartTimeProperty, value); }
        }

        public DateTime EndTime
        {
            get { return (DateTime) GetValue(EndTimeProperty); }
            private set { SetValue(EndTimeProperty, value); }
        }

        public StreamGeometry Geometry
        {
            get { return (StreamGeometry) GetValue(GeometryProperty); }
            private set { SetValue(GeometryProperty, value); }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Trendline) d).OnItemsSourceChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue);
        }

        private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            _valueGetter = null;
            _timeGetter = null;

            if (oldValue is INotifyCollectionChanged)
            {
                (oldValue as INotifyCollectionChanged).CollectionChanged -= OnCollectionChanged;
            }
            if (newValue != null)
            {
                if (newValue is INotifyCollectionChanged)
                {
                    (newValue as INotifyCollectionChanged).CollectionChanged += OnCollectionChanged;
                }
                var elementType = newValue.AsQueryable().ElementType;
                _valueGetter = TryMakePropertyGetDelegate<IConvertible>(elementType, ValuePath);
                _timeGetter = TryMakePropertyGetDelegate<DateTime>(elementType, TimePath);
            }
        }

        private static Func<object, T> TryMakePropertyGetDelegate<T>(Type instanceType, string propertyOrField)
        {
            try
            {
                var obj = Expression.Parameter(typeof (object), "o");
                var expr = Expression.Lambda<Func<object, T>>(Expression.Convert(Expression.PropertyOrField(Expression.Convert(obj, instanceType), propertyOrField), typeof (T)), obj);
                return expr.Compile();
            }
            catch
            {
                return null;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _isGeometryInvalid = true;
        }

        private static bool IsPathValid(object value)
        {
            var path = (string) value;
            return true; // path.IndexOfAny(new[] { '.' }) > -1;
        }

        private static void OnMinMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Trendline) d).OnMinMaxChanged(e);
        }

        private void OnMinMaxChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DoubleUtil.AreClose(MaxValue, MinValue))
            {
                _translate.Y = -MaxValue - 0.5; // as if max/min = value +/- 0.5
                _scale.ScaleY = -1.0;
            }
            else
            {
                _translate.Y = -MaxValue;
                _scale.ScaleY = -1.0/(MaxValue - MinValue);
            }
        }

        private static bool IsMinMaxValid(object value)
        {
            var num = (double) value;
            return !DoubleUtil.IsNaN(num) && !double.IsInfinity(num);
        }

        private void UpdateGeometry()
        {
            try
            {
                _previousUpdateTime = DateTime.UtcNow;
                EndTime = _previousUpdateTime;
                StartTime = _previousUpdateTime - TimeSpan;
                var isEmpty = true;
                var minValue = double.MaxValue;
                var maxValue = double.MinValue;
                using (var context = Geometry.Open())
                {
                    if (ItemsSource == null || _valueGetter == null || _timeGetter == null)
                    {
                        return;
                    }
                    var startTicks = StartTime.Ticks;
                    var endTicks = EndTime.Ticks;
                    var rangeTicks = endTicks - startTicks;
                    var startDrawingTicks = startTicks - rangeTicks/20;
                    var endDrawingTicks = endTicks + rangeTicks/20;

                    foreach (var dataValue in ItemsSource)
                    {
                        if (dataValue == null)
                        {
                            continue;
                        }
                        var time = _timeGetter(dataValue);
                        var tick = time.Ticks;
                        if (tick >= startDrawingTicks)
                        {
                            if (tick > endDrawingTicks)
                            {
                                break;
                            }
                            var value = Convert.ToDouble(_valueGetter(dataValue));
                            if (isEmpty)
                            {
                                context.BeginFigure(new Point((Double) (tick - startTicks)/rangeTicks, value), false, false);
                                isEmpty = false;
                            }
                            else
                            {
                                context.LineTo(new Point((Double) (tick - startTicks)/rangeTicks, value), true, true);
                            }
                            if (maxValue < value)
                            {
                                maxValue = value;
                            }
                            if (minValue > value)
                            {
                                minValue = value;
                            }
                        }
                    }
                }
                if (AutoRange && !isEmpty)
                {
                    MaxValue = maxValue;
                    MinValue = minValue;
                }
            }
            finally
            {
                _isGeometryInvalid = false;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!_isAnimationRunning && !DesignerProperties.GetIsInDesignMode(this))
            {
                _isAnimationRunning = true;
                CompositionTarget.Rendering += OnRendering;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_isAnimationRunning)
            {
                _isAnimationRunning = false;
                CompositionTarget.Rendering -= OnRendering;
            }
        }

        private void OnRendering(object sender, EventArgs eventArgs)
        {
            if (DateTime.UtcNow.Subtract(_previousUpdateTime) > TimeSpan.FromMilliseconds(41.66))
            {
                UpdateGeometry();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _chartArea = (GetTemplateChild("PART_ChartArea") as FrameworkElement) ?? this;
            _chartArea.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _renderScale.ScaleX = Math.Max(e.NewSize.Width - StrokeThickness, 0.0);
            _renderScale.ScaleY = Math.Max(e.NewSize.Height - StrokeThickness, 0.0);
            _renderTranslate.X = StrokeThickness/2.0;
            _renderTranslate.Y = StrokeThickness/2.0;
        }
    }
}