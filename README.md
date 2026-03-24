\# 🏆 Leaderboard Service - Prueba Técnica



\## ⏱️ Tiempo de desarrollo



Tiempo total invertido: \*\*4.5 horas\*\*





---



\## 📌 Descripción



Este proyecto implementa un servicio de leaderboard global que permite:



\* Registrar puntuaciones por usuario

\* Consultar el ranking (Top N)

\* Consultar el score acumulado de un usuario



El sistema soporta agregación por ventana de tiempo configurable (por defecto últimos 7 días) y actualización en \*\*near-real-time\*\*.



---



\## 🏗️ Tecnologías utilizadas



\* Backend: \*\*.NET Core 10 \*\*

\* Base de datos: \*\*PostgreSQL\*\*

\* Cache: \*\*Redis\*\*

\* ORM: Entity Framework Core

\* Arquitectura: Hexagonal + CQRS (MediatR)

\* Testing: xUnit + Moq



---



\## ⚙️ Requisitos previos



Antes de ejecutar el proyecto asegúrate de tener instalado:



\* .NET 10 SDK

\* Docker (para Redis)

\* PostgreSQL (local o remoto)



---



\## 🗄️ Configuración de Base de Datos



1\. Crear la base de datos en PostgreSQL:



```sql

CREATE DATABASE leaderboard\_db;

```



2\. Ejecutar el script ubicado en:



```text

/src/database/init.sql

```



Este script contiene:



\* Creación de tablas

\* Estructura inicial del sistema



---



\## 🚀 Configuración de Redis



Ejecutar Redis usando Docker:



```bash

docker run --name redis-leaderboard -p 6379:6379 -d redis

```



Verificar que esté corriendo:



```bash

docker ps

```



---



\## 🔧 Configuración del proyecto



Editar el archivo `appsettings.json`:



```json

{

&nbsp; "ConnectionStrings": {

&nbsp;   "Postgres": "Host=localhost;Port=5432;Database=leaderboard\_db;Username=postgres;Password=TU\_PASSWORD"

&nbsp; },

&nbsp; "Redis": {

&nbsp;   "ConnectionString": "localhost:6379"

&nbsp; }

}

```



---



\## ▶️ Ejecutar la aplicación



```bash

dotnet restore

dotnet build

dotnet run

```



---



\## 🌐 Endpoints disponibles



\### ➕ Registrar score



```http

POST /score

```



Payload:



```json

{

&nbsp; "userId": "user1",

&nbsp; "score": 10,

&nbsp; "timestamp": "2026-03-23T20:00:00Z"

}

```



---



\### 🏆 Obtener leaderboard



```http

GET /leaderboard?top=10```



\* `top`: número de usuarios



---



\### 👤 Obtener score de usuario



```http

GET /user/{id}/score```



---



\## 🧪 Ejecutar tests



Ejecutar todos los tests:



```bash

dotnet test

```



---



\## 🧠 Decisiones técnicas



\### 🔹 Uso combinado de PostgreSQL + Redis



\* PostgreSQL: fuente de verdad (persistencia)

\* Redis: cálculos agregados y consultas rápidas



Esto permite:



\* Baja latencia en lecturas

\* Alta escalabilidad

\* Soporte para near-real-time



---



\### 🔹 Ventana de tiempo configurable



El sistema permite configurar dinámicamente el rango de tiempo para el leaderboard mediante parámetros (`days`), con un valor por defecto de 7 días.



---



\### 🔹 Consistencia



\* Escrituras en PostgreSQL (fuente de verdad)

\* Actualización en Redis para lectura rápida

\* Estrategia eventual consistency



---



\### 🔹 Manejo de concurrencia



\* Redis usa operaciones atómicas (`SortedSetIncrement`)

\* PostgreSQL garantiza integridad de datos



---



\## 🔥 Escalabilidad (resumen)



Para soportar 100k req/s:



\* Redis cluster para lectura

\* Sharding en PostgreSQL

\* Uso de colas (Kafka/RabbitMQ) para ingestión

\* Horizontal scaling del API



---



\## 🛡️ Consideraciones adicionales



\* Validación de entrada en endpoints

\* Manejo de errores en cache (fallback a DB)

\* Uso de UTC para timestamps



---



\## 📌 Notas



\* Redis se usa como fuente principal para consultas de leaderboard

\* PostgreSQL se mantiene como respaldo y fuente de verdad

\* Los tests usan base de datos en memoria y mocks para cache



