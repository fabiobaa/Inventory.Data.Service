## ## Ajustar los promts mas acordes para utilizarlos con cursor


## ## Fase 1: Análisis y Diseño Inicial
Prompt 1 (El Punto de Partida):

"Adjunto un documento con los requisitos para un proyecto de optimización de inventario.Analiza el documento y resume el objetivo principal, el problema a resolver y los requisitos clave. Quiero entender la situación actual y las metas del proyecto."

Prompt 2 (Propuesta de Arquitectura):

"Basado en el análisis anterior, propón una arquitectura técnica moderna para resolver los problemas de latencia e inconsistencia. Justifica por qué tu propuesta es la adecuada y diseña los endpoints principales de la API siguiendo un enfoque RESTful."

## Fase 2: Definición de la Base Técnica
Prompt 3 (Pila Tecnológica y Estructura):

"Vamos a implementar esto con .NET 8. Recomienda una pila tecnológica específica, incluyendo el framework de API, una elección de base de datos para el prototipo que sea simple, y cómo estructurarías las carpetas del proyecto. Prioriza la simplicidad y las buenas prácticas."

Prompt 4 (Creación de Modelos y DbContext):

"Ahora, genera el código C# para los modelos de datos (Product, Store, InventoryProducts, QueuedMessage) y la clase DbContext de Entity Framework Core, configurada para el proveedor de base de datos en memoria."

## Fase 3: Implementación del Flujo Principal
Prompt 5 (El Consumidor Asíncrono):

"Genera el código para un BackgroundService (QueueProcessorService) que actúe como consumidor de la cola de eventos. Debe leer los mensajes pendientes de la tabla QueuedMessages en orden, procesar la lógica de negocio de forma atómica y manejar los errores de manera robusta."

Prompt 6 (Creación de Controladores):

"Siguiendo el principio de Separación de Responsabilidades, genera el código para los controladores de la API (ProductsController, StoresController, SalesController). Deben incluir los endpoints necesarios para gestionar los catálogos y para registrar una venta, encolando el evento."

## Fase 4: Refinamiento y Robustez
Prompt 7 (Estandarización de Respuestas):

"Quiero estandarizar todas las respuestas de la API. Genera una clase wrapper ApiResult<T> y refactoriza los controladores que creamos para que utilicen esta clase en todos sus endpoints."

Prompt 8 (Validación Centralizada):

"Integra FluentValidation al proyecto. Genera las clases de validación para los modelos y DTOs principales, incluyendo reglas que consulten la base de datos para verificar la existencia de productos y tiendas."

Prompt 9 (Manejo de Errores Global):

"Crea el código para un ErrorHandlerMiddleware que capture todas las excepciones no controladas y las devuelva usando nuestra estructura ApiResult<T> estándar."

## Fase 5: Observabilidad y Documentación
Prompt 10 (Endpoints de Monitoreo):

"Para mejorar la observabilidad, crea un DashboardController con un endpoint /summary que devuelva métricas macro del sistema, como el conteo total de productos, tiendas y el estado de la cola de mensajes."

Prompt 11 (Documentación Final):

"Finalmente, genera los archivos README.md y run.md para el proyecto. El README debe documentar la arquitectura final, las decisiones de diseño y los endpoints. El run.md debe proporcionar un flujo de prueba paso a paso para que cualquiera pueda ejecutar y verificar el prototipo."
