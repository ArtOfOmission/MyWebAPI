using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebAPI.Infrastructure.Resources
{
    public class LinkResource
    {
        public LinkResource(string href, string rel, string method)
        {
            this.Href = href;
            this.Rel = rel;
            this.Method = method;
        }

        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }

    }
}
