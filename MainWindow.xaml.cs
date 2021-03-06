using System;
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
            R = value.R;
            G = value.G;
            B = value.B;
        }

        public static implicit operator Color(ColorRGB rgb)
        {
            Color c = Color.FromRgb(rgb.R, rgb.G, rgb.B);
            return c;
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);
        }
        readonly Random r = new();
        int numbersSize = 300;
        int[] numbers = new int[300];
        bool slow;

        // Window events
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canv.Children.Clear();
            Redraw(numbers);
            sizeSlider.Width = mainGrid.ActualWidth * .4 - 90;
        }
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sizeSlider.ValueChanged += Slider_ValueChanged;
            sizeSlider.Value = 300;
            sizeLbl.Content = " \u03A3 = " + sizeSlider.Value;
            Generate();
        }

        // Controls events
        private void Button_Click_Generate(object sender, RoutedEventArgs e)
        {
            Generate();
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sizeLbl.Content = " \u03A3 = " + sizeSlider.Value;
            numbersSize = Convert.ToInt32(sizeSlider.Value);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            slow = speed.SelectedIndex == 0;

            switch (alg.SelectedIndex)
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
                    MergeSort(numbers);
                    break;
                case 4:
                    QuickSort(numbers);
                    break;
                default:
                    BubbleSort(numbers);
                    break;
            }
        }

        // Visualisation logic
        public void Generate()
        {
            numbers = new int[numbersSize];
            canv.Children.Clear();

            for (int i = 0; i < numbersSize; ++i)
            {
                numbers[i] = r.Next(1, 1000);
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
            Rectangle rect = new()
            {
                Height = canv.ActualHeight * size / 1000,
                Width = canv.ActualWidth / Convert.ToDouble(numbersSize),
                Fill = new SolidColorBrush(HSL2RGB(Convert.ToDouble(size) / 1000, 0.5, 0.5)),
                ToolTip = size
            };
            Canvas.SetLeft(rect, pos * canv.ActualWidth / Convert.ToDouble(numbersSize));
            Canvas.SetBottom(rect, 0);
            canv.Children.Add(rect);
        }
        public void SwapRect(Rectangle rect1, Rectangle rect2, int size1, int size2)
        {
            rect1.Height = canv.ActualHeight * size2 / 1000;
            rect2.Height = canv.ActualHeight * size1 / 1000;
            var c1 = ((SolidColorBrush)rect1.Fill).Color;
            var c2 = ((SolidColorBrush)rect2.Fill).Color;
            rect1.Fill = new SolidColorBrush(c2);
            rect2.Fill = new SolidColorBrush(c1);
            rect1.ToolTip = size2;
            rect2.ToolTip = size1;
        }

        // HSL to RGB converter
        public static ColorRGB HSL2RGB(double h, double sl, double l)

        {
            double r = l;
            double g = l;
            double b = l;
            double v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);

            if (v > 0)
            {
                double m = l + l - v;
                double sv = (v - m) / v;
                h *= 6.0;
                int sextant = (int)h;
                double fract = h - sextant;
                double vsf = v * sv * fract;
                double mid1 = m + vsf;
                double mid2 = v - vsf;

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
                        SwapRect((Rectangle)canv.Children[i], (Rectangle)canv.Children[i + 1], numbers[i], numbers[i + 1]);
                        Swap(ref numbers[i], ref numbers[i + 1]);
                        await Task.Delay(slow ? 1 : 0);
                    }
                    swapPos = i + 1;
                }
                n = swapPos;
                await Task.Delay(!slow ? 1 : 0);
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
                        SwapRect((Rectangle)canv.Children[j-1], (Rectangle)canv.Children[j], numbers[j-1], numbers[j]);
                        Swap(ref numbers[j - 1], ref numbers[j]);
                        await Task.Delay(slow ? 1 : 0);
                    }
                }
                await Task.Delay(!slow ? 1 : 0);
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
                    await Task.Delay(slow ? 1 : 0);
                }
                SwapRect((Rectangle)canv.Children[min], (Rectangle)canv.Children[i], numbers[min], numbers[i]);
                Swap(ref numbers[min], ref numbers[i]);
                await Task.Delay(!slow ? 1 : 0);
            }
            generateBtn.IsEnabled = true;
            sizeSlider.IsEnabled = true;
        }
        public async void QuickSort(int[] numbers)
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

                int pivot = numbers[endIndex];
                int i = (startIndex - 1);
                for (int j = startIndex; j <= endIndex - 1; ++j)
                {
                    if (numbers[j] <= pivot)
                    {
                        ++i;
                        SwapRect((Rectangle)canv.Children[i], (Rectangle)canv.Children[j], numbers[i], numbers[j]);
                        Swap(ref numbers[i], ref numbers[j]);
                        await Task.Delay(slow ? 1 : 0);
                    }
                }
                SwapRect((Rectangle)canv.Children[i+1], (Rectangle)canv.Children[endIndex], numbers[i+1], numbers[endIndex]);
                Swap(ref numbers[i + 1], ref numbers[endIndex]);
                await Task.Delay(slow ? 1 : 0);
                int p = i + 1;

                if (p - 1 > startIndex)
                {
                    stack[++top] = startIndex;
                    stack[++top] = p - 1;
                    await Task.Delay(!slow ? 1 : 0);
                }

                if (p + 1 < endIndex)
                {
                    stack[++top] = p + 1;
                    stack[++top] = endIndex;
                    await Task.Delay(!slow ? 1 : 0);
                }
            }
            generateBtn.IsEnabled = true;
            sizeSlider.IsEnabled = true;
        }
        public async void MergeSort(int[] numbers)
        {
            startBtn.IsEnabled = false;
            generateBtn.IsEnabled = false;
            sizeSlider.IsEnabled = false;

            int length = 1;
            while (length < numbers.Length)
            {
                int i = 0;

                while (i < numbers.Length)
                {
                    int l1 = i;
                    int r1 = i + length - 1;
                    int l2 = i + length;
                    int r2 = i + 2 * length - 1;
                    if (l2 >= numbers.Length)
                    {
                        break;
                    }
                    if (r2 >= numbers.Length)
                    {
                        r2 = numbers.Length - 1;
                    }

                    int[] temp = Merge(numbers, l1, r1, l2, r2);

                    for (int j = 0; j < r2-l1+1; ++j)
                    {
                        numbers[i+j] = temp[j];

                        if (slow)
                        {
                            canv.Children.Clear();
                            Redraw(numbers);
                            await Task.Delay(1);
                        }
                    }
                    i += length * 2;
                    if (!slow)
                    {
                        canv.Children.Clear();
                        Redraw(numbers);
                        await Task.Delay(1);
                    }
                }
                length *= 2;
            }
            generateBtn.IsEnabled = true;
            sizeSlider.IsEnabled = true;
        }
        public static int[] Merge(int[] arr, int l1, int r1, int l2, int r2)
        {
            int[] temp = new int[arr.Length];
            int count = 0;

            while (l1 <= r1 && l2 <= r2)
            {
                if (arr[l1] <= arr[l2])
                {
                    temp[count] = arr[l1];
                    count++;
                    l1++;
                }
                else
                {
                    temp[count] = arr[l2];
                    count++;
                    l2++;
                }
            }
            while (l1 <= r1)
            {
                temp[count] = arr[l1];
                count++;
                l1++;
            }
            while (l2 <= r2)
            {
                temp[count] = arr[l2];
                count++;
                l2++;
            }
            return temp;
        }
        public static void Swap(ref int i, ref int p)
        {
            int temp = i;
            i = p;
            p = temp;
        }
    }
}
