using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace Geta.VippyModule.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = "vippyvideo")]
    public class VippyVideoEditorSelectionEditorDescriptor : EditorDescriptor
    {
        public VippyVideoEditorSelectionEditorDescriptor()
        {
            ClientEditingClass = "geta.editors.VippyVideoSelection";
        }
    }
}