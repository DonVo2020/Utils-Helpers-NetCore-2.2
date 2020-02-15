using System;
using System.Linq.Expressions;

namespace MongoHelper
{
    public class UpdateCondition<TDocument, TField> where TDocument : class
    {
        public Expression<Func<TDocument, TField>> Property { get; set; }
        public TField NewValue { get; set; }

        public UpdateCondition(Expression<Func<TDocument, TField>> property, TField newValue)
        {
            Property = property;
            NewValue = newValue;
        }
    }
}
