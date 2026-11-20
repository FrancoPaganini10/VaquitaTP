# VaquitaTP
Trabajo Final Programación 4 

## Configuración de JWT

En desarrollo, configurá la clave localmente con user-secrets desde el directorio
`Vaquita`:

```powershell
dotnet user-secrets set "JwtKey" "<clave-aleatoria-de-al-menos-32-caracteres>"
```

En otros entornos, configurá la variable de entorno `JwtKey`. La aplicación
falla al iniciar con un mensaje claro si la clave no está configurada.
