using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BLL
{
    public class ArchivoDto
    {
        public int IdArchivo { get; set; }
        public string NombreOriginal { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string Tamano { get; set; } = string.Empty;
        public string FechaSubida { get; set; } = string.Empty;
        public int IdUsuarioSubidoPor { get; set; }
        public string SubidoPor { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
    }

    public class ArchivoService
    {
        private ArchivoRepository _repo = new();
        private const long MaxFileSizeBytes = 15 * 1024 * 1024; // Límite estándar de 15 MB

        private string GetStorageDirectory()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string targetDir = Path.Combine(baseDir, "ArchivosEquipos");
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            return targetDir;
        }

        public List<ArchivoDto> ObtenerArchivosPorEquipo(int idEquipo)
        {
            _repo = new ArchivoRepository();
            var archivos = _repo.GetByEquipo(idEquipo);

            return archivos.Select(a => new ArchivoDto
            {
                IdArchivo = a.IdArchivo,
                NombreOriginal = a.NombreOriginal,
                Extension = a.Extension.ToUpper(),
                Tamano = FormatearTamano(a.TamanoKB),
                FechaSubida = a.FechaSubida.ToString("dd/MM/yyyy HH:mm"),
                IdUsuarioSubidoPor = a.IdUsuarioSubidoPor,
                SubidoPor = a.UsuarioSubidoPor != null 
                    ? $"{a.UsuarioSubidoPor.Nombre} {a.UsuarioSubidoPor.Apellido}" 
                    : "Usuario Desconocido"
            }).ToList();
        }

        public List<ArchivoDto> ObtenerArchivosPorProyectoYEquipos(int idProyecto)
        {
            _repo = new ArchivoRepository();
            var archivos = _repo.GetByProyectoAndAllTeams(idProyecto);

            return archivos.Select(a => new ArchivoDto
            {
                IdArchivo = a.IdArchivo,
                NombreOriginal = a.NombreOriginal,
                Extension = a.Extension.ToUpper(),
                Tamano = FormatearTamano(a.TamanoKB),
                FechaSubida = a.FechaSubida.ToString("dd/MM/yyyy HH:mm"),
                IdUsuarioSubidoPor = a.IdUsuarioSubidoPor,
                SubidoPor = a.UsuarioSubidoPor != null 
                    ? $"{a.UsuarioSubidoPor.Nombre} {a.UsuarioSubidoPor.Apellido}" 
                    : "Usuario Desconocido",
                Origen = a.IdProyecto.HasValue 
                    ? "Proyecto General" 
                    : (a.Equipo != null ? $"Equipo: {a.Equipo.Nombre}" : "Equipo")
            }).ToList();
        }

        public void SubirArchivo(string rutaOrigen, int idEquipo, int idUsuarioLogueado)
        {
            if (!File.Exists(rutaOrigen))
                throw new FileNotFoundException("El archivo seleccionado ya no existe en la ruta de origen.");

            var fileInfo = new FileInfo(rutaOrigen);
            if (fileInfo.Length > MaxFileSizeBytes)
                throw new ArgumentException("El archivo excede el límite de tamaño estándar de 15 MB.");

            string storageDir = GetStorageDirectory();
            string nombreOriginal = fileInfo.Name;
            string extension = fileInfo.Extension;
            string nombreFisico = Guid.NewGuid().ToString() + extension;
            string rutaDestino = Path.Combine(storageDir, nombreFisico);

            // Copiar el archivo físicamente
            File.Copy(rutaOrigen, rutaDestino, true);

            // Crear el registro de base de datos
            var archivo = new Archivo
            {
                NombreOriginal = nombreOriginal,
                NombreFisico = nombreFisico,
                Extension = extension,
                TamanoKB = Math.Round((double)fileInfo.Length / 1024, 2),
                FechaSubida = DateTime.Now,
                IdEquipo = idEquipo,
                IdUsuarioSubidoPor = idUsuarioLogueado
            };

            _repo.Add(archivo);
            _repo.Save();
        }

        public void SubirArchivoProyecto(string rutaOrigen, int idProyecto, int idUsuarioLogueado, string nombrePersonalizado = "")
        {
            if (!File.Exists(rutaOrigen))
                throw new FileNotFoundException("El archivo seleccionado ya no existe en la ruta de origen.");

            var fileInfo = new FileInfo(rutaOrigen);
            if (fileInfo.Length > MaxFileSizeBytes)
                throw new ArgumentException("El archivo excede el límite de tamaño estándar de 15 MB.");

            string storageDir = GetStorageDirectory();
            string nombreOriginal = !string.IsNullOrWhiteSpace(nombrePersonalizado) ? nombrePersonalizado : fileInfo.Name;
            string extension = fileInfo.Extension;
            string nombreFisico = Guid.NewGuid().ToString() + extension;
            string rutaDestino = Path.Combine(storageDir, nombreFisico);

            // Copiar el archivo físicamente
            File.Copy(rutaOrigen, rutaDestino, true);

            // Crear el registro de base de datos
            var archivo = new Archivo
            {
                NombreOriginal = nombreOriginal,
                NombreFisico = nombreFisico,
                Extension = extension,
                TamanoKB = Math.Round((double)fileInfo.Length / 1024, 2),
                FechaSubida = DateTime.Now,
                IdProyecto = idProyecto,
                IdUsuarioSubidoPor = idUsuarioLogueado
            };

            _repo.Add(archivo);
            _repo.Save();
        }

        public void DescargarArchivo(int idArchivo, string rutaDestino)
        {
            _repo = new ArchivoRepository();
            var archivo = _repo.GetById(idArchivo);
            if (archivo == null)
                throw new Exception("El archivo no está registrado en la base de datos.");

            string storageDir = GetStorageDirectory();
            string rutaFisica = Path.Combine(storageDir, archivo.NombreFisico);

            if (!File.Exists(rutaFisica))
                throw new FileNotFoundException("El archivo físico no se encuentra en el servidor local.");

            File.Copy(rutaFisica, rutaDestino, true);
        }

        public string ObtenerRutaTemporalParaAbrir(int idArchivo)
        {
            _repo = new ArchivoRepository();
            var archivo = _repo.GetById(idArchivo);
            if (archivo == null)
                throw new Exception("El archivo no está registrado.");

            string storageDir = GetStorageDirectory();
            string rutaFisica = Path.Combine(storageDir, archivo.NombreFisico);

            if (!File.Exists(rutaFisica))
                throw new FileNotFoundException("El archivo físico no existe en el servidor local.");

            // Crear una copia en el directorio temporal del sistema para poder abrirlo
            string tempDir = Path.Combine(Path.GetTempPath(), "SCRUM_Archivos");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            string tempPath = Path.Combine(tempDir, archivo.NombreOriginal);
            File.Copy(rutaFisica, tempPath, true);

            return tempPath;
        }

        public void EliminarArchivo(int idArchivo, int idUsuarioLogueado, string rolUsuarioLogueado)
        {
            _repo = new ArchivoRepository();
            var archivo = _repo.GetById(idArchivo);
            if (archivo == null)
                return; // Ya no existe

            // Regla de negocio de permisos:
            // Solo el propietario, Jefes y Admins pueden eliminar.
            bool esPropietario = archivo.IdUsuarioSubidoPor == idUsuarioLogueado;
            bool esJefeOAdmin = rolUsuarioLogueado.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
                                rolUsuarioLogueado.Equals("Jefe", StringComparison.OrdinalIgnoreCase);

            if (!esPropietario && !esJefeOAdmin)
            {
                throw new UnauthorizedAccessException("No tienes permisos para eliminar este archivo. Solo el propietario, jefes o administradores pueden hacerlo.");
            }

            // Eliminar archivo físico
            string storageDir = GetStorageDirectory();
            string rutaFisica = Path.Combine(storageDir, archivo.NombreFisico);
            if (File.Exists(rutaFisica))
            {
                try
                {
                    File.Delete(rutaFisica);
                }
                catch (IOException)
                {
                    // Ignorar si está bloqueado temporalmente por otro proceso, pero continuar el borrado de la BD
                }
            }

            // Eliminar registro
            _repo.Delete(idArchivo);
            _repo.Save();
        }

        private string FormatearTamano(double tamanoKB)
        {
            if (tamanoKB >= 1024)
            {
                return $"{Math.Round(tamanoKB / 1024, 2)} MB";
            }
            return $"{tamanoKB} KB";
        }
    }
}
