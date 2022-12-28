using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using VAT.Shared.Extensions;

namespace VAT.Packaging
{
    public class SpawnableContent : GameObjectContent {
#if UNITY_EDITOR
        public override void ValidateAsset(bool isBuilding = false) {
            base.ValidateAsset(isBuilding);

            if (isBuilding && _mainAsset != null && _mainAsset.PackedAsset && Package != null) {
                var groupName = $"{Address.CleanAddress(Package.Title.ToLower())}_spawnables";

                _mainAsset.PackedAsset.MarkAsAddressable(groupName);
            }
        }
#endif
    }
}
