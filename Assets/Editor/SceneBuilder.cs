using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using JuegoMental;
using JuegoMental.Core;

public static class SceneBuilder
{
    const string Art = "Assets/Art/Generated/";
    const string PrefabDir = "Assets/Prefabs";
    const string SceneDir = "Assets/Scenes";

    static Sprite _groundSprite;   // suelo del usuario o tile generado
    static bool _groundTiled;      // true si usamos sprite del usuario (drawMode Tiled)
    static Sprite _bgSprite;       // fondo del usuario o bosque generado
    static Sprite _wallSprite;     // pared del usuario o suelo de respaldo
    static bool _wallTiled;
    static Sprite _doorSprite;     // puerta del usuario o generada
    static Sprite _playerSprite;   // personaje del usuario (sprite unico) o null = rig generado
    static GameObject _playerBullet; // bala que dispara el jugador

    [MenuItem("Juego Mental/Build Scenes")]
    public static void BuildAll()
    {
        EnsureLayer("Ground");
        EnsureLayer("Player");
        EnsureLayer("Enemy");
        EnsureWhiteBar();
        AssetDatabase.Refresh();

        // arte del usuario (si existe) tiene prioridad
        _groundTiled = TryImportUser("suelo", true, out _groundSprite);
        if (!_groundTiled) _groundSprite = S("tile_grass");
        if (!TryImportUser("fondo", false, out _bgSprite)) _bgSprite = S("bg_forest");
        _wallTiled = TryImportUser("pared", true, out _wallSprite);
        if (!_wallTiled) _wallSprite = _groundSprite;
        if (!TryImportUser("puerta", false, out _doorSprite, 2.6f)) _doorSprite = S("door");
        _playerSprite = null; // usar el rig articulado generado (no la imagen del usuario)

        Directory.CreateDirectory(PrefabDir);
        Directory.CreateDirectory(SceneDir);

        _playerBullet = BuildPlayerBulletPrefab();
        var player = BuildPlayerPrefab();
        var projectile = BuildProjectilePrefab();
        var melee = BuildEnemyPrefab("PatrolEnemy", "enemy_patrol", typeof(PatrolEnemy));
        var ranged = BuildEnemyPrefab("ChaserEnemy", "enemy_chaser", typeof(ChaserEnemy));
        var ce = ranged.GetComponent<ChaserEnemy>();
        ce.projectilePrefab = projectile;
        PrefabUtility.SavePrefabAsset(ranged);
        var good = BuildPickupPrefab("PickupGood", "item_good", PickupKind.Good);
        var bad = BuildPickupPrefab("PickupBad", "item_bad", PickupKind.Bad);
        var door = BuildDoorPrefab();

        BuildMainMenu();
        BuildHub(player, door);
        for (int f = 1; f <= 10; f++)
            BuildLevel(f, player, melee, ranged, good, bad, door);

        SetBuildSettings();
        AssetDatabase.SaveAssets();
        Debug.Log("Escenas construidas: MainMenu + Hub + 10 niveles");
    }

    static GameObject BuildProjectilePrefab()
    {
        var go = new GameObject("Projectile");
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = S("projectile"); sr.sortingOrder = 9;
        var rb = go.AddComponent<Rigidbody2D>(); rb.bodyType = RigidbodyType2D.Kinematic;
        var col = go.AddComponent<CircleCollider2D>(); col.isTrigger = true; col.radius = 0.25f;
        go.AddComponent<EnemyProjectile>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabDir + "/Projectile.prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    static GameObject BuildPlayerBulletPrefab()
    {
        var go = new GameObject("PlayerBullet");
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = S("p_bullet"); sr.sortingOrder = 9;
        var rb = go.AddComponent<Rigidbody2D>(); rb.bodyType = RigidbodyType2D.Kinematic;
        var col = go.AddComponent<BoxCollider2D>(); col.isTrigger = true; col.size = new Vector2(0.4f, 0.25f);
        go.AddComponent<PlayerBullet>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabDir + "/PlayerBullet.prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    // ---------- helpers ----------

    static Sprite S(string n) => AssetDatabase.LoadAssetAtPath<Sprite>(Art + n + ".png");

    // carga un PNG/JPG del usuario en Assets/Art/<base>.* y lo importa como Sprite
    static bool TryImportUser(string baseName, bool tile, out Sprite sprite, float targetHeight = 0f)
    {
        sprite = null;
        string[] exts = { ".png", ".jpg", ".jpeg" };
        foreach (var e in exts)
        {
            string p = "Assets/Art/" + baseName + e;
            if (!File.Exists(p)) continue;
            var ti = AssetImporter.GetAtPath(p) as TextureImporter;
            if (ti == null) continue;
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.filterMode = FilterMode.Bilinear;
            if (tile)
            {
                ti.wrapMode = TextureWrapMode.Repeat;
                var s = new TextureImporterSettings();
                ti.ReadTextureSettings(s);
                s.spriteMeshType = SpriteMeshType.FullRect;
                ti.SetTextureSettings(s);
            }
            ti.SaveAndReimport();
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(p);
            if (tex != null)
            {
                if (tile) ti.spritePixelsPerUnit = tex.height;            // tile ~1 unidad de alto
                else if (targetHeight > 0f) ti.spritePixelsPerUnit = tex.height / targetHeight;
                ti.SaveAndReimport();
            }
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>(p);
            return sprite != null;
        }
        return false;
    }

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
        if (_playerSprite != null) return BuildSpritePlayer();
        return BuildRigPlayer();
    }

    // Jugador con la imagen del usuario (un solo sprite) + rebote al caminar.
    static GameObject BuildSpritePlayer()
    {
        var go = new GameObject("Player") { tag = "Player", layer = LayerMask.NameToLayer("Player") };
        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var col = go.AddComponent<CapsuleCollider2D>();
        col.size = new Vector2(1.0f, 2.0f);
        col.offset = new Vector2(-0.2f, -0.05f);

        var visual = new GameObject("Visual");
        visual.transform.SetParent(go.transform);
        visual.transform.localPosition = Vector3.zero;
        var sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = _playerSprite; sr.sortingOrder = 10;

        var gc = new GameObject("GroundCheck"); gc.transform.SetParent(go.transform); gc.transform.localPosition = new Vector3(-0.2f, -1.05f, 0f);
        var ao = new GameObject("AttackOrigin"); ao.transform.SetParent(go.transform); ao.transform.localPosition = new Vector3(0.8f, 0.1f, 0f);

        var pc = go.AddComponent<PlayerController2D>();
        pc.groundCheck = gc.transform; pc.groundMask = Mask("Ground");
        pc.moveSpeed = 6f; pc.jumpForce = 12f; pc.maxJumps = 2;

        var pa = go.AddComponent<PlayerAttack>();
        pa.attackOrigin = ao.transform; pa.bulletPrefab = _playerBullet;

        var bob = go.AddComponent<SpriteBob>();
        bob.visual = visual.transform;

        go.AddComponent<CortisolSystem>();

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabDir + "/Player.prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    static GameObject BuildRigPlayer()
    {
        var go = new GameObject("Player") { tag = "Player", layer = LayerMask.NameToLayer("Player") };
        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var col = go.AddComponent<CapsuleCollider2D>();
        col.size = new Vector2(0.85f, 2.0f);
        col.offset = new Vector2(0f, -0.3f);

        // miembros (detras del torso)
        MakeLimb("LegBack",  go.transform, new Vector3(0.12f, -0.5f, 0f), "p_leg", 8, out _);
        MakeLimb("ArmBack",  go.transform, new Vector3(0.22f, 0.06f, 0f), "p_arm", 8, out _);
        var legBack = go.transform.Find("LegBack");
        var armBack = go.transform.Find("ArmBack");

        // torso con cara
        var torsoGo = new GameObject("Torso");
        torsoGo.transform.SetParent(go.transform);
        torsoGo.transform.localPosition = Vector3.zero;
        var tsr = torsoGo.AddComponent<SpriteRenderer>();
        tsr.sprite = S("p_torso"); tsr.sortingOrder = 10;

        // miembros frontales (delante del torso)
        var legFront = MakeLimb("LegFront", go.transform, new Vector3(-0.12f, -0.5f, 0f), "p_leg", 12, out _);
        Transform armFrontSprite;
        var armFront = MakeLimb("ArmFront", go.transform, new Vector3(-0.22f, 0.06f, 0f), "p_arm", 12, out armFrontSprite);

        // pistola siempre visible, sostenida al frente (apunta hacia donde mira)
        var gunGo = new GameObject("Gun");
        gunGo.transform.SetParent(go.transform);
        gunGo.transform.localPosition = new Vector3(0.45f, 0.05f, 0f);
        var gsr = gunGo.AddComponent<SpriteRenderer>();
        gsr.sprite = S("p_gun"); gsr.sortingOrder = 13;

        var gc = new GameObject("GroundCheck"); gc.transform.SetParent(go.transform); gc.transform.localPosition = new Vector3(0f, -1.2f, 0f);
        var ao = new GameObject("AttackOrigin"); ao.transform.SetParent(go.transform); ao.transform.localPosition = new Vector3(0.95f, 0.05f, 0f);

        var pc = go.AddComponent<PlayerController2D>();
        pc.groundCheck = gc.transform;
        pc.groundMask = Mask("Ground");
        pc.moveSpeed = 6f;
        pc.jumpForce = 12f;
        pc.maxJumps = 2;

        var anim = go.AddComponent<LimbAnimator>();
        anim.armFront = armFront; anim.armBack = armBack;
        anim.legFront = legFront; anim.legBack = legBack;
        anim.sword = null; // sin objeto que ocultar; la pistola va siempre visible

        var pa = go.AddComponent<PlayerAttack>();
        pa.attackOrigin = ao.transform;
        pa.bulletPrefab = _playerBullet;

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
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = _doorSprite; sr.sortingOrder = 3;
        var col = go.AddComponent<BoxCollider2D>(); col.isTrigger = true;
        col.size = _doorSprite.bounds.size; // ajusta al tamano de la puerta
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
        var bsr = bgGo.AddComponent<SpriteRenderer>();
        bsr.sprite = _bgSprite;
        bsr.sortingOrder = -100;
        // escala uniforme para cubrir la vista de la camara conservando proporcion
        Vector2 b = _bgSprite.bounds.size;
        float sc = Mathf.Max(21.5f / b.x, 12.5f / b.y) * 1.05f;
        bgGo.transform.localScale = new Vector3(sc, sc, 1f);

        return cam;
    }

    static GameObject CreatePlatform(Vector2 pos, Vector2 size)
    {
        var go = new GameObject("Platform") { layer = LayerMask.NameToLayer("Ground") };
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = _groundSprite; sr.sortingOrder = 1;
        go.transform.position = pos;

        if (_groundTiled)
        {
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = size;                       // repite la textura del usuario
            var c = go.AddComponent<BoxCollider2D>(); c.size = size;
        }
        else
        {
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            go.AddComponent<BoxCollider2D>();     // bounds del sprite (1u) * escala
        }
        return go;
    }

    static GameObject CreateWall(Vector2 pos, Vector2 size)
    {
        var go = new GameObject("Wall") { layer = LayerMask.NameToLayer("Ground") };
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = _wallSprite; sr.sortingOrder = 2;
        go.transform.position = pos;
        if (_wallTiled)
        {
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.size = size;
            var c = go.AddComponent<BoxCollider2D>(); c.size = size;
        }
        else
        {
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            go.AddComponent<BoxCollider2D>();
        }
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

        var pl = Spawn(player, new Vector3(0f, -1.7f, 0f));
        CreateCamera(new Color(0.30f, 0.30f, 0.40f), pl.transform);

        // base ancha
        CreatePlatform(new Vector2(0f, -3.5f), new Vector2(20f, 1f));

        // muros laterales del hub para no caerse de la base
        CreateWall(new Vector2(-10f, 6f), new Vector2(1f, 24f));
        CreateWall(new Vector2(10f, 6f), new Vector2(1f, 24f));

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

            // numero del nivel sobre la puerta
            var lblGo = new GameObject("DoorLabel");
            lblGo.transform.position = new Vector3(x, y + 1.9f, 0f);
            var tm = lblGo.AddComponent<TextMesh>();
            tm.text = (i + 1).ToString();
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.fontSize = 64; tm.characterSize = 0.18f; tm.color = Color.white;
            var f = UIFont();
            tm.font = f;
            var mr = lblGo.GetComponent<MeshRenderer>();
            mr.sharedMaterial = f.material;
            mr.sortingOrder = 20;
        }

        EditorSceneManager.SaveScene(scene, SceneDir + "/Hub.unity");
    }

    static void BuildLevel(int floor, GameObject player, GameObject melee, GameObject ranged,
                           GameObject good, GameObject bad, GameObject door)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        const float startX = -75f;
        const float endX = 75f;

        var pl = Spawn(player, new Vector3(startX, -1.2f, 0f));
        // tono de fondo levemente mas oscuro segun el piso
        float t = (floor - 1) / 9f;
        CreateCamera(new Color(0.20f - 0.06f * t, 0.32f - 0.10f * t, 0.36f - 0.10f * t), pl.transform);
        var cortisol = pl.GetComponent<CortisolSystem>();

        // suelo muy largo + muros
        CreatePlatform(new Vector2(0f, -3f), new Vector2(170f, 1f));
        CreateWall(new Vector2(startX - 1.5f, 1.5f), new Vector2(1.5f, 16f));
        CreateWall(new Vector2(endX + 1.5f, 1.5f), new Vector2(1.5f, 16f));

        // dificultad escalada por piso
        int meleeCount = Mathf.Clamp(3 + floor, 3, 14);
        int rangedCount = floor >= 3 ? Mathf.Clamp(floor - 2, 0, 7) : 0;

        for (int i = 0; i < 14; i++)
        {
            float x = startX + 6f + i * 10f;
            float py = (i % 2 == 0) ? 0.6f : 1.6f;
            CreatePlatform(new Vector2(x + 2f, py), new Vector2(4f, 0.4f));

            if (i < meleeCount) Spawn(melee, new Vector3(x, -2f, 0f));

            Spawn(good, new Vector3(x + 2f, py + 0.8f, 0f));
            if (i % 2 == 0) Spawn(bad, new Vector3(x + 5f, -2f, 0f));
        }

        // enemigos a distancia sobre plataformas (pisos altos)
        for (int j = 0; j < rangedCount; j++)
        {
            float rx = startX + 16f + j * (130f / Mathf.Max(1, rangedCount));
            Spawn(ranged, new Vector3(rx, 1.8f, 0f));
        }

        // puerta de salida al final
        var d = Spawn(door, new Vector3(endX, -1.7f, 0f));
        var ld = d.GetComponent<LevelDoor>();
        ld.mode = LevelDoor.Mode.ExitToHub;
        ld.floor = floor;

        BuildStressUI(cortisol);
        BuildGameOver(cortisol);

        EditorSceneManager.SaveScene(scene, SceneDir + $"/Level_{floor:00}.unity");
    }

    // ---------- helpers UI ----------

    static Font UIFont() => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

    static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
    }

    static GameObject NewCanvas(string name)
    {
        var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        EnsureEventSystem();
        return go;
    }

    static Text CreateText(Transform parent, string content, int size, Vector2 pos, Vector2 sizeDelta, TextAnchor anchor = TextAnchor.MiddleCenter)
    {
        var go = new GameObject("Text", typeof(Text));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.text = content; t.font = UIFont(); t.fontSize = size; t.color = Color.white; t.alignment = anchor;
        var rt = t.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = sizeDelta;
        return t;
    }

    static Button CreateButton(Transform parent, string label, Vector2 pos, Vector2 size, UnityAction action)
    {
        var go = new GameObject("Button", typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color(0.22f, 0.52f, 0.36f, 1f);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        var label2 = CreateText(go.transform, label, 26, Vector2.zero, size);
        label2.rectTransform.anchorMin = Vector2.zero; label2.rectTransform.anchorMax = Vector2.one;
        label2.rectTransform.offsetMin = Vector2.zero; label2.rectTransform.offsetMax = Vector2.zero;
        var btn = go.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(btn.onClick, action);
        return btn;
    }

    static void BuildMainMenu()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true; cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.10f, 0.13f, 0.20f);
        camGo.transform.position = new Vector3(0f, 0f, -10f);
        camGo.AddComponent<AudioListener>();

        var ui = new GameObject("UIActions").AddComponent<UIActions>();
        var canvas = NewCanvas("Canvas");

        CreateText(canvas.transform, "Bienvenido a Mundo Mental", 44, new Vector2(0f, 140f), new Vector2(900f, 120f));
        CreateText(canvas.transform, "Sube la torre sin que el cortisol te gane", 22, new Vector2(0f, 60f), new Vector2(800f, 50f));
        CreateButton(canvas.transform, "Jugar", new Vector2(0f, -40f), new Vector2(260f, 80f), ui.PlayGame);

        EditorSceneManager.SaveScene(scene, SceneDir + "/MainMenu.unity");
    }

    static void BuildGameOver(CortisolSystem cortisol)
    {
        var ui = new GameObject("UIActions").AddComponent<UIActions>();
        var canvas = NewCanvas("GameOverCanvas");

        var panel = new GameObject("Panel", typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
        var prt = panel.GetComponent<RectTransform>();
        prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one; prt.offsetMin = Vector2.zero; prt.offsetMax = Vector2.zero;

        CreateText(panel.transform, "Has perdido", 48, new Vector2(0f, 90f), new Vector2(700f, 120f));
        CreateButton(panel.transform, "Intentar de nuevo", new Vector2(0f, 0f), new Vector2(320f, 70f), ui.RestartLevel);
        CreateButton(panel.transform, "Salir", new Vector2(0f, -90f), new Vector2(320f, 70f), ui.QuitToMenu);

        var go = canvas.AddComponent<GameOverUI>();
        go.cortisol = cortisol;
        go.panel = panel;
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

        // texto de porcentaje a la derecha de la barra
        var pctGo = new GameObject("Percent", typeof(Text));
        pctGo.transform.SetParent(canvasGo.transform, false);
        var pct = pctGo.GetComponent<Text>();
        pct.text = "0%"; pct.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        pct.fontSize = 20; pct.color = Color.white; pct.alignment = TextAnchor.MiddleLeft;
        var pr = pct.rectTransform;
        pr.anchorMin = pr.anchorMax = new Vector2(0f, 1f); pr.pivot = new Vector2(0f, 1f);
        pr.anchoredPosition = new Vector2(378f, -18f); pr.sizeDelta = new Vector2(80f, 28f);

        var ui = canvasGo.AddComponent<StressBarUI>();
        ui.cortisol = cortisol;
        ui.fill = fill;
        ui.percent = pct;
    }

    static void SetBuildSettings()
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene(SceneDir + "/MainMenu.unity", true),
            new EditorBuildSettingsScene(SceneDir + "/Hub.unity", true)
        };
        for (int f = 1; f <= 10; f++)
            scenes.Add(new EditorBuildSettingsScene(SceneDir + $"/Level_{f:00}.unity", true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
