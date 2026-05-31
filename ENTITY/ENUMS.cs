using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class ENUMS
    {
        public enum RolUsuario 
        { 
            Admin, 
            Jefe, 
            Empleado 
        }
        public enum EstadoTarea
        {
            Pendiente,
            EnProgreso,
            Completada,
            Cancelada
        }

        public enum TipoAsignacion 
        { 
            Manual, 
            Automatica 
        }

        public enum EstadoAsignacion 
        { 
            Activa, 
            Finalizada, 
            Reasignada 
        }



    }
}
