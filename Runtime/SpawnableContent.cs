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

            if (isBuilding && MainGameObject != null && MainGameObject.PackedAsset && Package != null) {
                var groupName = $"{Address.CleanAddress(Package.Title.ToLower())}_spawnables";

                MainGameObject.PackedAsset.MarkAsAddressable(groupName);
            }
        }
#endif
    }
}
