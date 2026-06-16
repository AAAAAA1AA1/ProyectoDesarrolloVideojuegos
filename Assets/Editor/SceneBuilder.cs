using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using JuegoMental;
using JuegoMental.Core;

public static class SceneBuilder
{
    const string Art = "Assets/Art/Generated/";
    const string PrefabDir = "Assets/Prefabs";
    const string SceneDir = "Assets/Scenes";

    [MenuItem("Juego Mental/Build Scenes")]
    public static void BuildAll()
    {
        EnsureLayer("Ground");
        EnsureLayer("Player");
        EnsureLayer("Enemy");
        EnsureWhiteBar();
        AssetDatabase.Refresh();

        Directory.CreateDirectory(PrefabDir);
        Directory.CreateDirectory(SceneDir);

        var player = BuildPlayerPrefab();
        var patrol = BuildEnemyPrefab("PatrolEnemy", "enemy_patrol", typeof(PatrolEnemy));
        BuildEnemyPrefab("ChaserEnemy", "enemy_chaser", typeof(ChaserEnemy));
        var good = BuildPickupPrefab("PickupGood", "item_good", PickupKind.Good);
        var bad = BuildPickupPrefab("PickupBad", "item_bad", PickupKind.Bad);
        var door = BuildDoorPrefab();

        BuildHub(player, door);
        BuildLevel01(player, patrol, good, bad, door);

        SetBuildSettings();
        AssetDatabase.SaveAssets();
        Debug.Log("Escenas construidas: Hub + Level_01");
    }

    // ---------- helpers ----------

    static Sprite S(string n) => AssetDatabase.LoadAssetAtPath<Sprite>(Art + n + ".png");

    static void EnsureLayer(string name)
    {
        var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
        var so = new SerializedObject(asset);
        var layers = so.FindProperty("layers");
        for (int i = 8; i < layers.arraySize; i++)
            if (layers.GetArrayElementAtIndex(i).stringValue == name) return;
        for (int i = 8; i < layers.arraySize; i++)
        {
            var sp = layers.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(sp.stringValue)) { sp.stringValue = name; so.ApplyModifiedProperties(); return; }
        }
    }

    static void EnsureWhiteBar()
    {
        string p = Art + "bar_white.png";
        if (File.Exists(p)) return;
        Directory.CreateDirectory(Art);
        var tex = new Texture2D(4, 1, TextureFormat.RGBA32, false);
        var px = new Color32[4];
        for (int i = 0; i < 4; i++) px[i] = new Color32(255, 255, 255, 255);
        tex.SetPixels32(px); tex.Apply();
        File.WriteAllBytes(p, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(p);
        var ti = (TextureImporter)AssetImporter.GetAtPath(p);
        ti.textureType = TextureImporterType.Sprite;
        ti.filterMode = FilterMode.Point;
        ti.spritePixelsPerUnit = 4;
        ti.SaveAndReimport();
    }

    static int Mask(string layer) => 1 << LayerMask.NameToLayer(layer);

    // ---------- prefabs ----------

    // crea un miembro que rota desde su articulacion; el sprite cuelga hacia abajo
    static Transform MakeLimb(string name, Transform parent, Vector3 joint, string sprite, int order, out Transform spriteTr)
    {
        var limb = new GameObject(name).transform;
        limb.SetParent(parent);
        limb.localPosition = joint;
        limb.localRotation = Quaternion.identity;
        limb.localScale = Vector3.one;
        var sGo = new GameObject("S");
        sGo.transform.SetParent(limb);
        var sr = sGo.AddComponent<SpriteRenderer>();
        sr.sprite = S(sprite);
        sr.sortingOrder = order;
        float hUnits = sr.sprite.bounds.size.y;
        sGo.transform.localPosition = new Vector3(0f, -hUnits / 2f, 0f);
        spriteTr = sGo.transform;
        return limb;
    }

    static GameObject BuildPlayerPrefab()
    {
        var go = new GameObject("Player") { tag = "Player", layer = LayerMask.NameToLayer("Player") };
        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var col = go.AddComponent<CapsuleCollider2D>();
        col.size = new Vector2(0.7f, 1.6f);
        col.offset = new Vector2(0f, -0.05f);

        // miembros (detras del torso)
        MakeLimb("LegBack",  go.transform, new Vector3(0.1f, -0.15f, 0f), "p_leg", 8, out _);
        MakeLimb("ArmBack",  go.transform, new Vector3(0.18f, 0.45f, 0f), "p_arm", 8, out _);
        var legBack = go.transform.Find("LegBack");
        var armBack = go.transform.Find("ArmBack");

        // torso con cara
        var torsoGo = new GameObject("Torso");
        torsoGo.transform.SetParent(go.transform);
        torsoGo.transform.localPosition = Vector3.zero;
        var tsr = torsoGo.AddComponent<SpriteRenderer>();
        tsr.sprite = S("p_torso"); tsr.sortingOrder = 10;

        // miembros frontales (delante del torso)
        var legFront = MakeLimb("LegFront", go.transform, new Vector3(-0.1f, -0.15f, 0f), "p_leg", 12, out _);
        Transform armFrontSprite;
        var armFront = MakeLimb("ArmFront", go.transform, new Vector3(-0.18f, 0.45f, 0f), "p_arm", 12, out armFrontSprite);

        // espada en la mano del brazo frontal (oculta hasta atacar)
        var swordGo = new GameObject("Sword");
        swordGo.transform.SetParent(armFront);
        swordGo.transform.localPosition = new Vector3(0f, -0.55f, 0f);
        swordGo.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
        var swsr = swordGo.AddComponent<SpriteRenderer>();
        swsr.sprite = S("p_sword"); swsr.sortingOrder = 13;

        var gc = new GameObject("GroundCheck"); gc.transform.SetParent(go.transform); gc.transform.localPosition = new Vector3(0f, -0.8f, 0f);
        var ao = new GameObject("AttackOrigin"); ao.transform.SetParent(go.transform); ao.transform.localPosition = new Vector3(0.6f, 0f, 0f);

        var pc = go.AddComponent<PlayerController2D>();
        pc.groundCheck = gc.transform;
        pc.groundMask = Mask("Ground");
        pc.moveSpeed = 6f;
        pc.jumpForce = 12f;
        pc.maxJumps = 2;

        var anim = go.AddComponent<LimbAnimator>();
        anim.armFront = armFront; anim.armBack = armBack;
        anim.legFront = legFront; anim.legBack = legBack;
        anim.sword = swordGo;

        var pa = go.AddComponent<PlayerAttack>();
        pa.enemyMask = Mask("Enemy");
        pa.attackOrigin = ao.transform;

        go.AddComponent<CortisolSystem>();

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabDir + "/Player.prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    static GameObject BuildEnemyPrefab(string name, string sprite, System.Type comp)
    {
        var go = new GameObject(name) { layer = LayerMask.NameToLayer("Enemy") };
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = S(sprite); sr.sortingOrder = 10;
        var rb = go.AddComponent<Rigidbody2D>(); rb.bodyType = RigidbodyType2D.Kinematic;
        var col = go.AddComponent<BoxCollider2D>(); col.isTrigger = true; col.size = new Vector2(1f, 1f);
        var enemy = (EnemyBase)go.AddComponent(comp);

        var bar = new GameObject("HealthBar"); bar.transform.SetParent(go.transform); bar.transform.localPosition = new Vector3(0f, 0.8f, 0f);
        var fill = new GameObject("Fill"); fill.transform.SetParent(bar.transform); fill.transform.localPosition = Vector3.zero;
        var fsr = fill.AddComponent<SpriteRenderer>(); fsr.sprite = S("bar_white"); fsr.color = Color.red; fsr.sortingOrder = 11;
        fill.transform.localScale = new Vector3(1f, 0.15f, 1f);

        var hb = bar.AddComponent<EnemyHealthBar>();
        hb.enemy = enemy; hb.fill = fill.transform; hb.offset = new Vector3(0f, 0.8f, 0f);

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabDir + "/" + name + ".prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    static GameObject BuildPickupPrefab(string name, string sprite, PickupKind kind)
    {
        var go = new GameObject(name);
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = S(sprite); sr.sortingOrder = 5;
        var col = go.AddComponent<CircleCollider2D>(); col.isTrigger = true; col.radius = 0.5f;
        var p = go.AddComponent<Pickup>(); p.kind = kind; p.amount = 20f;
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabDir + "/" + name + ".prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    static GameObject BuildDoorPrefab()
    {
        var go = new GameObject("Door");
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = S("door"); sr.sortingOrder = 3;
        var col = go.AddComponent<BoxCollider2D>(); col.isTrigger = true; col.size = new Vector2(1.25f, 2f);
        go.AddComponent<LevelDoor>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabDir + "/Door.prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    // ---------- scene building blocks ----------

    static Camera CreateCamera(Color bg, Transform follow)
    {
        var go = new GameObject("Main Camera") { tag = "MainCamera" };
        var cam = go.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = bg;
        go.transform.position = new Vector3(0f, 0f, -10f);
        go.AddComponent<AudioListener>();
        var cf = go.AddComponent<CameraFollow>();
        cf.target = follow;

        // fondo que sigue la camara (siempre cubre la vista)
        var bgGo = new GameObject("Background");
        bgGo.transform.SetParent(go.transform);
        bgGo.transform.localPosition = new Vector3(0f, 0f, 20f);
        bgGo.transform.localScale = new Vector3(7f, 7f, 1f);
        var bsr = bgGo.AddComponent<SpriteRenderer>();
        bsr.sprite = S("bg_cave");
        bsr.sortingOrder = -100;

        return cam;
    }

    static GameObject CreatePlatform(Vector2 pos, Vector2 size)
    {
        var go = new GameObject("Platform") { layer = LayerMask.NameToLayer("Ground") };
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = S("tile_grass"); sr.sortingOrder = 1;
        go.transform.position = pos;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);
        go.AddComponent<BoxCollider2D>(); // hereda bounds del sprite (1u) * escala = size
        return go;
    }

    static GameObject Spawn(GameObject prefab, Vector3 pos)
    {
        var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        inst.transform.position = pos;
        return inst;
    }

    // ---------- scenes ----------

    static void BuildHub(GameObject player, GameObject door)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var pl = Spawn(player, new Vector3(0f, -2.2f, 0f));
        CreateCamera(new Color(0.30f, 0.30f, 0.40f), pl.transform);

        // base ancha
        CreatePlatform(new Vector2(0f, -3.5f), new Vector2(20f, 1f));

        // torre: 10 plataformas en zigzag, separadas para que el salto se sienta amplio
        for (int i = 0; i < 10; i++)
        {
            float y = -2.2f + (i + 1) * 1.9f;
            float x = (i % 2 == 0) ? -3.5f : 3.5f;
            CreatePlatform(new Vector2(x, y - 0.7f), new Vector2(5f, 0.5f));
            var d = Spawn(door, new Vector3(x, y + 0.35f, 0f));
            var ld = d.GetComponent<LevelDoor>();
            ld.mode = LevelDoor.Mode.EnterLevel;
            ld.floor = i + 1;
        }

        EditorSceneManager.SaveScene(scene, SceneDir + "/Hub.unity");
    }

    static void BuildLevel01(GameObject player, GameObject patrol, GameObject good, GameObject bad, GameObject door)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        const float startX = -75f;
        const float endX = 75f;

        var pl = Spawn(player, new Vector3(startX, -1.5f, 0f));
        CreateCamera(new Color(0.18f, 0.30f, 0.34f), pl.transform);
        var cortisol = pl.GetComponent<CortisolSystem>();

        // suelo muy largo
        CreatePlatform(new Vector2(0f, -3f), new Vector2(170f, 1f));

        // contenido repartido a lo ancho de forma deterministica
        for (int i = 0; i < 14; i++)
        {
            float x = startX + 6f + i * 10f;

            // plataforma flotante alternando altura
            float py = (i % 2 == 0) ? 0.6f : 1.6f;
            CreatePlatform(new Vector2(x + 2f, py), new Vector2(4f, 0.4f));

            // enemigo patrulla en el suelo
            Spawn(patrol, new Vector3(x, -2f, 0f));

            // objetos: bueno arriba en la plataforma, malo en el suelo
            Spawn(good, new Vector3(x + 2f, py + 0.8f, 0f));
            if (i % 2 == 0) Spawn(bad, new Vector3(x + 5f, -2f, 0f));
        }

        // puerta de salida al final
        var d = Spawn(door, new Vector3(endX, -2f, 0f));
        var ld = d.GetComponent<LevelDoor>();
        ld.mode = LevelDoor.Mode.ExitToHub;
        ld.floor = 1;

        // HUD: barra de cortisol
        BuildStressUI(cortisol);

        EditorSceneManager.SaveScene(scene, SceneDir + "/Level_01.unity");
    }

    static void BuildStressUI(CortisolSystem cortisol)
    {
        var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));

        // label
        var labelGo = new GameObject("Label", typeof(Text));
        labelGo.transform.SetParent(canvasGo.transform, false);
        var label = labelGo.GetComponent<Text>();
        label.text = "Cortisol:";
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 22; label.color = Color.white;
        var lrt = label.rectTransform;
        lrt.anchorMin = lrt.anchorMax = new Vector2(0f, 1f); lrt.pivot = new Vector2(0f, 1f);
        lrt.anchoredPosition = new Vector2(20f, -20f); lrt.sizeDelta = new Vector2(120f, 30f);

        // background bar
        var bgGo = new GameObject("BarBg", typeof(Image));
        bgGo.transform.SetParent(canvasGo.transform, false);
        bgGo.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
        var brt = ((Image)bgGo.GetComponent<Image>()).rectTransform;
        brt.anchorMin = brt.anchorMax = new Vector2(0f, 1f); brt.pivot = new Vector2(0f, 1f);
        brt.anchoredPosition = new Vector2(150f, -22f); brt.sizeDelta = new Vector2(220f, 24f);

        // fill
        var fillGo = new GameObject("Fill", typeof(Image));
        fillGo.transform.SetParent(bgGo.transform, false);
        var fill = fillGo.GetComponent<Image>();
        fill.color = new Color(0.8f, 0.1f, 0.1f, 1f);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        fill.fillAmount = 0f;
        var frt = fill.rectTransform;
        frt.anchorMin = Vector2.zero; frt.anchorMax = Vector2.one;
        frt.offsetMin = Vector2.zero; frt.offsetMax = Vector2.zero;

        var ui = canvasGo.AddComponent<StressBarUI>();
        ui.cortisol = cortisol;
        ui.fill = fill;
    }

    static void SetBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(SceneDir + "/Hub.unity", true),
            new EditorBuildSettingsScene(SceneDir + "/Level_01.unity", true),
        };
    }
}
