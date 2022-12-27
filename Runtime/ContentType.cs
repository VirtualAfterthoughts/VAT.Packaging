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
    }

    public static class ContentTypeExtensions {
        public static Type GetContentType(this ContentType contentType) {
            return contentType switch {
                ContentType.SPAWNABLE_CONTENT => typeof(SpawnableContent),
                ContentType.LEVEL_CONTENT => typeof(LevelContent),
                ContentType.AVATAR_CONTENT => typeof(Content),
                _ => typeof(Content),
            };
        }
    }
}
