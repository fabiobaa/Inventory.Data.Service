# Prototipo de Optimización de Inventario: Inventory.Data.Service

## 1. Descripción Breve
Este proyecto es un prototipo de backend para un **sistema de gestión de inventario distribuido**, diseñado para una cadena de tiendas minoristas. Reemplaza un sistema monolítico anterior que sufría de alta latencia (sincronización de 15 minutos) e inconsistencias de datos, lo que resultaba en una mala experiencia de usuario y pérdida de ventas.

El objetivo de este prototipo es demostrar una arquitectura moderna que optimiza la **consistencia del inventario**, reduce la **latencia de actualización** y sienta las bases para un sistema robusto, escalable y observable.

## 2. Decisiones Clave de Arquitectura
* **Arquitectura Orientada a Eventos (EDA)**: El núcleo del sistema es un flujo asíncrono. Las transacciones (como las ventas) se registran como eventos en una cola (simulada con una tabla de base de datos) y son procesadas por un servicio en segundo plano.
* **Manejo de Errores Global con Middleware**: Un middleware (`ErrorHandlerMiddleware`) atrapa todas las excepciones no controladas de la API, las registra y las devuelve en un formato JSON estandarizado.
* **Separación de Responsabilidades (SoC)**: La API se divide en controladores especializados con responsabilidades únicas:
    * `ProductController`: Gestiona el catálogo maestro de productos y el inventario.
    * `StoreController`: Gestiona el catálogo maestro de tiendas.
    * `SaleController`: Maneja las transacciones de negocio (ventas).
    * `QueueController` & `DashboardController`: Proporcionan endpoints de observabilidad para monitorear la salud del sistema.
* **Base de Datos de Prototipo**: Se optó por el **Proveedor En Memoria de Entity Framework Core**. Esta decisión simplifica enormemente el desarrollo, elimina los errores de bloqueo y es ideal para la fase de prototipado.
* **Validación Robusta con FluentValidation**: Toda la lógica de validación de los datos de entrada se ha externalizado a clases de validadores dedicadas.
* **Respuestas de API Estandarizadas**: Se utiliza una clase wrapper `ApiResult<T>` para todas las respuestas, asegurando una estructura consistente para el front-end.

## 3. Pila Tecnológica
* **Framework**: .NET 8 / ASP.NET Core
* **Acceso a Datos**: Entity Framework Core 8
* **Base de Datos (Prototipo)**: EF Core In-Memory Database Provider
* **Validación**: FluentValidation

## 4. API Endpoints Principales

### `StoreController` (`/api/stores`)
| Verbo  | Ruta | Descripción                             |
|--------|------|-----------------------------------------|
| `POST` | `/`  | Crea una o más tiendas en el catálogo.  |
| `GET`  | `/`  | Obtiene la lista de todas las tiendas.  |

### `ProductController` (`/api/products`)
| Verbo  | Ruta          | Descripción                                                |
|--------|---------------|------------------------------------------------------------|
| `POST` | `/`    | Crea uno o más productos en el catálogo maestro.           |
| `GET`  | `/`    | Obtiene la lista de todos los productos del catálogo.      |

### `InventoryController` (`/api/inventory`)
| Verbo  | Ruta          | Descripción                                                |
|--------|---------------|------------------------------------------------------------|
| `POST` | `/bulk-load`  | Carga o actualiza masivamente el inventario.               |
| `GET`  | `/`           | Busca en el inventario con filtros opcionales.             |


### `SalesController` (`/api/sales`)
| Verbo  | Ruta | Descripción                                     |
|--------|------|-------------------------------------------------|
| `POST` | `/`  | Registra un evento de venta en la cola.         |

### `QueueController` (`/api/queue`)
| Verbo  | Ruta        | Descripción                                     |
|--------|-------------|-------------------------------------------------|
| `GET`  | `/messages` | Monitorea la lista de mensajes en la cola.      |

### `DashboardController` (`/api/dashboard`)
| Verbo  | Ruta        | Descripción                                     |
|--------|-------------|-------------------------------------------------|
| `GET`  | `/summary`  | Obtiene un resumen macro con KPIs del sistema.  |