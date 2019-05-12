using System;

namespace ZeroDawn.Helper
{
    public class DecimaFileTypeAttribute : Attribute
    {
        public readonly DecimaFileTypeMagic Magic;

        public readonly string Name = string.Empty;

        public DecimaFileTypeAttribute(DecimaFileTypeMagic magic)
        {
            Magic = magic;
        }

        public DecimaFileTypeAttribute(DecimaFileTypeMagic magic, string name)
        {
            Magic = magic;
            Name = name;
        }

        public override object TypeId => this;

        public override bool Match(object obj)
        {
            if (obj is DecimaFileTypeAttribute attr) return attr.Magic == Magic;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is DecimaFileTypeAttribute attr) return attr.Magic == Magic;

            return false;
        }

        protected bool Equals(DecimaFileTypeAttribute other)
        {
            return base.Equals(other) && Magic == other.Magic;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ Magic.GetHashCode();
            }
        }

        public override bool IsDefaultAttribute()
        {
            return Magic == default;
        }

        public override string ToString()
        {
            return $"[DecimaFileType 0x{Magic:X16} {Name}]";
        }
    }
}
