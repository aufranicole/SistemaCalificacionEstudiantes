using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using SistemaCalificacionEstudiantes.Data;
using SistemaCalificacionEstudiantes.Helpers;
using SistemaCalificacionEstudiantes.Models;

namespace SistemaCalificacionEstudiantes.Forms
{
    public class FormMaterias : Form
    {
        private DataGridView dgv;
        private TextBox txtID, txtNombre, txtCodigo;
        private Button btnAgregar, btnActualizar, btnEliminar, btnLimpiar;
        private Button btnExportCSV, btnExportPDF;

        public FormMaterias()
        {
            InitializeUI();
            CargarDatos();
        }

        private void InitializeUI()
        {
            this.Text = "Gestión de Materias";
            this.Size = new System.Drawing.Size(800, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 252);
            this.Font = new System.Drawing.Font("Segoe UI", 9f);

            var lblTitle = new Label
            {
                Text = "MATERIAS",
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(30, 60, 120),
                Location = new System.Drawing.Point(20, 15), AutoSize = true
            };

            var pnl = new Panel
            {
                Location = new System.Drawing.Point(20, 55), Size = new System.Drawing.Size(740, 80),
                BackColor = System.Drawing.Color.White, BorderStyle = BorderStyle.FixedSingle
            };

            int lw = 80, tw = 180;
            pnl.Controls.Add(MakeLabel("ID:", 15, 20));
            txtID = MakeTextBox(15 + lw, 20, 100); pnl.Controls.Add(txtID);
            pnl.Controls.Add(MakeLabel("Nombre:", 230, 20));
            txtNombre = MakeTextBox(230 + lw, 20, tw); pnl.Controls.Add(txtNombre);
            pnl.Controls.Add(MakeLabel("Código:", 480, 20));
            txtCodigo = MakeTextBox(480 + lw, 20, 120); pnl.Controls.Add(txtCodigo);

            int bx = 20, by = 150, bw = 120, bh = 35, gap = 10;
            btnAgregar    = MakeButton("➕ Agregar",    bx,                   by, bw,  bh, System.Drawing.Color.FromArgb(34, 139, 34));
            btnActualizar = MakeButton("✏️ Actualizar", bx + (bw + gap),      by, bw,  bh, System.Drawing.Color.FromArgb(30, 100, 180));
            btnEliminar   = MakeButton("🗑️ Eliminar",   bx + (bw + gap) * 2,  by, bw,  bh, System.Drawing.Color.FromArgb(180, 40, 40));
            btnLimpiar    = MakeButton("🔄 Limpiar",    bx + (bw + gap) * 3,  by, bw,  bh, System.Drawing.Color.FromArgb(120, 120, 120));
            btnExportCSV  = MakeButton("📄 Export CSV", bx + (bw + gap) * 4 + 20,  by, 130, bh, System.Drawing.Color.FromArgb(30, 60, 120));
            btnExportPDF  = MakeButton("📕 Export PDF", bx + (bw + gap) * 4 + 160, by, 130, bh, System.Drawing.Color.FromArgb(180, 60, 0));

            btnAgregar.Click    += BtnAgregar_Click;
            btnActualizar.Click += BtnActualizar_Click;
            btnEliminar.Click   += BtnEliminar_Click;
            btnLimpiar.Click    += (s, e) => LimpiarCampos();
            btnExportCSV.Click  += (s, e) => { try { using (var ctx = new AppDbContext()) ExportHelper.ExportarMateriasCSV(ctx.Materias.ToList()); } catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } };
            btnExportPDF.Click  += (s, e) => { try { using (var ctx = new AppDbContext()) ExportHelper.ExportarMateriasPDF(ctx.Materias.ToList()); } catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } };

            dgv = new DataGridView
            {
                Location = new System.Drawing.Point(20, 200), Size = new System.Drawing.Size(740, 290),
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = System.Drawing.Color.White,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.FromArgb(30, 60, 120),
                    ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
                },
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                var row = dgv.Rows[e.RowIndex];
                txtID.Text     = row.Cells["MateriaID"].Value?.ToString();
                txtNombre.Text = row.Cells["Nombre"].Value?.ToString();
                txtCodigo.Text = row.Cells["Codigo"].Value?.ToString();
            };

            this.Controls.AddRange(new Control[] { lblTitle, pnl, btnAgregar, btnActualizar, btnEliminar, btnLimpiar, btnExportCSV, btnExportPDF, dgv });
        }

        private void CargarDatos()
        {
            try
            {
                using (var ctx = new AppDbContext())
                    dgv.DataSource = ctx.Materias.Select(m => new { m.MateriaID, m.Nombre, m.Codigo }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;
            try
            {
                using (var ctx = new AppDbContext())
                {
                    int id = int.Parse(txtID.Text.Trim());
                    if (ctx.Materias.Any(m => m.MateriaID == id))
                    { MessageBox.Show("Ya existe una materia con ese ID.", "ID Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    ctx.Materias.Add(new Materia { MateriaID = id, Nombre = txtNombre.Text.Trim(), Codigo = txtCodigo.Text.Trim() });
                    ctx.SaveChanges();
                }
                MessageBox.Show("Materia agregada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos(); CargarDatos();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnActualizar_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;
            try
            {
                using (var ctx = new AppDbContext())
                {
                    int id = int.Parse(txtID.Text.Trim());
                    var mat = ctx.Materias.Find(id);
                    if (mat == null) { MessageBox.Show("No existe esa materia.", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    mat.Nombre = txtNombre.Text.Trim();
                    mat.Codigo = txtCodigo.Text.Trim();
                    ctx.Entry(mat).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
                MessageBox.Show("Materia actualizada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos(); CargarDatos();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text) || !int.TryParse(txtID.Text, out int id))
            { MessageBox.Show("Ingrese un ID válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (MessageBox.Show("¿Eliminar esta materia?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                using (var ctx = new AppDbContext())
                {
                    var mat = ctx.Materias.Find(id);
                    if (mat == null) { MessageBox.Show("No existe esa materia.", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (ctx.Calificaciones.Any(c => c.MateriaID == id)) { MessageBox.Show("No se puede eliminar: tiene calificaciones asociadas.", "Restricción", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    ctx.Materias.Remove(mat);
                    ctx.SaveChanges();
                }
                MessageBox.Show("Materia eliminada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos(); CargarDatos();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtID.Text) || !int.TryParse(txtID.Text.Trim(), out int id) || id <= 0)
            { MessageBox.Show("El ID debe ser un número entero positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("El Nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            return true;
        }

        private void LimpiarCampos() => txtID.Text = txtNombre.Text = txtCodigo.Text = "";

        private Label MakeLabel(string t, int x, int y) => new Label { Text = t, Location = new System.Drawing.Point(x, y + 3), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold) };
        private TextBox MakeTextBox(int x, int y, int w) => new TextBox { Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(w, 24) };
        private Button MakeButton(string t, int x, int y, int w, int h, System.Drawing.Color c)
        {
            var b = new Button { Text = t, Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(w, h), BackColor = c, ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat, Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }
    }
}
