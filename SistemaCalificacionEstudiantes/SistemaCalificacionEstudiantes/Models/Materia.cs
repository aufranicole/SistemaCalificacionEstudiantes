using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaCalificacionEstudiantes.Models
{
    public class Materia
    {
        [Key]
        public int MateriaID { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; }

        [MaxLength(10)]
        public string Codigo { get; set; }

        public virtual ICollection<Calificacion> Calificaciones { get; set; }
    }
}
