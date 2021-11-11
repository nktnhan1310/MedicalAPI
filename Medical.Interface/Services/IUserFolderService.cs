using Medical.Entities;
using Medical.Entities.Search;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface.Services
{
    public interface IUserFolderService : IDomainService<UserFolders, SearchUserFolder>
    {
        Task<PagedList<UserFolderExtensions>> GetPagedListExtension(SearchUserFolder baseSearch);
    }
}
