using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Web.Hosting;
using Geta.VippyWrapper.Responses;

namespace Geta.VippyVpp
{
    /// <summary>
    ///     Summary for Vippy video
    /// </summary>
    public sealed class VideoSummary : UnifiedSummary
    {
        public VideoSummary(UnifiedFile file, Video video)
            : base(file)
        {
            var metaBuilder = new MetaBuilder(video)
                .Add(v => v.Description)
                .Add(v => v.Duration)
                .Add(v => v.EndViews)
                .Add(v => v.Height)
                .Add(v => v.HighQualityUrl)
                .Add(v => v.HighQualitySize)
                .Add(v => v.LowQualityUrl)
                .Add(v => v.LowQualitySize)
                .Add(v => v.OpenGraphUrl)
                .Add(v => v.OriginalUrl)
                .Add(v => v.Plays)
                .Add(v => v.PlaysToImpression)
                .Add(v => v.Size)
                .Add(v => v.StatusCode)
                .Add(v => v.StatusText)
                .Add(v => v.Tags, v => string.Join(", ", v.Tags.Select(t => t.Text).ToArray()))
                .Add(v => v.ThumbnailUrl)
                .Add(v => v.Uploaded)
                .Add(v => v.VideoId)
                .Add(v => v.Views)
                .Add(v => v.Width);
            metaBuilder.UpdateMeta(Dictionary);

            Title = video.Title;
        }

        public VideoSummary(UnifiedFile file, string filename)
            : base(file)
        {
            Title = filename;
        }


        /// <summary>
        ///    Current version does not support persisting, does nothing
        /// </summary>
        public override void SaveChanges()
        {
            // TODO: Implement saving data back to Vippy
        }

        /// <summary>
        ///    Current version does not support persisting, returns false 
        /// </summary>
        public override bool CanPersist
        {
            // TODO: Implement saving data back to Vippy
            get { return false; }
        }

        /// <summary>
        ///     Helper class to build metadata for video file
        /// </summary>
        private class MetaBuilder
        {
            private readonly Video _video;
            private readonly Dictionary<string, object> _meta;

            public MetaBuilder(Video video)
            {
                _video = video;
                _meta = new Dictionary<string, object>();
            }


            public MetaBuilder Add<TProperty>(Expression<Func<Video, TProperty>> action, Func<Video, object> valueAction = null)
            {
                var expression = (MemberExpression)action.Body;
                var propName = expression.Member.Name;
                var value = valueAction != null
                    ? valueAction(_video)
                    : _video.GetType().GetProperty(propName).GetValue(_video);
                _meta.Add(propName, value);
                return this;
            }

            public void UpdateMeta(IDictionary dictionary)
            {
                foreach (var m in _meta)
                {
                    dictionary[m.Key] = m.Value;
                }
            }
        }
    }
}