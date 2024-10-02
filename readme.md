
# TaskFlowManager API

## Descripción del Proyecto

TaskFlowManager es una API construida con ASP.NET Core que permite la gestión de usuarios, roles y tareas de trabajo (WorkTasks). Proporciona autenticación mediante JWT y permite a los usuarios con diferentes roles (Administrador, Supervisor, Empleado) gestionar tareas de manera efectiva.

## Despliegue en Heroku

La API ha sido desplegada en Heroku y puede ser accedida en la siguiente URL:
**TaskFlowManager API en Heroku: https://api-task-manager-dotnet-daf3f964d672.herokuapp.com/api/**

### Usuario Administrador por Defecto:
- **Email**: dorado@yopmail.com
- **Contraseña**: 4Mt0XvfT*

Este usuario tiene privilegios administrativos completos, como la creación, edición y eliminación de usuarios y tareas.

## Estructura del Proyecto

El proyecto sigue una arquitectura en capas bien definida:

### 1. Capa de Modelos (Models)

Contiene las clases principales como `User`, `Role`, `WorkTask`, `WorkTaskState`, las cuales representan el dominio del problema (gestión de tareas y roles de usuario). Estas clases están mapeadas directamente a las tablas de la base de datos usando Entity Framework Core.

### 2. Capa de Servicios (Services)

Aquí reside la lógica de negocio de la aplicación. Cada servicio gestiona operaciones específicas:
- `UserService`: Gestión de usuarios.
- `WorkTaskService`: Gestión de tareas.
- `RoleService`: Gestión de roles.
- `TaskStateService`: Gestión de los estados de las tareas.

### 3. Capa de Controladores (Controllers)

Utiliza ASP.NET Core para exponer los endpoints de la API. Los controladores actúan como intermediarios entre la capa de servicios y el cliente (por ejemplo, el front-end). Los controladores manejan solicitudes HTTP y retornan respuestas en formato JSON.

### 4. Capa de Repositorios (Repositories)

Los repositorios encapsulan la lógica de acceso a datos. Aquí se utilizan consultas a la base de datos a través de Entity Framework Core. Este patrón de diseño permite separar la lógica de negocio del acceso a datos.

### 5. Capa de DTOs (Data Transfer Objects)

Los DTOs permiten la transferencia de datos entre el cliente y la API. Son simples objetos que contienen únicamente las propiedades necesarias para la comunicación.

### 6. Capa de Filtros (Filters)

Incluye filtros personalizados como `EncryptResponseFilter`, los cuales permiten interceptar y modificar las respuestas de la API, como por ejemplo aplicar cifrado a las respuestas si está habilitado.

### 7. Configuración de Mapeo (Mapping Profile)

AutoMapper es utilizado para mapear entidades a DTOs y viceversa, lo que simplifica la conversión de objetos dentro de la aplicación.

### 8. Autenticación y Autorización

Se utiliza JWT (JSON Web Token) para la autenticación de los usuarios. Los roles definen qué acciones pueden realizar dentro de la API. El `JwtGenerator` es responsable de generar los tokens JWT que son enviados al cliente tras un inicio de sesión exitoso.

### 9. Migraciones (Migrations)

Entity Framework Core se utiliza para gestionar las migraciones de la base de datos, permitiendo aplicar cambios incrementales al esquema de la base de datos conforme evoluciona el proyecto.

## Flujo de Trabajo del Proyecto

### 1. Autenticación y Autorización
El sistema de autenticación utiliza Identity y JWT para verificar a los usuarios. Según el rol asignado, los usuarios tendrán diferentes permisos para acceder y modificar datos en la API.

### 2. Gestión de Tareas
Los usuarios con los roles de Administrador y Supervisor pueden crear, modificar y asignar tareas a otros usuarios. Los empleados solo pueden ver y actualizar el estado de sus tareas asignadas.

### 3. Cifrado de Respuestas
Dependiendo de la configuración en el archivo `appsettings.json`, las respuestas de la API pueden ser cifradas utilizando `EncryptResponseFilter`.

### 4. Base de Datos
Se utiliza SQL Server como sistema de gestión de bases de datos, y Entity Framework Core como ORM para interactuar con la base de datos. La cadena de conexión está configurada en el archivo `appsettings.json`.

## Creación Automática de Datos

En la primera ejecución de la aplicación, si no existen usuarios ni roles en la base de datos, el sistema creará automáticamente los siguientes datos:
- **Roles**: Administrador, Supervisor y Empleado.
- **Usuario Administrador por Defecto**: El usuario administrador será creado con los datos configurados en el archivo `appsettings.json`.

Esta creación automática de datos asegura que el sistema esté listo para su uso después de la primera ejecución sin necesidad de intervención manual.

## Archivo de Configuración (appsettings.json)

El archivo `appsettings.json` contiene configuraciones clave del proyecto, tales como:
- Configuración del JWT (secreto y expiración del token).
- Configuración de la base de datos (cadena de conexión).
- Configuración del cifrado de respuestas.
- Datos del usuario administrador por defecto.

**Nota**: El archivo `appsettings.json` no debe ser incluido en el repositorio público debido a que contiene información sensible como contraseñas y secretos.

## Colección de Postman

Dentro de la base del proyecto, encontrarás un archivo llamado `API Task Flow Manager.postman_collection.json`, que contiene la mayoría de los endpoints para realizar pruebas de la API.

## Instalación y Ejecución Local

### Requisitos:
- .NET 6.0 SDK
- SQL Server (Local o en Azure)

### Pasos para la instalación:
1. Clonar el repositorio.
2. Configurar la cadena de conexión a la base de datos en `appsettings.json`.
3. Ejecutar las migraciones para configurar la base de datos:  
   ```bash
   dotnet ef database update
   ```
4. Ejecutar la aplicación localmente:  
   ```bash
   dotnet run
   ```

### Ejecución con Docker:
La API también puede ser ejecutada dentro de un contenedor Docker. Asegúrate de que Docker esté instalado y configurado correctamente en tu sistema.
