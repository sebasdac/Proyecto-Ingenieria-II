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

        public virtual DbSet<Accessory> Accessories { get; set; } = null!;
        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<CarDetail> CarDetails { get; set; } = null!;
        public virtual DbSet<CarSale> CarSales { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<Cartitem> Cartitems { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
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

            modelBuilder.Entity<Accessory>(entity =>
            {
                entity.ToTable("ACCESSORIES");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Cost)
                    .HasColumnType("NUMBER(10,2)")
                    .HasColumnName("COST");

                entity.Property(e => e.Description)
                    .HasColumnType("CLOB")
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Quantity)
                    .HasColumnType("NUMBER")
                    .HasColumnName("QUANTITY");
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("CARS");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Model)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("MODEL");

                entity.Property(e => e.Year)
                    .HasColumnType("NUMBER")
                    .HasColumnName("YEAR");
            });

            modelBuilder.Entity<CarDetail>(entity =>
            {
                entity.ToTable("CAR_DETAILS");

                entity.Property(e => e.Id)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.CarId)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CAR_ID");

                entity.Property(e => e.Color)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("COLOR");

                entity.Property(e => e.Price)
                    .HasColumnType("NUMBER")
                    .HasColumnName("PRICE");

                entity.Property(e => e.Stock)
                    .HasColumnType("NUMBER")
                    .HasColumnName("STOCK");

                entity.Property(e => e.TransmissionType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TRANSMISSION_TYPE");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.CarDetails)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("SYS_C008375");
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

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("CARTS");

                entity.Property(e => e.Cartid)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("CARTID");

                entity.Property(e => e.Createddate)
                    .HasPrecision(6)
                    .HasColumnName("CREATEDDATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP\n   ");

                entity.Property(e => e.Userid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("USERID");
            });

            modelBuilder.Entity<Cartitem>(entity =>
            {
                entity.ToTable("CARTITEMS");

                entity.Property(e => e.Cartitemid)
                    .HasColumnType("NUMBER")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("CARTITEMID");

                entity.Property(e => e.Cartid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("CARTID");

                entity.Property(e => e.Price)
                    .HasColumnType("NUMBER(10,2)")
                    .HasColumnName("PRICE");

                entity.Property(e => e.Productid)
                    .HasColumnType("NUMBER")
                    .HasColumnName("PRODUCTID");

                entity.Property(e => e.Producttype)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PRODUCTTYPE");

                entity.Property(e => e.Quantity)
                    .HasColumnType("NUMBER")
                    .HasColumnName("QUANTITY");

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.Cartitems)
                    .HasForeignKey(d => d.Cartid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CARTITEMS_CARTS");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("CUSTOMERS");

                entity.HasIndex(e => e.Cedula, "SYS_C008347")
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

                entity.HasIndex(e => e.Cedula, "SYS_C008333")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "SYS_C008334")
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
                    .HasConstraintName("SYS_C008376");
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

                entity.HasIndex(e => e.Username, "SYS_C008322")
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
                    .HasConstraintName("SYS_C008378");
            });

            modelBuilder.HasSequence("CAR_DETAILS_SEQ");

            modelBuilder.HasSequence("CAR_SALES_SEQ");

            modelBuilder.HasSequence("CAR_SEQ");

            modelBuilder.HasSequence("CARS_SEQ");

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
