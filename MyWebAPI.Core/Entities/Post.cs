﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebAPI.Core.Entities
{
    /// <summary>
    /// 推文类
    /// </summary>
    public class Post : Entity
    {
        public string Title { get; set; }
        public string Body { get; set; }

        public string Author { get; set; }
        public DateTime LastModified { get; set; }
    }
}
