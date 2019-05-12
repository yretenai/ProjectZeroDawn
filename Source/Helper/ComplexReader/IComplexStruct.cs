using ZeroDawn.Core;
using ZeroDawn.Managers;

namespace ZeroDawn.Helper.ComplexReader
{
    public interface IComplexStruct
    {
        void Read(DecimaCoreFile file, DecimaManagerCollection manager);
    }
}
