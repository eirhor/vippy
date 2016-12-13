namespace Geta.VippyWrapper.Responses
{
    public class Tag
    {
        public string Id { get; set; }

        public string Text { get; set; }
    }

    /// <summary>
    /// Helper class to deserialize Archive tags to Tag class
    /// Vippy tags for videos has different property names than archive tags, but has same purpose
    /// TODO: check if JSON serializer can be used to map to different fields
    /// </summary>
    internal class ArchiveTag : Tag
    {
        public string TagId
        {
            get
            {
                return Id;
            }
            set
            {
                Id = value;
            }
        }

        public string TagText
        {
            get
            {
                return Text;
            }
            set
            {
                Text = value;
            }
        }

    }
}