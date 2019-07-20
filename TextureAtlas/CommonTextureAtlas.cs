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
using static Klyte.Commons.TextureAtlas.CommonTextureAtlas;

namespace Klyte.Commons.TextureAtlas
{
    public class CommonTextureAtlas : TextureAtlasDescriptor<CommonTextureAtlas, CommonResourceLoader, SpriteNames>
    {
        protected override int Width => 64;
        protected override int Height => 64;
        protected override string ResourceName => "commons.UI.Images.sprites.png";
        protected override string CommonName => "CommonSprites";
        public enum SpriteNames
        {
            K45Button, K45ButtonHovered, K45ButtonFocused, K45ButtonDisabled
        }
    }
}
