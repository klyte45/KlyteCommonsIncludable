using ColossalFramework;
using ColossalFramework.UI;
using Klyte.Commons.Interfaces;
using Klyte.Commons.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static Klyte.Commons.TextureAtlas.KCCommonTextureAtlas;

namespace Klyte.Commons.TextureAtlas
{
    public class KCCommonTextureAtlas : TextureAtlasDescriptor<KCCommonTextureAtlas, KCResourceLoader, SpriteNames>
    {
        protected override int Width => 43;
        protected override int Height => 49;
        protected override string ResourceName => "UI.Images.sprites.png";
        protected override string CommonName => "KlyteCommonsSprites";
        public enum SpriteNames
        {
            ToolbarIconGroup6Hovered, ToolbarIconGroup6Focused, ToolbarIconGroup6Pressed, KlyteMenuIcon
        }
    }
}
