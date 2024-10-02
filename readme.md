
# Proyecto PruebaTecnica_DVP_Net_Kubernetes

## Descripción
Este proyecto es una API basada en .NET para la gestión de tareas, usuarios y roles, utilizando Entity Framework Core y autenticación mediante JWT (JSON Web Tokens). El sistema incluye roles como Administrador, Supervisor y Empleado, con diferentes niveles de acceso y permisos.

## Arquitectura del Proyecto

### Capa de Modelos (Models):
En esta capa se encuentran las entidades principales como `User`, `Role`, `WorkTask`, `WorkTaskState`, que representan el dominio del problema (gestión de tareas y roles de usuario). Estas clases son las que se mapean directamente a las tablas de la base de datos mediante Entity Framework Core.

### Capa de Servicios (Services):
Aquí se encuentra la lógica de negocio. Cada servicio maneja operaciones específicas de una parte del dominio, por ejemplo:
- `UserService` para gestionar usuarios.
- `WorkTaskService` para gestionar tareas.
- `RoleService` para gestionar roles.
- `TaskStateService` para gestionar los estados de las tareas.

Estos servicios interactúan directamente con los repositorios o bases de datos para aplicar las reglas de negocio.

### Capa de Controladores (Controllers):
Utiliza ASP.NET Core para definir los endpoints de la API. Los controladores sirven como intermediarios entre la capa de presentación (front-end o cliente) y la capa de servicios, manejando las solicitudes HTTP y devolviendo respuestas JSON.

### Capa de Repositorio (Data/Repositories):
Se utilizan para encapsular el acceso a la base de datos. Aquí se encuentra la lógica de persistencia, utilizando Entity Framework Core para hacer queries a la base de datos. Este patrón es útil para desacoplar la lógica de acceso a datos del resto de la aplicación.

### Capa de DTOs (Data Transfer Objects):
Los DTOs (`Dtos folder`) son usados para la transferencia de datos entre el front-end y back-end. Estos objetos son simples, sin lógica, y sólo contienen las propiedades necesarias para enviar o recibir información de la API.

### Capa de Filtros (Filters):
Contiene filtros personalizados como `EncryptResponseFilter`, los cuales permiten interceptar las respuestas de la API para aplicar cifrado, por ejemplo, si el cifrado está habilitado.

### Capa de Configuración (MappingProfile):
La configuración de AutoMapper en `MappingProfile.cs` se utiliza para mapear las entidades de dominio a los DTOs y viceversa. AutoMapper facilita la conversión de objetos complejos sin necesidad de código repetitivo.

### Autenticación y Autorización (Token):
Se utiliza JWT (JSON Web Tokens) para la autenticación, con roles para controlar el acceso a las diferentes funcionalidades del sistema. El `JwtGenerator` genera los tokens JWT que se envían al cliente después del inicio de sesión exitoso.

### Capa de Migraciones (Migrations):
Contiene el historial de migraciones de la base de datos que permite versionar el esquema de la base de datos con los modelos de Entity Framework.

## Flujo de Trabajo del Proyecto

### Autenticación y Autorización:
El sistema utiliza Identity y JWT para autenticar usuarios. Según el rol asignado (Administrador, Supervisor, Empleado), los usuarios tienen acceso a diferentes funcionalidades como la creación, edición y eliminación de tareas o usuarios.

### Manejo de Tareas:
El administrador y supervisor pueden gestionar (CRUD) las tareas asignándolas a los usuarios. Los empleados sólo pueden visualizar y cambiar el estado de las tareas que les han sido asignadas.

### Cifrado:
Se implementa un filtro de cifrado en las respuestas de la API (`EncryptResponseFilter`). Según la configuración en el `appsettings.json`, se puede activar o desactivar este cifrado.

### Base de Datos:
Utiliza SQL Server como sistema de gestión de base de datos, y Entity Framework Core como ORM para manejar las operaciones de base de datos.

## Configuración
1. Clonar el repositorio.
2. Configurar la cadena de conexión a la base de datos en `appsettings.json`.
3. Ejecutar las migraciones de la base de datos: `dotnet ef database update`.
4. Ejecutar el proyecto: `dotnet run`.

## Despliegue
El proyecto está configurado para ser desplegado en Heroku con Docker, también para local con docker. Asegúrate de configurar correctamente las variables de entorno para la cadena de conexión de la base de datos y las claves JWT.

