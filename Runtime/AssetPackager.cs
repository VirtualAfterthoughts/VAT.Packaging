using Cysharp.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.AddressableAssets;

using VAT.Serialization.JSON;

using Newtonsoft.Json.Linq;

using System.Linq;
using System;

namespace VAT.Packaging {
    public class AssetPackager {
        public const string INTERNAL_PACKAGES_GROUP = "Internal Packages";
        public const string INTERNAL_PACKAGES_LABEL = "InternalPackage";

        private static AssetPackager _instance = null;
        public static AssetPackager Instance => _instance;

        private bool _isReady = false;
        private bool _isInitializing = false;

        private static Action _onPackagerReady = null;

        public static bool IsReady => Instance != null && Instance._isReady;

        private Dictionary<Address, Package> _loadedPackages;

        private Dictionary<Address, Content> _loadedContent;

        public AssetPackager(bool init = true) {
            if (init) {
                Init();
            }
        }
        
        public static void HookOnReady(Action action) {
            if (IsReady) {
                action.Invoke();
            }
            else {
                _onPackagerReady += action;
            }
        }

        public void Init() {
            if (_isInitializing)
                return;

            _isInitializing = true;

            _loadedPackages = new Dictionary<Address, Package>();
            _loadedContent = new Dictionary<Address, Content>();

            // Editor initialize
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                string packagePath = $"{CRYST_ASSETS_FOLDER}/{CRYST_PACKAGES_FOLDER}/";
                if (AssetDatabase.IsValidFolder($"Assets/{packagePath}")) {
                    string[] folders = Directory.GetDirectories(Application.dataPath + "/" + packagePath);

                    foreach (var folder in folders) {
                        string[] files = Directory.GetFiles(folder);

                        foreach (var file in files) {
                            if (!file.EndsWith(".asset"))
                                continue;

                            string final = file;
                            if (final.StartsWith(Application.dataPath)) {
                                final = "Assets" + final.Substring(Application.dataPath.Length);
                            }

                            var package = AssetDatabase.LoadAssetAtPath<Package>(final);
                            if (package != null)
                                LoadPackage(package);
                        }
                    }
                }

                _isReady = true;

                // Invoke ready
                _onPackagerReady?.Invoke();
                _onPackagerReady = null;

                return;
            }
#endif

            // Play mode/built initialize
            Internal_InitAsync().Forget();
        }

        private async UniTaskVoid Internal_InitAsync() {
            // Load built in packages
            var handle = await Addressables.LoadAssetsAsync<TextAsset>(INTERNAL_PACKAGES_LABEL, null).Task;

            foreach (var asset in handle) {
                string text = asset.text;
                JSONUnpacker unpacker = new(JObject.Parse(text));
                unpacker.UnpackRoot(out var package, Package.Create);

                if (package != null)
                    LoadPackage(package);
            }

            // Load external packages (mods)
            // Not implemented

            _isReady = true;

            // Invoke ready
            _onPackagerReady?.Invoke();
            _onPackagerReady = null;
        }

#if UNITY_EDITOR
        public const string CRYST_ASSETS_FOLDER = "_CrystAssets";
        public const string CRYST_PACKAGES_FOLDER = "Packages";
        public const string CRYST_TEXT_ASSETS_FOLDER = "Text Assets";

        [InitializeOnLoadMethod]
        public static void InitializeEditor() {
            // Create folders
            if (!AssetDatabase.IsValidFolder($"Assets/{CRYST_ASSETS_FOLDER}"))
                AssetDatabase.CreateFolder("Assets", CRYST_ASSETS_FOLDER);

            // Initialize asset packager
            if (_instance == null)
                _instance = new AssetPackager(true);
        }

        public static void RefreshEditorPackager() {
            _instance = null;
            _instance = new AssetPackager(true);
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void InitializeRuntime() {
            _instance = new AssetPackager();
        }

        public void LoadPackage(Package package) {
            if (_loadedPackages.ContainsKey(package.Address)) {
                Debug.LogError("Tried loading a package with an already loaded address!", package);
                return;
            }

            _loadedPackages.Add(package.Address, package);

            foreach (var content in package.Contents) {
                if (_loadedContent.ContainsKey(content.Address)) {
                    Debug.LogError("Tried loading content with an already loaded address!", content);
                    continue;
                }

                _loadedContent.Add(content.Address, content);
            }
        }

        public bool TryGetPackage(Address address, out Package package) {
            if (_loadedPackages.ContainsKey(address)) {
                package = _loadedPackages[address];
                return true;
            }

            package = default;
            return false;
        }

        public bool TryGetContent(Address address, out Content content) {
            if (_loadedContent.ContainsKey(address)) {
                content = _loadedContent[address];
                return true;
            }

            content = default;
            return false;
        }

        public IReadOnlyCollection<Package> GetPackages() {
            return _loadedPackages.Values;
        }

        public IReadOnlyCollection<Content> GetContents() {
            return _loadedContent.Values;
        }
    }
}
