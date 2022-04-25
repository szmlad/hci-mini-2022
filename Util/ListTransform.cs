using System;
using System.Collections.Generic;
using System.Linq;

namespace HCI.Util
{
    public class ListTransform<ValueType, TransformedType>
    {
        public List<ValueType> List { get; }
        public Func<ValueType, TransformedType> Transform { get; }
        public int Count
        {
            get => List.Count;
        }

        public ListTransform(List<ValueType> list, Func<ValueType, TransformedType> transform)
        {
            List = list;
            Transform = transform;
        }

        public IEnumerable<TransformedType> Apply() =>
            List.Select(Transform);
    }
}
