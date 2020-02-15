using System;
using System.Linq.Expressions;

namespace MongoHelper
{
    public class SortCondition<T> where T : class
    {
        public Expression<Func<T, object>> Property { get; set; }
        public SortDirection SortDirection { get; set; }

        public SortCondition(Expression<Func<T, object>> property, SortDirection sortType = SortDirection.Ascending)
        {
            Property = property;
            SortDirection = sortType;
        }
    }
}
