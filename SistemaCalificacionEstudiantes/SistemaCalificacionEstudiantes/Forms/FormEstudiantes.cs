using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using SistemaCalificacionEstudiantes.Data;
using SistemaCalificacionEstudiantes.Helpers;
using SistemaCalificacionEstudiantes.Models;

namespace SistemaCalificacionEstudiantes.Forms
{
    public class FormEstudiantes : Form
    {
        private DataGridView dgv;
        private TextBox txtID, txtNombre, txtApellido, txtMatricula;
        private Button btnAgregar, btnActualizar, btnEliminar, btnLimpiar;
        private Button btnExportCSV, btnExportPDF;
        private Label lblTitle;

        public FormEstudiantes()
        {
            InitializeUI();
            CargarDatos();
        }

        private void InitializeUI()
        {
            this.Text = "Gestión de Estudiantes";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 252);
            this.Font = new System.Drawing.Font("Segoe UI", 9f);

            // titulo del form
            lblTitle = new Label
            {
                Text = "ESTUDIANTES",
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(30, 60, 120),
                Location = new System.Drawing.Point(20, 15),
                AutoSize = true
            };

            // panel donde van los campos
            var pnlInputs = new Panel
            {
                Location = new System.Drawing.Point(20, 55),
                Size = new System.Drawing.Size(840, 110),
                BackColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            int lx = 15, ly = 15, lw = 90, tw = 160, spacing = 185;

            // campos de entrada
            pnlInputs.Controls.Add(MakeLabel("ID:", lx, ly));
            txtID = MakeTextBox(lx + lw, ly, tw); pnlInputs.Controls.Add(txtID);
            pnlInputs.Controls.Add(MakeLabel("Nombre:", lx + spacing, ly));
            txtNombre = MakeTextBox(lx + spacing + lw, ly, tw); pnlInputs.Controls.Add(txtNombre);
            pnlInputs.Controls.Add(MakeLabel("Apellido:", lx + spacing * 2, ly));
            txtApellido = MakeTextBox(lx + spacing * 2 + lw, ly, tw); pnlInputs.Controls.Add(txtApellido);
            pnlInputs.Controls.Add(MakeLabel("Matrícula:", lx, ly + 45));
            txtMatricula = MakeTextBox(lx + lw, ly + 45, tw); pnlInputs.Controls.Add(txtMatricula);

            // botones de accion
            int bx = 20, by = 180, bw = 120, bh = 35, bgap = 10;
            btnAgregar    = MakeButton("➕ Agregar",    bx,                        by, bw,  bh, System.Drawing.Color.FromArgb(34, 139, 34));
            btnActualizar = MakeButton("✏️ Actualizar", bx + (bw + bgap),          by, bw,  bh, System.Drawing.Color.FromArgb(30, 100, 180));
            btnEliminar   = MakeButton("🗑️ Eliminar",   bx + (bw + bgap) * 2,      by, bw,  bh, System.Drawing.Color.FromArgb(180, 40, 40));
            btnLimpiar    = MakeButton("🔄 Limpiar",    bx + (bw + bgap) * 3,      by, bw,  bh, System.Drawing.Color.FromArgb(120, 120, 120));
            btnExportCSV  = MakeButton("📄 Export CSV", bx + (bw + bgap) * 4 + 20, by, 130, bh, System.Drawing.Color.FromArgb(30, 60, 120));
            btnExportPDF  = MakeButton("📕 Export PDF", bx + (bw + bgap) * 4 + 160,by, 130, bh, System.Drawing.Color.FromArgb(180, 60, 0));

            btnAgregar.Click    += BtnAgregar_Click;
            btnActualizar.Click += BtnActualizar_Click;
            btnEliminar.Click   += BtnEliminar_Click;
            btnLimpiar.Click    += (s, e) => LimpiarCampos();
            btnExportCSV.Click  += (s, e) => ExportarCSV();
            btnExportPDF.Click  += (s, e) => ExportarPDF();

            // tabla donde se muestran los datos
            dgv = new DataGridView
            {
                Location = new System.Drawing.Point(20, 230),
                Size = new System.Drawing.Size(840, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.FromArgb(30, 60, 120),
                    ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
                },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.CellClick += Dgv_CellClick;

            this.Controls.AddRange(new Control[] { lblTitle, pnlInputs, btnAgregar, btnActualizar, btnEliminar, btnLimpiar, btnExportCSV, btnExportPDF, dgv });
        }

        private void CargarDatos()
        {
            try
            {
                using (var ctx = new AppDbContext())
                {
                    var lista = ctx.Estudiantes.ToList();
                    dgv.DataSource = lista.Select(e => new
                    {
                        e.EstudianteID, e.Nombre, e.Apellido, e.Matricula
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos(false)) return;
            try
            {
                using (var ctx = new AppDbContext())
                {
                    int id = int.Parse(txtID.Text.Trim());
                    if (ctx.Estudiantes.Any(x => x.EstudianteID == id))
                    {
                        MessageBox.Show("Ya existe un estudiante con ese ID.", "ID Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    var est = new Estudiante
                    {
                        EstudianteID = id,
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        Matricula = txtMatricula.Text.Trim()
                    };
                    ctx.Estudiantes.Add(est);
                    ctx.SaveChanges();
                }
                MessageBox.Show("Estudiante agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos(); CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnActualizar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos(true)) return;
            try
            {
                using (var ctx = new AppDbContext())
                {
                    int id = int.Parse(txtID.Text.Trim());
                    var est = ctx.Estudiantes.Find(id);
                    if (est == null)
                    {
                        MessageBox.Show("No existe un estudiante con ese ID.", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    est.Nombre = txtNombre.Text.Trim();
                    est.Apellido = txtApellido.Text.Trim();
                    est.Matricula = txtMatricula.Text.Trim();
                    ctx.Entry(est).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
                MessageBox.Show("Estudiante actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos(); CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text))
            {
                MessageBox.Show("Ingrese el ID del estudiante a eliminar.", "Campo Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtID.Text.Trim(), out int id))
            {
                MessageBox.Show("El ID debe ser un número entero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("¿Desea eliminar este estudiante?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                using (var ctx = new AppDbContext())
                {
                    var est = ctx.Estudiantes.Find(id);
                    if (est == null)
                    {
                        MessageBox.Show("No existe un estudiante con ese ID.", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    // no dejo borrar si tiene calificaciones
                    if (ctx.Calificaciones.Any(c => c.EstudianteID == id))
                    {
                        MessageBox.Show("No se puede eliminar: el estudiante tiene calificaciones registradas.", "Restricción", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    ctx.Estudiantes.Remove(est);
                    ctx.SaveChanges();
                }
                MessageBox.Show("Estudiante eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos(); CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            txtID.Text       = row.Cells["EstudianteID"].Value?.ToString();
            txtNombre.Text   = row.Cells["Nombre"].Value?.ToString();
            txtApellido.Text = row.Cells["Apellido"].Value?.ToString();
            txtMatricula.Text = row.Cells["Matricula"].Value?.ToString();
        }

        private bool ValidarCampos(bool idRequired)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text))
            { MessageBox.Show("El ID es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            if (!int.TryParse(txtID.Text.Trim(), out int id) || id <= 0)
            { MessageBox.Show("El ID debe ser un número entero positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("El Nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            { MessageBox.Show("El Apellido es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            return true;
        }

        private void ExportarCSV()
        {
            try { using (var ctx = new AppDbContext()) ExportHelper.ExportarEstudiantesCSV(ctx.Estudiantes.ToList()); }
            catch (Exception ex) { MessageBox.Show("Error exportando CSV: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ExportarPDF()
        {
            try { using (var ctx = new AppDbContext()) ExportHelper.ExportarEstudiantesPDF(ctx.Estudiantes.ToList()); }
            catch (Exception ex) { MessageBox.Show("Error exportando PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void LimpiarCampos() => txtID.Text = txtNombre.Text = txtApellido.Text = txtMatricula.Text = "";

        // metodos para crear los controles sin repetir tanto codigo
        private Label MakeLabel(string text, int x, int y) => new Label
        {
            Text = text, Location = new System.Drawing.Point(x, y + 3),
            AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
        };
        private TextBox MakeTextBox(int x, int y, int w) => new TextBox
        {
            Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(w, 24),
            Font = new System.Drawing.Font("Segoe UI", 9f)
        };
        private Button MakeButton(string text, int x, int y, int w, int h, System.Drawing.Color color)
        {
            var btn = new Button
            {
                Text = text, Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(w, h),
                BackColor = color, ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat, Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}
