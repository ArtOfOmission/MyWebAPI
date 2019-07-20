using MyWebAPI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyWebAPI.Infrastructure.DataBase
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyContext _myContext;

        public UnitOfWork(MyContext myContext)
        {
            this._myContext = myContext;
        }

        public async Task<bool> SaveAsync()
        {
            return await this._myContext.SaveChangesAsync() > 0;
        }
    }
}
