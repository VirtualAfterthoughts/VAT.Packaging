using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace VAT.Packaging
{
    public enum ContentType {
        SPAWNABLE_CONTENT = 1 << 0,
        LEVEL_CONTENT = 1 << 1,
        AVATAR_CONTENT = 1 << 2,
        SCRIPTABLE_OBJECT_CONTENT = 1 << 3,
        AUDIO_CLIP_CONTENT = 1 << 4,
    }

    public static class ContentTypeExtensions {
        public static Type GetContentType(this ContentType contentType) {
            return contentType switch {
                ContentType.SPAWNABLE_CONTENT => typeof(SpawnableContent),
                ContentType.LEVEL_CONTENT => typeof(LevelContent),
                ContentType.AVATAR_CONTENT => typeof(Content),
                ContentType.SCRIPTABLE_OBJECT_CONTENT => typeof(ScriptableObjectContent),
                ContentType.AUDIO_CLIP_CONTENT => typeof(AudioClipContent),
                _ => typeof(Content),
            };
        }
    }
}
