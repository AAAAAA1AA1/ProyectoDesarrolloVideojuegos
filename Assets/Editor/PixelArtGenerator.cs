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
        SaveSprite("p_arm.png",   Limb(3, 8, new Color32(70, 120, 70, 255), new Color32(220, 170, 120, 255)));
        SaveSprite("p_leg.png",   Limb(4, 9, new Color32(80, 60, 40, 255), new Color32(50, 40, 30, 255)));
        SaveSprite("p_sword.png", Sword());

        // Fondo tipo cueva (aproximacion al estilo enviado).
        SaveSprite("bg_cave.png", CaveBg());

        // Enemigos.
        SaveSprite("enemy_patrol.png",  Box(16, 16, new Color32(20, 20, 20, 255)));   // caja negra
        SaveSprite("enemy_chaser.png",  Circle(16, new Color32(200, 30, 30, 255)));    // circulo rojo

        // Objetos.
        SaveSprite("item_good.png",     Box(12, 16, new Color32(160, 200, 80, 255)));  // gameboy-ish
        SaveSprite("item_bad.png",      Box(10, 18, new Color32(80, 160, 220, 255)));  // celular-ish

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

    // Torso con cabeza y cara (12x20). Cabeza mas arriba. Sin brazos ni piernas.
    static Color32[] Torso()
    {
        int w = 12, h = 20;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        var skin = new Color32(220, 170, 120, 255);
        var hair = new Color32(90, 60, 30, 255);
        var shirt = new Color32(70, 120, 70, 255);
        var eyeB = new Color32(30, 30, 40, 255);
        var mouth = new Color32(150, 70, 70, 255);

        Rect(px, w, 2, 1, 8, 12, shirt);  // torso (cuerpo)
        Rect(px, w, 5, 12, 2, 2, skin);   // cuello
        Rect(px, w, 3, 13, 6, 5, skin);   // cabeza (mas arriba)
        Rect(px, w, 3, 17, 6, 2, hair);   // pelo
        Rect(px, w, 4, 15, 1, 1, eyeB);   // ojo izq
        Rect(px, w, 7, 15, 1, 1, eyeB);   // ojo der
        Rect(px, w, 5, 13, 2, 1, mouth);  // boca
        return px;
    }

    // Miembro vertical: parte alta de tela, parte baja "mano/pie".
    static Color32[] Limb(int w, int h, Color32 top, Color32 tip)
    {
        var px = Fill(w, h, top);
        for (int y = 0; y < 2; y++)
        for (int x = 0; x < w; x++)
            px[y * w + x] = tip;
        return px;
    }

    static Color32[] Sword()
    {
        int w = 12, h = 4;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        Rect(px, w, 0, 1, 3, 2, new Color32(110, 80, 50, 255));   // mango
        Rect(px, w, 3, 1, 9, 2, new Color32(210, 210, 225, 255)); // hoja
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
            case "item_good.png":     return (12, 16);
            case "item_bad.png":      return (10, 18);
            case "bg_forest.png":     return (96, 54);
            case "bg_cave.png":       return (64, 36);
            case "p_torso.png":       return (12, 20);
            case "p_arm.png":         return (3, 8);
            case "p_leg.png":         return (4, 9);
            case "p_sword.png":       return (12, 4);
            case "door.png":          return (20, 32);
            default:                  return (16, 16);
        }
    }
}
