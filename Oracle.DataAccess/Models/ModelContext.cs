using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Oracle.DataAccess.Models
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<CarSale> CarSales { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Inventory> Inventories { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<VacationRequest> VacationRequests { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseOracle("User Id=Proyecto;Password=proyectoinge2;Data Source=localhost:1521/orcl;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("PROYECTO")
                .UseCollation("USING_NLS_COMP");

            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("CARS");

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("\"PROYECTO\".\"CAR_SEQ\".\"NEXTVAL\"");

                entity.Property(e => e.Color)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("COLOR");

                entity.Property(e => e.Model)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("MODEL");

                entity.Property(e => e.Transmission)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("TRANSMISSION");

                entity.Property(e => e.Year)
                    .HasPrecision(4)
                    .HasColumnName("YEAR");
            });

            modelBuilder.Entity<CarSale>(entity =>
            {
                entity.ToTable("CAR_SALES");

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("\"PROYECTO\".\"CAR_SALES_SEQ\".\"NEXTVAL\"");

                entity.Property(e => e.CarModel)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("CAR_MODEL");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_NAME");

                entity.Property(e => e.Price)
                    .HasColumnType("NUMBER(10,2)")
                    .HasColumnName("PRICE");

                entity.Property(e => e.SaleDate)
                    .HasColumnType("DATE")
                    .HasColumnName("SALE_DATE");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("CUSTOMERS");

                entity.HasIndex(e => e.Cedula, "SYS_C007473")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("\"PROYECTO\".\"CUSTOMERS_SEQ\".\"NEXTVAL\"");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS");

                entity.Property(e => e.BirthDate)
                    .HasColumnType("DATE")
                    .HasColumnName("BIRTH_DATE");

                entity.Property(e => e.Cedula)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CEDULA");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PHONE");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("EMPLOYEES");

                entity.HasIndex(e => e.Cedula, "SYS_C007457")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "SYS_C007458")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("\"PROYECTO\".\"EMPLOYEES_SEQ\".\"NEXTVAL\"");

                entity.Property(e => e.Cedula)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CEDULA");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.EmployeeName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("EMPLOYEE_NAME");

                entity.Property(e => e.HireDate)
                    .HasColumnType("DATE")
                    .HasColumnName("HIRE_DATE");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("LAST_NAME");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PHONE");

                entity.Property(e => e.Position)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("position");

                entity.Property(e => e.Salary)
                    .HasColumnType("NUMBER(10,2)")
                    .HasColumnName("SALARY");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("INVENTORY");

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.CarId)
                    .HasPrecision(19)
                    .HasColumnName("CAR_ID");

                entity.Property(e => e.Quantity)
                    .HasPrecision(10)
                    .HasColumnName("QUANTITY");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Inventories)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("SYS_C007502");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("INVOICES");

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("\"PROYECTO\".\"INVOICES_SEQ\".\"NEXTVAL\"");

                entity.Property(e => e.InvoiceDate)
                    .HasColumnType("DATE")
                    .HasColumnName("INVOICE_DATE");

                entity.Property(e => e.OrderId)
                    .HasPrecision(19)
                    .HasColumnName("ORDER_ID");

                entity.Property(e => e.Status)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("NUMBER(10,2)")
                    .HasColumnName("TOTAL_AMOUNT");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("SYS_C007478");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("ORDERS");

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("\"PROYECTO\".\"ORDERS_SEQ\".\"NEXTVAL\"");

                entity.Property(e => e.CarModel)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("CAR_MODEL");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_NAME");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("DATE")
                    .HasColumnName("ORDER_DATE");

                entity.Property(e => e.OrderNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("ORDER_NUMBER");

                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("ORDER_STATUS");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USERS");

                entity.HasIndex(e => e.Username, "SYS_C007489")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("\"PROYECTO\".\"USERS_SEQ\".\"NEXTVAL\"");

                entity.Property(e => e.CreatedAt)
                    .HasPrecision(6)
                    .HasColumnName("CREATED_AT")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CustomerId)
                    .HasPrecision(19)
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PASSWORD");

                entity.Property(e => e.Role)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ROLE");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("USERNAME");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_CUSTOMER");
            });

            modelBuilder.Entity<VacationRequest>(entity =>
            {
                entity.ToTable("VACATION_REQUESTS");

                entity.Property(e => e.Id)
                    .HasPrecision(19)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("\"PROYECTO\".\"VACATION_REQUESTS_SEQ\".\"NEXTVAL\"");

                entity.Property(e => e.Comments)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("COMMENTS");

                entity.Property(e => e.EmployeeId)
                    .HasPrecision(19)
                    .HasColumnName("EMPLOYEE_ID");

                entity.Property(e => e.EndDate)
                    .HasColumnType("DATE")
                    .HasColumnName("END_DATE");

                entity.Property(e => e.StartDate)
                    .HasColumnType("DATE")
                    .HasColumnName("START_DATE");

                entity.Property(e => e.Status)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("STATUS");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.VacationRequests)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("SYS_C007483");
            });

            modelBuilder.HasSequence("CAR_SALES_SEQ");

            modelBuilder.HasSequence("CAR_SEQ");

            modelBuilder.HasSequence("CUSTOMERS_SEQ");

            modelBuilder.HasSequence("EMPLOYEES_SEQ");

            modelBuilder.HasSequence("INVENTORY_SEQ");

            modelBuilder.HasSequence("INVOICES_SEQ");

            modelBuilder.HasSequence("ORDERS_SEQ");

            modelBuilder.HasSequence("USERS_SEQ");

            modelBuilder.HasSequence("VACATION_REQUESTS_SEQ");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
