using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    public class AudioClipContent : ContentT<GameObject> {
        [SerializeField]
        private CrystAudioClip _mainAudioClip;

        public override CrystAsset MainAsset {
            get {
                return _mainAudioClip;
            }
            set {
                if (value != null && value.GetType() == typeof(CrystAsset)) {
                    _mainAudioClip = new CrystAudioClip(value.AssetGUID);
                }
                else {
                    _mainAudioClip = value as CrystAudioClip;
                }
            }
        }

        public CrystAudioClip MainAudioClip { get { return _mainAudioClip; } set { _mainAudioClip = value; } }

#if UNITY_EDITOR
        public override void ValidateAsset(bool isBuilding = false) {
            _mainAudioClip.ValidateGUID();
        }

        public override void SetAsset(Object asset) {
            if (asset is not AudioClip)
                throw new ArgumentException("Asset for content was not an Audio Clip.");

            MainAsset = new CrystAudioClip(asset as AudioClip);
        }
#endif
    }
}
