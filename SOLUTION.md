\## 🤖 Uso de la IA



Durante el desarrollo de la solución se utilizó Inteligencia Artificial como herramienta de apoyo en distintas fases del proceso, manteniendo siempre criterio técnico propio para la toma de decisiones.



\### 🔹 Diseño de arquitectura



Se utilizó \*\*ChatGPT (modelo GPT-4)\*\* para explorar posibles enfoques arquitectónicos a partir del requerimiento proporcionado.



Se le solicitó asumir el rol de arquitecto de software con el objetivo de:



\* Identificar tecnologías adecuadas

\* Proponer una arquitectura escalable

\* Sugerir componentes backend y de persistencia



A partir de las propuestas generadas, se evaluaron distintas alternativas. Por ejemplo:





&nbsp;\* Se consideró el uso de Node.js como tecnología backend

&nbsp;\* Sin embargo, se optó por .NET, debido a:

&nbsp;	\* Mejor integración con herramientas empresariales

&nbsp;	\* Mayor control en la estructura del proyecto

&nbsp;	\* Facilidad para implementar patrones como CQRS y arquitectura hexagonal



La arquitectura final (Hexagonal + CQRS) fue seleccionada tras analizar las opciones sugeridas y adaptarlas al contexto del problema.



---



\### 🔹 Implementación



Durante la fase de desarrollo se utilizó \*\*GitHub Copilot (modelo GPT-5.1 mini)\*\* integrado en el IDE para acelerar tareas como:



\* Configuración de conexión a PostgreSQL

\* Integración con Redis

\* Implementación de repositorios y servicios

\* Generación de tests unitarios e integración



En este caso, el uso fue más directo y enfocado, mediante prompts como:



\* Generación de tests a partir de endpoints

\* Cobertura de escenarios comunes y edge cases



---



\### 🔹 Consideraciones



El uso de IA permitió:



\* Acelerar el desarrollo

\* Reducir errores iniciales

\* Explorar alternativas de diseño



No obstante:



\* Todas las decisiones finales fueron evaluadas manualmente

\* Se priorizó el entendimiento completo de cada componente implementado

\* Se realizaron ajustes sobre el código generado para alinearlo con buenas prácticas



---



\## 🚀 Plan de despliegue



\### 🔹 Estrategia



El despliegue está diseñado para ser simple y reproducible mediante contenedores:



1\. Build del proyecto

2\. Empaquetado en contenedor Docker

3\. Configuración mediante variables de entorno

4\. Despliegue de los siguientes servicios:



&nbsp;  \* API (.NET)

&nbsp;  \* PostgreSQL

&nbsp;  \* Redis



---



\### 🔹 Pipeline básico



\#### CI (Integración continua)



\* Restauración de dependencias

\* Build del proyecto

\* Ejecución de tests automatizados



\#### CD (Despliegue continuo)



\* Publicación de imagen Docker

\* Despliegue automático en entorno objetivo



---



\## 🔄 Plan de rollback



En caso de fallo en producción:



1\. Mantener disponible la versión anterior del contenedor

2\. Revertir rápidamente a una versión estable

3\. Redis puede ser reconstruido a partir de PostgreSQL (fuente de verdad)



Esto garantiza una recuperación rápida con mínima pérdida de información.



---



\## 🛡️ Consideraciones de seguridad



Se contemplaron las siguientes medidas:



\* Validación de datos de entrada en todos los endpoints

\* Uso de DTOs para evitar exposición directa de entidades internas



\### 🔹 Mejoras propuestas



Para un entorno productivo se recomienda implementar:



\* Autenticación y autorización (JWT)

\* Rate limiting para prevenir abuso

\* Sanitización de datos de entrada

\* Uso de HTTPS obligatorio

\* Manejo seguro de credenciales mediante variables de entorno o vaults







\## 📈 Escala y Performance (100k req/s)



\### 🎯 Objetivo



Diseñar un plan para escalar el sistema hasta soportar aproximadamente \*\*100,000 requests por segundo\*\*, manteniendo baja latencia y alta disponibilidad.



---



\## 🔍 Supuestos



Para el análisis se consideran los siguientes escenarios:



\* 70% lecturas (`GET /leaderboard`, `GET /user/:id/score`)

\* 30% escrituras (`POST /score`)

\* Tamaño de respuesta pequeño (JSON ligero)

\* Redis como principal fuente de lectura



---



\## ⚙️ Distribución de carga



| Tipo de operación | %   | Req/s  |

| ----------------- | --- | ------ |

| Lecturas          | 70% | 70,000 |

| Escrituras        | 30% | 30,000 |



---



\## 🧩 Componentes a escalar



\### 🔹 1. API (.NET)



\* Escalado horizontal (stateless)

\* Múltiples instancias detrás de un load balancer



\#### Capacidad estimada:



\* ~2,000 req/s por instancia



\#### Requerimiento:



```text

100,000 / 2,000 ≈ 50 instancias

```



---



\### 🔹 2. Redis (lecturas y agregación)



Redis es el componente crítico para performance.



\#### Estrategia:



\* Uso de \*\*Redis Cluster\*\*

\* Sharding automático

\* Réplicas para lectura



\#### Capacidad estimada:



\* ~50k - 100k ops/sec por nodo



\#### Requerimiento:



\* 2–3 nodos principales

\* 2–3 réplicas



---



\### 🔹 3. PostgreSQL (persistencia)



No se usa para lecturas intensivas, solo escritura.



\#### Problema:



\* 30k writes/sec puede saturar una sola instancia



\#### Soluciones:



\* \*\*Particionamiento por fecha\*\*

\* \*\*Sharding\*\*

\* Uso de \*\*cola intermedia\*\*



---



\### 🔹 4. Cola de mensajes (opcional pero recomendado)



Para desacoplar escrituras:



\* Kafka / RabbitMQ



\#### Flujo:



```text

API → Cola → Worker → PostgreSQL + Redis

```



\#### Beneficios:



\* Absorbe picos de tráfico

\* Mejora resiliencia

\* Permite procesamiento async



---



\## ⚡ Estrategia de optimización



\### 🔹 Lecturas



\* Servidas directamente desde Redis

\* Complejidad: O(log N)

\* Latencia: < 10 ms



---



\### 🔹 Escrituras



\* Escritura rápida en Redis

\* Persistencia async en PostgreSQL



---



\## ⚠️ Hot Keys



\### Problema:



Usuarios con alto tráfico generan contención



---



\### Solución:



Particionar leaderboard por tiempo:



```text

leaderboard:2026-03-24

leaderboard:2026-03-23

```



Y agregar resultados dinámicamente



---



\## 📊 Latencia esperada



| Operación       | Latencia |

| --------------- | -------- |

| GET leaderboard | 5–10 ms  |

| GET user score  | 5–10 ms  |

| POST score      | 10–20 ms |



---



\## 🧠 Cuellos de botella



\### 1. Redis saturado



\* Solución: cluster + sharding



\### 2. PostgreSQL escritura



\* Solución: cola + particionado



\### 3. API CPU-bound



\* Solución: autoscaling horizontal



---



\## 🚀 Estrategia final de escalado



1\. API escalada horizontalmente

2\. Redis Cluster como capa principal de lectura

3\. Cola para desacoplar escrituras

4\. PostgreSQL particionado y optimizado

5\. Uso de caching agresivo



---



\## 🧭 Conclusión



El sistema puede escalar a 100k req/s mediante:



\* Separación de lectura/escritura (CQRS)

\* Uso de Redis como capa de alto rendimiento

\* Escalado horizontal de la API

\* Desacoplamiento mediante colas



Esta arquitectura permite mantener baja latencia y alta disponibilidad incluso bajo alta carga.



---



