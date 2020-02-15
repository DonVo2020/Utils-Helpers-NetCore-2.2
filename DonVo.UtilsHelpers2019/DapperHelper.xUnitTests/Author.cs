using System;

namespace DapperHelper.xUnitTests
{
    public class Author : BaseModel
    {
        public string NickName { get; set; }

        public string RealName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Address { get; set; }

        public Author() { }

        public Author(string nickName, string realName)
        {
            NickName = nickName;
            RealName = realName;
        }
    }
}
