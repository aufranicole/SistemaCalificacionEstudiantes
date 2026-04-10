using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCalificacionEstudiantes.Models
{
    public class Calificacion
    {
        [Key]
        public int CalificacionID { get; set; }

        [Required]
        public int EstudianteID { get; set; }

        [Required]
        public int MateriaID { get; set; }

        [Required, Range(0, 100)]
        public decimal Calificacion1 { get; set; }

        [Required, Range(0, 100)]
        public decimal Calificacion2 { get; set; }

        [Required, Range(0, 100)]
        public decimal Calificacion3 { get; set; }

        [Required, Range(0, 100)]
        public decimal Calificacion4 { get; set; }

        [Required, Range(0, 100)]
        public decimal Examen { get; set; }

        // esto lo calculo yo, no se guarda directo en la bd
        [NotMapped]
        public decimal TotalCalificacion
        {
            get
            {
                decimal promedio = (Calificacion1 + Calificacion2 + Calificacion3 + Calificacion4) / 4;
                return (promedio * 0.70m) + (Examen * 0.30m);
            }
        }

        // este si va a la bd, lo lleno antes de guardar
        public decimal TotalCalificacionDB { get; set; }

        [NotMapped]
        public string Clasificacion
        {
            get
            {
                decimal total = TotalCalificacion;
                if (total >= 90) return "A";
                if (total >= 80) return "B";
                if (total >= 70) return "C";
                return "F";
            }
        }

        public string ClasificacionDB { get; set; }

        [NotMapped]
        public string Estado => TotalCalificacion >= 70 ? "Aprobado" : "Reprobado";

        public string EstadoDB { get; set; }

        [ForeignKey("EstudianteID")]
        public virtual Estudiante Estudiante { get; set; }

        [ForeignKey("MateriaID")]
        public virtual Materia Materia { get; set; }

        // llamo esto antes de hacer el SaveChanges para que los calculados queden bien en la bd
        public void ActualizarCalculados()
        {
            TotalCalificacionDB = TotalCalificacion;
            ClasificacionDB = Clasificacion;
            EstadoDB = Estado;
        }
    }
}
