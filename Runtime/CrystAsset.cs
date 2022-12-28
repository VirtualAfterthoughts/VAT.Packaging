using Cysharp.Threading.Tasks;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

namespace VAT.Packaging {
    [Serializable]
    public class CrystAsset {
        protected Object _editorAsset;
        public Object EditorAsset { get { return _editorAsset; } set { _editorAsset = value; } }

        public virtual Object Asset {
            get {
                if (!_operationHandle.IsValid())
                    return null;

                return _operationHandle.Result as Object;
            }
        }
        private Type _assetType;
        public virtual Type AssetType {
            get {
                if (_assetType == null)
                    return typeof(Object);

                return _assetType;
            }
        }

        [SerializeField]
        [HideInInspector]
        private string _guid;
        public string AssetGUID => _guid;

        private AsyncOperationHandle _operationHandle;
        private AsyncOperationHandle<SceneInstance> _sceneHandle;
        public AsyncOperationHandle OperationHandle => _operationHandle;

        public CrystAsset(string guid) {
            _guid = guid;
        }

        public CrystAsset() {

        }

#if UNITY_EDITOR
        public void ValidateGUID(Object editorAsset) {
            _editorAsset = editorAsset;

            if (editorAsset == null) {
                _guid = null;
                return;
            }

            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_editorAsset, out string guid, out long _))
                _guid = guid;
        }
#endif

        public void LoadAsset<T>(Action<T> onLoaded) where T : Object {
            Internal_LoadAsset<T>(onLoaded).Forget();
        }

        protected async virtual UniTaskVoid Internal_LoadAsset<T>(Action<T> onLoaded) where T : Object {
            var asset = await LoadAssetAsync<T>();
            onLoaded?.Invoke(asset);
        }

        public async UniTask<T> LoadAssetAsync<T>() where T : Object {
            if (!_operationHandle.IsValid()) {
                await Internal_LoadAssetAsync<T>();
            }
            else if (!_operationHandle.IsDone) {
                await _operationHandle;
            }

            _assetType = Asset.GetType();

            return Asset as T;
        }

        protected virtual AsyncOperationHandle<T> Internal_LoadAssetAsync<T>() where T : Object {
            AsyncOperationHandle<T> result = default;
            if (!_operationHandle.IsValid()) {
                result = Addressables.LoadAssetAsync<T>(AssetGUID);
                _operationHandle = result;
            }

            return result;
        }

        public void LoadScene(Action<SceneInstance> onLoaded, LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true) {
            Internal_LoadScene(onLoaded, mode, activateOnLoad).Forget();
        }

        protected async virtual UniTaskVoid Internal_LoadScene(Action<SceneInstance> onLoaded, LoadSceneMode mode, bool activateOnLoad) {
            var scene = await LoadSceneAsync(mode, activateOnLoad);
            onLoaded?.Invoke(scene);
        }

        public async UniTask<SceneInstance> LoadSceneAsync(LoadSceneMode mode = LoadSceneMode.Single, bool activateOnLoad = true) {
            if (!_operationHandle.IsValid()) {
                await Internal_LoadSceneAsync(mode, activateOnLoad);
            }
            else if (!_operationHandle.IsDone) {
                await _operationHandle;
            }

            _assetType = _sceneHandle.Result.GetType();
            return _sceneHandle.Result;
        }

        protected virtual AsyncOperationHandle<SceneInstance> Internal_LoadSceneAsync(LoadSceneMode mode, bool activateOnLoad) {
            AsyncOperationHandle<SceneInstance> result = default;
            if (!_sceneHandle.IsValid()) {
                result = Addressables.LoadSceneAsync(AssetGUID, mode, activateOnLoad);
                _sceneHandle = result;
            }

            return result;
        }

        public virtual bool ReleaseAsset() {
            if (!_operationHandle.IsValid())
                return false;

            Addressables.Release(_operationHandle);
            _operationHandle = default;
            return true;
        }

        public void UnloadScene(Action onSceneUnloaded) {
            if (!_sceneHandle.IsValid()) {
                onSceneUnloaded?.Invoke();
            }
            else {
                Internal_UnloadScene(onSceneUnloaded).Forget();
            }
        }

        private async UniTaskVoid Internal_UnloadScene(Action onSceneUnloaded) {
            await UnloadSceneAsync();
            onSceneUnloaded?.Invoke();
        }

        public async UniTask UnloadSceneAsync() {
            if (!_sceneHandle.IsValid())
                return;

            // Store handle reference, but make the value invalid
            var handle = _sceneHandle;
            _sceneHandle = default;

            await Addressables.UnloadSceneAsync(handle, true);
        }
    }

    [Serializable]
    public class CrystAssetT<T> : CrystAsset where T : Object {
#if UNITY_EDITOR
        [SerializeField]
        private T _packedAsset = null;
        public T PackedAsset => _packedAsset;

        public void ValidateGUID() {
            base.ValidateGUID(_packedAsset);
        }
#endif

        public new T Asset => base.Asset as T;

        public CrystAssetT(string guid) : base(guid) { }

        public CrystAssetT(T packedAsset) {
            _packedAsset = packedAsset;
            ValidateGUID();
        }

        public void LoadAsset(Action<T> onLoaded) => base.LoadAsset<T>(onLoaded);

        public async UniTask<T> LoadAssetAsync() => await base.LoadAssetAsync<T>();
    }
}
