using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaCalificacionEstudiantes.Models
{
    public class Estudiante
    {
        [Key]
        public int EstudianteID { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        [Required, MaxLength(100)]
        public string Apellido { get; set; }

        [MaxLength(20)]
        public string Matricula { get; set; }

        public virtual ICollection<Calificacion> Calificaciones { get; set; }
    }
}
