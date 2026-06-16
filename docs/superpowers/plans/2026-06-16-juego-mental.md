# Juego Mental Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Construir el vertical slice de "Juego Mental": un plataformero 2D pixel-art con torre-hub, Level_01 jugable, sistema de cortisol, objetos buenos/malos, enemigos (patrulla/perseguidor) con vida, espada del jugador, y el MCP de Unity instalado.

**Architecture:** La lógica de juego pura (cortisol, pickup, vida de enemigo) vive en clases C# POCO sin dependencia de `UnityEngine`, envueltas por MonoBehaviours delgados. Esto permite TDD con tests EditMode corridos por CLI. El arte se genera por código (editor script que pinta `Texture2D`). El proyecto Unity se crea y configura por archivos (manifest.json, EditorSettings) para que sea determinista. El armado de escenas se hace al final vía MCP o pasos manuales guiados.

**Tech Stack:** Unity 6000.0.77f1 (LTS), C#, plantilla 2D (Built-in), Unity Test Framework (EditMode), Unity MCP (CoplayDev) + `uv`.

---

## Convenciones de rutas y comandos

- Raíz del proyecto / repo: `C:\Users\ricra\JuegoMental` (la carpeta Unity y el repo git son la misma).
- Editor Unity: `C:\Program Files\Unity\Hub\Editor\6000.0.77f1\Editor\Unity.exe`
- Shell: PowerShell. Para llamar al ejecutable con espacios usar el operador `&`.
- Comando estándar para correr tests EditMode (se reutiliza en varias tareas):

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.0.77f1\Editor\Unity.exe" `
  -runTests -batchmode `
  -projectPath "C:\Users\ricra\JuegoMental" `
  -testPlatform EditMode `
  -testResults "C:\Users\ricra\JuegoMental\TestResults.xml" `
  -logFile "C:\Users\ricra\JuegoMental\test-run.log"
```

Tras correr, leer resultado:

```powershell
Select-String -Path "C:\Users\ricra\JuegoMental\TestResults.xml" -Pattern 'result="(Passed|Failed)"' | Select-Object -First 5
```

> Nota: cada corrida arranca Unity en batch (lento, ~30–90s). Es esperado.

---

## File Structure

- `.gitignore` — ignora artefactos Unity (Library/, Temp/, etc.).
- `Packages/manifest.json` — paquetes 2D requeridos.
- `ProjectSettings/EditorSettings.asset` — modo de comportamiento por defecto 2D.
- `Assets/Scripts/Core/` — lógica pura POCO:
  - `CortisolModel.cs`, `PickupKind.cs`, `PickupModel.cs`, `EnemyHealth.cs`
- `Assets/Scripts/Runtime/` — MonoBehaviours:
  - `PlayerController2D.cs`, `PlayerAttack.cs`, `CortisolSystem.cs`, `StressBarUI.cs`,
    `Pickup.cs`, `EnemyBase.cs`, `PatrolEnemy.cs`, `ChaserEnemy.cs`,
    `EnemyHealthBar.cs`, `LevelDoor.cs`, `GameManager.cs`
- `Assets/Scripts/Config/LevelConfig.cs` — ScriptableObject por nivel.
- `Assets/Scripts/JuegoMental.Runtime.asmdef` — assembly del juego.
- `Assets/Editor/PixelArtGenerator.cs` — genera sprites PNG.
- `Assets/Editor/JuegoMental.Editor.asmdef` — assembly de editor.
- `Assets/Tests/JuegoMental.Tests.asmdef` + tests EditMode.
- `Assets/Art/Generated/` — PNG generados.
- `Assets/Scenes/Hub.unity`, `Assets/Scenes/Level_01.unity`.
- `Assets/Prefabs/` — prefabs de jugador, enemigos, objetos, puerta.

---

## Task 0: Skeleton del proyecto Unity + .gitignore

**Files:**
- Create: `C:\Users\ricra\JuegoMental\.gitignore`
- Create (vía CLI): estructura `Assets/`, `Packages/`, `ProjectSettings/`

- [ ] **Step 1: Crear el proyecto Unity vacío por CLI**

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.0.77f1\Editor\Unity.exe" `
  -batchmode -quit -createProject "C:\Users\ricra\JuegoMental" `
  -logFile "C:\Users\ricra\JuegoMental\create.log"
```

Esperado: aparecen carpetas `Assets/`, `Packages/`, `ProjectSettings/`. (El repo git y `docs/` ya existen; conviven.)

- [ ] **Step 2: Verificar creación**

```powershell
Test-Path "C:\Users\ricra\JuegoMental\ProjectSettings\ProjectVersion.txt"
```

Esperado: `True`.

- [ ] **Step 3: Crear `.gitignore` de Unity**

```gitignore
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/
*.csproj
*.sln
*.user
*.log
*.tmp
TestResults.xml
.vs/
.vsconfig
```

- [ ] **Step 4: Commit**

```powershell
cd C:\Users\ricra\JuegoMental
git add .gitignore Assets Packages ProjectSettings
git commit -m "chore: scaffold Unity 6000.0.77f1 project"
```

---

## Task 1: Configurar paquetes 2D y modo 2D

**Files:**
- Modify: `Packages/manifest.json`
- Modify: `ProjectSettings/EditorSettings.asset`

- [ ] **Step 1: Añadir paquetes 2D al `manifest.json`**

Asegurar que el objeto `dependencies` contiene estas líneas (añadir si faltan, conservar las existentes):

```json
"com.unity.2d.sprite": "1.0.0",
"com.unity.2d.tilemap": "1.0.0",
"com.unity.2d.pixel-perfect": "5.0.1",
"com.unity.test-framework": "1.4.5"
```

- [ ] **Step 2: Fijar modo de comportamiento 2D**

En `ProjectSettings/EditorSettings.asset`, fijar el campo `m_DefaultBehaviorMode` a `1` (0 = 3D, 1 = 2D).

- [ ] **Step 3: Validar que Unity resuelve paquetes (arranque en batch)**

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.0.77f1\Editor\Unity.exe" `
  -batchmode -quit -projectPath "C:\Users\ricra\JuegoMental" `
  -logFile "C:\Users\ricra\JuegoMental\resolve.log"
Select-String -Path "C:\Users\ricra\JuegoMental\resolve.log" -Pattern "error|Failed to resolve" -SimpleMatch
```

Esperado: sin errores de resolución; `Packages/packages-lock.json` generado.

- [ ] **Step 4: Commit**

```powershell
git add Packages/manifest.json Packages/packages-lock.json ProjectSettings/EditorSettings.asset
git commit -m "chore: add 2D packages and enable 2D mode"
```

---

## Task 2: Assemblies (asmdef) de runtime y tests

**Files:**
- Create: `Assets/Scripts/JuegoMental.Runtime.asmdef`
- Create: `Assets/Tests/JuegoMental.Tests.asmdef`

- [ ] **Step 1: Crear asmdef de runtime**

`Assets/Scripts/JuegoMental.Runtime.asmdef`:

```json
{
  "name": "JuegoMental.Runtime",
  "rootNamespace": "JuegoMental",
  "references": [],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "autoReferenced": true
}
```

- [ ] **Step 2: Crear asmdef de tests (EditMode)**

`Assets/Tests/JuegoMental.Tests.asmdef`:

```json
{
  "name": "JuegoMental.Tests",
  "references": ["JuegoMental.Runtime"],
  "includePlatforms": ["Editor"],
  "precompiledReferences": ["nunit.framework.dll"],
  "defineConstraints": ["UNITY_INCLUDE_TESTS"],
  "optionalUnityReferences": ["TestAssemblies"],
  "autoReferenced": false
}
```

- [ ] **Step 3: Commit**

```powershell
git add Assets/Scripts/JuegoMental.Runtime.asmdef Assets/Tests/JuegoMental.Tests.asmdef
git commit -m "chore: add runtime and test assembly definitions"
```

---

## Task 3: CortisolModel (lógica pura, TDD)

**Files:**
- Create: `Assets/Scripts/Core/CortisolModel.cs`
- Test: `Assets/Tests/CortisolModelTests.cs`

- [ ] **Step 1: Escribir el test que falla**

`Assets/Tests/CortisolModelTests.cs`:

```csharp
using NUnit.Framework;
using JuegoMental.Core;

public class CortisolModelTests
{
    [Test]
    public void StartsAtZero_NotLost()
    {
        var m = new CortisolModel(100f);
        Assert.AreEqual(0f, m.Value);
        Assert.IsFalse(m.IsLost);
    }

    [Test]
    public void Add_RaisesValue()
    {
        var m = new CortisolModel(100f);
        m.Add(30f);
        Assert.AreEqual(30f, m.Value);
    }

    [Test]
    public void Add_ClampsAtMax_AndMarksLost()
    {
        var m = new CortisolModel(100f);
        m.Add(150f);
        Assert.AreEqual(100f, m.Value);
        Assert.IsTrue(m.IsLost);
    }

    [Test]
    public void Add_NegativeLowersValue_ClampsAtZero()
    {
        var m = new CortisolModel(100f);
        m.Add(40f);
        m.Add(-100f);
        Assert.AreEqual(0f, m.Value);
        Assert.IsFalse(m.IsLost);
    }
}
```

- [ ] **Step 2: Correr y verificar que falla**

Usar el comando estándar de tests. Esperado: FAIL (no compila / `CortisolModel` no existe).

- [ ] **Step 3: Implementación mínima**

`Assets/Scripts/Core/CortisolModel.cs`:

```csharp
namespace JuegoMental.Core
{
    /// <summary>Estrés del jugador. Vida invertida: 0 = calma, max = derrota.</summary>
    public class CortisolModel
    {
        public float Max { get; }
        public float Value { get; private set; }
        public bool IsLost => Value >= Max;

        public CortisolModel(float max)
        {
            Max = max;
            Value = 0f;
        }

        /// <summary>delta positivo sube estrés, negativo lo baja. Clamp 0..Max.</summary>
        public void Add(float delta)
        {
            Value += delta;
            if (Value < 0f) Value = 0f;
            if (Value > Max) Value = Max;
        }
    }
}
```

- [ ] **Step 4: Correr y verificar que pasa**

Comando estándar de tests. Esperado: 4 tests Passed.

- [ ] **Step 5: Commit**

```powershell
git add Assets/Scripts/Core/CortisolModel.cs Assets/Tests/CortisolModelTests.cs
git commit -m "feat: cortisol model with clamp and lose condition"
```

---

## Task 4: PickupModel (lógica pura, TDD)

**Files:**
- Create: `Assets/Scripts/Core/PickupKind.cs`
- Create: `Assets/Scripts/Core/PickupModel.cs`
- Test: `Assets/Tests/PickupModelTests.cs`

- [ ] **Step 1: Escribir el test que falla**

`Assets/Tests/PickupModelTests.cs`:

```csharp
using NUnit.Framework;
using JuegoMental.Core;

public class PickupModelTests
{
    [Test]
    public void Good_AppliesNegativeDelta()
    {
        var m = new CortisolModel(100f);
        m.Add(50f);
        var p = new PickupModel(PickupKind.Good, 20f);
        p.ApplyTo(m);
        Assert.AreEqual(30f, m.Value);
    }

    [Test]
    public void Bad_AppliesPositiveDelta()
    {
        var m = new CortisolModel(100f);
        var p = new PickupModel(PickupKind.Bad, 20f);
        p.ApplyTo(m);
        Assert.AreEqual(20f, m.Value);
    }
}
```

- [ ] **Step 2: Correr y verificar que falla**

Comando estándar. Esperado: FAIL (tipos no existen).

- [ ] **Step 3: Implementación mínima**

`Assets/Scripts/Core/PickupKind.cs`:

```csharp
namespace JuegoMental.Core
{
    public enum PickupKind { Good, Bad }
}
```

`Assets/Scripts/Core/PickupModel.cs`:

```csharp
namespace JuegoMental.Core
{
    /// <summary>Objeto recogible. amount es siempre positivo; el tipo decide el signo.</summary>
    public class PickupModel
    {
        public PickupKind Kind { get; }
        public float Amount { get; }

        public PickupModel(PickupKind kind, float amount)
        {
            Kind = kind;
            Amount = amount;
        }

        public void ApplyTo(CortisolModel cortisol)
        {
            float delta = Kind == PickupKind.Bad ? Amount : -Amount;
            cortisol.Add(delta);
        }
    }
}
```

- [ ] **Step 4: Correr y verificar que pasa**

Esperado: tests Passed (los de Task 3 siguen pasando).

- [ ] **Step 5: Commit**

```powershell
git add Assets/Scripts/Core/PickupKind.cs Assets/Scripts/Core/PickupModel.cs Assets/Tests/PickupModelTests.cs
git commit -m "feat: pickup model applies signed delta by kind"
```

---

## Task 5: EnemyHealth (lógica pura, TDD)

**Files:**
- Create: `Assets/Scripts/Core/EnemyHealth.cs`
- Test: `Assets/Tests/EnemyHealthTests.cs`

- [ ] **Step 1: Escribir el test que falla**

`Assets/Tests/EnemyHealthTests.cs`:

```csharp
using NUnit.Framework;
using JuegoMental.Core;

public class EnemyHealthTests
{
    [Test]
    public void StartsFull_NotDead()
    {
        var h = new EnemyHealth(3f);
        Assert.AreEqual(1f, h.Fraction);
        Assert.IsFalse(h.IsDead);
    }

    [Test]
    public void Damage_ReducesHp()
    {
        var h = new EnemyHealth(4f);
        h.Damage(1f);
        Assert.AreEqual(0.75f, h.Fraction);
    }

    [Test]
    public void Damage_ToZero_IsDead()
    {
        var h = new EnemyHealth(2f);
        h.Damage(5f);
        Assert.AreEqual(0f, h.Fraction);
        Assert.IsTrue(h.IsDead);
    }
}
```

- [ ] **Step 2: Correr y verificar que falla**

Esperado: FAIL (`EnemyHealth` no existe).

- [ ] **Step 3: Implementación mínima**

`Assets/Scripts/Core/EnemyHealth.cs`:

```csharp
namespace JuegoMental.Core
{
    public class EnemyHealth
    {
        public float Max { get; }
        public float Current { get; private set; }
        public bool IsDead => Current <= 0f;
        public float Fraction => Max <= 0f ? 0f : Current / Max;

        public EnemyHealth(float max)
        {
            Max = max;
            Current = max;
        }

        public void Damage(float amount)
        {
            Current -= amount;
            if (Current < 0f) Current = 0f;
        }
    }
}
```

- [ ] **Step 4: Correr y verificar que pasa**

Esperado: todos los tests Passed.

- [ ] **Step 5: Commit**

```powershell
git add Assets/Scripts/Core/EnemyHealth.cs Assets/Tests/EnemyHealthTests.cs
git commit -m "feat: enemy health with damage and death"
```

---

## Task 6: Instalar el MCP de Unity (CoplayDev)

**Files:**
- Modify: `Packages/manifest.json`
- Create/Modify: configuración del cliente MCP (`.mcp.json` en el repo)

> Requiere red. El bridge se importa al abrir Unity. `uv` ejecuta el server Python.

- [ ] **Step 1: Instalar `uv`**

```powershell
powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"
```

Verificar:

```powershell
& "$env:USERPROFILE\.local\bin\uv.exe" --version
```

Esperado: imprime versión de `uv`.

- [ ] **Step 2: Añadir el paquete bridge al `manifest.json`**

Añadir a `dependencies`:

```json
"com.coplaydev.unity-mcp": "https://github.com/CoplayDev/unity-mcp.git?path=/UnityMcpBridge"
```

- [ ] **Step 3: Resolver e importar el bridge (arranque batch)**

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.0.77f1\Editor\Unity.exe" `
  -batchmode -quit -projectPath "C:\Users\ricra\JuegoMental" `
  -logFile "C:\Users\ricra\JuegoMental\mcp-import.log"
Select-String -Path "C:\Users\ricra\JuegoMental\mcp-import.log" -Pattern "error" -SimpleMatch
```

Esperado: sin errores; el paquete aparece en `packages-lock.json`.

- [ ] **Step 4: Registrar el server MCP para el proyecto**

Crear `C:\Users\ricra\JuegoMental\.mcp.json`:

```json
{
  "mcpServers": {
    "unityMCP": {
      "command": "C:\\Users\\ricra\\.local\\bin\\uv.exe",
      "args": [
        "run",
        "--directory",
        "C:\\Users\\ricra\\JuegoMental\\Library\\PackageCache\\com.coplaydev.unity-mcp\\UnityMcpServer~\\src",
        "server.py"
      ]
    }
  }
}
```

> Si la ruta de `UnityMcpServer~` difiere tras la importación, ajustar al path real bajo `Library/PackageCache`. Confirmar con:
> ```powershell
> Get-ChildItem "C:\Users\ricra\JuegoMental\Library\PackageCache" -Filter "com.coplaydev*" | Select-Object FullName
> ```

- [ ] **Step 5: Indicar al usuario el paso manual de Unity**

El usuario debe abrir el proyecto en Unity una vez y verificar en el menú `Window > Unity MCP` que el bridge está "Connected". Documentar esto en el commit y avisar.

- [ ] **Step 6: Commit**

```powershell
git add Packages/manifest.json Packages/packages-lock.json .mcp.json
git commit -m "chore: install Unity MCP bridge and register server"
```

---

## Task 7: PixelArtGenerator (editor script)

**Files:**
- Create: `Assets/Editor/JuegoMental.Editor.asmdef`
- Create: `Assets/Editor/PixelArtGenerator.cs`
- Output: PNGs en `Assets/Art/Generated/`

- [ ] **Step 1: Crear asmdef de editor**

`Assets/Editor/JuegoMental.Editor.asmdef`:

```json
{
  "name": "JuegoMental.Editor",
  "references": ["JuegoMental.Runtime"],
  "includePlatforms": ["Editor"],
  "autoReferenced": true
}
```

- [ ] **Step 2: Crear el generador**

`Assets/Editor/PixelArtGenerator.cs` — un `MenuItem` que pinta texturas y las guarda como PNG con import settings de sprite (Point filter, 16 PPU). Genera: `bg_forest.png`, `tile_grass.png`, `tile_dirt.png`, `player_idle.png`, `player_walk.png`, `player_attack.png`, `enemy_patrol.png`, `enemy_chaser.png`, `item_good.png`, `item_bad.png`, `door.png`.

```csharp
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

        // Personaje (humanoide simple verde, 16x24). Frames idle/walk/attack.
        SaveSprite("player_idle.png",   Player(false, false));
        SaveSprite("player_walk.png",   Player(true,  false));
        SaveSprite("player_attack.png", Player(false, true));

        // Enemigos.
        SaveSprite("enemy_patrol.png",  Box(16, 16, new Color32(20,20,20,255)));   // caja negra
        SaveSprite("enemy_chaser.png",  Circle(16, new Color32(200,30,30,255)));   // circulo rojo

        // Objetos.
        SaveSprite("item_good.png",     Box(12, 16, new Color32(160,200,80,255))); // gameboy-ish
        SaveSprite("item_bad.png",      Box(10, 18, new Color32(80,160,220,255))); // celular-ish

        // Mundo.
        SaveSprite("tile_grass.png",    Box(16, 16, new Color32(70,150,70,255)));
        SaveSprite("tile_dirt.png",     Box(16, 16, new Color32(90,60,50,255)));
        SaveSprite("bg_forest.png",     Box(64, 36, new Color32(110,170,110,255)));
        SaveSprite("door.png",          Box(20, 32, new Color32(120,80,40,255)));

        AssetDatabase.Refresh();
        Debug.Log("Pixel art generado en " + Dir);
    }

    static Color32[] Player(bool stride, bool attack)
    {
        int w = 16, h = 24;
        var px = Fill(w, h, new Color32(0,0,0,0));
        var skin = new Color32(220,170,120,255);
        var shirt = new Color32(70,120,70,255);
        var pants = new Color32(80,60,40,255);
        Rect(px,w, 5,16, 6,4, skin);                 // cabeza
        Rect(px,w, 4,9,  8,7, shirt);                // torso
        Rect(px,w, 4,3,  3,6, pants);                // pierna izq
        Rect(px,w, 9,3,  3,6, pants);                // pierna der
        if (stride) Rect(px,w, 9,1, 3,2, pants);     // paso
        if (attack) Rect(px,w, 12,10, 4,2, new Color32(200,200,210,255)); // espada
        return px;
    }

    static Color32[] Box(int w, int h, Color32 c) => Fill(w, h, c);

    static Color32[] Circle(int d, Color32 c)
    {
        var px = Fill(d, d, new Color32(0,0,0,0));
        float r = d / 2f;
        for (int y = 0; y < d; y++)
        for (int x = 0; x < d; x++)
            if ((x-r+0.5f)*(x-r+0.5f) + (y-r+0.5f)*(y-r+0.5f) <= r*r)
                px[y*d+x] = c;
        return px;
    }

    static Color32[] Fill(int w, int h, Color32 c)
    {
        var px = new Color32[w*h];
        for (int i = 0; i < px.Length; i++) px[i] = c;
        return px;
    }

    static void Rect(Color32[] px, int w, int x0, int y0, int rw, int rh, Color32 c)
    {
        for (int y = y0; y < y0+rh; y++)
        for (int x = x0; x < x0+rw; x++)
            if (x >= 0 && x < w && y >= 0 && y*w+x < px.Length) px[y*w+x] = c;
    }

    static void SaveSprite(string name, Color32[] px)
    {
        // deducir tamaño: el primer SaveSprite pasa arrays cuadrados/rect conocidos
        int total = px.Length;
        // Mapear por nombre a dimensiones usadas arriba:
        (int w, int h) = DimsFor(name, total);
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

    static (int,int) DimsFor(string name, int total)
    {
        switch (name)
        {
            case "player_idle.png":
            case "player_walk.png":
            case "player_attack.png": return (16,24);
            case "enemy_patrol.png":
            case "enemy_chaser.png":
            case "tile_grass.png":
            case "tile_dirt.png":     return (16,16);
            case "item_good.png":     return (12,16);
            case "item_bad.png":      return (10,18);
            case "bg_forest.png":     return (64,36);
            case "door.png":          return (20,32);
            default:                  return (16,16);
        }
    }
}
```

- [ ] **Step 3: Ejecutar el generador por CLI**

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.0.77f1\Editor\Unity.exe" `
  -batchmode -quit -projectPath "C:\Users\ricra\JuegoMental" `
  -executeMethod PixelArtGenerator.Generate `
  -logFile "C:\Users\ricra\JuegoMental\art-gen.log"
```

- [ ] **Step 4: Verificar PNGs**

```powershell
Get-ChildItem "C:\Users\ricra\JuegoMental\Assets\Art\Generated" -Filter *.png | Measure-Object | Select-Object Count
```

Esperado: Count = 11.

- [ ] **Step 5: Commit**

```powershell
git add Assets/Editor Assets/Art/Generated
git commit -m "feat: code-generated pixel art sprites"
```

---

## Task 8: PlayerController2D + CortisolSystem + StressBarUI (MonoBehaviours)

**Files:**
- Create: `Assets/Scripts/Runtime/PlayerController2D.cs`
- Create: `Assets/Scripts/Runtime/CortisolSystem.cs`
- Create: `Assets/Scripts/Runtime/StressBarUI.cs`

> Estos envuelven la lógica POCO ya testeada. No se les hace unit test (dependen de Unity runtime); se prueban en PlayMode manual en Task 12.

- [ ] **Step 1: PlayerController2D**

`Assets/Scripts/Runtime/PlayerController2D.cs`:

```csharp
using UnityEngine;

namespace JuegoMental
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float jumpForce = 9f;
        public Transform groundCheck;
        public LayerMask groundMask;

        Rigidbody2D _rb;
        SpriteRenderer _sr;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            float x = Input.GetAxisRaw("Horizontal");
            _rb.linearVelocity = new Vector2(x * moveSpeed, _rb.linearVelocity.y);
            if (x != 0 && _sr != null) _sr.flipX = x < 0;

            if (Input.GetButtonDown("Jump") && IsGrounded())
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }

        bool IsGrounded()
        {
            if (groundCheck == null) return true;
            return Physics2D.OverlapCircle(groundCheck.position, 0.15f, groundMask);
        }
    }
}
```

- [ ] **Step 2: CortisolSystem (wrapper de CortisolModel)**

`Assets/Scripts/Runtime/CortisolSystem.cs`:

```csharp
using System;
using UnityEngine;
using JuegoMental.Core;

namespace JuegoMental
{
    public class CortisolSystem : MonoBehaviour
    {
        public float max = 100f;
        public event Action OnChanged;
        public event Action OnLost;

        CortisolModel _model;
        public float Value => _model.Value;
        public float Max => _model.Max;
        public float Fraction => _model.Value / _model.Max;

        void Awake() => _model = new CortisolModel(max);

        public void Add(float delta)
        {
            if (_model.IsLost) return;
            _model.Add(delta);
            OnChanged?.Invoke();
            if (_model.IsLost) OnLost?.Invoke();
        }
    }
}
```

- [ ] **Step 3: StressBarUI**

`Assets/Scripts/Runtime/StressBarUI.cs`:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace JuegoMental
{
    public class StressBarUI : MonoBehaviour
    {
        public CortisolSystem cortisol;
        public Image fill; // Image type Filled, Horizontal

        void OnEnable()
        {
            if (cortisol != null) cortisol.OnChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            if (cortisol != null) cortisol.OnChanged -= Refresh;
        }

        void Refresh()
        {
            if (fill != null && cortisol != null) fill.fillAmount = cortisol.Fraction;
        }
    }
}
```

- [ ] **Step 4: Compilar (arranque batch sin errores)**

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.0.77f1\Editor\Unity.exe" `
  -batchmode -quit -projectPath "C:\Users\ricra\JuegoMental" `
  -logFile "C:\Users\ricra\JuegoMental\compile1.log"
Select-String -Path "C:\Users\ricra\JuegoMental\compile1.log" -Pattern "error CS" -SimpleMatch
```

Esperado: sin líneas `error CS`.

- [ ] **Step 5: Commit**

```powershell
git add Assets/Scripts/Runtime/PlayerController2D.cs Assets/Scripts/Runtime/CortisolSystem.cs Assets/Scripts/Runtime/StressBarUI.cs
git commit -m "feat: player movement, cortisol system, stress bar UI"
```

---

## Task 9: PlayerAttack (espada)

**Files:**
- Create: `Assets/Scripts/Runtime/PlayerAttack.cs`

- [ ] **Step 1: Implementar el ataque**

`Assets/Scripts/Runtime/PlayerAttack.cs`:

```csharp
using UnityEngine;

namespace JuegoMental
{
    public class PlayerAttack : MonoBehaviour
    {
        public float damage = 1f;
        public float range = 0.8f;
        public float cooldown = 0.4f;
        public LayerMask enemyMask;
        public Transform attackOrigin; // punto frente al jugador

        float _next;
        SpriteRenderer _sr;

        void Awake() => _sr = GetComponent<SpriteRenderer>();

        void Update()
        {
            if ((Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)) && Time.time >= _next)
            {
                _next = Time.time + cooldown;
                DoAttack();
            }
        }

        void DoAttack()
        {
            Vector2 dir = (_sr != null && _sr.flipX) ? Vector2.left : Vector2.right;
            Vector2 origin = attackOrigin != null ? (Vector2)attackOrigin.position : (Vector2)transform.position + dir * 0.5f;
            var hits = Physics2D.OverlapCircleAll(origin, range, enemyMask);
            foreach (var h in hits)
            {
                var enemy = h.GetComponent<EnemyBase>();
                if (enemy != null) enemy.TakeDamage(damage);
            }
        }
    }
}
```

> Depende de `EnemyBase.TakeDamage(float)` definido en Task 10.

- [ ] **Step 2: Commit (compila tras Task 10; por ahora se guarda)**

```powershell
git add Assets/Scripts/Runtime/PlayerAttack.cs
git commit -m "feat: melee sword attack"
```

---

## Task 10: Enemigos (EnemyBase, PatrolEnemy, ChaserEnemy, EnemyHealthBar)

**Files:**
- Create: `Assets/Scripts/Runtime/EnemyBase.cs`
- Create: `Assets/Scripts/Runtime/PatrolEnemy.cs`
- Create: `Assets/Scripts/Runtime/ChaserEnemy.cs`
- Create: `Assets/Scripts/Runtime/EnemyHealthBar.cs`

- [ ] **Step 1: EnemyBase (vida + daño por contacto al cortisol)**

`Assets/Scripts/Runtime/EnemyBase.cs`:

```csharp
using UnityEngine;
using JuegoMental.Core;

namespace JuegoMental
{
    public abstract class EnemyBase : MonoBehaviour
    {
        public float maxHp = 3f;
        public float contactCortisol = 15f; // sube al jugador al tocar
        public float contactCooldown = 1f;

        protected EnemyHealth Health;
        float _nextContact;

        public System.Action<float> OnHealthChanged; // fraction

        protected virtual void Awake() => Health = new EnemyHealth(maxHp);

        public void TakeDamage(float amount)
        {
            Health.Damage(amount);
            OnHealthChanged?.Invoke(Health.Fraction);
            if (Health.IsDead) Destroy(gameObject);
        }

        void OnCollisionStay2D(Collision2D col) => TryContact(col.collider);
        void OnTriggerStay2D(Collider2D other) => TryContact(other);

        void TryContact(Collider2D other)
        {
            if (Time.time < _nextContact) return;
            var c = other.GetComponentInParent<CortisolSystem>();
            if (c != null)
            {
                _nextContact = Time.time + contactCooldown;
                c.Add(contactCortisol);
            }
        }
    }
}
```

- [ ] **Step 2: PatrolEnemy**

`Assets/Scripts/Runtime/PatrolEnemy.cs`:

```csharp
using UnityEngine;

namespace JuegoMental
{
    public class PatrolEnemy : EnemyBase
    {
        public float speed = 2f;
        public float range = 3f;

        Vector2 _start;
        int _dir = 1;

        protected override void Awake()
        {
            base.Awake();
            _start = transform.position;
        }

        void Update()
        {
            transform.Translate(Vector2.right * _dir * speed * Time.deltaTime);
            if (Mathf.Abs(transform.position.x - _start.x) >= range) _dir = -_dir;
        }
    }
}
```

- [ ] **Step 3: ChaserEnemy**

`Assets/Scripts/Runtime/ChaserEnemy.cs`:

```csharp
using UnityEngine;

namespace JuegoMental
{
    public class ChaserEnemy : EnemyBase
    {
        public float speed = 2.5f;
        public float detectRange = 6f;
        Transform _player;

        protected override void Awake()
        {
            base.Awake();
            var p = GameObject.FindWithTag("Player");
            if (p != null) _player = p.transform;
        }

        void Update()
        {
            if (_player == null) return;
            float dist = Vector2.Distance(transform.position, _player.position);
            if (dist <= detectRange)
            {
                Vector2 to = (_player.position - transform.position).normalized;
                transform.Translate(new Vector2(to.x, 0f) * speed * Time.deltaTime);
            }
        }
    }
}
```

- [ ] **Step 4: EnemyHealthBar (UI mundo sobre el enemigo)**

`Assets/Scripts/Runtime/EnemyHealthBar.cs`:

```csharp
using UnityEngine;

namespace JuegoMental
{
    public class EnemyHealthBar : MonoBehaviour
    {
        public EnemyBase enemy;
        public Transform fill;      // escala X 0..1
        public Vector3 offset = new Vector3(0f, 0.8f, 0f);

        Vector3 _fullScale;

        void Start()
        {
            if (fill != null) _fullScale = fill.localScale;
            if (enemy != null) enemy.OnHealthChanged += SetFraction;
        }

        void OnDestroy()
        {
            if (enemy != null) enemy.OnHealthChanged -= SetFraction;
        }

        void LateUpdate()
        {
            if (enemy != null) transform.position = enemy.transform.position + offset;
        }

        void SetFraction(float f)
        {
            if (fill != null) fill.localScale = new Vector3(_fullScale.x * f, _fullScale.y, _fullScale.z);
        }
    }
}
```

- [ ] **Step 5: Compilar sin errores**

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.0.77f1\Editor\Unity.exe" `
  -batchmode -quit -projectPath "C:\Users\ricra\JuegoMental" `
  -logFile "C:\Users\ricra\JuegoMental\compile2.log"
Select-String -Path "C:\Users\ricra\JuegoMental\compile2.log" -Pattern "error CS" -SimpleMatch
```

Esperado: sin `error CS` (también valida que `PlayerAttack` de Task 9 compila).

- [ ] **Step 6: Commit**

```powershell
git add Assets/Scripts/Runtime/EnemyBase.cs Assets/Scripts/Runtime/PatrolEnemy.cs Assets/Scripts/Runtime/ChaserEnemy.cs Assets/Scripts/Runtime/EnemyHealthBar.cs
git commit -m "feat: enemies with health bar, patrol and chase behaviors"
```

---

## Task 11: Pickup, LevelDoor, GameManager, LevelConfig

**Files:**
- Create: `Assets/Scripts/Runtime/Pickup.cs`
- Create: `Assets/Scripts/Runtime/LevelDoor.cs`
- Create: `Assets/Scripts/Runtime/GameManager.cs`
- Create: `Assets/Scripts/Config/LevelConfig.cs`

- [ ] **Step 1: Pickup (wrapper de PickupModel)**

`Assets/Scripts/Runtime/Pickup.cs`:

```csharp
using UnityEngine;
using JuegoMental.Core;

namespace JuegoMental
{
    public class Pickup : MonoBehaviour
    {
        public PickupKind kind = PickupKind.Good;
        public float amount = 15f;

        void OnTriggerEnter2D(Collider2D other)
        {
            var c = other.GetComponentInParent<CortisolSystem>();
            if (c == null) return;
            new PickupModel(kind, amount).ApplyTo(c);
            Destroy(gameObject);
        }
    }
}
```

- [ ] **Step 2: GameManager (progreso + carga de escenas + game over)**

`Assets/Scripts/Runtime/GameManager.cs`:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JuegoMental
{
    public class GameManager : MonoBehaviour
    {
        const string UnlockedKey = "unlocked_floor";

        public static int UnlockedFloor
        {
            get => Mathf.Max(1, PlayerPrefs.GetInt(UnlockedKey, 1));
            set { PlayerPrefs.SetInt(UnlockedKey, value); PlayerPrefs.Save(); }
        }

        public static void EnterLevel(int floor)
        {
            if (floor > UnlockedFloor) return;
            SceneManager.LoadScene($"Level_{floor:00}");
        }

        public static void CompleteLevel(int floor)
        {
            if (floor + 1 > UnlockedFloor) UnlockedFloor = floor + 1;
            SceneManager.LoadScene("Hub");
        }

        public static void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public static void GoToHub() => SceneManager.LoadScene("Hub");
    }
}
```

- [ ] **Step 3: LevelDoor (hub: entra; nivel: completa)**

`Assets/Scripts/Runtime/LevelDoor.cs`:

```csharp
using UnityEngine;

namespace JuegoMental
{
    public class LevelDoor : MonoBehaviour
    {
        public enum Mode { EnterLevel, ExitToHub }
        public Mode mode = Mode.EnterLevel;
        public int floor = 1;

        bool _playerInside;

        void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) _playerInside = true; }
        void OnTriggerExit2D(Collider2D other)  { if (other.CompareTag("Player")) _playerInside = false; }

        void Update()
        {
            if (!_playerInside) return;
            if (mode == Mode.EnterLevel && Input.GetKeyDown(KeyCode.E))
                GameManager.EnterLevel(floor);
            else if (mode == Mode.ExitToHub && Input.GetKeyDown(KeyCode.E))
                GameManager.CompleteLevel(floor);
        }

        public bool Unlocked => floor <= GameManager.UnlockedFloor;
    }
}
```

- [ ] **Step 4: LevelConfig (ScriptableObject)**

`Assets/Scripts/Config/LevelConfig.cs`:

```csharp
using UnityEngine;

namespace JuegoMental
{
    [CreateAssetMenu(menuName = "Juego Mental/Level Config", fileName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int floor = 1;
        public int patrolEnemies = 2;
        public int chaserEnemies = 0;
        public float cortisolMax = 100f;
        public float enemyContactCortisol = 15f;
    }
}
```

- [ ] **Step 5: Conectar game over al CortisolSystem**

Modificar `Assets/Scripts/Runtime/CortisolSystem.cs`: añadir un manejador opcional que al `OnLost` invoque `GameManager.RestartLevel()`. Añadir al final de `Awake`:

```csharp
            OnLost += () => GameManager.RestartLevel();
```

(Insertar esa línea dentro de `Awake`, después de `_model = new CortisolModel(max);`.)

- [ ] **Step 6: Compilar sin errores**

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.0.77f1\Editor\Unity.exe" `
  -batchmode -quit -projectPath "C:\Users\ricra\JuegoMental" `
  -logFile "C:\Users\ricra\JuegoMental\compile3.log"
Select-String -Path "C:\Users\ricra\JuegoMental\compile3.log" -Pattern "error CS" -SimpleMatch
```

Esperado: sin `error CS`.

- [ ] **Step 7: Commit**

```powershell
git add Assets/Scripts/Runtime/Pickup.cs Assets/Scripts/Runtime/LevelDoor.cs Assets/Scripts/Runtime/GameManager.cs Assets/Scripts/Config/LevelConfig.cs Assets/Scripts/Runtime/CortisolSystem.cs
git commit -m "feat: pickups, doors, game manager, level config, game over restart"
```

---

## Task 12: Armar escenas Hub y Level_01

> Esta tarea usa el **MCP de Unity** (preferido) o el editor manual. Es trabajo de escena (GameObjects, prefabs, wiring) que no se puede hacer solo por archivos de forma confiable. Si el MCP está conectado (Task 6), dirigir el editor por MCP; si no, guiar al usuario con estos pasos exactos.

**Prefabs a crear (`Assets/Prefabs/`):**
- `Player.prefab`: SpriteRenderer (`player_idle`), Rigidbody2D (freeze rotation Z), CapsuleCollider2D, tag `Player`, layer `Player`; componentes `PlayerController2D`, `PlayerAttack`, `CortisolSystem`. Hijo `GroundCheck` (Transform en los pies). En `PlayerController2D`: `groundMask` = layer Ground. En `PlayerAttack`: `enemyMask` = layer Enemy, `attackOrigin` = hijo frente.
- `PatrolEnemy.prefab`: SpriteRenderer (`enemy_patrol`), Rigidbody2D (kinematic), BoxCollider2D, layer `Enemy`; componentes `PatrolEnemy`, `EnemyHealthBar` (con hijo barra: fondo + `fill` que escala en X).
- `ChaserEnemy.prefab`: igual pero sprite `enemy_chaser` y componente `ChaserEnemy`.
- `PickupGood.prefab`: SpriteRenderer (`item_good`), CircleCollider2D (isTrigger), `Pickup` kind=Good.
- `PickupBad.prefab`: SpriteRenderer (`item_bad`), CircleCollider2D (isTrigger), `Pickup` kind=Bad.
- `Door.prefab`: SpriteRenderer (`door`), BoxCollider2D (isTrigger), `LevelDoor`.

**Layers a crear:** `Ground`, `Player`, `Enemy`.

- [ ] **Step 1: Crear layers y tag Player**

En `ProjectSettings/TagManager.asset` añadir layers `Ground`, `Player`, `Enemy` y asegurar tag `Player`. (Por MCP o manual via Edit > Project Settings > Tags and Layers.)

- [ ] **Step 2: Crear los prefabs listados arriba**

Por MCP (crear GameObjects, añadir componentes, asignar sprites de `Assets/Art/Generated`, guardar como prefab) o manual.

- [ ] **Step 3: Construir `Assets/Scenes/Hub.unity`**

- Cámara con Pixel Perfect Camera.
- Suelo y plataformas verticales (BoxCollider2D, layer Ground) formando una torre que se sube saltando.
- Instancia `Player` en la base.
- 10 instancias de `Door` (mode=EnterLevel, floor=1..10) en cada piso. Door con floor > `GameManager.UnlockedFloor` puede mostrarse pero `EnterLevel` la ignora.
- Canvas UI opcional con texto del piso.
- Marcar como escena de inicio (Build Settings, índice 0).

- [ ] **Step 4: Construir `Assets/Scenes/Level_01.unity`**

- Cámara Pixel Perfect + fondo `bg_forest` (SpriteRenderer al fondo).
- Suelo con tiles `tile_grass`/`tile_dirt` y algunas plataformas.
- Instancia `Player`.
- Canvas con `StressBarUI`: barra roja arriba con label "Cortisol:" (Image Filled como `fill`, enlazada al `CortisolSystem` del player).
- 2 `PatrolEnemy`, algunos `PickupGood` y `PickupBad`.
- Una `Door` (mode=ExitToHub, floor=1) como salida.
- Añadir `Level_01` a Build Settings.

- [ ] **Step 5: Añadir escenas a Build Settings**

Editar `ProjectSettings/EditorBuildSettings.asset` para incluir `Hub` (index 0) y `Level_01` (index 1). (Por MCP o manual.)

- [ ] **Step 6: Commit**

```powershell
git add Assets/Prefabs Assets/Scenes ProjectSettings/TagManager.asset ProjectSettings/EditorBuildSettings.asset
git commit -m "feat: hub tower scene and Level_01 with all systems wired"
```

---

## Task 13: Playtest del vertical slice

> Verificación manual en el editor (PlayMode). El MCP no reemplaza la observación humana del juego.

- [ ] **Step 1: Abrir Unity y entrar a PlayMode en `Hub`**

El usuario abre el proyecto, abre `Hub`, presiona Play.

- [ ] **Step 2: Checklist de verificación**

Confirmar cada punto:
- [ ] El jugador se mueve (flechas/A-D) y salta (espacio).
- [ ] Subir la torre y pararse en la puerta del piso 1 + E carga `Level_01`.
- [ ] En `Level_01`: la barra de cortisol arriba refleja cambios.
- [ ] Recoger objeto bueno baja el cortisol; objeto malo lo sube.
- [ ] Tocar un enemigo sube el cortisol; la barra de vida del enemigo se ve.
- [ ] Atacar (J) reduce la vida del enemigo y lo destruye a 0.
- [ ] Llenar el cortisol reinicia el nivel (game over).
- [ ] Salir por la puerta del nivel vuelve al hub y desbloquea el piso 2.

- [ ] **Step 3: Anotar fallos y corregir**

Para cada fallo, usar la skill `superpowers:systematic-debugging`. Corregir, recompilar, re-probar.

- [ ] **Step 4: Commit final del slice**

```powershell
git add -A
git commit -m "chore: vertical slice playtest fixes"
```

---

## Después del slice (fuera de este plan)

Tras aprobar el vertical slice, replicar `Level_01` → `Level_02..10` con `LevelConfig` por piso (más enemigos, perseguidores en pisos altos, mayor dificultad), y conectar las 10 puertas del hub. Cada nivel se valida igual que en Task 13.
