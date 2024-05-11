using OpenTK.Graphics.ES20;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SkiaSharpOpenGLBenchmark
{
    public partial class Form1 : Form
    {
        private static SKMatrix viewMatrix = SKMatrix.MakeIdentity();
        private Point lastPoint;
        public Form1()
        {
            InitializeComponent();
            skglControl1.MouseWheel += SkglControl1_MouseWheel;
        }

        private void SkglControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            // 获取滚轮滚动的增量
            float delta = e.Delta;

            // 计算缩放因子
            float scaleFactor = delta > 0 ? 1.1f : 0.9f;

            // 获取当前鼠标位置
            Point point = e.Location;

            // 更新视图矩阵
            var makeScale = SKMatrix.MakeScale(scaleFactor, scaleFactor, (float)point.X, (float)point.Y);
            SKMatrix.PostConcat(ref viewMatrix, makeScale);
            // 重新绘制

            skglControl1.Invalidate();
        }

        Random rand = new Random(0);

        private void skglControl1_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            // 应用视图矩阵
            canvas.Concat(ref viewMatrix);

            // 芯片中间的间隔
            const int dX = 15;
            const int dY = 15;

            // 视野显示宽度高度
            const int viewShowWidthPx = 30;
            const int viewShowHeightPx = 30;
            // 创建绘制参数和矩形对象
            var paint = new SKPaint { Color = SKColors.LightBlue };
            // 开始绘制
            canvas.Clear(SKColors.White); // 清空画布

            for (var row = 0; row < lineCount; row++)
            {
                for (var col = 0; col < lineCount; col++)
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
        }

        int lineCount;
        List<double> renderTimesMsec = new List<double>();

        private void Benchmark(int lineCount, int times = 10)
        {
            rand = new Random(0);
            renderTimesMsec.Clear();
            this.lineCount = lineCount;
            Stopwatch stopwatch = new Stopwatch();
            for (int i = 0; i < times; i++)
            {
                stopwatch.Restart();
                skglControl1.Invalidate();
                Application.DoEvents();
                stopwatch.Stop();

                renderTimesMsec.Add(1000.0 * stopwatch.ElapsedTicks / Stopwatch.Frequency);
                double mean = renderTimesMsec.Sum() / renderTimesMsec.Count();
                Debug.WriteLine($"Render {renderTimesMsec.Count:00} " +
                                $"took {renderTimesMsec.Last():0.000} ms " +
                                $"(running mean: {mean:0.000} ms)");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Benchmark(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Benchmark(10);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Benchmark(100);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Benchmark(1000);
        }

        private unsafe void skglControl1_MouseMove(object sender, MouseEventArgs e)
        {
            // 如果鼠标左键被按下，则进行移动操作
            if (e.Button == MouseButtons.Left)
            {
                Point point = e.Location;
                SKPoint delta = new SKPoint((float)(point.X - lastPoint.X), (float)(point.Y - lastPoint.Y));
                var makeTranslation = SKMatrix.MakeTranslation(delta.X, delta.Y);
                SKMatrix.PostConcat(ref viewMatrix, makeTranslation);

                lastPoint = point;

                skglControl1.Invalidate();
            }
    
        }

        private void skglControl1_MouseDown(object sender, MouseEventArgs e)
        {
            // 记录鼠标按下时的位置
            lastPoint = e.Location;
        }

        private void skglControl1_MouseUp(object sender, MouseEventArgs e)
        {
            // 清空记录的位置
            lastPoint = new Point();
        }
    }
}