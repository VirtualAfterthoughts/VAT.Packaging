using Newtonsoft.Json.Linq;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using VAT.Serialization.JSON;
using Object = UnityEngine.Object;

namespace VAT.Packaging {
    public interface IContent {
        CrystAsset MainAsset { get; }
    }

    public class Content : Shippable, IJSONPackable, IContent {
        [SerializeField]
        protected Package _package;
        public Package Package { get { return _package; } set { _package = value; } }

        [NonSerialized]
        protected CrystAsset _mainAsset;
        public CrystAsset MainAsset => _mainAsset;

        public static Content Create(Type type) {
            if (!type.IsSubclassOf(typeof(Content)))
                throw new Exception("Type does not inherit from Content.");

            var content = CreateInstance(type) as Content;
            return content;
        }

#if UNITY_EDITOR
        public void OnValidate() {
            // Validate empty strings
            if (string.IsNullOrWhiteSpace(_title))
                _title = "My Content";

            // Update the address
            if (_package != null)
                Address = Address.BuildAddress(_package.Author, _package.Title, _title);

            // Save asset info
            ValidateAsset();
        }
        public virtual void ValidateAsset() { }

        public virtual void SetAsset(Object asset) { }
#endif

        public void Pack(JSONPacker packer, JObject json) {
#if UNITY_EDITOR
            ValidateAsset();
#endif

            json.Add("address", Address.ID);
            json.Add("title", Title);
            json.Add("description", Description);

            if (_mainAsset != null)
                json.Add("mainAsset", _mainAsset.AssetGUID);

            if (_package != null)
                json.Add("package", packer.PackReference(_package));
        }

        public void Unpack(JSONUnpacker unpacker, JToken token) {
            var json = (JObject)token;

            if (json.TryGetValue("address", out var address)) {
                _address = new Address(address.ToString());
            }

            if (json.TryGetValue("title", out var title)) {
                _title = title.ToString();
            }

            if (json.TryGetValue("description", out var description)) {
                _description = description.ToString();
            }

            if (json.TryGetValue("mainAsset", out var mainAsset)) {
                _mainAsset = new CrystAsset(mainAsset.ToString());
            }

            if (json.TryGetValue("package", out var package)) {
                unpacker.TryCreateFromReference(package, out _package, Package.Create);
            }
        }
    }

    public class ContentT<T> : Content where T : Object {
        public new CrystAssetT<T> MainAsset => base.MainAsset as CrystAssetT<T>;
    }
}