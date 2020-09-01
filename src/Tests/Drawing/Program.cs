﻿// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using WInterop.Gdi;
using WInterop.GdiPlus;
using WInterop.Windows;
using System.Drawing;

namespace Windows101
{
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            Windows.CreateMainWindowAndRun(new DrawRectangle(), windowTitle: "Hello!");
        }
    }

    public class DrawRectangle : WindowClass
    {
        private readonly Pen _bluePen = new Pen(Color.Blue);
        private readonly Brush _blueBrush = new Brush(Color.Blue);

        protected override LResult WindowProcedure(WindowHandle window, MessageType message, WParam wParam, LParam lParam)
        {
            switch (message)
            {
                case MessageType.Paint:
                    {
                        using DeviceContext dc = window.BeginPaint();
                        using Graphics graphics = new (dc);

                        Rectangle first = new Rectangle(new Point(50, 50), new Size(50, 50));
                        dc.Rectangle(first);
                        graphics.DrawRectangle(_bluePen, first);

                        Rectangle second = new Rectangle(new Point(150, 50), new Size(50, 50));
                        dc.FillRectangle(second, StockBrush.Black);
                        graphics.FillRectangle(_blueBrush, second);

                        // Flip to see what underlap we have
                        Rectangle third = new Rectangle(new Point(250, 50), new Size(50, 50));
                        graphics.FillRectangle(_blueBrush, third);
                        dc.FillRectangle(third, StockBrush.Black);

                        return 0;
                    }
            }

            return base.WindowProcedure(window, message, wParam, lParam);
        }
    }
}
