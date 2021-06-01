using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medical.Models
{
    public class UserLoginModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? HospitalId { get; set; }
        public string HospitalName { get; set; }
        public string FullName
        {
            get
            {
                return LastName + " " + FirstName;
            }
        }
        public string Phone { get; set; }
        public string Email { get; set; }
        public IList<RoleModel> Roles { get; set; }
    }
}
