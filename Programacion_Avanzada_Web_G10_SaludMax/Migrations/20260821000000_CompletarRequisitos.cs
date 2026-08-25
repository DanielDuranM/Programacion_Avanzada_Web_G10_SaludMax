using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Programacion_Avanzada_Web_G10_SaludMax.Data;

#nullable disable

namespace Programacion_Avanzada_Web_G10_SaludMax.Migrations;

[DbContext(typeof(ApplicationDbContext)), Migration("20260821000000_CompletarRequisitos")]
public class CompletarRequisitos : Migration
{
    protected override void Up(MigrationBuilder m)
    {
        m.DropForeignKey("FK_Citas_ServiciosMedicos_ServicioMedicoId", "Citas");
        m.DropForeignKey("FK_Citas_Usuarios_UsuarioId", "Citas");
        m.AlterColumn<string>("Nombre", "Usuarios", "nvarchar(100)", maxLength: 100, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(max)");
        m.AlterColumn<string>("Correo", "Usuarios", "nvarchar(160)", maxLength: 160, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(max)");
        m.AlterColumn<string>("Contrasena", "Usuarios", "nvarchar(255)", maxLength: 255, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(max)");
        m.AlterColumn<string>("Nombre", "ServiciosMedicos", "nvarchar(100)", maxLength: 100, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(max)");
        m.AlterColumn<string>("Descripcion", "ServiciosMedicos", "nvarchar(500)", maxLength: 500, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(max)");
        m.CreateIndex("IX_Usuarios_Correo", "Usuarios", "Correo", unique: true);
        m.CreateIndex("IX_Citas_Fecha_HorarioId", "Citas", new[] { "Fecha", "HorarioId" }, unique: true, filter: "[Estado] <> 3");
        m.AddForeignKey("FK_Citas_ServiciosMedicos_ServicioMedicoId", "Citas", "ServicioMedicoId", "ServiciosMedicos", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        m.AddForeignKey("FK_Citas_Usuarios_UsuarioId", "Citas", "UsuarioId", "Usuarios", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder m)
    {
        m.DropForeignKey("FK_Citas_ServiciosMedicos_ServicioMedicoId", "Citas");
        m.DropForeignKey("FK_Citas_Usuarios_UsuarioId", "Citas");
        m.DropIndex("IX_Usuarios_Correo", "Usuarios");
        m.DropIndex("IX_Citas_Fecha_HorarioId", "Citas");
        m.AlterColumn<string>("Nombre", "Usuarios", "nvarchar(max)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(100)", oldMaxLength: 100);
        m.AlterColumn<string>("Correo", "Usuarios", "nvarchar(max)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(160)", oldMaxLength: 160);
        m.AlterColumn<string>("Contrasena", "Usuarios", "nvarchar(max)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(255)", oldMaxLength: 255);
        m.AlterColumn<string>("Nombre", "ServiciosMedicos", "nvarchar(max)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(100)", oldMaxLength: 100);
        m.AlterColumn<string>("Descripcion", "ServiciosMedicos", "nvarchar(max)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(500)", oldMaxLength: 500);
        m.AddForeignKey("FK_Citas_ServiciosMedicos_ServicioMedicoId", "Citas", "ServicioMedicoId", "ServiciosMedicos", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
        m.AddForeignKey("FK_Citas_Usuarios_UsuarioId", "Citas", "UsuarioId", "Usuarios", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
    }
}
