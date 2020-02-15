using System;
using System.Collections.Generic;

namespace DapperHelper.xUnitTests
{
    public class Article : BaseModel
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public ArticleStatus Status { get; set; }

        public DateTime UpdateTime { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }

        public IEnumerable<Comment> Comments { get; set; }
    }
}
