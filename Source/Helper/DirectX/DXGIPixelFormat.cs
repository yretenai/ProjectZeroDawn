using System.Diagnostics.CodeAnalysis;

namespace ZeroDawn.Helper.DirectX
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum DXGIPixelFormat : byte
    {
        UNKNOWN = 0x00,

        [DXGIPixelFormatInfo(0x80)] R32G32B32A32_TYPELESS = 0x01,

        [DXGIPixelFormatInfo(0x80)] R32G32B32A32_FLOAT = 0x02,

        [DXGIPixelFormatInfo(0x80)] R32G32B32A32_UINT = 0x03,

        [DXGIPixelFormatInfo(0x80)] R32G32B32A32_SINT = 0x04,

        [DXGIPixelFormatInfo(0x60)] R32G32B32_TYPELESS = 0x05,

        [DXGIPixelFormatInfo(0x60)] R32G32B32_FLOAT = 0x06,

        [DXGIPixelFormatInfo(0x60)] R32G32B32_UINT = 0x07,

        [DXGIPixelFormatInfo(0x60)] R32G32B32_SINT = 0x08,

        [DXGIPixelFormatInfo(0x40)] R16G16B16A16_TYPELESS = 0x09,

        [DXGIPixelFormatInfo(0x40)] R16G16B16A16_FLOAT = 0x0A,

        [DXGIPixelFormatInfo(0x40)] R16G16B16A16_UNORM = 0x0B,

        [DXGIPixelFormatInfo(0x40)] R16G16B16A16_UINT = 0x0C,

        [DXGIPixelFormatInfo(0x40)] R16G16B16A16_SNORM = 0x0D,

        [DXGIPixelFormatInfo(0x40)] R16G16B16A16_SINT = 0x0E,

        [DXGIPixelFormatInfo(0x40)] R32G32_TYPELESS = 0x0F,

        [DXGIPixelFormatInfo(0x40)] R32G32_FLOAT = 0x10,

        [DXGIPixelFormatInfo(0x40)] R32G32_UINT = 0x11,

        [DXGIPixelFormatInfo(0x40)] R32G32_SINT = 0x12,

        [DXGIPixelFormatInfo(0x40)] R32G8X24_TYPELESS = 0x13,

        [DXGIPixelFormatInfo(0x40)] D32_FLOAT_S8X24_UINT = 0x14,

        [DXGIPixelFormatInfo(0x40)] R32_FLOAT_X8X24_TYPELESS = 0x15,

        [DXGIPixelFormatInfo(0x40)] X32_TYPELESS_G8X24_UINT = 0x16,

        [DXGIPixelFormatInfo(0x20)] R10G10B10A2_TYPELESS = 0x17,

        [DXGIPixelFormatInfo(0x20)] R10G10B10A2_UNORM = 0x18,

        [DXGIPixelFormatInfo(0x20)] R10G10B10A2_UINT = 0x19,

        [DXGIPixelFormatInfo(0x20)] R11G11B10_FLOAT = 0x1A,

        [DXGIPixelFormatInfo(0x20)] R8G8B8A8_TYPELESS = 0x1B,

        [DXGIPixelFormatInfo(0x20)] R8G8B8A8_UNORM = 0x1C,

        [DXGIPixelFormatInfo(0x20)] R8G8B8A8_UNORM_SRGB = 0x1D,

        [DXGIPixelFormatInfo(0x20)] R8G8B8A8_UINT = 0x1E,

        [DXGIPixelFormatInfo(0x20)] R8G8B8A8_SNORM = 0x1F,

        [DXGIPixelFormatInfo(0x20)] R8G8B8A8_SINT = 0x20,

        [DXGIPixelFormatInfo(0x20)] R16G16_TYPELESS = 0x21,

        [DXGIPixelFormatInfo(0x20)] R16G16_FLOAT = 0x22,

        [DXGIPixelFormatInfo(0x20)] R16G16_UNORM = 0x23,

        [DXGIPixelFormatInfo(0x20)] R16G16_UINT = 0x24,

        [DXGIPixelFormatInfo(0x20)] R16G16_SNORM = 0x25,

        [DXGIPixelFormatInfo(0x20)] R16G16_SINT = 0x26,

        [DXGIPixelFormatInfo(0x20)] R32_TYPELESS = 0x27,

        [DXGIPixelFormatInfo(0x20)] D32_FLOAT = 0x28,

        [DXGIPixelFormatInfo(0x20)] R32_FLOAT = 0x29,

        [DXGIPixelFormatInfo(0x20)] R32_UINT = 0x2A,

        [DXGIPixelFormatInfo(0x20)] R32_SINT = 0x2B,

        [DXGIPixelFormatInfo(0x20)] R24G8_TYPELESS = 0x2C,

        [DXGIPixelFormatInfo(0x20)] D24_UNORM_S8_UINT = 0x2D,

        [DXGIPixelFormatInfo(0x20)] R24_UNORM_X8_TYPELESS = 0x2E,

        [DXGIPixelFormatInfo(0x20)] X24_TYPELESS_G8_UINT = 0x2F,

        [DXGIPixelFormatInfo(0x10)] R8G8_TYPELESS = 0x30,

        [DXGIPixelFormatInfo(0x10)] R8G8_UNORM = 0x31,

        [DXGIPixelFormatInfo(0x10)] R8G8_UINT = 0x32,

        [DXGIPixelFormatInfo(0x10)] R8G8_SNORM = 0x33,

        [DXGIPixelFormatInfo(0x10)] R8G8_SINT = 0x34,

        [DXGIPixelFormatInfo(0x10)] R16_TYPELESS = 0x35,

        [DXGIPixelFormatInfo(0x10)] R16_FLOAT = 0x36,

        [DXGIPixelFormatInfo(0x10)] D16_UNORM = 0x37,

        [DXGIPixelFormatInfo(0x10)] R16_UNORM = 0x38,

        [DXGIPixelFormatInfo(0x10)] R16_UINT = 0x39,

        [DXGIPixelFormatInfo(0x10)] R16_SNORM = 0x3A,

        [DXGIPixelFormatInfo(0x10)] R16_SINT = 0x3B,

        [DXGIPixelFormatInfo(0x08)] R8_TYPELESS = 0x3C,

        [DXGIPixelFormatInfo(0x08)] R8_UNORM = 0x3D,

        [DXGIPixelFormatInfo(0x08)] R8_UINT = 0x3E,

        [DXGIPixelFormatInfo(0x08)] R8_SNORM = 0x3F,

        [DXGIPixelFormatInfo(0x08)] R8_SINT = 0x40,

        [DXGIPixelFormatInfo(0x08)] A8_UNORM = 0x41,

        [DXGIPixelFormatInfo(0x20)] R9G9B9E5_SHAREDEXP = 0x43,

        [DXGIPixelFormatInfo(0x20)] R8G8_B8G8_UNORM = 0x44,

        [DXGIPixelFormatInfo(0x20)] G8R8_G8B8_UNORM = 0x45,

        [DXGIPixelFormatInfo(0x04, 0x04)] BC1_TYPELESS = 0x46,

        [DXGIPixelFormatInfo(0x04, 0x04)] BC1_UNORM = 0x47,

        [DXGIPixelFormatInfo(0x04, 0x04)] BC1_UNORM_SRGB = 0x48,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC2_TYPELESS = 0x49,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC2_UNORM = 0x4A,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC2_UNORM_SRGB = 0x4B,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC3_TYPELESS = 0x4C,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC3_UNORM = 0x4D,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC3_UNORM_SRGB = 0x4E,

        [DXGIPixelFormatInfo(0x04, 0x04)] BC4_TYPELESS = 0x4F,

        [DXGIPixelFormatInfo(0x04, 0x04)] BC4_UNORM = 0x50,

        [DXGIPixelFormatInfo(0x04, 0x04)] BC4_SNORM = 0x51,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC5_TYPELESS = 0x52,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC5_UNORM = 0x53,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC5_SNORM = 0x54,

        [DXGIPixelFormatInfo(0x10)] B5G6R5_UNORM = 0x55,

        [DXGIPixelFormatInfo(0x10)] B5G5R5A1_UNORM = 0x56,

        [DXGIPixelFormatInfo(0x20)] B8G8R8A8_UNORM = 0x57,

        [DXGIPixelFormatInfo(0x20)] B8G8R8X8_UNORM = 0x58,

        [DXGIPixelFormatInfo(0x20)] R10G10B10_XR_BIAS_A2_UNORM = 0x59,

        [DXGIPixelFormatInfo(0x20)] B8G8R8A8_TYPELESS = 0x5A,

        [DXGIPixelFormatInfo(0x20)] B8G8R8A8_UNORM_SRGB = 0x5B,

        [DXGIPixelFormatInfo(0x20)] B8G8R8X8_TYPELESS = 0x5C,

        [DXGIPixelFormatInfo(0x20)] B8G8R8X8_UNORM_SRGB = 0x5D,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC6H_TYPELESS = 0x5E,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC6H_UF16 = 0x5F,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC6H_SF16 = 0x60,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC7_TYPELESS = 0x61,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC7_UNORM = 0x62,

        [DXGIPixelFormatInfo(0x08, 0x04)] BC7_UNORM_SRGB = 0x63,

        [DXGIPixelFormatInfo(0x10)] B4G4R4A4_UNORM = 0x73
    }
}
