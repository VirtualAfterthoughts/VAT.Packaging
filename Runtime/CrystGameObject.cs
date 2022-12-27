using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class CrystGameObject : CrystAssetT<GameObject> {
        public CrystGameObject(string guid) : base(guid) { }

        public CrystGameObject(GameObject go) : base(go) { }
    }
}
