using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyWebAPI.Core.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> SaveAsync();
    }
}
