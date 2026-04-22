# Portal Académico — Gestión de Cursos y Matrículas

## Stack

- ASP.NET Core MVC (.NET 8) + Identity
- EF Core con SQLite
- Razor Views

## Cómo correr localmente

git clone https://github.com/matias1605/portal-academico.git
cd PortalAcademico
dotnet restore
dotnet ef database update
dotnet run

Accede en: http://localhost:5000

## Credenciales de prueba

| Usuario                                   | Contraseña  | Rol         |
| ----------------------------------------- | ----------- | ----------- |
| coordinador@uni.edu                       | Coord@1234! | Coordinador |
| Registrarse en /Identity/Account/Register | -           | Estudiante  |

## Migraciones

dotnet ef migrations add NombreMigracion
dotnet ef database update

## Variables de entorno en Render

| Variable                               | Valor                   |
| -------------------------------------- | ----------------------- |
| ASPNETCORE_ENVIRONMENT                 | Production              |
| ASPNETCORE_URLS                        | http://0.0.0.0:8080     |
| ConnectionStrings\_\_DefaultConnection | Data Source=/tmp/app.db |

## URL en Render

https://portal-academico.onrender.com

## Estructura de ramas

- feature/bootstrap-dominio — Modelos y seed
- feature/catalogo-cursos — Catálogo y filtros
- feature/matriculas — Inscripción y validaciones
- feature/panel-coordinador — Panel CRUD coordinador
- deploy/render — Configuración despliegue
