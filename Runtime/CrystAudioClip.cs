using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    [Serializable]
    public class CrystAudioClip : CrystAssetT<AudioClip> {
        public CrystAudioClip(string guid) : base(guid) { }

        public CrystAudioClip(AudioClip clip) : base(clip) { }
    }
}
