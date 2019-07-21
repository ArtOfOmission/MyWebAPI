using MyWebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebAPI.Core.Entities
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }
    }
}
