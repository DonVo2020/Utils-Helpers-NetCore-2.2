using System;
using System.Collections.Generic;

namespace AutoMapperSamples
{
    public class Article
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string TypeName { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }

    public class ArticleDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public IEnumerable<string> Comments { get; set; }
    }
}