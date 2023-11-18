using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapy_Dgii_RNC.Clases
{
    public  class Empresa
    {
        public  string RNC { get; set; }
        public  string Razon_Social { get; set; }
        public  string Nombre { get; set; }
        public  string Categoria { get; set; }
        public  string Pagos { get; set; }
        public  string Estado { get; set; }
        public  string Actividad { get; set; }
        public  string Administracion { get; set; }
    }

    public class EmpresasCollection
    {
        public List<Empresa> Empresas { get; set; }
    }
}
