﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebAPI.Infrastructure.Resources
{
    /// <summary>
    /// 文章resource
    /// </summary>
    public class PostResource
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public string Author { get; set; }
        public DateTime UpdateTime { get; set; }

    }
}
