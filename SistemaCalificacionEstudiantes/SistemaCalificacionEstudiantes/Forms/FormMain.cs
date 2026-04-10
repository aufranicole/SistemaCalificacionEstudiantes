using System;
using System.Drawing;
using System.Windows.Forms;
using SistemaCalificacionEstudiantes.Data;

namespace SistemaCalificacionEstudiantes.Forms
{
    public class FormMain : Form
    {
        public FormMain()
        {
            InitializeUI();
            InicializarBaseDeDatos();
        }

        private void InitializeUI()
        {
            this.Text = "Sistema de Calificación de Estudiantes";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 252);
            this.Font = new Font("Segoe UI", 9f);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // cabecera azul de arriba
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(30, 60, 120)
            };

            var lblHeader = new Label
            {
                Text = "SISTEMA DE CALIFICACIÓN DE ESTUDIANTES",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            var lblSub = new Label
            {
                Text = "INF-4312  |  UFHEC",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 200, 240),
                TextAlign = ContentAlignment.BottomCenter,
                Location = new Point(0, 52),
                Size = new Size(700, 20)
            };

            pnlHeader.Controls.Add(lblHeader);
            pnlHeader.Controls.Add(lblSub);

            // los botones del menu principal
            var pnlMenu = new Panel
            {
                Location = new Point(50, 110),
                Size = new Size(580, 320),
                BackColor = Color.Transparent
            };

            // cada opcion del menu
            var btnEstudiantes   = MakeMenuCard("👥", "Estudiantes",    "Gestionar estudiantes registrados",       0,   0,   Color.FromArgb(30, 100, 180));
            var btnMaterias      = MakeMenuCard("📚", "Materias",       "Gestionar materias del sistema",          300, 0,   Color.FromArgb(34, 120, 34));
            var btnCalificaciones = MakeMenuCard("📊", "Calificaciones", "Registrar y consultar calificaciones",    0,   160, Color.FromArgb(140, 60, 10));
            var btnSalir         = MakeMenuCard("🚪", "Salir",          "Cerrar el sistema",                       300, 160, Color.FromArgb(100, 100, 100));

            btnEstudiantes.Click    += (s, e) => new FormEstudiantes().ShowDialog();
            btnMaterias.Click       += (s, e) => new FormMaterias().ShowDialog();
            btnCalificaciones.Click += (s, e) => new FormCalificaciones().ShowDialog();
            btnSalir.Click += (s, e) =>
            {
                if (MessageBox.Show("¿Desea salir del sistema?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Application.Exit();
            };

            pnlMenu.Controls.AddRange(new Control[] { btnEstudiantes, btnMaterias, btnCalificaciones, btnSalir });

            // pie de pagina
            var lblFooter = new Label
            {
                Text = $"© {DateTime.Now.Year} - Sistema de Calificación  |  Proyecto Final INF-4312",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 440),
                Size = new Size(680, 20)
            };

            this.Controls.AddRange(new Control[] { pnlHeader, pnlMenu, lblFooter });
        }

        private Button MakeMenuCard(string icon, string title, string desc, int x, int y, Color color)
        {
            var btn = new Button
            {
                Location = new Point(x, y),
                Size = new Size(270, 140),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Text = $"{icon}\r\n\r\n{title}\r\n{desc}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(color, 0.15f);

            // efecto hover en los botones
            btn.MouseEnter += (s, e) => btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.MouseLeave += (s, e) => btn.Font = new Font("Segoe UI", 9, FontStyle.Regular);

            return btn;
        }

        private void InicializarBaseDeDatos()
        {
            try
            {
                using (var ctx = new AppDbContext())
                    ctx.Database.Initialize(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar la base de datos:\n" + ex.Message,
                    "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
