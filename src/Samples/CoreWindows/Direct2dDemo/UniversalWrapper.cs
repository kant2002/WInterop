using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Direct2dDemo
{
    [RuntimeCallableWrapper(typeof(WInterop.DirectWrite.IFactory))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IFactory))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IGeometry))]
    [RuntimeCallableWrapper(typeof(WInterop.DirectWrite.IRenderingParams))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IStrokeStyle))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.ISimplifiedGeometrySink))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.ITesselationSink))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IWindowRenderTarget))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.ISolidColorBrush))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IPathGeometry))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IGeometrySink))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IBitmapRenderTarget))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IBitmap))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IBitmapBrush))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IGradientStopCollection))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IRadialGradientBrush))]
    [RuntimeCallableWrapper(typeof(WInterop.DirectWrite.ITextFormat))]
    [RuntimeCallableWrapper(typeof(WInterop.DirectWrite.ITextLayout))]
    [RuntimeCallableWrapper(typeof(WInterop.DirectWrite.IFontCollection))]
    [RuntimeCallableWrapper(typeof(WInterop.DirectWrite.IFontFace))]
    [RuntimeCallableWrapper(typeof(WInterop.DirectWrite.IInlineObject))]
    [RuntimeCallableWrapper(typeof(WInterop.DirectWrite.ITypography))]
    [RuntimeCallableWrapper(typeof(WInterop.DirectWrite.ITextRenderer))]
    [RuntimeCallableWrapper(typeof(WInterop.Direct2d.IDrawingStateBlock))]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal sealed partial class UniversalWrapper : IDisposable
    {
        private readonly IntPtr instance;

        public UniversalWrapper(IntPtr instance)
        {
            this.instance = instance;
            Marshal.AddRef(instance);
        }

        public void Dispose()
        {
            Marshal.Release(instance);
        }
    }
}
