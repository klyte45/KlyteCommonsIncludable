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
using static Klyte.Commons.TextureAtlas.LineUtilsTextureAtlas;

namespace Klyte.Commons.TextureAtlas
{
    public class LineUtilsTextureAtlas : TextureAtlasDescriptor<LineUtilsTextureAtlas, KCResourceLoader, SpriteNames>
    {
        protected override string ResourceName => "UI.Images.lineFormat.png";
        protected override string CommonName => "KCLinearLineSprites";
        public enum SpriteNames
        {
            MapIcon,
            OvalIcon,
            RoundedHexagonIcon,
            RoundedPentagonIcon,
            RoundedTriangleIcon,
            OctagonIcon,
            HeptagonIcon,
            _10StarIcon,
            _9StarIcon,
            _7StarIcon,
            _6StarIcon,
            _5StarIcon,
            _4StarIcon,
            _3StarIcon,
            CameraIcon,
            MountainIcon,
            ConeIcon,
            TriangleIcon,
            CrossIcon,
            DepotIcon,
            LinearHalfStation,
            LinearStation,
            LinearBg,
            PentagonIcon,
            TrapezeIcon,
            DiamondIcon,
            _8StarIcon,
            CableCarIcon,
            ParachuteIcon,
            HexagonIcon,
            SquareIcon,
            CircleIcon,
            RoundedSquareIcon,
            ShipIcon,
            AirplaneIcon,
            TaxiIcon,
            DayIcon,
            NightIcon,
            DisabledIcon,
            NoBudgetIcon,
            BulletTrainImage,
            LowBusImage,
            HighBusImage,
            VehicleLinearMap,
            RegionalTrainIcon
        };
    }
}
