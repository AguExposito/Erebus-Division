# Guía de Desarrollo: Generador de Mazmorras Procedurales

## 📋 Índice
1. [Concepto General](#concepto-general)
2. [Arquitectura del Sistema](#arquitectura-del-sistema)
3. [Componentes Principales](#componentes-principales)
4. [Cómo Usar el Generador](#cómo-usar-el-generador)
5. [Personalización y Extensión](#personalización-y-extensión)
6. [Mejoras Futuras](#mejoras-futuras)

---

## 🎯 Concepto General

Este generador crea mazmorras procedurales con las siguientes características:

- **Una sola planta**: Todas las habitaciones están en el mismo nivel
- **Estructura conectada**: Habitaciones unidas por pasillos
- **Generación procedural**: Cada mazmorra es única
- **Tipos de habitaciones variados**: Oficinas, almacenes, habitaciones grandes, etc.
- **Sistema de cuadrícula**: Basado en una cuadrícula para posicionamiento preciso

---

## 🏗️ Arquitectura del Sistema

### Flujo de Generación

```
1. Inicialización
   └─> Crear cuadrícula vacía
   └─> Limpiar habitaciones anteriores

2. Generación de Habitaciones
   └─> Crear habitación inicial (Entrance) en el centro
   └─> Generar habitaciones aleatorias respetando separación mínima
   └─> Asignar tipos de habitaciones según probabilidades

3. Conexión de Habitaciones
   └─> Conectar habitaciones adyacentes (probabilidad configurable)
   └─> Asegurar que todas las habitaciones sean accesibles (BFS)
   └─> Crear pasillos visuales entre habitaciones conectadas

4. Finalización
   └─> Spawnear jugador en la entrada
   └─> Aplicar efectos y contenido a las habitaciones
```

### Estructura de Datos

- **Cuadrícula (Grid)**: Matriz 2D que representa el espacio de la mazmorra
  - `0` = Vacío
  - `1` = Habitación
  - `2` = Pasillo

- **Nodos de Habitación**: Cada habitación es un nodo con:
  - Posición en la cuadrícula (GridX, GridY)
  - Posición en el mundo (WorldPosition)
  - Tipo de habitación
  - Lista de habitaciones conectadas
  - Estado (visitada, limpiada, etc.)

---

## 🔧 Componentes Principales

### 1. `ProceduralDungeonGenerator`

**Responsabilidades:**
- Generar la estructura completa de la mazmorra
- Gestionar la cuadrícula de generación
- Conectar habitaciones
- Asegurar accesibilidad

**Parámetros Importantes:**
- `gridWidth` / `gridHeight`: Tamaño de la cuadrícula
- `cellSize`: Tamaño de cada celda en unidades del mundo
- `minRooms` / `maxRooms`: Rango de habitaciones a generar
- `minRoomSeparation`: Separación mínima entre habitaciones
- `connectionChance`: Probabilidad de conectar habitaciones adyacentes

### 2. `DungeonRoom`

**Responsabilidades:**
- Representar una habitación individual
- Gestionar conexiones con otras habitaciones
- Contener enemigos, loot y props
- Manejar estado (visitada, limpiada)

**Propiedades Clave:**
- `RoomType`: Tipo de habitación
- `ConnectedRooms`: Lista de habitaciones conectadas
- `IsVisited` / `IsCleared`: Estado de la habitación
- `HasEnemies` / `HasLoot`: Contenido de la habitación

### 3. `DungeonRoomType` (Enum)

Tipos de habitaciones disponibles:
- `Entrance`: Punto de entrada (siempre en el centro)
- `Office`: Oficina pequeña
- `Storage`: Almacén (mayor probabilidad de loot)
- `LargeRoom`: Habitación grande (más enemigos)
- `DeadEnd`: Callejón sin salida (alto loot, pocos enemigos)

---

## 🚀 Cómo Usar el Generador

### Configuración Básica

1. **Crear un GameObject vacío** en la escena
2. **Agregar el componente** `ProceduralDungeonGenerator`
3. **Configurar los parámetros** en el Inspector:
   - Ajustar tamaño de cuadrícula
   - Configurar rango de habitaciones
   - Asignar prefabs de habitaciones y pasillos

### Configuración de Prefabs

**Opción 1: Prefab Genérico**
- Asignar un `roomPrefab` genérico que se usará para todos los tipos

**Opción 2: Prefabs Específicos**
- Crear un ScriptableObject o asignar directamente en el Inspector:
  - `entrancePrefab`
  - `officePrefab`
  - `storagePrefab`
  - `largeRoomPrefab`
  - `deadEndPrefab`

### Ejemplo de Uso en Código

```csharp
// Obtener referencia al generador
ProceduralDungeonGenerator generator = GetComponent<ProceduralDungeonGenerator>();

// Generar mazmorra
generator.GenerateDungeon();

// Regenerar mazmorra
generator.RegenerateDungeon();

// Obtener información
List<DungeonRoom> rooms = generator.GetGeneratedRooms();
DungeonRoom startRoom = generator.GetStartRoom();
```

---

## 🎨 Personalización y Extensión

### Agregar Nuevos Tipos de Habitaciones

1. **Agregar al enum** `DungeonRoomType`:
```csharp
public enum DungeonRoomType
{
    // ... tipos existentes
    Laboratory,  // Nuevo tipo
    SecurityRoom // Otro nuevo tipo
}
```

2. **Actualizar** `GetRandomRoomType()` en el generador para incluir los nuevos tipos

3. **Agregar lógica específica** en `DungeonRoom.SetupRoomProperties()`

### Modificar Probabilidades de Conexión

Ajustar `connectionChance` en el Inspector:
- **Valores bajos (30-50%)**: Mazmorras más laberínticas
- **Valores altos (70-90%)**: Mazmorras más abiertas y conectadas

### Implementar Spawn de Enemigos Real

En `DungeonRoom.SpawnEnemies()`:
```csharp
private void SpawnEnemies()
{
    int enemyCount = CalculateEnemyCount();
    
    for (int i = 0; i < enemyCount; i++)
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
        Enemies.Add(enemy);
    }
}
```

### Implementar Spawn de Loot Real

Similar al de enemigos, pero usando prefabs de loot:
```csharp
private void SpawnLoot()
{
    int lootCount = CalculateLootCount();
    
    for (int i = 0; i < lootCount; i++)
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject loot = Instantiate(lootPrefab, spawnPos, Quaternion.identity, transform);
        LootItems.Add(loot);
    }
}
```

### Sistema de Sockets para Conexiones Precisas

Para conexiones más precisas entre habitaciones, puedes usar el sistema de sockets:

1. **Agregar puntos de conexión** en los prefabs de habitaciones
2. **Usar `RoomSocket`** para marcar puntos de entrada/salida
3. **Conectar pasillos** desde los sockets en lugar de desde el centro

---

## 🔮 Mejoras Futuras

### Características Sugeridas

1. **Sistema de Puertas**
   - Puertas que se abren/cierran
   - Puertas bloqueadas que requieren llaves
   - Puertas con puzzles

2. **Variación de Tamaños**
   - Habitaciones de diferentes tamaños
   - Pasillos de diferentes anchos
   - Habitaciones irregulares

3. **Decoración Procedural**
   - Props que se spawnean según el tipo de habitación
   - Variación en la disposición de muebles
   - Efectos de iluminación dinámicos

4. **Sistema de Eventos**
   - Eventos aleatorios en habitaciones
   - Trampas
   - Encuentros especiales

5. **Optimización**
   - Culling de habitaciones lejanas
   - Generación asíncrona mejorada
   - Pooling de objetos

6. **Navegación**
   - Integración con NavMesh
   - Generación automática de NavMesh por habitación
   - Waypoints para IA

7. **Guardado/Carga**
   - Serialización del estado de la mazmorra
   - Guardado de habitaciones visitadas
   - Persistencia del contenido de habitaciones

### Algoritmos Alternativos

**Binary Space Partitioning (BSP)**
- Dividir el espacio recursivamente
- Crear habitaciones en cada partición
- Más control sobre el tamaño de habitaciones

**Cellular Automata**
- Generar estructura inicial con reglas simples
- Aplicar reglas de evolución
- Resultados más orgánicos

**Minimum Spanning Tree (MST)**
- Garantizar conexión mínima
- Agregar conexiones adicionales aleatorias
- Menos pasillos redundantes

---

## 📝 Notas de Implementación

### Consideraciones de Rendimiento

- **Generación asíncrona**: Usar corrutinas para mazmorras grandes
- **Límite de habitaciones**: No exceder 50-100 habitaciones sin optimización
- **Culling**: Considerar ocultar habitaciones lejanas

### Debugging

El generador incluye visualización con Gizmos:
- **Verde**: Habitación de entrada
- **Azul**: Oficinas
- **Cian**: Almacenes
- **Magenta**: Habitaciones grandes
- **Rojo**: Callejones sin salida
- **Amarillo**: Conexiones entre habitaciones

Activa Gizmos en la vista Scene para ver la estructura.

---

## 🐛 Solución de Problemas

### Las habitaciones se superponen
- Aumentar `minRoomSeparation`
- Reducir `maxRooms`
- Aumentar tamaño de la cuadrícula

### No todas las habitaciones son accesibles
- Asegurar que `ensureAllRoomsAccessible` esté activado
- Aumentar `connectionChance`

### La generación es muy lenta
- Activar `useCoroutines`
- Reducir `maxRooms`
- Reducir tamaño de la cuadrícula

### El jugador spawnea fuera de la mazmorra
- Verificar que la habitación inicial se crea correctamente
- Verificar posición del jugador en relación a `startRoom`

---

## 📚 Referencias

- **Generación Procedural**: Conceptos generales de generación procedural
- **Procedural Generation**: Conceptos generales de generación procedural
- **Graph Theory**: Para algoritmos de conexión (BFS, MST)

---

¡Buena suerte con tu generador de mazmorras! 🎮

