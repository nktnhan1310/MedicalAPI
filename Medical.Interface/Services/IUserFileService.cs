using Medical.Entities;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IUserFileService : IDomainService<UserFiles, SearchUserFile>
    {
        Task<PagedList<UserFileExtensions>> GetPagedListExtension(SearchUserFile baseSearch);
    }

}
