# No More Cats

3D first-person shooter postavený na OpenTK (OpenGL) v C#. Cílem hráče je střílet kočky, které se spawní na procedurálně generovaném terénu.

## Jak spustit

**Požadavky:** .NET 10 SDK

```bash
dotnet run
```

## Ovládání

| Klávesa | Akce |
|---------|------|
| W / A / S / D | Pohyb |
| Myš | Rozhlížení |
| Mezerník | Skok |
| Levé tlačítko myši | Střelba |
| ↑ / ↓ | Zvýšit / snížit počet nepřátel |
| Escape | Ukončit hru |

## Technologie

- **OpenTK 4.9** — OpenGL, windowing, vstup
- **StbImageSharp** — načítání textur
- **SimplexNoise** — generování terénu

## Struktura projektu

```
├── Camera.cs               # FPS kamera s gravitací a skokem
├── Window.cs               # Hlavní herní smyčka
├── Components/
│   ├── ModelObject.cs      # GPU mesh s transformací a hitboxem
│   ├── Primitives.cs       # Továrna na geometrii (box, válec, koule, ray)
│   └── World/
│       ├── Chunk.cs        # Voxelový terén s výškovou mapou
│       ├── Sky.cs          # Skybox
│       ├── Blok.cs         # Jedna voxelová kostka
│       └── BlokData.cs     # Data ploch a UV souřadnice
├── Graphics/
│   ├── ShaderProgram.cs    # Kompilace a správa shaderů
│   ├── VAO / VBO / IBO.cs  # OpenGL buffer wrappers
│   └── Texture.cs          # Načítání a binding textur
├── Shaders/                # GLSL vertex a fragment shadery
└── Textures/               # Herní textury
```
