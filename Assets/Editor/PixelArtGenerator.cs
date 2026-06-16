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

        // Personaje (humanoide simple, 16x24). Frames idle/walk/attack.
        SaveSprite("player_idle.png",   Player(false, false));
        SaveSprite("player_walk.png",   Player(true,  false));
        SaveSprite("player_attack.png", Player(false, true));

        // Enemigos.
        SaveSprite("enemy_patrol.png",  Box(16, 16, new Color32(20, 20, 20, 255)));   // caja negra
        SaveSprite("enemy_chaser.png",  Circle(16, new Color32(200, 30, 30, 255)));    // circulo rojo

        // Objetos.
        SaveSprite("item_good.png",     Box(12, 16, new Color32(160, 200, 80, 255)));  // gameboy-ish
        SaveSprite("item_bad.png",      Box(10, 18, new Color32(80, 160, 220, 255)));  // celular-ish

        // Mundo.
        SaveSprite("tile_grass.png",    Box(16, 16, new Color32(70, 150, 70, 255)));
        SaveSprite("tile_dirt.png",     Box(16, 16, new Color32(90, 60, 50, 255)));
        SaveSprite("bg_forest.png",     Box(64, 36, new Color32(110, 170, 110, 255)));
        SaveSprite("door.png",          Box(20, 32, new Color32(120, 80, 40, 255)));

        AssetDatabase.Refresh();
        Debug.Log("Pixel art generado en " + Dir);
    }

    static Color32[] Player(bool stride, bool attack)
    {
        int w = 16, h = 24;
        var px = Fill(w, h, new Color32(0, 0, 0, 0));
        var skin = new Color32(220, 170, 120, 255);
        var shirt = new Color32(70, 120, 70, 255);
        var pants = new Color32(80, 60, 40, 255);
        Rect(px, w, 5, 16, 6, 4, skin);                  // cabeza
        Rect(px, w, 4, 9,  8, 7, shirt);                 // torso
        Rect(px, w, 4, 3,  3, 6, pants);                 // pierna izq
        Rect(px, w, 9, 3,  3, 6, pants);                 // pierna der
        if (stride) Rect(px, w, 9, 1, 3, 2, pants);      // paso
        if (attack) Rect(px, w, 12, 10, 4, 2, new Color32(200, 200, 210, 255)); // espada
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
            case "player_walk.png":
            case "player_attack.png": return (16, 24);
            case "item_good.png":     return (12, 16);
            case "item_bad.png":      return (10, 18);
            case "bg_forest.png":     return (64, 36);
            case "door.png":          return (20, 32);
            default:                  return (16, 16);
        }
    }
}
