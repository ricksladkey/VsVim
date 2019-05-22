﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Vim.VisualStudio.Specific.Implementation.WordCompletion.Legacy;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Editor;

#if VS_SPECIFIC_2019
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Vim.VisualStudio.Specific.Implementation.WordCompletion.Async;
#endif

namespace Vim.VisualStudio.Specific.Implementation.WordCompletion
{
    /// <summary>
    /// This type is responsible for providing word completion sessions over a given ITextView
    /// instance and given set of words.
    /// 
    /// Properly integrating with the IntelliSense stack here is a bit tricky.  In order to participate
    /// in any completion session you must provide an ICompletionSource for the lifetime of the 
    /// ITextView.  Ideally we don't want to provide any completion information unless we are actually
    /// starting a word completion session
    /// </summary>
    [Export(typeof(IVimSpecificService))]
    internal sealed class VimWordCompletionUtil : VimSpecificService, IWordCompletionSessionFactory
    {
#if VS_SPECIFIC_2019
        private readonly IAsyncCompletionBroker _asyncCompletionBroker;
        private readonly WordAsyncCompletionSessionFactory _asyncFactory;
        private readonly WordLegacyCompletionSessionFactory _legacyFactory;

        [ImportingConstructor]
        internal VimWordCompletionUtil(
            Lazy<IVimHost> vimHost,
            IAsyncCompletionBroker asyncCompletionBroker,
            ICompletionBroker completionBroker,
            IIntellisenseSessionStackMapService intellisenseSessionStackMapService,
            [Import(AllowDefault = true)] IVsEditorAdaptersFactoryService vsEditorAdapterFactoryService = null)
            :base(vimHost)
        {
            _asyncCompletionBroker = asyncCompletionBroker;
            _asyncFactory = new WordAsyncCompletionSessionFactory(asyncCompletionBroker, vsEditorAdapterFactoryService);
            _legacyFactory = new WordLegacyCompletionSessionFactory(completionBroker, intellisenseSessionStackMapService);
        }

        private FSharpOption<IWordCompletionSession> CreateWordCompletionSession(ITextView textView, SnapshotSpan wordSpan, IEnumerable<string> wordCollection, bool isForward)
        {
            return _asyncCompletionBroker.IsCompletionSupported(textView.TextBuffer.ContentType)
                ? _asyncFactory.CreateWordCompletionSession(textView, wordSpan, wordCollection, isForward)
                : _legacyFactory.CreateWordCompletionSession(textView, wordSpan, wordCollection, isForward);
        }

#elif VS_SPECIFIC_2015 || VS_SPECIFIC_2017

        private readonly WordLegacyCompletionSessionFactory _legacyFactory;

        [ImportingConstructor]
        internal VimWordCompletionUtil(
            Lazy<IVimHost> vimHost,
            ICompletionBroker completionBroker,
            IIntellisenseSessionStackMapService intellisenseSessionStackMapService)
            :base(vimHost)
        {
            _legacyFactory = new WordLegacyCompletionSessionFactory(completionBroker, intellisenseSessionStackMapService);
        }

        private FSharpOption<IWordCompletionSession> CreateWordCompletionSession(ITextView textView, SnapshotSpan wordSpan, IEnumerable<string> wordCollection, bool isForward)
        {
            return _legacyFactory.CreateWordCompletionSession(textView, wordSpan, wordCollection, isForward);
        }

#else
#error Unsupported configuration
#endif

        #region IWordCompletionSessionFactory

        FSharpOption<IWordCompletionSession> IWordCompletionSessionFactory.CreateWordCompletionSession(ITextView textView, SnapshotSpan wordSpan, IEnumerable<string> wordCollection, bool isForward)
        {
            return CreateWordCompletionSession(textView, wordSpan, wordCollection, isForward);
        }

        #endregion
    }
}

