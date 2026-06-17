using UnityEngine;
using UnityEditor;
using System.IO;

public static class PixelArtGenerator
{
    const string Dir = "Assets/Art/Generated";

    [MenuItem("Juego Mental/Generate Pixel Art")]
    public static void Generate()
    {
        Directory.CreateDirectory(Dir);

        // Personaje (humanoide simple, 16x24). Frames idle / caminar A-B / ataque.
        SaveSprite("player_idle.png",     Player(0, false));
        SaveSprite("player_walk_a.png",   Player(1, false));
        SaveSprite("player_walk_b.png",   Player(2, false));
        SaveSprite("player_attack.png",   Player(0, true));

        // Partes articuladas (personaje con huesos: torso+cabeza, brazo, pierna, espada).
        SaveSprite("p_torso.png", Torso());
        SaveSprite("p_arm.png",   ArmSprite());
        SaveSprite("p_leg.png",   LegSprite());
        SaveSprite("p_gun.png",   Gun());
        SaveSprite("p_bullet.png", Bullet());

        // Fondo tipo cueva (aproximacion al estilo enviado).
        SaveSprite("bg_cave.png", CaveBg());

        // Enemigos con diseno.
        SaveSprite("enemy_patrol.png",  EnemyMelee());    // baboso melee
        SaveSprite("enemy_chaser.png",  EnemyRanged());   // volador a distancia
        SaveSprite("projectile.png",    Projectile());    // proyectil de estres

        // Objetos con diseno.
        SaveSprite("item_good.png",     ItemGood());       // gameboy (relax)
        SaveSprite("item_bad.png",      ItemBad());        // celular (estres)

        // Mundo.
        SaveSprite("tile_grass.png",    Box(16, 16, new Color32(70, 150, 70, 255)));
        SaveSprite("tile_dirt.png",     Box(16, 16, new Color32(90, 60, 50, 255)));
        SaveSprite("bg_forest.png",     ForestBg());
        SaveSprite("door.png",          Box(20, 32, new Color32(120, 80, 40, 255)));

        AssetDatabase.Refresh();
        Debug.Log("Pixel art generado en " + Dir);
    }

    // legPose: 0 = quieto, 1 = paso A (pierna der adelante), 2 = paso B (pierna izq adelante)
    static Color32[] Player(int legPose, bool attack)
    {
        int w = 16, h = 24;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        var skin = new Color32(220, 170, 120, 255);
        var hair = new Color32(90, 60, 30, 255);
        var shirt = new Color32(70, 120, 70, 255);
        var pants = new Color32(80, 60, 40, 255);
        var eyeW = new Color32(245, 245, 245, 255);
        var eyeB = new Color32(30, 30, 40, 255);
        var mouth = new Color32(150, 70, 70, 255);

        Rect(px, w, 5, 15, 6, 5, skin);                  // cabeza
        Rect(px, w, 5, 19, 6, 2, hair);                  // pelo
        // cara
        Rect(px, w, 6, 17, 1, 1, eyeW); Rect(px, w, 6, 17, 1, 1, eyeB); // ojo izq
        Rect(px, w, 9, 17, 1, 1, eyeB);                  // ojo der
        Rect(px, w, 7, 15, 2, 1, mouth);                 // boca

        Rect(px, w, 4, 9, 8, 6, shirt);                  // torso

        // piernas segun pose
        if (legPose == 1)
        {
            Rect(px, w, 4, 3, 3, 6, pants);              // izq atras
            Rect(px, w, 9, 4, 3, 5, pants);              // der adelante (levantada)
        }
        else if (legPose == 2)
        {
            Rect(px, w, 4, 4, 3, 5, pants);              // izq adelante
            Rect(px, w, 9, 3, 3, 6, pants);              // der atras
        }
        else
        {
            Rect(px, w, 4, 3, 3, 6, pants);
            Rect(px, w, 9, 3, 3, 6, pants);
        }

        if (attack) Rect(px, w, 12, 10, 4, 2, new Color32(200, 200, 210, 255)); // espada
        return px;
    }

    static readonly Color32 OL = new Color32(28, 22, 34, 255);   // contorno
    static readonly Color32 Skin = new Color32(238, 194, 150, 255);
    static readonly Color32 SkinSh = new Color32(205, 160, 120, 255);
    static readonly Color32 Hair = new Color32(120, 76, 40, 255);
    static readonly Color32 HairSh = new Color32(88, 54, 28, 255);
    static readonly Color32 Shirt = new Color32(64, 150, 98, 255);
    static readonly Color32 ShirtSh = new Color32(44, 110, 70, 255);
    static readonly Color32 Pants = new Color32(82, 62, 42, 255);
    static readonly Color32 PantsSh = new Color32(60, 44, 30, 255);
    static readonly Color32 Boot = new Color32(44, 34, 30, 255);

    // anade contorno oscuro alrededor de la silueta (pixeles transparentes vecinos)
    static void Outline(Color32[] px, int w, int h)
    {
        var copy = (Color32[])px.Clone();
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            if (copy[y * w + x].a != 0) continue;
            bool near = false;
            for (int dy = -1; dy <= 1 && !near; dy++)
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx, ny = y + dy;
                if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                if (copy[ny * w + nx].a != 0) { near = true; break; }
            }
            if (near) px[y * w + x] = OL;
        }
    }

    // Torso con cabeza y cara bonita (14x24). Sin brazos ni piernas.
    static Color32[] Torso()
    {
        int w = 14, h = 24;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        var eyeW = new Color32(250, 250, 252, 255);
        var pupil = new Color32(50, 55, 80, 255);
        var mouth = new Color32(180, 90, 90, 255);
        var belt = new Color32(95, 62, 38, 255);

        // torso con sombreado
        Rect(px, w, 3, 3, 8, 11, Shirt);
        Rect(px, w, 3, 3, 2, 11, ShirtSh);   // sombra lateral
        Rect(px, w, 3, 3, 8, 2, belt);       // cinturon
        // cuello
        Rect(px, w, 6, 14, 2, 2, Skin);
        // cabeza
        Rect(px, w, 4, 16, 6, 6, Skin);
        Rect(px, w, 9, 16, 1, 6, SkinSh);    // sombra mejilla
        // pelo
        Rect(px, w, 4, 21, 6, 2, Hair);
        Rect(px, w, 3, 18, 1, 4, Hair);
        Rect(px, w, 10, 18, 1, 4, HairSh);
        // cara
        Rect(px, w, 5, 18, 1, 2, eyeW); Rect(px, w, 5, 18, 1, 1, pupil); // ojo izq
        Rect(px, w, 8, 18, 1, 2, eyeW); Rect(px, w, 8, 18, 1, 1, pupil); // ojo der
        Rect(px, w, 6, 17, 2, 1, mouth);                                 // sonrisa

        Outline(px, w, h);
        return px;
    }

    static Color32[] ArmSprite()
    {
        int w = 5, h = 11;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        Rect(px, w, 1, 3, 3, 8, Shirt);    // manga
        Rect(px, w, 1, 3, 1, 8, ShirtSh);  // sombra
        Rect(px, w, 1, 1, 3, 2, Skin);     // mano
        Outline(px, w, h);
        return px;
    }

    static Color32[] LegSprite()
    {
        int w = 6, h = 12;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        Rect(px, w, 1, 3, 4, 9, Pants);    // pantalon
        Rect(px, w, 1, 3, 1, 9, PantsSh);  // sombra
        Rect(px, w, 1, 1, 4, 3, Boot);     // bota
        Outline(px, w, h);
        return px;
    }

    static Color32[] Gun()
    {
        int w = 14, h = 7;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        var metal = new Color32(95, 100, 112, 255);
        var metalSh = new Color32(60, 65, 78, 255);
        var grip = new Color32(75, 52, 40, 255);
        Rect(px, w, 1, 3, 11, 3, metal);     // cuerpo/canon
        Rect(px, w, 1, 3, 11, 1, metalSh);   // sombra inferior
        Rect(px, w, 2, 0, 3, 3, grip);       // empunadura
        Rect(px, w, 12, 4, 2, 2, metal);     // boca del canon
        Outline(px, w, h);
        return px;
    }

    static Color32[] Bullet()
    {
        int w = 6, h = 4;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        Rect(px, w, 0, 1, 4, 2, new Color32(235, 195, 70, 255));  // cuerpo
        Rect(px, w, 4, 1, 2, 2, new Color32(205, 120, 40, 255));  // punta
        Outline(px, w, h);
        return px;
    }

    // Enemigo melee: baboso verde con ojos enojados (18x16).
    static Color32[] EnemyMelee()
    {
        int w = 18, h = 16;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        var body = new Color32(96, 178, 96, 255);
        var bodySh = new Color32(62, 132, 66, 255);
        var eyeW = new Color32(250, 250, 250, 255);
        var pupil = new Color32(40, 40, 50, 255);
        var mouth = new Color32(40, 30, 30, 255);

        Rect(px, w, 2, 1, 14, 3, body);   // base ancha
        Rect(px, w, 3, 4, 12, 5, body);
        Rect(px, w, 5, 9, 8, 3, body);    // cupula
        Rect(px, w, 2, 1, 3, 10, bodySh); // sombra lateral izq
        // ojos
        Rect(px, w, 6, 6, 2, 2, eyeW); Rect(px, w, 6, 6, 1, 1, pupil);
        Rect(px, w, 10, 6, 2, 2, eyeW); Rect(px, w, 11, 6, 1, 1, pupil);
        // cejas enojadas
        Rect(px, w, 6, 8, 2, 1, mouth); Rect(px, w, 10, 8, 2, 1, mouth);
        // boca
        Rect(px, w, 7, 3, 4, 1, mouth);
        Outline(px, w, h);
        return px;
    }

    // Enemigo a distancia: ojo volador morado con alas (18x16).
    static Color32[] EnemyRanged()
    {
        int w = 18, h = 16;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        var body = new Color32(150, 90, 190, 255);
        var bodySh = new Color32(110, 60, 150, 255);
        var wing = new Color32(90, 50, 130, 255);
        var eyeW = new Color32(250, 250, 250, 255);
        var pupil = new Color32(180, 40, 60, 255);

        // alas
        Rect(px, w, 0, 7, 4, 4, wing); Rect(px, w, 14, 7, 4, 4, wing);
        // cuerpo
        Rect(px, w, 5, 4, 8, 8, body);
        Rect(px, w, 6, 11, 6, 1, body);
        Rect(px, w, 5, 4, 2, 8, bodySh);
        // ojo grande
        Rect(px, w, 7, 6, 4, 4, eyeW); Rect(px, w, 8, 7, 2, 2, pupil);
        Outline(px, w, h);
        return px;
    }

    // Proyectil de estres (8x8): orbe rojo-morado.
    static Color32[] Projectile()
    {
        int d = 8;
        var px = Circle(d, new Color32(200, 60, 80, 255));
        // nucleo mas oscuro
        Rect(px, d, 3, 3, 2, 2, new Color32(120, 30, 50, 255));
        Outline(px, d, d);
        return px;
    }

    // Item bueno: gameboy verde (relax) (14x18).
    static Color32[] ItemGood()
    {
        int w = 14, h = 18;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        var shell = new Color32(170, 205, 120, 255);
        var shellSh = new Color32(130, 165, 90, 255);
        var screen = new Color32(120, 170, 110, 255);
        var btn = new Color32(120, 70, 90, 255);

        Rect(px, w, 2, 1, 10, 16, shell);
        Rect(px, w, 2, 1, 2, 16, shellSh);
        Rect(px, w, 4, 9, 6, 6, screen);   // pantalla
        Rect(px, w, 4, 4, 2, 2, btn); Rect(px, w, 8, 4, 2, 2, btn); // botones
        Outline(px, w, h);
        return px;
    }

    // Item malo: celular con notificacion roja (12x18).
    static Color32[] ItemBad()
    {
        int w = 12, h = 18;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        var shell = new Color32(70, 80, 95, 255);
        var shellSh = new Color32(48, 56, 70, 255);
        var screen = new Color32(150, 200, 230, 255);
        var notif = new Color32(220, 60, 60, 255);

        Rect(px, w, 2, 1, 8, 16, shell);
        Rect(px, w, 2, 1, 2, 16, shellSh);
        Rect(px, w, 3, 3, 6, 11, screen);  // pantalla
        Rect(px, w, 7, 13, 2, 2, notif);   // notificacion
        Outline(px, w, h);
        return px;
    }

    // Bosque metroidvania: troncos oscuros + claros de luz azul-verde + suelo.
    static Color32[] ForestBg()
    {
        int w = 96, h = 54;
        var px = Fill(w, h, new Color32(26, 56, 46, 255)); // verde muy oscuro
        // claros de luz (bandas verticales azul-verde)
        int[] gaps = { 14, 40, 66, 86 };
        foreach (int gx in gaps)
        {
            Rect(px, w, gx, 12, 7, 40, new Color32(90, 165, 160, 255));
            Rect(px, w, gx + 1, 12, 5, 40, new Color32(120, 200, 195, 255));
        }
        // troncos de arbol oscuros
        int[] trunks = { 4, 26, 52, 76 };
        foreach (int tx in trunks)
            Rect(px, w, tx, 10, 6, 44, new Color32(18, 40, 34, 255));
        // copa/canopy arriba
        Rect(px, w, 0, 46, w, 8, new Color32(22, 50, 38, 255));
        // pasto y tierra
        Rect(px, w, 0, 8, w, 4, new Color32(70, 150, 70, 255));
        Rect(px, w, 0, 0, w, 8, new Color32(115, 72, 45, 255));
        return px;
    }

    static Color32[] CaveBg()
    {
        int w = 64, h = 36;
        var px = Fill(w, h, new Color32(38, 66, 78, 255)); // teal oscuro
        // resplandor central mas claro
        Rect(px, w, 22, 14, 20, 18, new Color32(70, 105, 115, 255));
        Rect(px, w, 27, 16, 10, 16, new Color32(95, 135, 145, 255));
        // pilares de roca
        Rect(px, w, 6, 9, 6, 24, new Color32(120, 140, 150, 255));
        Rect(px, w, 29, 9, 6, 22, new Color32(120, 140, 150, 255));
        Rect(px, w, 52, 9, 6, 24, new Color32(120, 140, 150, 255));
        // pasto arriba del suelo
        Rect(px, w, 0, 7, w, 3, new Color32(70, 150, 70, 255));
        // tierra
        Rect(px, w, 0, 0, w, 7, new Color32(115, 72, 45, 255));
        return px;
    }

    static Color32[] Box(int w, int h, Color32 c) => Fill(w, h, c);

    static Color32[] Circle(int d, Color32 c)
    {
        var px = Fill(d, d, new Color32(0, 0, 0, 0));
        float r = d / 2f;
        for (int y = 0; y < d; y++)
        for (int x = 0; x < d; x++)
            if ((x - r + 0.5f) * (x - r + 0.5f) + (y - r + 0.5f) * (y - r + 0.5f) <= r * r)
                px[y * d + x] = c;
        return px;
    }

    static Color32[] Fill(int w, int h, Color32 c)
    {
        var px = new Color32[w * h];
        for (int i = 0; i < px.Length; i++) px[i] = c;
        return px;
    }

    static void Rect(Color32[] px, int w, int x0, int y0, int rw, int rh, Color32 c)
    {
        for (int y = y0; y < y0 + rh; y++)
        for (int x = x0; x < x0 + rw; x++)
            if (x >= 0 && x < w && y >= 0 && y * w + x < px.Length) px[y * w + x] = c;
    }

    static void SaveSprite(string name, Color32[] px)
    {
        (int w, int h) = DimsFor(name);
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.SetPixels32(px);
        tex.Apply();
        var path = Path.Combine(Dir, name);
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);
        var ti = (TextureImporter)AssetImporter.GetAtPath(path);
        ti.textureType = TextureImporterType.Sprite;
        ti.filterMode = FilterMode.Point;
        ti.spritePixelsPerUnit = 16;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.SaveAndReimport();
    }

    static (int, int) DimsFor(string name)
    {
        switch (name)
        {
            case "player_idle.png":
            case "player_walk_a.png":
            case "player_walk_b.png":
            case "player_attack.png": return (16, 24);
            case "item_good.png":     return (14, 18);
            case "item_bad.png":      return (12, 18);
            case "enemy_patrol.png":
            case "enemy_chaser.png":  return (18, 16);
            case "projectile.png":    return (8, 8);
            case "bg_forest.png":     return (96, 54);
            case "bg_cave.png":       return (64, 36);
            case "p_torso.png":       return (14, 24);
            case "p_arm.png":         return (5, 11);
            case "p_leg.png":         return (6, 12);
            case "p_gun.png":         return (14, 7);
            case "p_bullet.png":      return (6, 4);
            case "door.png":          return (20, 32);
            default:                  return (16, 16);
        }
    }
}
