using System.Collections.Generic;

namespace Import
{
    public class Image 
    {
        public string Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public IList<Tag> Tags { get; set; }

        public IList<Region> Regions { get; set; }
    }
}