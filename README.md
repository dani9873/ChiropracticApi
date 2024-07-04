# Chiropractic API

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET Core](https://img.shields.io/badge/.NET%20Core-5.0-blue)

## Descripción

La **Chiropractic API** es una aplicación diseñada para gestionar los servicios de una clínica quiropráctica. Permite la administración de usuarios, roles, servicios, imágenes asociadas a los servicios, citas y el historial de citas.

## Características

- **Gestión de usuarios**: Registro, autenticación y roles.
- **Servicios**: Creación, actualización y eliminación de servicios quiroprácticos.
- **Citas**: Gestión de citas, incluyendo creación, actualización y cancelación.
- **Historial de citas**: Registro y seguimiento del historial de citas de los pacientes.
- **Imágenes de servicios**: Asignación y gestión de imágenes a los servicios quiroprácticos.

## Tecnologías Utilizadas

- **.NET Core 5.0**
- **Entity Framework Core**
- **JWT para autenticación**
- **AutoMapper**
- **BCrypt.Net para hashing de contraseñas**
- **SQL Server/MySQL** para base de datos

## Requisitos Previos

- **.NET SDK 5.0 o superior**
- **SQL Server/MySQL**
- **Visual Studio 2019 o superior** (opcional)

## Configuración del Proyecto

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/dani9873/ChiropracticApi
   cd chiropracticApi

## Endpoints Principales

### Autenticación

- `POST /api/auth/register`: Registrar un nuevo usuario.
- `POST /api/auth/login`: Autenticar un usuario y generar un token JWT.

### Usuarios

- `GET /api/users`: Obtener todos los usuarios.
- `GET /api/users/{id}`: Obtener un usuario por ID.
- `PUT /api/users/{id}`: Actualizar un usuario.
- `DELETE /api/users/{id}`: Eliminar un usuario.

### Servicios

- `GET /api/services`: Obtener todos los servicios.
- `GET /api/services/{id}`: Obtener un servicio por ID.
- `POST /api/services`: Crear un nuevo servicio.
- `PUT /api/services/{id}`: Actualizar un servicio.
- `DELETE /api/services/{id}`: Eliminar un servicio.

### Citas

- `GET /api/appointments`: Obtener todas las citas.
- `GET /api/appointments/{id}`: Obtener una cita por ID.
- `POST /api/appointments`: Crear una nueva cita.
- `PUT /api/appointments/{id}`: Actualizar una cita.
- `DELETE /api/appointments/{id}`: Eliminar una cita.

### Historial de Citas

- `GET /api/appointment-history`: Obtener el historial de citas.
- `GET /api/appointment-history/{id}`: Obtener un historial de citas por ID.
- `POST /api/appointment-history`: Crear un nuevo registro en el historial de citas.
- `PUT /api/appointment-history/{id}`: Actualizar un registro en el historial de citas.
- `DELETE /api/appointment-history/{id}`: Eliminar un registro del historial de citas.

## Comandos .NET para ASP.NET Core

### Crear un nuevo proyecto ASP.NET Core Web API
   dotnet new webapi -n MiProyecto
   cd MiProyecto

### Agregar paquetes necesarios para JWT y Entity Framework Core
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package Microsoft.EntityFrameworkCore.Design
### Crear controladores y modelos
    dotnet new controller -n UserController
    dotnet new controller -n ServiceController
    dotnet new controller -n AppointmentController
    dotnet new controller -n AppointmentHistoryController
    dotnet new controller -n AuthController
### Generar migraciones y actualizar base de datos (Entity Framework Core)
    dotnet ef migrations add InitialCreate
    dotnet ef database update
### Ejecutar la aplicación
    dotnet run



