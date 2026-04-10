using System;
using System.Windows.Forms;
using SistemaCalificacionEstudiantes.Forms;

namespace SistemaCalificacionEstudiantes
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // atrapo cualquier error que no se capture en otro lado
            Application.ThreadException += (sender, e) =>
            {
                MessageBox.Show("Error inesperado: " + e.Exception.Message,
                    "Error del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                MessageBox.Show("Error crítico: " + e.ExceptionObject.ToString(),
                    "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            Application.Run(new FormMain());
        }
    }
}
