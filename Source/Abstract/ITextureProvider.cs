using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ZeroDawn.Abstract
{
    // ReSharper disable once ClassNeverInstantiated.Global
    /// <inheritdoc />
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    // ReSharper disable once InconsistentNaming
    public abstract class ITextureProvider : IEnumerable<ITexture>
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public virtual int TextureCount { get; } = 0;
        public virtual ITexture[] Textures { get; protected set; } = default;

        public IEnumerator<ITexture> GetEnumerator()
        {
            return Textures.OfType<ITexture>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ITexture GetTexture(int index)
        {
            return Textures[index];
        }

        public ITexture GetTexture(string name)
        {
            return Textures.First(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<string> GetTextureNames()
        {
            return Textures.Select(x => x.Name);
        }
    }
}
