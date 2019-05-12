using System;
using System.Runtime.CompilerServices;
using ZeroDawn.Core;

namespace ZeroDawn.Helper
{
    public class CoreTuple : ITuple, IDisposable
    {
        /// <summary>
        /// </summary>
        /// <param name="core"></param>
        /// <param name="hash"></param>
        public CoreTuple(DecimaCoreFile core, long hash)
        {
            Core = core;
            Hash = hash;
        }

        /// <summary>
        /// </summary>
        /// <param name="tuple"></param>
        private CoreTuple((DecimaCoreFile, long) tuple)
        {
            (Core, Hash) = tuple;
        }

        /// <summary>
        /// </summary>
        /// <param name="core"></param>
        /// <param name="hash"></param>
        /// <param name="text"></param>
        public CoreTuple(DecimaCoreFile core, long hash, string text)
        {
            Core = core;
            Hash = hash;
            Text = text;
        }

        /// <summary>
        /// </summary>
        /// <param name="tuple"></param>
        private CoreTuple((DecimaCoreFile, long, string) tuple)
        {
            (Core, Hash, Text) = tuple;
        }

        /// <summary>
        /// </summary>
        public DecimaCoreFile Core { get; }

        /// <summary>
        /// </summary>
        public long Hash { get; }

        /// <summary>
        /// </summary>
        private string TextBase { get; set; }

        /// <summary>
        /// </summary>
        public string Text
        {
            get => TextBase ?? Hash.ToString("X16");
            set => TextBase = value;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Core.Dispose();
        }

        /// <inheritdoc />
        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    default:
                    case 0:
                        return Core;
                    case 1:
                        return Hash;
                    case 2:
                        return Text;
                }
            }
        }

        /// <inheritdoc />
        public int Length => 3;

        /// <summary>
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static implicit operator DecimaCoreFile(CoreTuple tuple)
        {
            return tuple.Core;
        }

        /// <summary>
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static implicit operator long(CoreTuple tuple)
        {
            return tuple.Hash;
        }

        /// <summary>
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static implicit operator string(CoreTuple tuple)
        {
            return tuple.Text;
        }

        /// <summary>
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static implicit operator CoreTuple((DecimaCoreFile, long) tuple)
        {
            return new CoreTuple(tuple);
        }

        /// <summary>
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static implicit operator CoreTuple((DecimaCoreFile, long, string) tuple)
        {
            return new CoreTuple(tuple);
        }

        /// <summary>
        /// </summary>
        /// <param name="core"></param>
        /// <param name="hash"></param>
        /// <param name="text"></param>
        public void Deconstruct(out DecimaCoreFile core, out long hash, out string text)
        {
            core = Core;
            hash = Hash;
            text = Text;
        }

        /// <summary>
        /// </summary>
        /// <param name="core"></param>
        /// <param name="hash"></param>
        public void Deconstruct(out DecimaCoreFile core, out long hash)
        {
            core = Core;
            hash = Hash;
        }

        /// <summary>
        /// </summary>
        /// <param name="core"></param>
        public void Deconstruct(out DecimaCoreFile core)
        {
            core = Core;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Core != default ? Core.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ Hash.GetHashCode();
                hashCode = (hashCode * 397) ^ (Text != default ? Text.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Text} - {Core}";
        }
    }
}
