﻿using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;
using Vim.Interpreter;

namespace Vim.VisualStudio
{
    /// <summary>
    /// State of the active tab group in Visual Studio
    /// </summary>
    public readonly struct WindowFrameState
    {
        public static WindowFrameState Default
        {
            get { return new WindowFrameState(activeWindowFrameIndex: 0, windowFrameCount: 1); }
        }

        public readonly int ActiveWindowFrameIndex;
        public readonly int WindowFrameCount;

        public WindowFrameState(int activeWindowFrameIndex, int windowFrameCount)
        {
            ActiveWindowFrameIndex = activeWindowFrameIndex;
            WindowFrameCount = windowFrameCount;
        }
    }

    /// <summary>
    /// Factory for producing IVersionService instances.  This is an interface for services which
    /// need to vary in implementation between versions of Visual Studio
    /// </summary>
    public interface ISharedService
    {
        /// <summary>
        /// Is this the active IVsWindow frame which has focus?  This method is used during macro
        /// running and hence must account for view changes which occur during a macro run.  Say by the
        /// macro containing the 'gt' command.  Unfortunately these don't fully process through Visual
        /// Studio until the next UI thread pump so we instead have to go straight to the view controller
        /// </summary>
        bool IsActiveWindowFrame(IVsWindowFrame vsWindowFrame);

        /// <summary>
        /// Get the state of the active tab group in Visual Studio
        /// </summary>
        WindowFrameState GetWindowFrameState();

        /// <summary>
        /// Go to the tab with the specified index
        /// </summary>
        void GoToTab(int index);

        /// <summary>
        /// Run C# Script.
        /// </summary>
        /// <returns></returns>
        void RunCSharpScript(IVimBuffer vimBuffer, CallInfo callInfo, bool createEachTime);
    }

    /// <summary>
    /// Factory which is associated with a specific version of Visual Studio
    /// </summary>
    public interface ISharedServiceVersionFactory
    {
        /// <summary>
        /// Version of Visual Studio this implementation is tied to
        /// </summary>
        VisualStudioVersion Version { get; }

        ISharedService Create();
    }

    /// <summary>
    /// Consumable interface which will provide an ISharedService implementation.  This is a MEF 
    /// importable component
    /// </summary>
    public interface ISharedServiceFactory
    {
        /// <summary>
        /// Create an instance of IVsSharedService if it's applicable for the current version
        /// </summary>
        ISharedService Create();
    }
}
