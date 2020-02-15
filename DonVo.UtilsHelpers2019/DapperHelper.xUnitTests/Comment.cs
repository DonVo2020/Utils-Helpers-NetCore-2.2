using System;

namespace DapperHelper.xUnitTests
{
    public class Comment : BaseModel
    {
        public int ArticleId { get; set; }

        public Article Article { get; set; }

        public string Content { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
