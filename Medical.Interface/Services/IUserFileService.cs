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
<<<<<<< HEAD

        Task<bool> UpdateFolderForFile(List<int> userFileIds, int userFolderId);
=======
>>>>>>> f087f7d996cf4bb89ac4ae0233c6e75869ec2608
    }

}
