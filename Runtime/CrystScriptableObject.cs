using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class CrystScriptableObject : CrystAssetT<ScriptableObject> {
        public CrystScriptableObject(string guid) : base(guid) { }

        public CrystScriptableObject(ScriptableObject obj) : base(obj) { }
    }
}
