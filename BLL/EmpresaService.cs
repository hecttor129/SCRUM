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
        /// Actualiza los datos de la empresa.
        /// </summary>
        public void EditarEmpresa(Empresa empresa)
        {
            if (string.IsNullOrWhiteSpace(empresa.Nombre))
                throw new System.Exception("El nombre de la empresa es obligatorio.");

            if (empresa.Nombre.Length > 80)
                throw new System.Exception("El nombre no puede superar 80 caracteres.");

            if (!string.IsNullOrWhiteSpace(empresa.Nit) && empresa.Nit.Length > 20)
                throw new System.Exception("El NIT no puede superar 20 caracteres.");

            _repo.Update(empresa);
            _repo.Save();
        }
    }
}
