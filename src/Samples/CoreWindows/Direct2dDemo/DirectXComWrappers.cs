using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Direct2dDemo
{
    [ComCallableWrapper(typeof(WInterop.Direct2d.IFactory))]
    unsafe partial class DirectXComWrappers : ComWrappers
    {
        static ComWrappers.ComInterfaceEntry* direct2DFactoryEntry;

        static DirectXComWrappers()
        {
            direct2DFactoryEntry = CreateDirect2DFactoryEntry();
        }

        private static ComInterfaceEntry* CreateDirect2DFactoryEntry()
        {
            CreateIFactoryProxyVtbl(out var vtbl);

            var comInterfaceEntryMemory = RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(DirectXComWrappers), sizeof(ComInterfaceEntry) * 1);
            var wrapperEntry = (ComInterfaceEntry*)comInterfaceEntryMemory.ToPointer();
            wrapperEntry[0].IID = new Guid(WInterop.Direct2d.InterfaceIds.IID_ID2D1Factory);
            wrapperEntry[0].Vtable = vtbl;
            return wrapperEntry;
        }

        protected override unsafe ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count)
        {
            if (obj is WInterop.Direct2d.IFactory)
            {
                count = 1;
                return direct2DFactoryEntry;
            }

            count = 0;
            return null;
        }

        protected override object CreateObject(IntPtr externalComObject, CreateObjectFlags flags)
        {
            return new UniversalWrapper(externalComObject);
        }

        protected override void ReleaseObjects(IEnumerable objects)
        {
        }
    }
}
