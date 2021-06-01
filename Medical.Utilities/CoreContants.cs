using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Utilities
{
    public class CoreContants
    {
        public const string ViewAll = "ViewAll";
        public const string Delete = "Delete";
        public const string AddNew = "AddNew";
        public const string View = "View";
        public const string FullControl = "FullControl";
        public const string Update = "Update";
        public const string Approve = "Approve";
        public const string Import = "Import";
        public const string Upload = "Upload";
        public const string Download = "Download";
        public const string DeleteFile = "DeleteFile";
        public const string Export = "Export";

        public const string USER_GROUP = "USER";
        public const string ADMIN_GROUP = "ADMIN";

        /// <summary>
        /// Danh mục quyền
        /// </summary>
        public enum PermissionContants
        {
            ViewAll = 1,
            View = 2,
            AddNew = 3,
            Update = 4,
            Delete = 5,
            Import = 6,
            Upload = 7,
            Download = 8,
            Export = 9
        }

    }
}
