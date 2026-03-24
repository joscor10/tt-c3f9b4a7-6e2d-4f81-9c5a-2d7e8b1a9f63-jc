\# 🏗️ Arquitectura de la Solución



\## 📌 Descripción general



La solución implementa un sistema de leaderboard global utilizando una arquitectura basada en:



\* \*\*Arquitectura Hexagonal (Ports \& Adapters)\*\*

\* \*\*CQRS (Command Query Responsibility Segregation)\*\*

\* \*\*Cache + Base de datos (Redis + PostgreSQL)\*\*



El objetivo es garantizar:



\* Alta disponibilidad

\* Baja latencia en consultas

\* Escalabilidad horizontal

\* Consistencia eventual



---



\## 🧱 Componentes principales



\### 1. API (ASP.NET Core)



\* Exposición de endpoints REST:



&nbsp; \* `POST /score`

&nbsp; \* `GET /leaderboard`

&nbsp; \* `GET /user/{id}/score`

\* Actúa como punto de entrada al sistema



---



\### 2. Application Layer (CQRS + MediatR)



\* Maneja la lógica de casos de uso

\* Separa:



&nbsp; \* \*\*Commands\*\* (escrituras)

&nbsp; \* \*\*Queries\*\* (lecturas)



Ejemplo:



\* `AddScoreCommand`

\* `GetLeaderboardQuery`

\* `GetUserScoreQuery`



---



\### 3. Domain Layer



\* Contiene:



&nbsp; \* Entidades (ScoreEvent)

&nbsp; \* Interfaces (Ports)

\* Define contratos sin depender de infraestructura



---



\### 4. Infrastructure Layer



Implementa los puertos definidos en el dominio:



\#### 🔹 PostgreSQL



\* Almacena eventos de score

\* Fuente de verdad del sistema



\#### 🔹 Redis



\* Almacena leaderboard agregado

\* Permite consultas rápidas (O(log N))



---



\## 🔄 Flujo de datos



\### ➕ Escritura (POST /score)



1\. Cliente envía score

2\. API recibe request

3\. Command Handler procesa

4\. Se guarda en PostgreSQL

5\. Se actualiza Redis (Sorted Set)



---



\### 📊 Lectura (GET /leaderboard)



1\. API recibe request

2\. Query Handler consulta Redis

3\. Redis retorna top N

4\. Se devuelve respuesta



---



\### 👤 Consulta usuario (GET /user/:id/score)



1\. API recibe request

2\. Query Handler consulta Redis

3\. Retorna score agregado



---



\## 🧠 Decisiones de arquitectura



\### 🔹 Uso de Arquitectura Hexagonal



Permite:



\* Separar dominio de infraestructura

\* Facilitar testing (mock de Redis y DB)

\* Cambiar tecnologías sin afectar lógica



---



\### 🔹 Uso de CQRS



Separación clara entre:



\* Escrituras → PostgreSQL

\* Lecturas → Redis



Beneficios:



\* Mejor rendimiento

\* Escalabilidad independiente

\* Código más claro



---



\### 🔹 Uso combinado de PostgreSQL + Redis



\#### PostgreSQL:



\* Persistencia

\* Consistencia

\* Historial completo



\#### Redis:



\* Lecturas rápidas

\* Agregación en tiempo real

\* Ranking eficiente



---



\### 🔹 Near Real-Time Updates



Se utiliza Redis Sorted Sets:



\* Incremento atómico (`ZINCRBY`)

\* Actualización inmediata del ranking



---



\### 🔹 Consistencia



Se implementa \*\*consistencia eventual\*\*:



\* PostgreSQL = fuente de verdad

\* Redis = cache derivado



Trade-off:



\* Puede existir desincronización temporal

\* Se gana performance y escalabilidad



---



\### 🔹 Manejo de concurrencia



\* Redis maneja operaciones atómicas

\* PostgreSQL asegura integridad de datos



---



\## ⚠️ Hot Keys (puntos críticos)



Problema:



\* Usuarios muy activos generan contención en Redis



Solución propuesta:



\* Buckets por día:



&nbsp; ```

&nbsp; leaderboard:2026-03-23

&nbsp; ```

\* Distribuye carga

\* Permite agregación por ventana de tiempo



---



\## 📈 Escalabilidad



Para soportar 100k req/s:



\### Escritura



\* API escalada horizontalmente

\* Uso de colas (Kafka/RabbitMQ) para desacoplar escritura



\### Lectura



\* Redis Cluster

\* Replicas para lectura



\### Base de datos



\* Particionamiento por fecha

\* Sharding



---



\## 🧩 Diagrama de arquitectura



```

&nbsp;               ┌──────────────────────┐

&nbsp;               │      Cliente         │

&nbsp;               └─────────┬────────────┘

&nbsp;                         │

&nbsp;                         ▼

&nbsp;               ┌──────────────────────┐

&nbsp;               │   ASP.NET Core API   │

&nbsp;               └─────────┬────────────┘

&nbsp;                         │

&nbsp;       ┌─────────────────┼─────────────────┐

&nbsp;       ▼                                   ▼

┌───────────────┐                 ┌─────────────────┐

│  Commands     │                 │    Queries      │

│ (MediatR)     │                 │   (MediatR)     │

└──────┬────────┘                 └────────┬────────┘

&nbsp;      │                                   │

&nbsp;      ▼                                   ▼

┌───────────────┐                 ┌─────────────────┐

│ PostgreSQL    │                 │     Redis       │

│ (Source Truth)│                 │ (Leaderboard)   │

└───────────────┘                 └─────────────────┘

```



---



\## 🧭 Resumen



La arquitectura implementada permite:



\* Alta performance en lecturas

\* Consistencia controlada

\* Escalabilidad horizontal

\* Separación clara de responsabilidades



Esta combinación (Hexagonal + CQRS + Cache) es común en sistemas de alto rendimiento como leaderboards, sistemas de scoring y analytics.



