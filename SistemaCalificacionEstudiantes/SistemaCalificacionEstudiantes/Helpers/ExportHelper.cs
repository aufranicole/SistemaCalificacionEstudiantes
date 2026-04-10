using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SistemaCalificacionEstudiantes.Models;

namespace SistemaCalificacionEstudiantes.Helpers
{
    public static class ExportHelper
    {
        public static void ExportarEstudiantesCSV(List<Estudiante> lista)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "Estudiantes_" + DateTime.Now.ToString("yyyyMMdd");
                if (sfd.ShowDialog() != DialogResult.OK) return;

                var sb = new StringBuilder();
                sb.AppendLine("EstudianteID,Nombre,Apellido,Matricula");
                foreach (var e in lista)
                    sb.AppendLine($"{e.EstudianteID},{e.Nombre},{e.Apellido},{e.Matricula}");

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Exportado exitosamente.", "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static void ExportarMateriasCSV(List<Materia> lista)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "Materias_" + DateTime.Now.ToString("yyyyMMdd");
                if (sfd.ShowDialog() != DialogResult.OK) return;

                var sb = new StringBuilder();
                sb.AppendLine("MateriaID,Nombre,Codigo");
                foreach (var m in lista)
                    sb.AppendLine($"{m.MateriaID},{m.Nombre},{m.Codigo}");

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Exportado exitosamente.", "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static void ExportarCalificacionesCSV(List<Calificacion> lista)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "Calificaciones_" + DateTime.Now.ToString("yyyyMMdd");
                if (sfd.ShowDialog() != DialogResult.OK) return;

                var sb = new StringBuilder();
                sb.AppendLine("ID,Estudiante,Materia,Cal1,Cal2,Cal3,Cal4,Examen,Total,Clasificacion,Estado");
                foreach (var c in lista)
                {
                    string est = c.Estudiante != null ? $"{c.Estudiante.Nombre} {c.Estudiante.Apellido}" : c.EstudianteID.ToString();
                    string mat = c.Materia != null ? c.Materia.Nombre : c.MateriaID.ToString();
                    sb.AppendLine($"{c.CalificacionID},{est},{mat},{c.Calificacion1},{c.Calificacion2},{c.Calificacion3},{c.Calificacion4},{c.Examen},{c.TotalCalificacionDB},{c.ClasificacionDB},{c.EstadoDB}");
                }

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Exportado exitosamente.", "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // estilos del pdf
        private static Font FontTitulo => new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD, BaseColor.DARK_GRAY);
        private static Font FontSubtitulo => new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.WHITE);
        private static Font FontNormal => new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL);
        private static BaseColor ColorHeader = new BaseColor(30, 60, 120);
        private static BaseColor ColorRow1 = new BaseColor(240, 245, 255);
        private static BaseColor ColorRow2 = BaseColor.WHITE;

        private static PdfPCell HeaderCell(string text)
        {
            var cell = new PdfPCell(new Phrase(text, FontSubtitulo))
            {
                BackgroundColor = ColorHeader,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 6
            };
            return cell;
        }

        private static PdfPCell DataCell(string text, bool isAlt)
        {
            var cell = new PdfPCell(new Phrase(text, FontNormal))
            {
                BackgroundColor = isAlt ? ColorRow1 : ColorRow2,
                Padding = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            return cell;
        }

        public static void ExportarEstudiantesPDF(List<Estudiante> lista)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = "Estudiantes_" + DateTime.Now.ToString("yyyyMMdd");
                if (sfd.ShowDialog() != DialogResult.OK) return;

                var doc = new Document(PageSize.A4, 30, 30, 40, 30);
                PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                doc.Open();
                doc.Add(new Paragraph("Reporte de Estudiantes", FontTitulo) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 15 });
                doc.Add(new Paragraph($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}", FontNormal) { Alignment = Element.ALIGN_RIGHT, SpacingAfter = 10 });

                var table = new PdfPTable(4) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 1f, 3f, 3f, 2f });
                table.AddCell(HeaderCell("ID"));
                table.AddCell(HeaderCell("Nombre"));
                table.AddCell(HeaderCell("Apellido"));
                table.AddCell(HeaderCell("Matrícula"));

                for (int i = 0; i < lista.Count; i++)
                {
                    var e = lista[i]; bool alt = i % 2 == 0;
                    table.AddCell(DataCell(e.EstudianteID.ToString(), alt));
                    table.AddCell(DataCell(e.Nombre, alt));
                    table.AddCell(DataCell(e.Apellido, alt));
                    table.AddCell(DataCell(e.Matricula ?? "-", alt));
                }
                doc.Add(table);
                doc.Close();
                MessageBox.Show("PDF generado exitosamente.", "PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static void ExportarMateriasPDF(List<Materia> lista)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = "Materias_" + DateTime.Now.ToString("yyyyMMdd");
                if (sfd.ShowDialog() != DialogResult.OK) return;

                var doc = new Document(PageSize.A4, 30, 30, 40, 30);
                PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                doc.Open();
                doc.Add(new Paragraph("Reporte de Materias", FontTitulo) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 15 });
                doc.Add(new Paragraph($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}", FontNormal) { Alignment = Element.ALIGN_RIGHT, SpacingAfter = 10 });

                var table = new PdfPTable(3) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 1f, 4f, 2f });
                table.AddCell(HeaderCell("ID")); table.AddCell(HeaderCell("Nombre")); table.AddCell(HeaderCell("Código"));

                for (int i = 0; i < lista.Count; i++)
                {
                    var m = lista[i]; bool alt = i % 2 == 0;
                    table.AddCell(DataCell(m.MateriaID.ToString(), alt));
                    table.AddCell(DataCell(m.Nombre, alt));
                    table.AddCell(DataCell(m.Codigo ?? "-", alt));
                }
                doc.Add(table);
                doc.Close();
                MessageBox.Show("PDF generado exitosamente.", "PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static void ExportarCalificacionesPDF(List<Calificacion> lista)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = "Calificaciones_" + DateTime.Now.ToString("yyyyMMdd");
                if (sfd.ShowDialog() != DialogResult.OK) return;

                var doc = new Document(PageSize.A4.Rotate(), 20, 20, 40, 20);
                PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                doc.Open();
                doc.Add(new Paragraph("Reporte de Calificaciones", FontTitulo) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 15 });
                doc.Add(new Paragraph($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}", FontNormal) { Alignment = Element.ALIGN_RIGHT, SpacingAfter = 10 });

                var table = new PdfPTable(11) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 0.5f, 2.5f, 2f, 1f, 1f, 1f, 1f, 1f, 1.2f, 1f, 1.2f });
                foreach (string h in new[] { "ID", "Estudiante", "Materia", "Cal.1", "Cal.2", "Cal.3", "Cal.4", "Examen", "Total", "Clasif.", "Estado" })
                    table.AddCell(HeaderCell(h));

                for (int i = 0; i < lista.Count; i++)
                {
                    var c = lista[i]; bool alt = i % 2 == 0;
                    string est = c.Estudiante != null ? $"{c.Estudiante.Nombre} {c.Estudiante.Apellido}" : c.EstudianteID.ToString();
                    string mat = c.Materia != null ? c.Materia.Nombre : c.MateriaID.ToString();
                    table.AddCell(DataCell(c.CalificacionID.ToString(), alt));
                    table.AddCell(DataCell(est, alt));
                    table.AddCell(DataCell(mat, alt));
                    table.AddCell(DataCell(c.Calificacion1.ToString("0.00"), alt));
                    table.AddCell(DataCell(c.Calificacion2.ToString("0.00"), alt));
                    table.AddCell(DataCell(c.Calificacion3.ToString("0.00"), alt));
                    table.AddCell(DataCell(c.Calificacion4.ToString("0.00"), alt));
                    table.AddCell(DataCell(c.Examen.ToString("0.00"), alt));
                    table.AddCell(DataCell(c.TotalCalificacionDB.ToString("0.00"), alt));
                    table.AddCell(DataCell(c.ClasificacionDB, alt));
                    table.AddCell(DataCell(c.EstadoDB, alt));
                }
                doc.Add(table);
                doc.Close();
                MessageBox.Show("PDF generado exitosamente.", "PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
