using DAL;
using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static ENTITY.ENUMS;

namespace BLL
{
    /// <summary>
    /// Item para poblar el ComboBox de superior en UsuarioFormWindow.
    /// </summary>
    public class UsuarioSuperiorItem
    {
        public int    IdUsuario       { get; set; }
        public string NombreCompleto  { get; set; } = string.Empty;
        public int    NivelJerarquico { get; set; }
        public string NombreConNivel  => $"{NombreCompleto} (Nivel {NivelJerarquico})";
    }

    /// <summary>
    /// Service que centraliza toda la lógica de autenticación y gestión de usuarios.
    /// </summary>
    public class UsuarioService
    {
        private readonly UsuarioRepository             _repo;
        private readonly RelacionJerarquicaRepository  _relacionRepo;

        public UsuarioService()
        {
            _repo         = new UsuarioRepository();
            _relacionRepo = new RelacionJerarquicaRepository();
        }

        // ── Utilidad privada ─────────────────────────────────────────────────

        private static string HashPassword(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder(64);
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        // ── Consultas ────────────────────────────────────────────────────────

        /// <summary>
        /// Verifica si existe al menos un Admin activo en el sistema.
        /// </summary>
        public bool ExisteAdmin() => _repo.ExisteAdmin();

        /// <summary>
        /// Retorna todos los usuarios (activos e inactivos) para UsuariosWindow.
        /// </summary>
        public List<Usuario> ObtenerTodos() => _repo.GetAllConInactivos();

        /// <summary>
        /// Retorna el superior directo actual de un usuario (para pre-cargar el form de edición).
        /// </summary>
        public RelacionJerarquica ObtenerSuperiorActual(int idUsuario)
            => _relacionRepo.GetSuperiorActual(idUsuario);

        /// <summary>
        /// Retorna la lista de posibles superiores (Admin + Jefes activos) para el ComboBox.
        /// </summary>
        public List<UsuarioSuperiorItem> ObtenerSupervisoresDisponibles()
        {
            return _repo.GetSupervisoresDisponibles()
                .Select(u => new UsuarioSuperiorItem
                {
                    IdUsuario       = u.IdUsuario,
                    NombreCompleto  = $"{u.Nombre} {u.Apellido}".Trim(),
                    NivelJerarquico = u.NivelJerarquico
                })
                .ToList();
        }

        // ── Autenticación ────────────────────────────────────────────────────

        /// <summary>
        /// Intenta autenticar al usuario. Si es exitoso, puebla SesionActual y retorna true.
        /// </summary>
        public bool Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new Exception("Completa todos los campos.");

            string hash    = HashPassword(password);
            var    usuario = _repo.GetByEmailYPassword(email.Trim().ToLower(), hash);

            if (usuario == null)
                return false;

            SesionActual.IdUsuario      = usuario.IdUsuario;
            SesionActual.NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}".Trim();
            SesionActual.Rol            = usuario.Rol;
            SesionActual.NivelJerarquico = usuario.NivelJerarquico;

            return true;
        }

        // ── Creación ─────────────────────────────────────────────────────────

        /// <summary>
        /// Crea el único Admin del sistema. Solo se debe llamar cuando ExisteAdmin() == false.
        /// </summary>
        public void CrearPrimerAdmin(string nombre, string apellido, string email, string password)
        {
            ValidarCamposBase(nombre, apellido, email, password);

            if (_repo.EmailExiste(email.Trim()))
                throw new Exception("Ese correo ya está registrado.");

            var admin = new Usuario
            {
                Nombre          = nombre.Trim(),
                Apellido        = apellido.Trim(),
                Email           = email.Trim().ToLower(),
                password        = HashPassword(password),
                Rol             = RolUsuario.Admin,
                NivelJerarquico = 1,
                Activo          = 1,
                FechaCreacion   = DateTime.Now
            };

            _repo.Add(admin);
            _repo.Save();
            // El Admin no tiene RelacionJerarquica (no tiene superior)
        }

        /// <summary>
        /// Crea un nuevo usuario (Jefe o Empleado). Solo el Admin puede invocar esto.
        /// </summary>
        public void CrearUsuario(
            string nombre, string apellido, string email,
            string password, RolUsuario rol, int idSuperior)
        {
            ValidarCamposBase(nombre, apellido, email, password);

            if (rol == RolUsuario.Admin)
                throw new Exception("No se puede crear otro administrador.");

            if (_repo.EmailExiste(email.Trim()))
                throw new Exception("Ese correo ya está registrado.");

            var superior = _repo.GetById(idSuperior);
            if (superior == null)
                throw new Exception("El superior seleccionado no existe.");

            var nuevo = new Usuario
            {
                Nombre          = nombre.Trim(),
                Apellido        = apellido.Trim(),
                Email           = email.Trim().ToLower(),
                password        = HashPassword(password),
                Rol             = rol,
                NivelJerarquico = superior.NivelJerarquico + 1,
                Activo          = 1,
                FechaCreacion   = DateTime.Now
            };

            _repo.Add(nuevo);
            _repo.Save();

            _relacionRepo.Crear(idSuperior, nuevo.IdUsuario);
        }

        // ── Edición ──────────────────────────────────────────────────────────

        /// <summary>
        /// Edita los datos de un usuario existente (no Admin). Actualiza la relación jerárquica si cambia el superior.
        /// </summary>
        public void EditarUsuario(
            int idUsuario, string nombre, string apellido,
            string email, RolUsuario rol, decimal? salario, int idSuperior)
        {
            var usuario = _repo.GetById(idUsuario)
                ?? throw new Exception("Usuario no encontrado.");

            if (usuario.Rol == RolUsuario.Admin)
                throw new Exception("No se puede editar al administrador.");

            ValidarNombreApellidoEmail(nombre, apellido, email);

            if (rol == RolUsuario.Admin)
                throw new Exception("No se puede asignar el rol de administrador.");

            if (_repo.EmailExiste(email.Trim(), exceptoIdUsuario: idUsuario))
                throw new Exception("Ese correo ya está en uso por otro usuario.");

            var superior = _repo.GetById(idSuperior)
                ?? throw new Exception("El superior seleccionado no existe.");

            usuario.Nombre          = nombre.Trim();
            usuario.Apellido        = apellido.Trim();
            usuario.Email           = email.Trim().ToLower();
            usuario.Rol             = rol;
            usuario.NivelJerarquico = superior.NivelJerarquico + 1;
            usuario.Salario         = salario;

            _repo.Update(usuario);
            _repo.Save();

            // Actualizar relación jerárquica si el superior cambió
            var relacionActual = _relacionRepo.GetSuperiorActual(idUsuario);
            if (relacionActual == null || relacionActual.IdJefe != idSuperior)
            {
                _relacionRepo.CerrarRelacion(idUsuario);
                _relacionRepo.Crear(idSuperior, idUsuario);
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario (hashea antes de guardar).
        /// </summary>
        public void CambiarPassword(int idUsuario, string nuevaPassword)
        {
            if (string.IsNullOrWhiteSpace(nuevaPassword) || nuevaPassword.Length < 6)
                throw new Exception("La contraseña debe tener al menos 6 caracteres.");

            var usuario = _repo.GetById(idUsuario)
                ?? throw new Exception("Usuario no encontrado.");

            usuario.password = HashPassword(nuevaPassword);
            _repo.Update(usuario);
            _repo.Save();
        }

        // ── Activación / Desactivación ───────────────────────────────────────

        /// <summary>
        /// Desactiva un usuario (soft delete). No se puede desactivar al Admin.
        /// </summary>
        public void DesactivarUsuario(int idUsuario)
        {
            var usuario = _repo.GetById(idUsuario)
                ?? throw new Exception("Usuario no encontrado.");

            if (usuario.Rol == RolUsuario.Admin)
                throw new Exception("No se puede desactivar al administrador.");

            usuario.Activo = 0;
            _repo.Update(usuario);
            _repo.Save();
        }

        /// <summary>
        /// Reactiva un usuario previamente desactivado.
        /// </summary>
        public void ReactivarUsuario(int idUsuario)
        {
            var usuario = _repo.GetById(idUsuario)
                ?? throw new Exception("Usuario no encontrado.");

            usuario.Activo = 1;
            _repo.Update(usuario);
            _repo.Save();
        }

        // ── Validaciones ─────────────────────────────────────────────────────

        private static void ValidarCamposBase(
            string nombre, string apellido, string email, string password)
        {
            ValidarNombreApellidoEmail(nombre, apellido, email);

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new Exception("La contraseña debe tener al menos 6 caracteres.");
        }

        private static void ValidarNombreApellidoEmail(
            string nombre, string apellido, string email)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre es obligatorio.");
            if (nombre.Length > 15)
                throw new Exception("El nombre no puede superar 15 caracteres.");

            if (string.IsNullOrWhiteSpace(apellido))
                throw new Exception("El apellido es obligatorio.");
            if (apellido.Length > 15)
                throw new Exception("El apellido no puede superar 15 caracteres.");

            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("El correo es obligatorio.");
            if (email.Length > 30)
                throw new Exception("El correo no puede superar 30 caracteres.");
            if (!email.Contains('@') || !email.Contains('.'))
                throw new Exception("El formato del correo no es válido.");
        }
    }
}
