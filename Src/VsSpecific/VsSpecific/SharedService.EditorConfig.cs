using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;

namespace Vim.VisualStudio.Specific
{
#if VS_SPECIFIC_2012 || VS_SPECIFIC_2013 || VS_SPECIFIC_2015

    internal partial class SharedService 
    {
        private bool GetInsertFinalNewLine(IEditorOptions options)
        {
            return false;
        }
    }

#else

    // Implement EditorConfig related methods only supported on
    // VS2017 or later.
    internal partial class SharedService 
    {
        private bool GetInsertFinalNewLine(IEditorOptions options)
        {
            return DefaultOptionExtensions.GetInsertFinalNewLine(options);
        }
    }

#endif
}

