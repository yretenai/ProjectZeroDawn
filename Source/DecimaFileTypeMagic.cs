namespace ZeroDawn
{
    public enum DecimaFileTypeMagic : ulong
    {
        /// <summary>
        ///     Prefetch file
        /// </summary>
        Prefetch = 0xF34A76FAD0A1E0D7,

        /// <summary>
        ///     @2 Texture file
        /// </summary>
        Texture2x = 0x9C78E9FDC6042A60,

        /// <summary>
        ///     Texture file
        /// </summary>
        Texture = 0x5D5C08775CFEADF8,

        /// <summary>
        ///     Simple Text files
        /// </summary>
        SimpleText = 0xB89A596B420BB2E2,

        /// <summary>
        ///     Simple Text file table
        /// </summary>
        Collection = 0xA97082C73B2BC4BB,

        /// <summary>
        ///     Game Settings
        /// </summary>
        GameSettings = 0x2A3C7DB790214B9C,

        /// <summary>
        ///     Game property links
        /// </summary>
        GameProperty = 0x403AEDA2FF627F90,

        /// <summary>
        ///     Game world info
        /// </summary>
        GameWorld = 0x8EE8B64CC2B58D30,

        /// <summary>
        ///     Level Data
        /// </summary>
        LevelData = 0x70540C4A1D6A2F97,

        /// <summary>
        ///     Level Property, usually binds to a file
        /// </summary>
        LevelProperty = 0x3C044275D70A124B,

        /// <summary>
        ///     Level Fact Property, usually binds to a file and also some facts
        /// </summary>
        LevelFactProperty = 0x921397E07300755A,

        /// <summary>
        ///     Streaming Level Tiles
        /// </summary>
        LevelTilesProperty = 0x58834E044E7D8AA2,

        /// <summary>
        ///     ?
        /// </summary>
        CollisionTrigger = 0xA5B38AE190F649E2,

        /// <summary>
        ///     ?
        /// </summary>
        FactCollisionTrigger = 0x2A7D008CFEDA40F7,

        /// <summary>
        ///     ?
        /// </summary>
        PhysicsCollisionResource = 0x325766D1175671D7,

        /// <summary>
        ///     ?
        /// </summary>
        EntitySinglePlayerDeathCamera = 0x74D4997FC49137B3,

        /// <summary>
        ///     ?
        /// </summary>
        EntityFlyOverCamera = 0x93632BD2DE8F4099,

        /// <summary>
        ///     ?
        /// </summary>
        EntityFXTemplate = 0x78C2D25A923D336D,

        /// <summary>
        ///     ?
        /// </summary>
        EntityMissionManager = 0xC85E1B50F815F1F5,

        /// <summary>
        ///     ?
        /// </summary>
        HitResponseArea = 0xA617B06C0F166693,

        /// <summary>
        ///     ?
        /// </summary>
        EntityPlayerCharacter = 0xCB23C23725A6DAFB,

        /// <summary>
        ///     Drawable Map Tile Info
        /// </summary>
        DrawableMapTiles = 0xE7058BFB9ACA35FC
    }
}
