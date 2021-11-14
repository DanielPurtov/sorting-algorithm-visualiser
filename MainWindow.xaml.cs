using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SortingVisualiser
{
    // Builds ColorRGB structs
    public struct ColorRGB
    {
        public byte R;
        public byte G;
        public byte B;

        public ColorRGB(Color value)
        {
            this.R = value.R;
            this.G = value.G;
            this.B = value.B;
        }

        public static implicit operator Color(ColorRGB rgb)
        {
            Color c = Color.FromRgb(rgb.R, rgb.G, rgb.B);
            return c;
        }

        public static explicit operator ColorRGB(Color c)
        {
            return new ColorRGB(c);
        }

    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);

        }
        Random r = new Random();

        static int numbersSize = 300;
        int[] numbers = new int[numbersSize];
        bool slow = true;

        // Window events
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canv.Children.Clear();
            Redraw(numbers);

            var width = mainGrid.ActualWidth;
            sizeSlider.Width = width * .4 - 90;
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sizeSlider.ValueChanged += slider_ValueChanged;
            sizeSlider.Value = 300;
            sizeLbl.Content = " \u03A3 = " + sizeSlider.Value;
            Generate();
        }

        // Controls events
        private void Button_Click_Generate(object sender, RoutedEventArgs e)
        {
            Generate();
        }
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sizeLbl.Content = " \u03A3 = " + sizeSlider.Value;
            numbersSize = Convert.ToInt32(sizeSlider.Value);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            canv.Children.Clear();
            Redraw(numbers);

            if (speed.SelectedIndex == 0)
            {
                slow = true;
            }
            else
            {
                slow = false;
            }
            switch (algorithms.SelectedIndex)
            {
                case 0:
                    BubbleSort(numbers);
                    break;
                case 1:
                    InsertionSort(numbers);
                    break;
                case 2:
                    SelectionSort(numbers);
                    break;
                case 3:
                    QuickSortIterative(numbers);
                    break;
                default:
                    BubbleSort(numbers);
                    break;
            }
        }

        // Visualization logic
        public void Generate()
        {
            numbers = new int[numbersSize];
            canv.Children.Clear();

            for (int i = 0; i < numbersSize; ++i)
            {
                numbers[i] = r.Next(0, 1000);
            }
            Redraw(numbers);
            startBtn.IsEnabled = true;
        }
        public void Redraw(int[] arr)
        {
            for (int i = 0; i < arr.Length; ++i)
            {
                DrawRect(i, arr[i]);
            }
        }
        public void DrawRect(int pos, int size)
        {
            Rectangle rect = new Rectangle();
            rect.Height = canv.ActualHeight * size / 1000;
            var width = canv.ActualWidth / Convert.ToDouble(numbersSize);
            rect.Width = width;

            Canvas.SetLeft(rect, pos * width);
            Canvas.SetBottom(rect, 0);

            ColorRGB c = HSL2RGB(Convert.ToDouble(size) / 1000, 0.5, 0.5);
            SolidColorBrush brush = new SolidColorBrush(c);
            rect.Fill = brush;

            rect.ToolTip = size;

            canv.Children.Add(rect);
        }

        // HSL to RGB converter
        public static ColorRGB HSL2RGB(double h, double sl, double l)

        {
            double v;
            double r, g, b;

            r = l;
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);

            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;

                switch (sextant)

                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;

                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;

                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;

                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;

                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;

                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            ColorRGB rgb;
            rgb.R = Convert.ToByte(r * 255.0f);
            rgb.G = Convert.ToByte(g * 255.0f);
            rgb.B = Convert.ToByte(b * 255.0f);
            return rgb;
        }

        // Sorting engines
        public async void BubbleSort(int[] numbers)
        {
            startBtn.IsEnabled = false;
            generateBtn.IsEnabled = false;
            sizeSlider.IsEnabled = false;

            int n = numbers.Length;
            do
            {
                int swapPos = 0;

                for (int i = 0; i < n - 1; ++i)
                {
                    if (numbers[i] > numbers[i + 1])
                    {
                        Swap(ref numbers[i], ref numbers[i + 1]);
                        if (slow)
                        {
                            canv.Children.Clear();
                            Redraw(numbers);
                            await Task.Delay(1);
                        }
                    }
                    swapPos = i + 1;
                }
                n = swapPos;
                if (!slow)
                {
                    canv.Children.Clear();
                    Redraw(numbers);
                    await Task.Delay(1);
                }
            }
            while (n > 1);

            generateBtn.IsEnabled = true;
            sizeSlider.IsEnabled = true;
        }
        public async void InsertionSort(int[] numbers)
        {
            startBtn.IsEnabled = false;
            generateBtn.IsEnabled = false;
            sizeSlider.IsEnabled = false;

            for (int i = 0; i < numbers.Length - 1; ++i)
            {
                for (int j = i + 1; j > 0; --j)
                {
                    if (numbers[j - 1] > numbers[j])
                    {
                        Swap(ref numbers[j - 1], ref numbers[j]);
                        if (slow)
                        {
                            canv.Children.Clear();
                            Redraw(numbers);
                            await Task.Delay(1);
                        }
                    }
                }
                if (!slow)
                {
                    canv.Children.Clear();
                    Redraw(numbers);
                    await Task.Delay(1);
                }
            }
            generateBtn.IsEnabled = true;
            sizeSlider.IsEnabled = true;
        }
        public async void SelectionSort(int[] numbers)
        {
            startBtn.IsEnabled = false;
            generateBtn.IsEnabled = false;
            sizeSlider.IsEnabled = false;

            for (int i = 0; i < numbers.Length - 1; ++i)
            {
                int min = i;
                for (int j = i + 1; j < numbers.Length; ++j)
                {
                    if (numbers[j] < numbers[min])
                    {
                        min = j;
                    }
                }
                Swap(ref numbers[min], ref numbers[i]);
                canv.Children.Clear();
                Redraw(numbers);
                await Task.Delay(1);
            }
            generateBtn.IsEnabled = true;
            sizeSlider.IsEnabled = true;
        }
        public async void QuickSortIterative(int[] numbers)
        {
            startBtn.IsEnabled = false;
            generateBtn.IsEnabled = false;
            sizeSlider.IsEnabled = false;

            int startIndex = 0;
            int endIndex = numbers.Length - 1;
            int top = -1;
            int[] stack = new int[numbers.Length];

            stack[++top] = startIndex;
            stack[++top] = endIndex;

            while (top >= 0)
            {
                endIndex = stack[top--];
                startIndex = stack[top--];

                //int p = partition(numbers, startIndex, endIndex);

                int pivot = numbers[endIndex];
                int i = (startIndex - 1);
                for (int j = startIndex; j <= endIndex - 1; ++j)
                {
                    if (numbers[j] <= pivot)
                    {
                        ++i;
                        Swap(ref numbers[i], ref numbers[j]);
                        if (slow)
                        {
                            canv.Children.Clear();
                            Redraw(numbers);
                            await Task.Delay(1);
                        }
                    }
                }

                Swap(ref numbers[i + 1], ref numbers[endIndex]);

                if (slow)
                {
                    canv.Children.Clear();
                    Redraw(numbers);
                    await Task.Delay(1);
                }
                int p = i + 1;

                if (p - 1 > startIndex)
                {
                    stack[++top] = startIndex;
                    stack[++top] = p - 1;
                    if (!slow)
                    {
                        canv.Children.Clear();
                        Redraw(numbers);
                        await Task.Delay(1);
                    }
                }

                if (p + 1 < endIndex)
                {
                    stack[++top] = p + 1;
                    stack[++top] = endIndex;
                    if (!slow)
                    {
                        canv.Children.Clear();
                        Redraw(numbers);
                        await Task.Delay(1);
                    }
                }
            }
            generateBtn.IsEnabled = true;
            sizeSlider.IsEnabled = true;
        }
        public void Swap(ref int i, ref int p)
        {
            int temp = i;
            i = p;
            p = temp;
        }

        // Deprecated for now ...
        /*
            public void QuickSort (int[] numbers, int low, int high)
            {

                if (low < high)
                {
                    int p = partition(numbers, low, high);
                    QuickSort(numbers, 1, p - 1);

                    QuickSort(numbers, p + 1, high);
                }
            }
            public int partition(int[] numbers, int low, int high)
            {
                int pivot = numbers[high];
                int i = (low - 1);
                for (int j = low; j <= high - 1; ++j)
                {
                    if (numbers[j] <= pivot)
                    {
                        ++i;
                        Swap(ref numbers[i], ref numbers[j]);
                    }
                }
                Swap(ref numbers[i + 1], ref numbers[high]);
                return i + 1;
            }

        */
    }
}
