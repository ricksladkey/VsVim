using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;

namespace Vim.VisualStudio.Specific
{
#if VS_SPECIFIC_2017

    internal partial class SharedService 
    {
        private bool GetInsertFinalNewLine(IEditorOptions options)
        {
            return DefaultOptionExtensions.GetInsertFinalNewLine(options);
        }
    }
#else

    internal partial class SharedService 
    {
        private bool GetInsertFinalNewLine(IEditorOptions options)
        {
            return false;
        }
    }

#endif
}

