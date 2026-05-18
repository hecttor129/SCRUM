using DAL;
using ENTITY;
using System.Linq;

namespace BLL
{
    /// <summary>
    /// Service para operaciones de negocio relacionadas con Empresa.
    /// </summary>
    public class EmpresaService
    {
        private readonly EmpresaRepository _repo;

        public EmpresaService()
        {
            _repo = new EmpresaRepository();
        }

        /// <summary>
        /// Retorna la primera empresa registrada en el sistema.
        /// </summary>
        public Empresa ObtenerEmpresa()
        {
            return _repo.GetFirst();
        }

        /// <summary>
        /// Crea o actualiza los datos de la empresa.
        /// </summary>
        public void GuardarEmpresa(int idEmpresa, string nombre, string descripcion, string nit, string telefono, string correo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new System.Exception("El nombre de la empresa es obligatorio.");

            if (nombre.Length > 80)
                throw new System.Exception("El nombre no puede superar 80 caracteres.");

            if (!string.IsNullOrWhiteSpace(nit) && nit.Length > 20)
                throw new System.Exception("El NIT no puede superar 20 caracteres.");

            Empresa empresa;
            if (idEmpresa == 0)
            {
                empresa = new Empresa
                {
                    Activo = 1,
                    FechaCreacion = System.DateTime.UtcNow
                };
                _repo.Add(empresa);
            }
            else
            {
                empresa = _repo.GetFirst() ?? new Empresa();
                _repo.Update(empresa);
            }

            empresa.Nombre = nombre;
            empresa.Descripcion = descripcion;
            empresa.Nit = nit;
            empresa.Telefono = telefono;
            empresa.Correo = correo;

            _repo.Save();
        }
    }
}
