using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Object = UnityEngine.Object;

namespace VAT.Packaging
{
    public interface IContentReference {
        Address Address { get; }
        bool TryGetContent(out Content content);
    }

    [Serializable]
    public class ContentReference : IContentReference {
        [SerializeField]
        protected Address _address = Address.EMPTY;

        public Address Address => _address;

        public bool TryGetContent(out Content content)
        {
            content = null;

            if (!AssetPackager.IsReady)
                return false;

            return AssetPackager.Instance.TryGetContent(_address, out content);
        }
    }

    [Serializable]
    public class ContentReferenceT<T> : ContentReference where T : Content {
        public bool TryGetContent(out T content) {
            content = null;
            base.TryGetContent(out var otherContent);

            if (otherContent is T cast) {
                content = cast;
                return true;
            }
            
            return false;
        }
    }
}
