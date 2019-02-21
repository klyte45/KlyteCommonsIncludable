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

namespace Klyte.Commons.TextureAtlas
{
    public class KwyttoTextureAtlas : TextureAtlasDescriptor<KwyttoTextureAtlas, KCResourceLoader>
    {
        protected override int Height => 256;
        protected override int Width => 256;
        protected override string ResourceName => "UI.Images.Kwytto.png";
        protected override string CommonName => "KlyteKwyttoSprites";
        public override string[] SpriteNames => new string[] {
                    "Icon",
                    "Normal",
                    "LookR",
                    "LookL",
                    "Disgusting",
                    "Fofi"
                };
    }
}
