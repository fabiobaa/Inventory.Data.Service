# Guía de Ejecución - Inventory.Data.Service

## 📋 Tabla de Contenidos
- [Prerrequisitos](#prerrequisitos)
- [Configuración del Entorno](#configuración-del-entorno)
- [Ejecución Local](#ejecución-local)
- [Ejecución con Docker](#ejecución-con-docker)
- [Configuración de la Base de Datos](#configuración-de-la-base-de-datos)
- [Endpoints de la API](#endpoints-de-la-api)
- [Ejemplos de Uso](#ejemplos-de-uso)
- [Monitoreo y Observabilidad](#monitoreo-y-observabilidad)
- [Solución de Problemas](#solución-de-problemas)

## 🔧 Prerrequisitos

### Software Requerido
- **.NET 8 SDK** o superior
- **Visual Studio 2022**(recomendado) o **Visual Studio Code** 
- **Docker Desktop** (opcional, para contenedores)
- **Git** (para clonar el repositorio)

### Verificar Instalación
```bash
# Verificar versión de .NET
dotnet --version

# Verificar Docker (opcional)
docker --version
```

## ⚙️ Configuración del Entorno

### 1. Clonar el Repositorio
```bash
git clone https://github.com/fabiobaa/Inventory.Data.Service.git
cd Inventory.Data.Service
```

### 2. Restaurar Dependencias
```bash
dotnet restore
```

### 3. Compilar el Proyecto
```bash
dotnet build
```

## 🚀 Ejecución Local

### Opción 1: Visual Studio
1. Abrir `Inventory.Data.Service.sln` en Visual Studio
2. Presionar **F5** o hacer clic en **Iniciar depuración**
3. El navegador se abrirá automáticamente en `https://localhost:5026`

### Opción 2: Línea de Comandos
```bash
# Ejecutar en modo desarrollo
dotnet run

# Ejecutar en modo release
dotnet run --configuration Release
```

### Opción 3: Visual Studio Code
1. Abrir la carpeta del proyecto en VS Code
2. Presionar **F5** o usar la paleta de comandos: `Ctrl+Shift+P` → "Debug: Start Debugging"

## 🐳 Ejecución con Docker

### Construir la Imagen
```bash
docker build -t inventory-service .
```

### Ejecutar el Contenedor
```bash
# Ejecutar en puerto 8080
docker run -p 8080:8080 inventory-service

# Ejecutar en modo interactivo
docker run -it -p 8080:8080 inventory-service
```

### Docker Compose (Recomendado)
```yaml
# docker-compose.yml
version: '3.8'
services:
  inventory-service:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
```

```bash
# Ejecutar con Docker Compose
docker-compose up --build
```

## 🗄️ Configuración de la Base de Datos

### Base de Datos In-Memory
El proyecto utiliza **Entity Framework Core In-Memory Database** por defecto, lo que significa:
- ✅ No requiere configuración adicional
- ✅ Datos se pierden al reiniciar la aplicación
- ✅ Ideal para desarrollo y testing

### Cambiar a Base de Datos Real (Opcional)
Para usar SQL Server, modificar `Program.cs`:

```csharp
// Reemplazar esta línea:
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseInMemoryDatabase("InventoryDB"));

// Por esta:
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(connectionString));
```

## 📡 Endpoints de la API

### Swagger UI
Una vez ejecutado, acceder a:
- **Local**: `https://localhost:5026/swagger`
- **Docker**: `http://localhost:8080/swagger`

### Endpoints Principales

#### Gestión de Productos
```http
POST /api/products/bulk-load
GET /api/products
```

#### Gestión de Tiendas
```http
POST /api/stores/bulk-load
GET /api/stores
```

#### Gestión de Inventario
```http
POST /api/inventory/bulk-load
GET /api/inventory?storeId=STORE001&productId=PROD001
```

#### Procesamiento de Ventas
```http
POST /api/sales/bulk-load
```

#### Monitoreo
```http
GET /api/queue/messages?status=Pendiente&limit=10
GET /api/dashboard/summary
```

## 💡 Ejemplos de Uso

### 1. Crear Productos
```bash
curl -X POST "https://localhost:5026/api/products/bulk-load" \
  -H "Content-Type: application/json" \
  -d '[
    {
      "productId": "PROD001",
      "name": "Laptop Dell XPS 13"
    },
    {
      "productId": "PROD002", 
      "name": "Mouse Logitech MX Master"
    }
  ]'
```

### 2. Crear Tiendas
```bash
curl -X POST "https://localhost:5026/api/stores/bulk-load" \
  -H "Content-Type: application/json" \
  -d '[
    {
      "storeId": "STORE001",
      "name": "Tienda Centro"
    },
    {
      "storeId": "STORE002",
      "name": "Tienda Norte"
    }
  ]'
```

### 3. Cargar Inventario
```bash
curl -X POST "https://localhost:5026/api/inventory/bulk-load" \
  -H "Content-Type: application/json" \
  -d '[
    {
      "productId": "PROD001",
      "storeId": "STORE001",
      "quantity": 50
    },
    {
      "productId": "PROD002",
      "storeId": "STORE001", 
      "quantity": 100
    }
  ]'
```

### 4. Registrar Venta
```bash
curl -X POST "https://localhost:5026/api/sales/bulk-load" \
  -H "Content-Type: application/json" \
  -d '[
    {
      "transactionId": "TXN001",
      "productId": "PROD001",
      "storeId": "STORE001",
      "quantitySold": 2
    }
  ]'
```

### 5. Consultar Inventario
```bash
curl "https://localhost:5026/api/inventory?storeId=STORE001"
```

## 📊 Monitoreo y Observabilidad

### Dashboard del Sistema
```bash
curl "https://localhost:5026/api/dashboard/summary"
```

**Respuesta esperada:**
```json
{
  "success": true,
  "data": {
    "totalProductsInCatalog": 2,
    "totalStores": 2,
    "totalInventoryRecords": 2,
    "totalUnitsInStock": 150,
    "queueStatus": {
      "Pendiente": 0,
      "Procesado": 1,
      "Error": 0
    }
  }
}
```

### Estado de la Cola
```bash
curl "https://localhost:5026/api/queue/messages?status=Pendiente&limit=10"
```

### Logs de Rendimiento
Los logs incluyen métricas de rendimiento automáticamente:
```
info: Inventory.Data.Service.Middleware.PerformanceMetricsMiddleware[0]
      Request POST /api/sales/bulk-load finished in 45ms with status 202
```

## 🔍 Solución de Problemas

### Problemas Comunes

#### 1. Puerto en Uso
```bash
# Error: "Address already in use"
# Solución: Cambiar puerto en launchSettings.json
```

#### 2. Certificado SSL
```bash
# Error: "The SSL connection could not be established"
# Solución: Usar HTTP en lugar de HTTPS para desarrollo
```

#### 3. Base de Datos Vacía
```bash
# Los datos se pierden al reiniciar (comportamiento esperado con In-Memory DB)
# Solución: Cargar datos de prueba después de cada reinicio
```

### Comandos de Diagnóstico

#### Verificar Estado del Servicio
```bash
curl "https://localhost:5026/api/dashboard/summary"
```

#### Verificar Logs
```bash
# En Visual Studio: Ver ventana de Output
# En línea de comandos: Los logs aparecen en la consola
```

#### Verificar Dependencias
```bash
dotnet list package
```

## 📝 Notas Importantes

### ⚠️ Limitaciones del Prototipo
- **Base de Datos In-Memory**: Los datos se pierden al reiniciar
- **Cola Simulada**: No es un message broker real
- **Sin Autenticación**: No hay sistema de seguridad implementado

### 🔄 Flujo de Datos
1. **Cargar Catálogos**: Productos y Tiendas
2. **Cargar Inventario**: Cantidades por tienda
3. **Registrar Ventas**: Se procesan de forma asíncrona
4. **Monitorear**: Estado de la cola y métricas

### 🚀 Próximos Pasos
- Implementar base de datos real (SQL Server/PostgreSQL)
- Agregar message broker (RabbitMQ/Azure Service Bus)
- Implementar autenticación y autorización
- Agregar health checks
- Implementar caching (Redis)

---

## 📞 Soporte

Para problemas o preguntas:
- **GitHub Issues**: [https://github.com/fabiobaa/Inventory.Data.Service/issues](https://github.com/fabiobaa/Inventory.Data.Service/issues)
- **API Desplegada**: [https://test-pub-service-mana-inventory-djazchbed9bfe7h8.canadacentral-01.azurewebsites.net/swagger](https://test-pub-service-mana-inventory-djazchbed9bfe7h8.canadacentral-01.azurewebsites.net/swagger)

---