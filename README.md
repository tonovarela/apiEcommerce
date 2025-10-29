# API Ecommerce

API REST desarrollada con ASP.NET Core 8.0 para gestión de ecommerce, implementando Entity Framework Core con SQL Server.

## 🚀 Características

- **Framework**: ASP.NET Core 8.0
- **Base de Datos**: SQL Server con Entity Framework Core 9.0
- **Documentación**: Swagger/OpenAPI integrado
- **Contenedores**: Docker Compose para SQL Server
- **Arquitectura**: Clean Architecture con Repository Pattern

## 📋 Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop) (para la base de datos)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

## 🛠️ Instalación y Configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/tonovarela/apiEcommerce.git
cd apiEcommerce
```

### 2. Configurar la base de datos

#### Opción A: Usar Docker (Recomendado)

```bash
# Navegar al directorio del proyecto
cd ApiEcommerce

# Iniciar SQL Server en Docker
docker-compose up -d
```

Esto creará un contenedor con SQL Server 2022 con las siguientes credenciales:
- **Server**: localhost,1433
- **Usuario**: sa
- **Contraseña**: MyStrongPass123
- **Base de Datos**: ApiEcommerceNET8

#### Opción B: SQL Server local

Si prefieres usar una instancia local de SQL Server, modifica la cadena de conexión en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ConexionSql": "Server=tu-servidor;Database=ApiEcommerceNET8;Integrated Security=true;TrustServerCertificate=true"
  }
}
```

### 3. Ejecutar migraciones

```bash
# Navegar al directorio del proyecto
cd ApiEcommerce

# Aplicar migraciones a la base de datos
dotnet ef database update
```

### 4. Ejecutar la aplicación

```bash
# Restaurar dependencias
dotnet restore

# Ejecutar en modo desarrollo
dotnet run
```

La API estará disponible en:
- **HTTPS**: https://localhost:7219
- **HTTP**: http://localhost:5219
- **Swagger UI**: https://localhost:7219/swagger

## 📁 Estructura del Proyecto

```
ApiEcommerce/
├── Controllers/          # Controladores de la API
├── Data/                # Contexto de Entity Framework
│   └── ApplicationDBContext.cs
├── Models/              # Modelos de datos
│   ├── Entities/        # Entidades del dominio
│   │   └── Category.cs
│   └── Dtos/           # Data Transfer Objects
├── Repository/          # Patrón Repository
│   └── IRepository/    # Interfaces del repositorio
├── Mapping/            # Configuración de AutoMapper
├── Migrations/         # Migraciones de Entity Framework
├── Properties/         # Configuración de launch
├── appsettings.json    # Configuración de la aplicación
├── docker-compose.yml  # Configuración de Docker
└── Program.cs          # Punto de entrada de la aplicación
```

## 🗂️ Modelos de Datos

### Category (Categoría)

```csharp
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
}
```

## 🔧 Tecnologías Utilizadas

- **ASP.NET Core 8.0** - Framework web
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Base de datos
- **Swagger/OpenAPI** - Documentación de API
- **Docker** - Containerización
- **AutoMapper** - Mapeo de objetos (configurado)

### Paquetes NuGet

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.10" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
```

## 🐳 Docker

### Comandos útiles de Docker Compose

```bash
# Iniciar servicios en segundo plano
docker-compose up -d

# Ver logs de los servicios
docker-compose logs

# Detener servicios
docker-compose down

# Detener y eliminar volúmenes (⚠️ Borra datos)
docker-compose down -v
```

## 🔨 Comandos de Entity Framework

```bash
# Crear una nueva migración
dotnet ef migrations add NombreDeLaMigracion

# Aplicar migraciones pendientes
dotnet ef database update

# Revertir a una migración específica
dotnet ef database update NombreDeLaMigracion

# Eliminar la última migración (solo si no se ha aplicado)
dotnet ef migrations remove

# Ver el estado de las migraciones
dotnet ef migrations list
```

## 🚦 Endpoints de la API

Una vez que el proyecto esté ejecutándose, puedes acceder a la documentación completa de la API en:

**Swagger UI**: https://localhost:7219/swagger

### Endpoints principales (ejemplo)

```
GET    /api/categories      # Obtener todas las categorías
GET    /api/categories/{id} # Obtener una categoría por ID
POST   /api/categories      # Crear una nueva categoría
PUT    /api/categories/{id} # Actualizar una categoría
DELETE /api/categories/{id} # Eliminar una categoría
```

## 🧪 Pruebas

### Archivo de pruebas HTTP

El proyecto incluye un archivo `ApiEcommerce.http` para realizar pruebas rápidas de los endpoints.

### Usar con herramientas externas

- **Postman**: Importa la especificación OpenAPI desde `/swagger/v1/swagger.json`
- **Insomnia**: Usa la URL base `https://localhost:7219`
- **curl**: Ejemplos disponibles en Swagger UI

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📝 Notas de Desarrollo

### Configuración de Desarrollo

- El proyecto usa **nullable reference types** habilitado
- **Implicit usings** están habilitados para mayor limpieza del código
- La API está configurada para usar HTTPS por defecto en desarrollo

### Variables de Entorno

Para diferentes entornos, puedes configurar:

```bash
# Desarrollo
ASPNETCORE_ENVIRONMENT=Development

# Producción
ASPNETCORE_ENVIRONMENT=Production
```

### Logging

El proyecto está configurado con logging por defecto:
- **Information** para logs generales
- **Warning** para ASP.NET Core específicamente

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE.md](LICENSE.md) para más detalles.

## 👥 Autor

**Antonio Varela** - [@tonovarela](https://github.com/tonovarela)

---

⭐ ¡No olvides dar una estrella si este proyecto te fue útil!
