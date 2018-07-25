﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using WInterop.Gdi.Types;
using WInterop.Support;
using WInterop.Support.Buffers;
using WInterop.Windows.Types;

namespace WInterop.Gdi
{
    public static partial class GdiMethods
    {
        public static int GetDeviceCapability(in DeviceContext deviceContext, DeviceCapability capability)
        {
            return Imports.GetDeviceCaps(deviceContext, capability);
        }

        public unsafe static DeviceContext CreateDeviceContext(string driver, string device)
        {
            return Imports.CreateDCW(driver, device, null, null);
        }

        /// <summary>
        /// Returns an in memory device context that is compatible with the specified device.
        /// </summary>
        /// <param name="deviceContext">An existing device context or new DeviceContext() for the application's current screen.</param>
        /// <returns>A 1 by 1 monochrome memory device context.</returns>
        public unsafe static DeviceContext CreateCompatibleDeviceContext(in DeviceContext deviceContext)
        {
            return Imports.CreateCompatibleDC(deviceContext);
        }

        public unsafe static DeviceContext CreateInformationContext(string driver, string device)
        {
            return Imports.CreateICW(driver, device, null, null);
        }

        /// <summary>
        /// Get the device context for the client area of the specified window.
        /// </summary>
        /// <param name="window">The window handle, or null for the entire screen.</param>
        public static DeviceContext GetDeviceContext(WindowHandle window)
        {
            return new DeviceContext(Imports.GetDC(window), window);
        }

        /// <summary>
        /// Get the device context for the specified window.
        /// </summary>
        /// <param name="window">The window handle, or null for the primary display monitor.</param>
        /// <returns>Returns a device context for the entire window, not just the client area.</returns>
        public static DeviceContext GetWindowDeviceContext(WindowHandle window)
        {
            return new DeviceContext(Imports.GetWindowDC(window), window);
        }

        /// <summary>
        /// Enumerate display device info for the given device name.
        /// </summary>
        /// <param name="deviceName">The device to enumerate or null for all devices.</param>
        public static IEnumerable<DISPLAY_DEVICE> EnumerateDisplayDevices(string deviceName)
        {
            uint index = 0;
            DISPLAY_DEVICE device = new DISPLAY_DEVICE()
            {
                cb = DISPLAY_DEVICE.s_size
            };

            while (Imports.EnumDisplayDevicesW(deviceName, index, ref device, 0))
            {
                yield return device;
                index++;
                device = new DISPLAY_DEVICE()
                {
                    cb = DISPLAY_DEVICE.s_size
                };
            }

            yield break;
        }

        public static IEnumerable<DEVMODE> EnumerateDisplaySettings(string deviceName, uint modeIndex = 0)
        {
            DEVMODE mode = new DEVMODE()
            {
                dmSize = DEVMODE.s_size
            };

            while (Imports.EnumDisplaySettingsW(deviceName, modeIndex, ref mode))
            {
                yield return mode;

                if (modeIndex == GdiDefines.ENUM_CURRENT_SETTINGS || modeIndex == GdiDefines.ENUM_REGISTRY_SETTINGS)
                    break;

                modeIndex++;
                mode = new DEVMODE()
                {
                    dmSize = DEVMODE.s_size
                };
            }

            yield break;
        }

        /// <summary>
        /// Selects the given object into the specified device context.
        /// </summary>
        /// <returns>The previous object or null if failed OR null if the given object was a region.</returns>
        public static GdiObjectHandle SelectObject(in DeviceContext deviceContext, GdiObjectHandle @object)
        {
            HGDIOBJ handle = Imports.SelectObject(deviceContext, @object);
            if (handle.IsInvalid)
                return default;

            ObjectType type = Imports.GetObjectType(@object);
            if (type == ObjectType.Region)
                return default;

            return new GdiObjectHandle(handle, ownsHandle: false);
        }

        public static PenHandle GetCurrentPen(in DeviceContext deviceContext)
        {
            return new PenHandle(Imports.GetCurrentObject(deviceContext, ObjectType.Pen), ownsHandle: false);
        }

        public static BrushHandle GetStockBrush(StockBrush brush)
        {
            return new BrushHandle(Imports.GetStockObject((int)brush), ownsHandle: false);
        }

        public static PenHandle GetStockPen(StockPen pen)
        {
            return new PenHandle(Imports.GetStockObject((int)pen), ownsHandle: false);
        }

        public static PenHandle CreatePen(PenStyle style, int width, COLORREF color)
        {
            return new PenHandle(Imports.CreatePen(style, width, color));
        }

        public static PenHandle CreatePen(PenStyleExtended style, uint width, COLORREF color, PenEndCap endCap = PenEndCap.Round, PenJoin join = PenJoin.Round)
        {
            LOGBRUSH brush = new LOGBRUSH
            {
                lbColor = color,
                lpStyle = BrushStyle.Solid
            };

            return new PenHandle(Imports.ExtCreatePen(
                (uint)style | (uint)PenType.Geometric | (uint)endCap | (uint)join,
                width,
                in brush,
                0,
                null));
        }

        public unsafe static COLORREF GetPenColor(PenHandle pen)
        {
            switch (Imports.GetObjectType(pen))
            {
                case ObjectType.Pen:
                    LOGPEN logPen = new LOGPEN();
                    int size = sizeof(LOGPEN);
                    if (Imports.GetObjectW(pen, size, &logPen) == size)
                        return logPen.lopnColor;
                    break;
                case ObjectType.ExtendedPen:
                    BufferHelper.BufferInvoke((HeapBuffer buffer) =>
                    {
                        size = Imports.GetObjectW(pen, 0, null);
                        if (size == 0)
                            throw new InvalidOperationException();
                        buffer.EnsureByteCapacity((ulong)size);
                        size = Imports.GetObjectW(pen, size, buffer.VoidPointer);
                        if (size < sizeof(EXTLOGPEN))
                            throw new InvalidOperationException();
                        return ((EXTLOGPEN*)buffer.VoidPointer)->elpColor;
                    });
                    break;
            }

            throw new InvalidOperationException();
        }

        public static FontHandle GetStockFont(StockFont font)
        {
            return new FontHandle(Imports.GetStockObject((int)font), ownsHandle: false);
        }

        public static BrushHandle CreateSolidBrush(COLORREF color)
        {
            return new BrushHandle(Imports.CreateSolidBrush(color));
        }

        public static BrushHandle CreateSolidBrush(byte red, byte green, byte blue)
        {
            return new BrushHandle(Imports.CreateSolidBrush(new COLORREF(red, green, blue)));
        }

        public static bool UpdateWindow(WindowHandle window)
        {
            return Imports.UpdateWindow(window);
        }

        public static bool ValidateRectangle(WindowHandle window, ref RECT rect)
        {
            return Imports.ValidateRect(window, ref rect);
        }

        /// <summary>
        /// Calls BeginPaint and returns the created DeviceContext. Disposing the returned DeviceContext will call EndPaint.
        /// </summary>
        public static DeviceContext BeginPaint(WindowHandle window)
        {
            return new DeviceContext(Imports.BeginPaint(window, out PAINTSTRUCT paintStruct), window, paintStruct);
        }

        /// <summary>
        /// Calls BeginPaint and returns the created DeviceContext. Disposing the returned DeviceContext will call EndPaint.
        /// </summary>
        public static DeviceContext BeginPaint(WindowHandle window, out PAINTSTRUCT paintStruct)
        {
            return new DeviceContext(Imports.BeginPaint(window, out paintStruct), window, paintStruct);
        }

        public static COLORREF GetTextColor(in DeviceContext deviceContext)
        {
            return Imports.GetTextColor(deviceContext);
        }

        public static COLORREF SetTextColor(in DeviceContext deviceContext, COLORREF color)
        {
            return Imports.SetTextColor(deviceContext, color);
        }

        public static unsafe bool TextOut(in DeviceContext deviceContext, int x, int y, string text)
        {
            fixed (char* s = text)
                return Imports.TextOutW(deviceContext, x, y, s, text.Length);
        }

        public static unsafe bool TextOut(in DeviceContext deviceContext, int x, int y, char* text, int length)
        {
            return Imports.TextOutW(deviceContext, x, y, text, length);
        }

        public static unsafe int DrawText(in DeviceContext deviceContext, string text, RECT rect, TextFormat format)
        {
            if ((format & TextFormat.ModifyString) == 0)
            {
                // The string won't be changed, we can just pin
                fixed (char* c = text)
                {
                    return Imports.DrawTextW(deviceContext, c, text.Length, ref rect, format);
                }
            }

            HDC hDC = deviceContext;

            return BufferHelper.BufferInvoke((StringBuffer buffer) =>
            {
                buffer.EnsureCharCapacity((uint)(text.Length + 5));
                buffer.Append(text);
                return Imports.DrawTextW(hDC, buffer.CharPointer, (int)buffer.Length, ref rect, format);
            });
        }

        public static TextAlignment SetTextAlignment(in DeviceContext deviceContext, TextAlignment alignment)
        {
            return Imports.SetTextAlign(deviceContext, alignment);
        }

        public static bool GetTextMetrics(in DeviceContext deviceContext, out TEXTMETRIC metrics)
        {
            return Imports.GetTextMetricsW(deviceContext, out metrics);
        }

        public unsafe static bool InvalidateRectangle(WindowHandle window, RECT rect, bool erase)
        {
            return Imports.InvalidateRect(window, &rect, erase);
        }

        public unsafe static bool Invalidate(WindowHandle window, bool erase = true)
        {
            return Imports.InvalidateRect(window, null, erase);
        }

        public static COLORREF GetPixel(in DeviceContext deviceContext, int x, int y)
        {
            return Imports.GetPixel(deviceContext, x, y);
        }

        public static bool SetPixel(in DeviceContext deviceContext, int x, int y, COLORREF color)
        {
            return Imports.SetPixelV(deviceContext, x, y, color);
        }

        public unsafe static bool MoveTo(in DeviceContext deviceContext, int x, int y)
        {
            return Imports.MoveToEx(deviceContext, x, y, null);
        }

        public static bool LineTo(in DeviceContext deviceContext, int x, int y)
        {
            return Imports.LineTo(deviceContext, x, y);
        }

        public static PolyFillMode GetPolyFillMode(in DeviceContext deviceContext)
        {
            return Imports.GetPolyFillMode(deviceContext);
        }

        public static PolyFillMode SetPolyFillMode(in DeviceContext deviceContext, PolyFillMode fillMode)
        {
            return Imports.SetPolyFillMode(deviceContext, fillMode);
        }

        public unsafe static bool Polygon(in DeviceContext deviceContext, params Point[] points)
        {
            return Polygon(deviceContext, points.AsSpan());
        }

        public unsafe static bool Polygon(in DeviceContext deviceContext, ReadOnlySpan<Point> points)
        {
            return Imports.Polygon(deviceContext, ref MemoryMarshal.GetReference(points), points.Length);
        }

        public static bool Polyline(in DeviceContext deviceContext, params Point[] points)
        {
            return Polyline(deviceContext, points);
        }

        public static bool Polyline(in DeviceContext deviceContext, ReadOnlySpan<Point> points)
        {
            return Imports.Polyline(deviceContext, ref MemoryMarshal.GetReference(points), points.Length);
        }

        public static bool Rectangle(in DeviceContext deviceContext, int left, int top, int right, int bottom)
        {
            return Imports.Rectangle(deviceContext, left, top, right, bottom);
        }

        public static bool Rectangle(in DeviceContext deviceContext, RECT rectangle)
        {
            return Imports.Rectangle(deviceContext, rectangle.left, rectangle.top, rectangle.right, rectangle.bottom);
        }

        public static bool Ellipse(in DeviceContext deviceContext, int left, int top, int right, int bottom)
        {
            return Imports.Ellipse(deviceContext, left, top, right, bottom);
        }

        public static bool RoundRectangle(in DeviceContext deviceContext, RECT rectangle, int cornerWidth, int cornerHeight)
        {
            return Imports.RoundRect(deviceContext, rectangle.left, rectangle.top, rectangle.right, rectangle.bottom, cornerWidth, cornerHeight);
        }

        public static bool RoundRectangle(in DeviceContext deviceContext, int left, int top, int right, int bottom, int cornerWidth, int cornerHeight)
        {
            return Imports.RoundRect(deviceContext, left, top, right, bottom, cornerWidth, cornerHeight);
        }

        public static bool FillRectangle(in DeviceContext deviceContext, RECT rectangle, BrushHandle brush)
        {
            return Imports.FillRect(deviceContext, ref rectangle, brush);
        }

        public static bool FrameRectangle(in DeviceContext deviceContext, RECT rectangle, BrushHandle brush)
        {
            return Imports.FrameRect(deviceContext, ref rectangle, brush);
        }

        public static bool InvertRectangle(in DeviceContext deviceContext, RECT rectangle)
        {
            return Imports.InvertRect(deviceContext, ref rectangle);
        }

        public static bool DrawFocusRectangle(in DeviceContext deviceContext, RECT rectangle)
        {
            return Imports.DrawFocusRect(deviceContext, ref rectangle);
        }

        public unsafe static bool PolyBezier(in DeviceContext deviceContext, params Point[] points)
        {
            return PolyBezier(deviceContext, points.AsSpan());
        }

        public unsafe static bool PolyBezier(in DeviceContext deviceContext, ReadOnlySpan<Point> points)
        {
             return Imports.PolyBezier(deviceContext, ref MemoryMarshal.GetReference(points), (uint)points.Length);
        }

        public static RegionHandle CreateEllipticRegion(int left, int top, int right, int bottom)
        {
            return Imports.CreateEllipticRgn(left, top, right, bottom);
        }

        public static RegionHandle CreateRectangleRegion(int left, int top, int right, int bottom)
        {
            return Imports.CreateRectRgn(left, top, right, bottom);
        }

        public static RegionType CombineRegion(RegionHandle destination, RegionHandle sourceOne, RegionHandle sourceTwo, CombineRegionMode mode)
        {
            return Imports.CombineRgn(destination, sourceOne, sourceTwo, mode);
        }

        public static RegionType SelectClippingRegion(in DeviceContext deviceContext, RegionHandle region)
        {
            return Imports.SelectClipRgn(deviceContext, region);
        }

        public static unsafe bool SetViewportOrigin(in DeviceContext deviceContext, int x, int y)
        {
            return Imports.SetViewportOrgEx(deviceContext, x, y, null);
        }

        public static unsafe bool SetWindowOrigin(in DeviceContext deviceContext, int x, int y)
        {
            return Imports.SetWindowOrgEx(deviceContext, x, y, null);
        }

        /// <summary>
        /// Shared brush, handle doesn't need disposed.
        /// </summary>
        public static BrushHandle GetSystemColorBrush(SystemColor systemColor)
        {
            return new BrushHandle(Imports.GetSysColorBrush(systemColor), ownsHandle: false);
        }

        public static COLORREF GetBackgroundColor(in DeviceContext deviceContext)
        {
            return Imports.GetBkColor(deviceContext);
        }

        public static COLORREF GetBrushColor(in DeviceContext deviceContext)
        {
            return Imports.GetDCBrushColor(deviceContext);
        }

        public static COLORREF SetBackgroundColor(in DeviceContext deviceContext, COLORREF color)
        {
            return Imports.SetBkColor(deviceContext, color);
        }

        public static BackgroundMode SetBackgroundMode(in DeviceContext deviceContext, BackgroundMode mode)
        {
            return Imports.SetBkMode(deviceContext, mode);
        }

        public static BackgroundMode GetBackgroundMode(in DeviceContext deviceContext)
        {
            return Imports.GetBkMode(deviceContext);
        }

        public static PenMixMode SetRasterOperation(in DeviceContext deviceContext, PenMixMode foregroundMixMode)
        {
            return Imports.SetROP2(deviceContext, foregroundMixMode);
        }

        public static PenMixMode GetRasterOperation(in DeviceContext deviceContext)
        {
            return Imports.GetROP2(deviceContext);
        }

        public static FontHandle CreateFont(
            int height,
            int width,
            int escapement,
            int orientation,
            FontWeight weight,
            bool italic,
            bool underline,
            bool strikeout,
            CharacterSet characterSet,
            OutputPrecision outputPrecision,
            ClippingPrecision clippingPrecision,
            Quality quality,
            FontPitch pitch,
            FontFamily family,
            string typeface)
        {
            return Imports.CreateFontW(height, width, escapement, orientation, weight, italic, underline, strikeout, (uint)characterSet,
                (uint)outputPrecision, (uint)clippingPrecision, (uint)quality, (uint)((byte)pitch | (byte)family), typeface);
        }

        private static int EnumerateFontCallback(
            ref ENUMLOGFONTEXDV fontAttributes,
            ref NEWTEXTMETRICEX textMetrics,
            FontTypes fontType,
            LPARAM lParam)
        {
            var info = (List<FontInformation>)GCHandle.FromIntPtr(lParam).Target;
            info.Add(new FontInformation { FontType = fontType, TextMetrics = textMetrics, FontAttributes = fontAttributes });
            return 1;
        }

        public static IEnumerable<FontInformation> EnumerateFontFamilies(in DeviceContext deviceContext, CharacterSet characterSet, string faceName)
        {
            LOGFONT logFont = new LOGFONT
            {
                lfCharSet = characterSet,
            };

            logFont.lfFaceName.CopyFrom(faceName);

            List<FontInformation> info = new List<FontInformation>();
            GCHandle gch = GCHandle.Alloc(info, GCHandleType.Normal);
            try
            {
                int result = Imports.EnumFontFamiliesExW(deviceContext, ref logFont, EnumerateFontCallback, GCHandle.ToIntPtr(gch), 0);
            }
            finally
            {
                gch.Free();
            }

            return info;
        }

        public static bool ScreenToClient(WindowHandle window, ref Point point)
        {
            return Imports.ScreenToClient(window, ref point);
        }

        public static bool ClientToScreen(WindowHandle window, ref Point point)
        {
            return Imports.ClientToScreen(window, ref point);
        }

        public static bool DeviceToLogical(in DeviceContext deviceContext, params Point[] points)
        {
            return DeviceToLogical(deviceContext, points.AsSpan());
        }

        public static bool DeviceToLogical(in DeviceContext deviceContext, ReadOnlySpan<Point> points)
        {
            return Imports.DPtoLP(deviceContext, ref MemoryMarshal.GetReference(points), points.Length);
        }

        public unsafe static bool LogicalToDevice(in DeviceContext deviceContext, params Point[] points)
        {
            return LogicalToDevice(deviceContext, points);
        }

        public unsafe static bool LogicalToDevice(in DeviceContext deviceContext, ReadOnlySpan<Point> points)
        {
            return Imports.LPtoDP(deviceContext, ref MemoryMarshal.GetReference(points), points.Length);
        }

        public unsafe static bool OffsetWindowOrigin(in DeviceContext deviceContext, int x, int y)
        {
            return Imports.OffsetWindowOrgEx(deviceContext, x, y, null);
        }

        public unsafe static bool OffsetViewportOrigin(in DeviceContext deviceContext, int x, int y)
        {
            return Imports.OffsetViewportOrgEx(deviceContext, x, y, null);
        }

        public unsafe static bool SetWindowExtents(in DeviceContext deviceContext, int x, int y)
        {
            return Imports.SetWindowExtEx(deviceContext, x, y, null);
        }

        public unsafe static bool SetViewportExtents(in DeviceContext deviceContext, int x, int y)
        {
            return Imports.SetViewportExtEx(deviceContext, x, y, null);
        }

        public static MapMode SetMapMode(in DeviceContext deviceContext, MapMode mapMode)
        {
            return Imports.SetMapMode(deviceContext, mapMode);
        }
    }
}
