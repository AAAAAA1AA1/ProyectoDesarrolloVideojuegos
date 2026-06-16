# Juego Mental — Diseño

**Fecha:** 2026-06-16
**Estado:** Aprobado para planificación
**Enfoque de construcción:** A (vertical slice primero)
**Editor Unity:** 6000.0.77f1 (LTS), plantilla 2D (Built-in)

## Resumen

Plataformero 2D pixel-art con temática de manejo del estrés. El jugador sube
una torre (hub vertical) con 10 puertas; cada puerta lleva a un nivel. En cada
nivel el jugador esquiva o combate enemigos, evita objetos malos, recoge objetos
buenos, y llega a la puerta de salida sin que la barra de **cortisol** (estrés)
se llene. El cortisol funciona como vida invertida: empieza vacío y al llenarse
se pierde el nivel.

## Alcance de la primera entrega (Vertical Slice — Enfoque A)

Construir y dejar jugable:
- Escena `Hub` (torre vertical con 10 puertas; solo el piso 1 desbloqueado).
- Escena `Level_01` completa con TODOS los sistemas funcionando.
- Generador de arte pixel por código.
- MCP de Unity instalado y configurado.

Los niveles 2–10 se replican como contenido **después** de aprobar el slice.

## Configuración del proyecto

- Editor: `6000.0.77f1`. Plantilla: 2D (Built-in render pipeline).
- Paquetes: 2D Tilemap, 2D Pixel Perfect Camera.
- Input: sistema de input legacy (teclado) para mantenerlo simple.
- Estructura de carpetas: `Assets/Scripts`, `Assets/Scenes`, `Assets/Art/Generated`,
  `Assets/Prefabs`, `Assets/Config`.

## Controles

- Mover: flechas o A/D.
- Saltar: barra espaciadora.
- Atacar (espada): J (o click izquierdo).
- Entrar a puerta: E.

## Sistemas / Componentes (C#)

### PlayerController2D
Movimiento horizontal + salto con `Rigidbody2D`. Detección de suelo para permitir
salto. Voltea el sprite según dirección.

### CortisolSystem
Barra 0 → máximo.
- **Sube** al: recibir contacto de enemigo, recoger objeto malo.
- **Baja** al: recoger objeto bueno.
- Sin subida automática por tiempo en la base (opción futura).
- Valor siempre clamp 0..máx.
- Al llegar al máximo: **game over del nivel** → overlay "Reintentar" → reinicia nivel.

### StressBarUI
Barra roja en la parte superior con etiqueta "Cortisol:". Refleja el valor de
`CortisolSystem`.

### Pickup
Objeto recogible con tipo **Bueno** o **Malo** y un valor delta de cortisol
(negativo = baja, positivo = sube). Desaparece al tocarse. Representa objetos
como celular, gameboy, etc.

### PlayerAttack
Espada cuerpo a cuerpo. Tecla de ataque genera un hitbox al frente del personaje
durante la animación de ataque, con cooldown. Aplica daño a enemigos en rango.

### EnemyBase (+ PatrolEnemy, ChaserEnemy)
- **HP** + barra de vida flotante (UI en espacio de mundo sobre el enemigo).
  Muere al llegar a 0 HP.
- Contacto con el jugador **sube cortisol**.
- `PatrolEnemy`: camina de un lado a otro entre límites.
- `ChaserEnemy`: detecta al jugador y se mueve hacia él.
- Pisos bajos = patrullan; pisos altos = persiguen (dificultad sube con la torre).

### LevelDoor
- En el `Hub`: al interactuar (E), carga la escena del nivel correspondiente si
  está desbloqueado.
- En un nivel: puerta de salida; al alcanzarla, marca el nivel como completado,
  vuelve al `Hub` y desbloquea el siguiente piso.

### GameManager
Gestiona carga de escenas, estado de juego (jugando / game over), reinicio de
nivel y progreso. Persiste el último piso desbloqueado en `PlayerPrefs`.

### LevelConfig (ScriptableObject)
Por nivel: tipo y cantidad de enemigos, dificultad de cortisol, parámetros. Un
asset por nivel.

### PixelArtGenerator (Editor script)
Genera por código (`Texture2D`/`SetPixels`) sprites pixel-art parecidos al estilo
de referencia y los guarda como PNG en `Assets/Art/Generated`:
- Personaje humanoide con animaciones idle / caminar / salto / ataque (con espada).
- Fondo de bosque verde, capa de suelo (pasto) y tierra.
- Objetos recogibles (celular, gameboy, etc.).
- Enemigos (patrulla y perseguidor).
Sprites diseñados para ser reemplazables fácilmente por arte final.

## Flujo de juego

1. **Hub:** el jugador aparece en la base de la torre. Sube por plataformas.
2. Llega a la puerta del piso N (solo desbloqueados son accesibles). Pulsa E.
3. `GameManager` carga `Level_N`.
4. **Nivel:** el jugador aparece, `CortisolSystem` activo. Recoge objetos buenos
   (baja cortisol), evita objetos malos (sube), combate o esquiva enemigos
   (contacto sube cortisol; espada los mata).
5. Llega a la puerta de salida → nivel completado → vuelve al `Hub`, desbloquea
   piso N+1.
6. Si el cortisol se llena → game over → overlay "Reintentar" → reinicia el nivel.

## Manejo de errores / casos borde

- Cortisol siempre clamp 0..máx.
- Torre **lineal**: solo puedes entrar a pisos desbloqueados; debes completar en orden.
- Game over con overlay de reintento; reinicia el nivel actual.
- Progreso guardado en `PlayerPrefs` (último piso desbloqueado).

## Pruebas

- **EditMode (lógica pura):**
  - `CortisolSystem`: clamp 0..máx, condición de derrota al llenarse, baja/sube por delta.
  - `Pickup`: aplica delta correcto según tipo.
  - `EnemyBase`: daño reduce HP, muerte a 0 HP.
- **PlayMode / manual:** movimiento, salto, ataque, puertas, transición hub↔nivel
  se prueban manualmente en el editor.

## MCP

Instalar **Unity MCP (CoplayDev)**:
- Instalar `uv` (gestor de Python) en el sistema.
- Añadir el paquete bridge al proyecto (Package Manager via git URL del repo CoplayDev).
- Registrar el server MCP en el cliente (claude).
- Puede requerir abrir Unity una vez para importar/auto-instalar el bridge.

## Fuera de alcance (por ahora)

- Niveles 2–10 (se hacen tras aprobar el slice).
- Subida de cortisol por tiempo.
- Vida del jugador separada del cortisol.
- Audio, menús avanzados, guardado en nube.
