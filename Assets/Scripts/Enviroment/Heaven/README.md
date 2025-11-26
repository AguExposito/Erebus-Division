# Heaven Dungeon Generator

Un generador automático de mazmorras con temática celestial que utiliza una matriz multidimensional para definir el ancho y largo en bloques, generando habitaciones interconectadas aleatoriamente.

## Características

- **Generación basada en matriz**: Define el tamaño de la mazmorra usando una matriz multidimensional
- **Habitaciones temáticas del cielo**: Diferentes tipos de habitaciones con temática celestial
- **Conexiones automáticas**: Los pasillos se generan automáticamente entre habitaciones cercanas
- **Sistema de sockets**: Control preciso de las conexiones entre habitaciones
- **Efectos visuales**: Iluminación y efectos especiales temáticos del cielo
- **Generación por corrutinas**: Evita caídas de FPS durante la generación

## Componentes Principales

### 1. HeavenDungeonGenerator
El componente principal que maneja la generación de la mazmorra.

**Configuración:**
- `dungeonWidth`: Ancho de la mazmorra en bloques
- `dungeonHeight`: Alto de la mazmorra en bloques
- `blockSize`: Tamaño de cada bloque en unidades del mundo
- `minRooms` / `maxRooms`: Número mínimo y máximo de habitaciones
- `heavenRoomPrefab`: Prefab para las habitaciones del cielo
- `heavenHallwayPrefab`: Prefab para los pasillos

### 2. HeavenRoom
Representa una habitación individual con sus propiedades y conexiones.

**Tipos de habitaciones:**
- `Sanctuary`: Habitación de inicio (verde)
- `Chapel`: Capilla regular (azul)
- `Altar`: Altar especial (amarillo)
- `Garden`: Jardín pacífico (cian)
- `Library`: Biblioteca (magenta)
- `Treasury`: Tesorería (rojo)
- `Boss`: Habitación del jefe final (negro)

### 3. HeavenRoomSocket
Maneja las conexiones entre habitaciones.

**Tipos de sockets:**
- `Entrance`: Entrada
- `Exit`: Salida
- `Connection`: Conexión normal
- `Secret`: Conexión secreta

### 4. HeavenHallway
Representa los pasillos que conectan las habitaciones.

### 5. HeavenDungeonSettings
ScriptableObject para configurar la generación.

## 🚀 Configuración Rápida

### **Paso 1: Crear el Generador**
1. Crea un **GameObject vacío** en tu escena
2. Nómbralo `HeavenDungeonGenerator`
3. Añade el componente `HeavenDungeonGenerator`

### **Paso 2: Configuración Básica (OBLIGATORIO)**
En el inspector, configura estos valores:

```
Dungeon Matrix Settings:
├── Dungeon Width: 8        (ancho en bloques)
├── Dungeon Height: 8       (alto en bloques)  
└── Block Size: 3.0         (tamaño de cada bloque)

Room Generation:
├── Min Rooms: 4            (mínimo de habitaciones)
└── Max Rooms: 6            (máximo de habitaciones)

Generation Settings:
├── Use Coroutines: ✓       (evita caídas de FPS)
└── Max Rooms Per Frame: 2  (habitaciones por frame)
```

### **Paso 3: Probar la Generación**

**Opción A - Script Simple:**
Añade el componente `HeavenDungeonSimple` para configuración automática básica.

**Opción B - Script Avanzado:**
Añade el componente `HeavenDungeonExample` y usa:
- **R** - Regenerar mazmorra
- **M** - Ver matriz en consola
- **P** - Ver posiciones de habitaciones

> 📖 **Guía Completa**: Ver `SETUP_GUIDE.md` para configuración detallada

### 2. Generación Automática

La mazmorra se genera automáticamente al iniciar la escena. También puedes regenerarla usando:

```csharp
// Regenerar la mazmorra
dungeonGenerator.RegenerateDungeon();

// Limpiar la mazmorra
dungeonGenerator.ClearDungeon();
```

### 3. Uso del Ejemplo

Incluye el script `HeavenDungeonExample` para probar la funcionalidad:

- **R**: Regenerar mazmorra
- **C**: Limpiar mazmorra
- **D**: Alternar modo debug
- **M**: Imprimir matriz en consola
- **P**: Imprimir posiciones de habitaciones

### 4. Configuración Rápida

Usa `HeavenDungeonConfig` para configuraciones predefinidas:

```csharp
// Configuraciones predefinidas
config.SetSmallDungeon();   // 5x5, 3-4 habitaciones
config.SetMediumDungeon();  // 8x8, 5-7 habitaciones  
config.SetLargeDungeon();   // 12x12, 8-12 habitaciones

// Configuración personalizada
config.SetDungeonSize(10, 10);
config.SetRoomCount(6, 8);
config.SetBlockSize(2.5f);
```

## Configuración Avanzada

### Matriz de la Mazmorra

La mazmorra se define usando una matriz 2D donde:
- `0` = Espacio vacío
- `1` = Habitación ocupada

**Mejoras implementadas:**
- ✅ **Posicionamiento correcto**: Las habitaciones ahora se posicionan correctamente según la matriz
- ✅ **Centrado automático**: La mazmorra se centra alrededor del origen (0,0,0)
- ✅ **Conexiones inteligentes**: Los pasillos se crean solo entre habitaciones adyacentes en la matriz
- ✅ **Prevención de bucles infinitos**: Límite de intentos para evitar colgarse
- ✅ **Debug mejorado**: Métodos para visualizar la matriz y posiciones

```csharp
// Acceder a la matriz
int[,] matrix = dungeonGenerator.GetDungeonMatrix();
int width = matrix.GetLength(0);
int height = matrix.GetLength(1);

// Visualizar matriz en consola
dungeonGenerator.PrintMatrix();

// Ver posiciones de habitaciones
dungeonGenerator.PrintRoomPositions();
```

### Personalización de Habitaciones

```csharp
// Obtener habitación específica
HeavenRoom room = dungeonGenerator.GetRoomAt(x, y);

// Obtener habitación por tipo
HeavenRoom sanctuary = dungeonGenerator.GetRoomByType(HeavenRoomType.Sanctuary);

// Visitar habitación
room.VisitRoom();

// Limpiar habitación
room.ClearRoom();
```

### Configuración de Conexiones

```csharp
// Conectar habitaciones
room1.ConnectToRoom(room2);

// Desconectar habitaciones
room1.DisconnectFromRoom(room2);

// Verificar conexión
bool isConnected = room1.IsConnectedTo(room2);
```

## Temática del Cielo

### Colores y Materiales
- **Suelos**: Materiales dorados y blancos
- **Paredes**: Texturas celestiales
- **Iluminación**: Colores cálidos y dorados
- **Efectos**: Partículas y luces suaves

### Tipos de Decoración
- Altares y símbolos religiosos
- Plantas y elementos naturales
- Libros y elementos de conocimiento
- Tesoros celestiales

## Optimización

### Generación por Corrutinas
```csharp
// Habilitar generación por corrutinas para evitar caídas de FPS
dungeonGenerator.useCoroutines = true;
dungeonGenerator.maxRoomsPerFrame = 3;
```

### Configuración de Rendimiento
- Usa habitaciones más grandes para reducir el número total
- Limita el número máximo de habitaciones
- Usa LOD (Level of Detail) para decoraciones distantes

## Debugging

### Visualización en Scene View
- Las habitaciones se muestran con colores diferentes según su tipo
- Las conexiones se muestran como líneas amarillas
- Los sockets se muestran como esferas

### Logs de Debug
```csharp
// Habilitar logs detallados
Debug.Log("Dungeon generated with " + rooms.Count + " rooms");
```

## Ejemplos de Uso

### Generar Mazmorra Pequeña
```csharp
dungeonGenerator.dungeonWidth = 5;
dungeonGenerator.dungeonHeight = 5;
dungeonGenerator.minRooms = 3;
dungeonGenerator.maxRooms = 5;
dungeonGenerator.RegenerateDungeon();
```

### Generar Mazmorra Grande
```csharp
dungeonGenerator.dungeonWidth = 15;
dungeonGenerator.dungeonHeight = 15;
dungeonGenerator.minRooms = 8;
dungeonGenerator.maxRooms = 12;
dungeonGenerator.useCoroutines = true;
dungeonGenerator.RegenerateDungeon();
```

## Notas Importantes

1. **Prefabs Requeridos**: Asegúrate de tener prefabs para habitaciones, pasillos, suelos y paredes
2. **Materiales**: Asigna materiales temáticos del cielo para mejor apariencia
3. **Iluminación**: La iluminación se ajusta automáticamente según el tipo de habitación
4. **NavMesh**: Considera rebakear el NavMesh después de la generación para IA

## Troubleshooting

### La mazmorra no se genera
- Verifica que los prefabs estén asignados
- Comprueba que los valores de la matriz sean válidos
- Revisa los logs de consola para errores

### Rendimiento bajo
- Habilita la generación por corrutinas
- Reduce el número de habitaciones
- Usa habitaciones más grandes

### Conexiones incorrectas
- Verifica la distancia máxima de conexión
- Asegúrate de que las habitaciones estén lo suficientemente cerca
- Revisa la configuración de los sockets
