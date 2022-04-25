using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace HCI.Chart
{
    public abstract class Chart
    {
        protected static readonly List<Brush> Colors = new()
        {
            Brushes.DeepPink,
            Brushes.DarkGreen,
            Brushes.Crimson,
            Brushes.Black,
        };

        protected readonly Canvas Canvas;

        public Chart(Canvas canvas)
        {
            Canvas = canvas;
        }

        public void Draw()
        {
            Canvas.Children.Clear();
            Init();
            DrawGrid();
            DrawLegend();
            DrawObject();
        }

        public abstract void Init();
        public abstract void DrawGrid();
        public abstract void DrawLegend();
        public abstract void DrawObject();
    }
}
