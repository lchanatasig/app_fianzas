using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace app_fianzas.Models;

public partial class AppFianzaUnidosContext : DbContext
{
    public AppFianzaUnidosContext()
    {
    }

    public AppFianzaUnidosContext(DbContextOptions<AppFianzaUnidosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Beneficiario> Beneficiarios { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<EmpresaFinanza> EmpresaFinanzas { get; set; }

    public virtual DbSet<EstadoFianza> EstadoFianzas { get; set; }

    public virtual DbSet<HistorialEmpresa> HistorialEmpresas { get; set; }

    public virtual DbSet<LogErrore> LogErrores { get; set; }

    public virtual DbSet<Perfil> Perfils { get; set; }

    public virtual DbSet<Prendum> Prenda { get; set; }

    public virtual DbSet<SolicitudFianza> SolicitudFianzas { get; set; }

    public virtual DbSet<SolicitudFianzaHistorial> SolicitudFianzaHistorials { get; set; }

    public virtual DbSet<TipoEmpresa> TipoEmpresas { get; set; }

    public virtual DbSet<TipoFianza> TipoFianzas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=app_fianza_unidos;User Id=sa;Password=Sur2o22--;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Beneficiario>(entity =>
        {
            entity.HasKey(e => e.BeneficiarioId).HasName("PK__benefici__916FBDE7CDFE673E");

            entity.ToTable("beneficiario");

            entity.Property(e => e.BeneficiarioId).HasColumnName("beneficiario_id");
            entity.Property(e => e.CiRucBeneficiario)
                .HasMaxLength(13)
                .HasColumnName("ci_ruc_beneficiario");
            entity.Property(e => e.DireccionBeneficiario)
                .HasMaxLength(500)
                .HasColumnName("direccion_beneficiario");
            entity.Property(e => e.EmailBeneficiario)
                .HasMaxLength(100)
                .HasColumnName("email_beneficiario");
            entity.Property(e => e.EstadoBeneficiario)
                .HasDefaultValue((byte)1)
                .HasColumnName("estado_beneficiario");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.NombreBeneficiario)
                .HasMaxLength(255)
                .HasColumnName("nombre_beneficiario");
            entity.Property(e => e.TelefonoBeneficiario)
                .HasMaxLength(15)
                .HasColumnName("telefono_beneficiario");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.EmpresaId).HasName("PK__empresa__536BE4A2A22E4AB3");

            entity.ToTable("empresa", tb => tb.HasTrigger("trg_empresa_actualizacion"));

            entity.HasIndex(e => e.CiEmpresa, "UQ__empresa__B31BADA9904E87AF").IsUnique();

            entity.HasIndex(e => e.EmailEmpresa, "UQ__empresa__EFC2C9F7829072B1").IsUnique();

            entity.Property(e => e.EmpresaId).HasColumnName("empresa_id");
            entity.Property(e => e.CiEmpresa)
                .HasMaxLength(13)
                .HasColumnName("ci_empresa");
            entity.Property(e => e.DireccionEmpresa)
                .HasMaxLength(255)
                .HasColumnName("direccion_empresa");
            entity.Property(e => e.EmailEmpresa)
                .HasMaxLength(100)
                .HasColumnName("email_empresa");
            entity.Property(e => e.EstadoEmpresa)
                .HasDefaultValue((byte)1)
                .HasColumnName("estado_empresa");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.NombreEmpresa)
                .HasMaxLength(255)
                .HasColumnName("nombre_empresa");
            entity.Property(e => e.TelefonoEmpresa)
                .HasMaxLength(15)
                .HasColumnName("telefono_empresa");
            entity.Property(e => e.TipoEmpresaId).HasColumnName("tipo_empresa_id");

            entity.HasOne(d => d.TipoEmpresa).WithMany(p => p.Empresas)
                .HasForeignKey(d => d.TipoEmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_empresa_tipo");
        });

        modelBuilder.Entity<EmpresaFinanza>(entity =>
        {
            entity.HasKey(e => e.EmpresaFinanzasId).HasName("PK__empresa___727A5DFF13D4E3BE");

            entity.ToTable("empresa_finanzas");

            entity.Property(e => e.EmpresaFinanzasId).HasColumnName("empresa_finanzas_id");
            entity.Property(e => e.ActivoCorriente)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("activo_corriente");
            entity.Property(e => e.ActivoFijo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("activo_fijo");
            entity.Property(e => e.Capital)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("capital");
            entity.Property(e => e.CupoTotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("cupo_total");
            entity.Property(e => e.EmpresaId).HasColumnName("empresa_id");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.Perdida)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("perdida");
            entity.Property(e => e.Reserva)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("reserva");
            entity.Property(e => e.Utilidad)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("utilidad");
            entity.Property(e => e.Ventas)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("ventas");

            entity.HasOne(d => d.Empresa).WithMany(p => p.EmpresaFinanzas)
                .HasForeignKey(d => d.EmpresaId)
                .HasConstraintName("fk_finanzas_empresa");
        });

        modelBuilder.Entity<EstadoFianza>(entity =>
        {
            entity.HasKey(e => e.EstadoFianzaId).HasName("PK__estado_f__71E6F9ED811DAFDA");

            entity.ToTable("estado_fianza");

            entity.HasIndex(e => e.Nombre, "UQ__estado_f__72AFBCC63710A187").IsUnique();

            entity.Property(e => e.EstadoFianzaId).HasColumnName("estado_fianza_id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<HistorialEmpresa>(entity =>
        {
            entity.HasKey(e => e.HistorialId).HasName("PK__historia__68FE18EE2A03E146");

            entity.ToTable("historial_empresas");

            entity.HasIndex(e => e.FechaActualizacion, "idx_historial_fecha");

            entity.Property(e => e.HistorialId).HasColumnName("historial_id");
            entity.Property(e => e.EmpresaId).HasColumnName("empresa_id");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.HistorialActivoC)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("historial_activoC");
            entity.Property(e => e.HistorialActivoF)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("historial_activoF");
            entity.Property(e => e.HistorialCapital)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("historial_capital");
            entity.Property(e => e.HistorialCupoAsignado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("historial_cupo_asignado");
            entity.Property(e => e.HistorialCupoRestante)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("historial_cupo_restante");
            entity.Property(e => e.HistorialOperacion)
                .HasMaxLength(50)
                .HasColumnName("historial_operacion");
            entity.Property(e => e.HistorialPerdida)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("historial_perdida");
            entity.Property(e => e.HistorialReserva)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("historial_reserva");
            entity.Property(e => e.HistorialUtilidad)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("historial_utilidad");
            entity.Property(e => e.HistorialVentas)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("historial_ventas");

            entity.HasOne(d => d.Empresa).WithMany(p => p.HistorialEmpresas)
                .HasForeignKey(d => d.EmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_historial_empresa");
        });

        modelBuilder.Entity<LogErrore>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__log_erro__9E2397E05F41D88A");

            entity.ToTable("log_errores");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.ErrorDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("error_date");
            entity.Property(e => e.ErrorMessage)
                .HasMaxLength(4000)
                .HasColumnName("error_message");
        });

        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.HasKey(e => e.IdPerfil).HasName("PK__perfil__1D1C87688B6F1740");

            entity.ToTable("perfil");

            entity.Property(e => e.IdPerfil).HasColumnName("id_perfil");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.NombrePerfil)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre_perfil");
        });

        modelBuilder.Entity<Prendum>(entity =>
        {
            entity.HasKey(e => e.PrendaId).HasName("PK__prenda__011BC654F8551B25");

            entity.ToTable("prenda");

            entity.Property(e => e.PrendaId).HasColumnName("prenda_id");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.PrendaArchivo).HasColumnName("prenda_archivo");
            entity.Property(e => e.PrendaBien)
                .HasMaxLength(255)
                .HasColumnName("prenda_bien");
            entity.Property(e => e.PrendaCustodio)
                .HasMaxLength(255)
                .HasColumnName("prenda_custodio");
            entity.Property(e => e.PrendaDescripcion)
                .HasMaxLength(255)
                .HasColumnName("prenda_descripcion");
            entity.Property(e => e.PrendaEstado)
                .HasDefaultValue((byte)1)
                .HasColumnName("prenda_estado");
            entity.Property(e => e.PrendaFechaConstatacion).HasColumnName("prenda_fecha_constatacion");
            entity.Property(e => e.PrendaResponsableConstatacion)
                .HasMaxLength(255)
                .HasColumnName("prenda_responsable_constatacion");
            entity.Property(e => e.PrendaTipo)
                .HasMaxLength(255)
                .HasColumnName("prenda_tipo");
            entity.Property(e => e.PrendaUbicacion)
                .HasMaxLength(500)
                .HasColumnName("prenda_ubicacion");
            entity.Property(e => e.PrendaValor)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("prenda_valor");
        });

        modelBuilder.Entity<SolicitudFianza>(entity =>
        {
            entity.HasKey(e => e.SolicitudFianzaId).HasName("PK__solicitu__C018B34EFB84E668");

            entity.ToTable("solicitud_fianza", tb => tb.HasTrigger("trg_solicitud_actualizacion"));

            entity.HasIndex(e => e.FechaSolicitud, "idx_solicitud_fecha");

            entity.Property(e => e.SolicitudFianzaId).HasColumnName("solicitud_fianza_id");
            entity.Property(e => e.AprobacionLegal)
                .HasDefaultValue(false)
                .HasColumnName("aprobacion_legal");
            entity.Property(e => e.AprobacionTecnica)
                .HasDefaultValue(false)
                .HasColumnName("aprobacion_tecnica");
            entity.Property(e => e.BeneficiarioId).HasColumnName("beneficiario_id");
            entity.Property(e => e.EmpresaId).HasColumnName("empresa_id");
            entity.Property(e => e.EstadoFianzaId).HasColumnName("estado_fianza_id");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaInicioVigencia).HasColumnName("fecha_inicio_vigencia");
            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_solicitud");
            entity.Property(e => e.MontoContrato)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_contrato");
            entity.Property(e => e.MontoFianza)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_fianza");
            entity.Property(e => e.MontoGarantia)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto_garantia");
            entity.Property(e => e.ObjetoContrato).HasColumnName("objeto_contrato");
            entity.Property(e => e.PlazoGarantiaDias).HasColumnName("plazo_garantia_dias");
            entity.Property(e => e.PrendaId).HasColumnName("prenda_id");
            entity.Property(e => e.SectorFianza)
                .HasMaxLength(255)
                .HasColumnName("sector_fianza");
            entity.Property(e => e.TipoFianzaId).HasColumnName("tipo_fianza_id");

            entity.HasOne(d => d.Beneficiario).WithMany(p => p.SolicitudFianzas)
                .HasForeignKey(d => d.BeneficiarioId)
                .HasConstraintName("fk_solicitud_beneficiario");

            entity.HasOne(d => d.Empresa).WithMany(p => p.SolicitudFianzas)
                .HasForeignKey(d => d.EmpresaId)
                .HasConstraintName("fk_solicitud_empresa");

            entity.HasOne(d => d.EstadoFianza).WithMany(p => p.SolicitudFianzas)
                .HasForeignKey(d => d.EstadoFianzaId)
                .HasConstraintName("fk_solicitud_estado");

            entity.HasOne(d => d.Prenda).WithMany(p => p.SolicitudFianzas)
                .HasForeignKey(d => d.PrendaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_solicitud_prenda");

            entity.HasOne(d => d.TipoFianza).WithMany(p => p.SolicitudFianzas)
                .HasForeignKey(d => d.TipoFianzaId)
                .HasConstraintName("fk_solicitud_tipo_fianza");

            entity.HasMany(d => d.PrendaNavigation).WithMany(p => p.SolicitudFianzasNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "SolicitudFianzaPrendum",
                    r => r.HasOne<Prendum>().WithMany()
                        .HasForeignKey("PrendaId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_sfp_prenda"),
                    l => l.HasOne<SolicitudFianza>().WithMany()
                        .HasForeignKey("SolicitudFianzaId")
                        .HasConstraintName("fk_sfp_solicitud"),
                    j =>
                    {
                        j.HasKey("SolicitudFianzaId", "PrendaId").HasName("pk_solicitud_fianza_prenda");
                        j.ToTable("solicitud_fianza_prenda");
                        j.IndexerProperty<int>("SolicitudFianzaId").HasColumnName("solicitud_fianza_id");
                        j.IndexerProperty<int>("PrendaId").HasColumnName("prenda_id");
                    });
        });

        modelBuilder.Entity<SolicitudFianzaHistorial>(entity =>
        {
            entity.HasKey(e => e.HistorialId).HasName("PK__solicitu__68FE18EE3B139C91");

            entity.ToTable("solicitud_fianza_historial");

            entity.HasIndex(e => e.FechaCambio, "idx_historial_fecha");

            entity.Property(e => e.HistorialId).HasColumnName("historial_id");
            entity.Property(e => e.EstadoFianzaId).HasColumnName("estado_fianza_id");
            entity.Property(e => e.FechaCambio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_cambio");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(1000)
                .HasColumnName("observaciones");
            entity.Property(e => e.SolicitudFianzaId).HasColumnName("solicitud_fianza_id");
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id");

            entity.HasOne(d => d.EstadoFianza).WithMany(p => p.SolicitudFianzaHistorials)
                .HasForeignKey(d => d.EstadoFianzaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_historial_estado");

            entity.HasOne(d => d.SolicitudFianza).WithMany(p => p.SolicitudFianzaHistorials)
                .HasForeignKey(d => d.SolicitudFianzaId)
                .HasConstraintName("fk_historial_fianza");

            entity.HasOne(d => d.Usuario).WithMany(p => p.SolicitudFianzaHistorials)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_historial_usuario");
        });

        modelBuilder.Entity<TipoEmpresa>(entity =>
        {
            entity.HasKey(e => e.TipoEmpresaId).HasName("PK__tipo_emp__A3E275933DF96019");

            entity.ToTable("tipo_empresa");

            entity.Property(e => e.TipoEmpresaId).HasColumnName("tipo_empresa_id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValue((byte)1)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.NombreTipo)
                .HasMaxLength(100)
                .HasColumnName("nombre_tipo");
        });

        modelBuilder.Entity<TipoFianza>(entity =>
        {
            entity.HasKey(e => e.TipoFianzaId).HasName("PK__tipo_fia__2830B2690CEB3AA9");

            entity.ToTable("tipo_fianza");

            entity.HasIndex(e => e.CodigoFianza, "UQ__tipo_fia__0482DF0BBB370CF5").IsUnique();

            entity.HasIndex(e => e.Nombre, "UQ__tipo_fia__72AFBCC6CBACFF4D").IsUnique();

            entity.Property(e => e.TipoFianzaId).HasColumnName("tipo_fianza_id");
            entity.Property(e => e.CodigoFianza)
                .HasMaxLength(50)
                .HasColumnName("codigo_fianza");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValue((byte)1)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.Requisitos)
                .HasMaxLength(1000)
                .HasColumnName("requisitos");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuario__4E3E04AD080F90E6");

            entity.ToTable("usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("contrasena");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.EstadoUsuario)
                .HasDefaultValue((byte)1)
                .HasColumnName("estado_usuario");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdPerfil).HasColumnName("id_perfil");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre_usuario");

            entity.HasOne(d => d.IdPerfilNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuario_perfil");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
