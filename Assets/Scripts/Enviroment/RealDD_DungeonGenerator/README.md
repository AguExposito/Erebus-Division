# Generador de Mazmorras Procedurales

Sistema completo de generación procedural de mazmorras en una sola planta.

## 🚀 Inicio Rápido

1. **Agregar el generador a la escena:**
   - Crear un GameObject vacío
   - Agregar componente `ProceduralDungeonGenerator`
   - Configurar parámetros en el Inspector

2. **Configurar prefabs:**
   - Asignar prefabs de habitaciones y pasillos
   - O crear un `RoomPrefabsSO` ScriptableObject

3. **Generar mazmorra:**
   - La mazmorra se genera automáticamente al iniciar
   - O llamar `GenerateDungeon()` manualmente

## 📁 Archivos Incluidos

- **ProceduralDungeonGenerator.cs**: Generador principal
- **DungeonRoom.cs**: Componente de habitación individual
- **RoomPrefabsSO.cs**: ScriptableObject para prefabs
- **EjemploUso.cs**: Ejemplos de uso del sistema
- **GUIA_DESARROLLO.md**: Guía completa de desarrollo

## 🎮 Características

✅ Generación procedural en una sola planta  
✅ Sistema de cuadrícula para posicionamiento preciso  
✅ Múltiples tipos de habitaciones  
✅ Conexiones automáticas entre habitaciones  
✅ Asegura que todas las habitaciones sean accesibles  
✅ Generación asíncrona con corrutinas  
✅ Sistema de sockets para conexiones precisas  
✅ Debug visualization con Gizmos  

## 📖 Documentación

Ver `GUIA_DESARROLLO.md` para documentación completa.

## 🔧 Próximos Pasos

1. Crear prefabs de habitaciones
2. Configurar prefabs en el generador
3. Ajustar parámetros según tus necesidades
4. Implementar spawn de enemigos y loot
5. Personalizar tipos de habitaciones

