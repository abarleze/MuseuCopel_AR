using System;
using System.Collections.Generic;
using GYAInternal.Json;

//using UnityEngine;
//using System.Collections;
//using GYAInternal.Json.Converters;

// GYA Asset Info - User maintained asset info for each package

namespace XeirGYA
{
    public class GYAAssetInfo
    {
        public List<AssetInfo> AssetsInfo { get; set; }

        public GYAAssetInfo()
        {
            AssetsInfo = new List<AssetInfo>();
        }

        public class AssetInfo
        {
            [JsonConverter(typeof(GYA.StringToIntConverter))]
            public int id { get; set; }

            public string filePath { get; set; }

            public int[] groups { get; set; }

            public string linkForum { get; set; }
            public string notes { get; set; }

            public DateTimeOffset purchaseDate { get; set; }

            [JsonConverter(typeof(GYA.StringToIntConverter))]
            public int purchasePrice { get; set; }

            public AssetInfo()
            {
                id = 0;
                filePath = String.Empty;

                groups = new int[0];

                linkForum = String.Empty;
                notes = String.Empty;

                //titleAlt = String.Empty;
                purchaseDate = DateTimeOffset.MinValue;
                purchasePrice = 0;
            }
        }
    }
}