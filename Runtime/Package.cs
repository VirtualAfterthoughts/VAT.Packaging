using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using VAT.Serialization.JSON;

namespace VAT.Packaging
{
    public class Package : Shippable, IJSONPackable {
        public const string BUILT_NAME = "package.json";

        [SerializeField]
        [Tooltip("The author of the package.")]
        private string _author;
        public string Author { get { return _author; } set { _author = value; } }

        [SerializeField]
        [Tooltip("Whether or not the package is an internal game package.")]
        private bool _internal;
        public bool Internal { get { return _internal; } set { _internal = value; } }

        [SerializeField]
        [Tooltip("The contents packed within this package.")]
        private List<Content> _contents;
        public List<Content> Contents {
            get {
                _contents ??= new List<Content>();

                return _contents; 
            }
            set { _contents = value; }
        }

        public static Package Create(Type type) {
            if (!type.IsSubclassOf(typeof(Package)) && !(type == typeof(Package)))
                throw new Exception("Type does not inherit from Package.");

            var package = CreateInstance(type) as Package;
            return package;
        }

#if UNITY_EDITOR
        public void OnValidate() {
            // Validate empty strings
            if (string.IsNullOrWhiteSpace(_author))
                _author = "Author";

            if (string.IsNullOrWhiteSpace(_title))
                _title = "My Package";

            // Update the address
            Address = Address.BuildAddress(_author, "Package", _title);

            // Verify contents
            _contents.RemoveAll((c) => c == null);
        }
#endif

        public void Pack(JSONPacker packer, JObject json) {
            json.Add("address", Address.ID);
            json.Add("title", Title);
            json.Add("description", Description);
            json.Add("author", Author);
            json.Add("internal", _internal);

            JArray contentArray = new();
            foreach (var content in _contents) {
                contentArray.Add(packer.PackReference(content));
            }
            json.Add("contents", contentArray);
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

            if (json.TryGetValue("author", out var author)) {
                _author = author.ToString();
            }

            if (json.TryGetValue("internal", out var isInternal)) {
                _internal = isInternal.ToObject<bool>();
            }

            if (json.TryGetValue("contents", out var contents)) {
                _contents = new List<Content>();

                var contentArray = (JArray)contents;
                foreach (var reference in contentArray) {
                    if (unpacker.TryCreateFromReference(reference, out var content, Content.Create)) {
                        _contents.Add(content);
                    }
                }
            }
        }
    }
}
