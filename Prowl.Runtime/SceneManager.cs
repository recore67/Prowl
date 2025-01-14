using System;
using System.Collections.Generic;
using System.Linq;

namespace Prowl.Runtime.SceneManagement;

public static class SceneManager
{
    private static readonly List<GameObject> _gameObjects = new();
    internal static HashSet<int> _dontDestroyOnLoad = new();

    public static Scene MainScene { get; private set; } = new();

    public static List<GameObject> AllGameObjects => _gameObjects;

    public static bool AllowGameObjectConstruction = true;

    public static event Action PreFixedUpdate;
    public static event Action PostFixedUpdate;

    private static Tag? StoredScene;
    private static Guid StoredSceneID;

    public static void Initialize()
    {
        GameObject.Internal_Constructed += OnGameObjectConstructed;
        GameObject.Internal_DestroyCommitted += OnGameObjectDestroyCommitted;
        InstantiateNewScene();
    }

    public static void StoreScene()
    {
        Debug.If(StoredScene != null, "Scene is already stored.");
        // Serialize the Scene manually to save its state
        // exclude objects with the DontSave hideFlag
        GameObject[] GameObjects = AllGameObjects.Where(x => !x.hideFlags.HasFlag(HideFlags.DontSave) && !x.hideFlags.HasFlag(HideFlags.HideAndDontSave)).ToArray();
        StoredScene = TagSerializer.Serialize(GameObjects);
        StoredSceneID = MainScene.AssetID;
    }

    public static void RestoreScene()
    {
        Debug.IfNull(StoredScene, "Scene is not stored.");
        Clear();
        var deserialized = TagSerializer.Deserialize<GameObject[]>(StoredScene);
        MainScene.AssetID = StoredSceneID;
        StoredScene = null;
    }

    public static void InstantiateNewScene()
    {
        var go = new GameObject("Directional Light");
        go.AddComponent<DirectionalLight>(); // Will auto add Transform as DirectionLight requires it
        go.transform.localEulerAngles = new System.Numerics.Vector3(130, 45, 0);
        var alGo = new GameObject("Ambient Light");
        var al = alGo.AddComponent<AmbientLight>();
        al.skyIntensity = 0.4f;
        al.groundIntensity = 0.1f;

        var cam = new GameObject("Main Camera");
        cam.tag = "Main Camera";
        cam.transform.position = new(0, 0, -10);
        cam.AddComponent<Camera>();
    }

    private static void OnGameObjectConstructed(GameObject go)
    {
        if (!AllowGameObjectConstruction) return;
        lock (_gameObjects)
            _gameObjects.Add(go);
    }

    private static void OnGameObjectDestroyCommitted(GameObject go)
    {
        lock (_gameObjects)
            _gameObjects.Remove(go);

        go.parent?.children.Remove(go);
    }

    public static void Clear()
    {
        for (int i = 0; i < _gameObjects.Count; i++)
            if (!_dontDestroyOnLoad.Contains(_gameObjects[i].InstanceID))
                _gameObjects[i].Destroy();
        EngineObject.HandleDestroyed();
        _gameObjects.Clear();
        Physics.Dispose();
        Physics.Initialize();
        MainScene = new();
    }

    public static void Update()
    {
        EngineObject.HandleDestroyed();

        for (int i=0; i< _gameObjects.Count; i++)
            foreach (var comp in _gameObjects[i].GetComponents<MonoBehaviour>())
            {
                if (!comp.HasStarted)
                {
                    try
                    {
#warning TODO: Awake should be called immediately after the creation of the component/gameobject not in the first frame
                        try {
                            comp.HasStarted |= comp.Internal_Awake();
                        }
                        catch (Exception e) {
                            Debug.LogError($"Error in {comp.GetType().Name}.Awake of {_gameObjects[i].Name}: {e.Message} \n StackTrace: {e.StackTrace}");
                            comp.HasStarted = true; // An error counts as started
                        }
                        if(comp.HasStarted)
                            comp.Internal_Start();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error in {comp.GetType().Name}.Start of {_gameObjects[i].Name}: {e.Message} \n StackTrace: {e.StackTrace}");
                    }
                }

                try
                {
                    comp.UpdateCoroutines();
                    comp.Internal_Update();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error in {comp.GetType().Name}.Update of {_gameObjects[i].Name}: {e.Message} \n StackTrace: {e.StackTrace}");
                }
            }

        for (int i = 0; i < _gameObjects.Count; i++)
            foreach (var comp in _gameObjects[i].GetComponents<MonoBehaviour>())
            {
                try
                {
                    comp.Internal_LateUpdate();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error in {comp.GetType().Name}.LateUpdate of {_gameObjects[i].Name}: {e.Message} \n StackTrace: {e.StackTrace}");
                }
            }

        for (int i = 0; i < _gameObjects.Count; i++)
            foreach (var comp in _gameObjects[i].GetComponents<MonoBehaviour>())
            {
                try
                {
                    comp.UpdateEndOfFrameCoroutines();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error in {comp.GetType().Name}.UpdateEndOfFrameCoroutines of {_gameObjects[i].Name}: {e.Message} \n StackTrace: {e.StackTrace}");
                }
            }
    }

    public static void PhysicsUpdate()
    {
        PreFixedUpdate?.Invoke();
        for (int i = 0; i < _gameObjects.Count; i++)
            foreach (var comp in _gameObjects[i].GetComponents<MonoBehaviour>())
            {
                try
                {
                    comp.Internal_FixedUpdate();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error in {comp.GetType().Name}.FixedUpdate of {_gameObjects[i].Name}: {e.Message} \n StackTrace: {e.StackTrace}");
                }
            }
        PostFixedUpdate?.Invoke();
    }

    public static void Draw() {
        var Cameras = MonoBehaviour.FindObjectsOfType<Camera>().ToList();
        Cameras.Sort((a, b) => a.RenderOrder.CompareTo(b.DrawOrder));
        foreach (var cam in Cameras) 
            if(cam.EnabledInHierarchy)
                cam.Render(-1, -1);
    }

    public static void LoadScene(Scene scene)
    {
        Clear();
        MainScene = scene;
        MainScene.InstantiateScene();
        for (int i = 0; i < _gameObjects.Count; i++)
            foreach (var comp in _gameObjects[i].GetComponents<MonoBehaviour>())
                comp.Internal_OnSceneLoaded();
    }

    public static void LoadScene(AssetRef<Scene> scene)
    {
        if(scene.IsAvailable == false) throw new Exception("Scene is not available.");
        Clear();
        MainScene = scene.Res;
        MainScene.InstantiateScene();
        for (int i = 0; i < _gameObjects.Count; i++)
            foreach (var comp in _gameObjects[i].GetComponents<MonoBehaviour>())
                comp.Internal_OnSceneLoaded();
    }
}

