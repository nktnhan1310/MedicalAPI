using Medical.Interface.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Service.Factory
{
    public interface IDbContextFactory
    {
        string ConnectionString { get; set; }
        IMedicalDbContext Create();
    }
}
