using System;

namespace ZeroDawn.Helper.ComplexReader
{
    public class ComplexInfoAttribute : Attribute
    {
        public ComplexInfoAttribute()
        {
        }

        public ComplexInfoAttribute(string name)
        {
            FieldName = name;
        }

        public ComplexInfoAttribute(int count)
        {
            Count = count;
        }

        public string FieldName { get; set; }
        public long Count { get; set; }

        public string Conditional { get; set; }

        public string Comment { get; set; }

        public bool FromUnknownData { get; set; } = false;
    }
}
