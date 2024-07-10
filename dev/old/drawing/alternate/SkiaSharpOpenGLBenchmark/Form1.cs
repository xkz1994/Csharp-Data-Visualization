using SkiaSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

// ReSharper disable LocalizableElement

namespace SkiaSharpOpenGLBenchmark
{
    public partial class Form1 : Form
    {
        private static SKMatrix _viewMatrix = SKMatrix.MakeIdentity();
        private Point _lastPoint;
        private int _rowAndColumnCount;
        private Stopwatch _stopwatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();

            skglControl1.BackColor = Color.Aqua;
            skglControl1.MouseUp += skglControl1_MouseUp;
            skglControl1.MouseDown += skglControl1_MouseDown;
            skglControl1.MouseMove += skglControl1_MouseMove;
            skglControl1.MouseWheel += SkglControl1_MouseWheel;
            skglControl1.SizeChanged += (sender, args) => skglControl1.Invalidate();
        }

        private void skglControl1_MouseUp(object sender, MouseEventArgs e)
        {
            // 清空记录的位置
            _lastPoint = new Point();
        }

        private void skglControl1_MouseDown(object sender, MouseEventArgs e)
        {
            // 记录鼠标按下时的位置
            _lastPoint = e.Location;
        }

        private void skglControl1_MouseMove(object sender, MouseEventArgs e)
        {
            // 如果鼠标左键被按下，则进行移动操作
            if (e.Button != MouseButtons.Left) return;

            var point = e.Location;
            var delta = new SKPoint(point.X - _lastPoint.X, point.Y - _lastPoint.Y);
            var makeTranslation = SKMatrix.MakeTranslation(delta.X, delta.Y);
            SKMatrix.PostConcat(ref _viewMatrix, makeTranslation);
            _lastPoint = point;

            skglControl1.Invalidate();
        }

        private void SkglControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            // 获取滚轮滚动的增量
            float delta = e.Delta;

            // 计算缩放因子
            var scaleFactor = delta > 0 ? 1.1f : 0.9f;

            // 获取当前鼠标位置
            var point = e.Location;

            // 更新视图矩阵
            var makeScale = SKMatrix.MakeScale(scaleFactor, scaleFactor, point.X, point.Y);
            SKMatrix.PostConcat(ref _viewMatrix, makeScale);

            // 重新绘制
            skglControl1.Invalidate();
        }

        private void skglControl1_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
        {
            _stopwatch.Restart();

            var canvas = e.Surface.Canvas;
            // 应用视图矩阵
            canvas.Concat(ref _viewMatrix);

            // 芯片中间的间隔
            const int dX = 15;
            const int dY = 15;

            // 视野显示宽度高度
            const int viewShowWidthPx = 30;
            const int viewShowHeightPx = 30;
            // 创建绘制参数和矩形对象
            using var paint = new SKPaint();
            paint.Color = SKColors.LightBlue;
            paint.IsAntialias = false;
            // 开始绘制
            canvas.Clear(SKColors.White); // 清空画布

            for (var row = 0; row < _rowAndColumnCount; row++)
            {
                for (var col = 0; col < _rowAndColumnCount; col++)
                {
                    // 计算矩形的坐标
                    var xView = col * viewShowWidthPx;
                    var yView = row * viewShowHeightPx;
                    var xCell1 = xView + dX;
                    var yCell1 = yView + dY;
                    var xCell2 = xView + viewShowWidthPx;
                    var yCell2 = yView + viewShowHeightPx;
                    var rect = new SKRect
                    {
                        // 设置矩形的位置
                        Left = xCell1,
                        Top = yCell1,
                        Right = xCell2,
                        Bottom = yCell2
                    };


                    // 绘制矩形
                    canvas.DrawRect(rect, paint);
                }
            }

            _stopwatch.Stop();
            label1.Text = $"running mean: {_stopwatch.Elapsed.Milliseconds:0.000} ms";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            _rowAndColumnCount = 10;
            skglControl1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _rowAndColumnCount = 100;
            skglControl1.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _rowAndColumnCount = 500;
            skglControl1.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _rowAndColumnCount = 1000;
            skglControl1.Invalidate();
        }
    }
}