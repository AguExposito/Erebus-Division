# 🏰 Guía de Configuración - Heaven Dungeon Generator

## 📋 Configuración Manual Paso a Paso

### **Paso 1: Crear el Generador**
1. Crea un **GameObject vacío** en tu escena
2. Nómbralo `HeavenDungeonGenerator`
3. Añade el componente `HeavenDungeonGenerator`

### **Paso 2: Configurar la Matriz (OBLIGATORIO)**
En el inspector, ajusta estos valores:

```
Dungeon Matrix Settings:
├── Dungeon Width: 12        (ancho en bloques)
├── Dungeon Height: 12       (alto en bloques)  
└── Block Size: 5.0          (tamaño de cada bloque en unidades)
```

**⚠️ IMPORTANTE**: Las habitaciones ahora tienen una **separación mínima de 1 bloque** entre ellas para permitir pasillos. Esto significa que:
- Cada habitación ocupa 1 celda de la matriz
- No puede haber habitaciones en celdas adyacentes (Norte, Sur, Este, Oeste, ni diagonales)
- Los pasillos se crean automáticamente en el espacio entre habitaciones

**Valores Recomendados según Tamaño:**

| Tamaño | Width x Height | Block Size | Min Rooms | Max Rooms | Espacio Total |
|--------|---------------|------------|-----------|-----------|---------------|
| **Pequeño** | 10x10 | 4.0 | 4-5 | 6-7 | ~40x40 unidades |
| **Mediano** ⭐ | **12x12** | **5.0** | **6-8** | **8-10** | **~60x60 unidades** |
| **Grande** | 16x16 | 6.0 | 10-12 | 14-16 | ~96x96 unidades |
| **Muy Grande** | 20x20 | 8.0 | 15-18 | 20-25 | ~160x160 unidades |

**Fórmula para calcular máximo de habitaciones:**
- Con separación mínima de N bloques, cada habitación necesita espacio de (2N+1)x(2N+1)
- Separación de 1 bloque: espacio de 3x3 por habitación
- Separación de 2 bloques: espacio de 5x5 por habitación  
- Separación de 3 bloques: espacio de 7x7 por habitación
- Máximo teórico ≈ (Width × Height) / (2N+1)²
- Recomendado práctico ≈ (Width × Height) / ((2N+1)² × 1.5) (para evitar espacios muy densos)

**Ejemplo de cálculo para 12x12 con separación de 2 bloques:**
- Espacio por habitación: 5x5 = 25 bloques
- Máximo teórico: 144 / 25 = 5 habitaciones
- Recomendado práctico: 144 / 37.5 = 3 habitaciones
- **Configuración óptima**: Min=4, Max=6 ⭐

### **Parámetro de Separación Mínima**

El parámetro `Min Room Separation` controla cuánto espacio debe haber entre habitaciones:

- **1**: Habitaciones adyacentes (sin espacio entre ellas)
- **2**: 1 bloque de separación (recomendado) ⭐
- **3**: 2 bloques de separación (más espacioso)
- **4+**: Muy espacioso (pocas habitaciones)

**Recomendaciones:**
- **Separación 1**: Para mazmorras densas y compactas
- **Separación 2**: Para mazmorras balanceadas (recomendado)
- **Separación 3+**: Para mazmorras amplias y espaciadas

### **Paso 4: Asignar Prefabs (OPCIONAL)**
```
Heaven Prefabs:
├── Heaven Room Prefab: [Asignar prefab de habitación]
├── Heaven Hallway Prefab: [Asignar prefab de pasillo]
├── Heaven Floor Prefab: [Asignar prefab de suelo]
└── Heaven Wall Prefab: [Asignar prefab de pared]
```

**Nota**: Si no asignas prefabs, se crearán objetos básicos automáticamente.

### **Paso 5: Configurar Rendimiento**
```
Generation Settings:
├── Use Coroutines: ✓       (evita caídas de FPS)
└── Max Rooms Per Frame: 2  (habitaciones por frame)
```

### **Paso 6: Configurar Tema (OPCIONAL)**
```
Heaven Theme:
├── Heaven Floor Material: [Material para suelos]
├── Heaven Wall Material: [Material para paredes]
└── Heaven Ambient Color: (0.8, 0.9, 1.0, 1.0)
```

## 🎮 Configuración Básica Estándar

### **Configuración Mínima Recomendada (Separación de 1 Bloque):**
```
Dungeon Width: 12
Dungeon Height: 12
Block Size: 5.0
Min Rooms: 6
Max Rooms: 10
Use Coroutines: ✓
Max Rooms Per Frame: 2
```

### **Configuración Avanzada:**
```
Dungeon Width: 16
Dungeon Height: 16
Block Size: 6.0
Min Rooms: 10
Max Rooms: 15
Use Coroutines: ✓
Max Rooms Per Frame: 3
```

## 🔗 Sistema de Conectividad Garantizada

El sistema ahora garantiza que **TODAS las habitaciones sean accesibles** mediante:

1. **Conexión de Habitaciones Adyacentes**:
   - Conecta habitaciones que están separadas por exactamente 1 bloque (2 bloques de distancia en la matriz)
   - Crea pasillos automáticamente en el espacio intermedio

2. **Detección de Grupos Aislados**:
   - Usa BFS (Breadth-First Search) para encontrar grupos desconectados
   - Conecta automáticamente los grupos más cercanos

3. **Verificación de Accesibilidad**:
   - Presiona **C** en el modo de prueba para ver el análisis de conectividad
   - Todos los grupos aislados se conectan automáticamente

## 🚀 Probar la Generación

### **Opción A: Script de Ejemplo**
1. Añade el componente `HeavenDungeonExample` al mismo GameObject
2. Usa las teclas:
   - **R**: Regenerar mazmorra
   - **C**: Limpiar mazmorra
   - **M**: Ver matriz en consola
   - **P**: Ver posiciones de habitaciones

### **Opción B: Código Manual**
```csharp
// Obtener referencia al generador
HeavenDungeonGenerator generator = FindObjectOfType<HeavenDungeonGenerator>();

// Regenerar mazmorra
generator.RegenerateDungeon();

// Ver información de debug
generator.PrintMatrix();
generator.PrintRoomPositions();
```

## 🔧 Solución de Problemas

### **Problema: Las habitaciones no se generan**
- ✅ Verifica que `Dungeon Width` y `Dungeon Height` sean mayores a 0
- ✅ Verifica que `Min Rooms` y `Max Rooms` sean válidos
- ✅ Revisa la consola para errores

### **Problema: Las habitaciones se superponen**
- ✅ Aumenta el `Block Size` para más separación
- ✅ Reduce el número de habitaciones
- ✅ Aumenta el tamaño de la matriz

### **Problema: Rendimiento bajo**
- ✅ Activa `Use Coroutines`
- ✅ Reduce `Max Rooms Per Frame` a 1
- ✅ Reduce el tamaño de la matriz

### **Problema: No se ven los pasillos**
- ✅ Verifica que `Heaven Hallway Prefab` esté asignado
- ✅ Revisa que las habitaciones estén cerca en la matriz
- ✅ Usa `PrintMatrix()` para ver la distribución

## 📊 Entender la Matriz

La matriz funciona así:
```
Matriz 8x8 con Block Size 3.0:
┌─────────────────────────────────┐
│  ·  ·  ·  ·  ·  ·  ·  ·  │ 24u │
│  ·  ·  ·  ·  ·  ·  ·  ·  │ 21u │
│  ·  ·  ·  ·  ·  ·  ·  ·  │ 18u │
│  ·  ·  ·  ·  ·  ·  ·  ·  │ 15u │
│  ·  ·  ·  ·  ·  ·  ·  ·  │ 12u │
│  ·  ·  ·  ·  ·  ·  ·  ·  │  9u │
│  ·  ·  ·  ·  ·  ·  ·  ·  │  6u │
│  ·  ·  ·  ·  ·  ·  ·  ·  │  3u │
└─────────────────────────────────┘
  -12u -9u -6u -3u  0u  3u  6u  9u
```

- **·** = Espacio vacío
- **█** = Habitación ocupada
- Cada celda = `Block Size` unidades en el mundo
- La mazmorra se centra automáticamente en (0,0,0)

## ✅ Checklist de Configuración

- [ ] GameObject creado con `HeavenDungeonGenerator`
- [ ] `Dungeon Width` y `Dungeon Height` configurados
- [ ] `Block Size` ajustado según necesidades
- [ ] `Min Rooms` y `Max Rooms` establecidos
- [ ] `Use Coroutines` activado
- [ ] Prefabs asignados (opcional)
- [ ] Materiales asignados (opcional)
- [ ] Probado con `HeavenDungeonExample` o código manual

## 🎯 Resultado Esperado

Con la configuración estándar deberías obtener:
- Una mazmorra de 8x8 bloques (24x24 unidades)
- 4-6 habitaciones distribuidas aleatoriamente
- Pasillos conectando habitaciones adyacentes
- Habitación inicial (Sanctuary) en el centro
- Generación sin caídas de FPS
