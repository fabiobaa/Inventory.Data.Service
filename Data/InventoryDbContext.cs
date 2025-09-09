using Inventory.Data.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Data.Service.Data
{
    public class InventoryDbContext : DbContext
    {

        // El constructor permite que la configuración de la base de datos
        // se inyecte desde Program.cs.
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        // Representa la tabla 'Inventory'Inventario.
        public DbSet<InventoryModel> Inventory { get; set; }
        // Representa la tabla 'QueuedMessages', que actúa como nuestra cola de eventos.
        public DbSet<QueuedMessageModel> QueuedMessages { get; set; }
        // Representa la tabla 'Products', Productos.
        public DbSet<ProductModel> Products { get; set; }
        // Representa la tabla 'Stores', Tiendas.
        public DbSet<StoreModel> Stores { get; set; }



        // Este método se usa para configurar detalles del modelo de datos.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aquí definimos que la tabla 'InventoryProducts' tiene una clave
            // primaria compuesta por ProductId y StoreId.
            modelBuilder.Entity<InventoryModel>()
                .HasKey(item => new { item.ProductId, item.StoreId });
        }

    }
}
