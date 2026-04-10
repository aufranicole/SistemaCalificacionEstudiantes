using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using SistemaCalificacionEstudiantes.Data;
using SistemaCalificacionEstudiantes.Helpers;
using SistemaCalificacionEstudiantes.Models;

namespace SistemaCalificacionEstudiantes.Forms
{
    public class FormCalificaciones : Form
    {
        private DataGridView dgv;
        private TextBox txtID, txtEstudianteID, txtMateriaID;
        private NumericUpDown nudCal1, nudCal2, nudCal3, nudCal4, nudExamen;
        private Label lblTotal, lblClasificacion, lblEstado;
        private Button btnAgregar, btnActualizar, btnEliminar, btnLimpiar;
        private Button btnExportCSV, btnExportPDF;

        public FormCalificaciones()
        {
            InitializeUI();
            CargarDatos();
        }

        private void InitializeUI()
        {
            this.Text = "Gestión de Calificaciones";
            this.Size = new System.Drawing.Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 252);
            this.Font = new System.Drawing.Font("Segoe UI", 9f);

            var lblTitle = new Label
            {
                Text = "CALIFICACIONES",
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(30, 60, 120),
                Location = new System.Drawing.Point(20, 15), AutoSize = true
            };

            // panel de entrada de datos
            var pnl = new Panel
            {
                Location = new System.Drawing.Point(20, 55),
                Size = new System.Drawing.Size(1040, 190),
                BackColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // primera fila: los IDs
            int y1 = 15;
            pnl.Controls.Add(MkLbl("ID Calificación:", 10, y1));
            txtID = MkTxt(130, y1, 80); pnl.Controls.Add(txtID);
            pnl.Controls.Add(MkLbl("EstudianteID:", 240, y1));
            txtEstudianteID = MkTxt(350, y1, 80); pnl.Controls.Add(txtEstudianteID);
            pnl.Controls.Add(MkLbl("MateriaID:", 460, y1));
            txtMateriaID = MkTxt(550, y1, 80); pnl.Controls.Add(txtMateriaID);

            // segunda fila: las notas
            int y2 = 55;
            string[] cals = { "Cal. 1:", "Cal. 2:", "Cal. 3:", "Cal. 4:", "Examen:" };
            nudCal1   = MkNud(10 + 70, y2);
            nudCal2   = MkNud(10 + 70 + 180, y2);
            nudCal3   = MkNud(10 + 70 + 360, y2);
            nudCal4   = MkNud(10 + 70 + 540, y2);
            nudExamen = MkNud(10 + 70 + 720, y2);
            int[] nxArr = { 10, 10 + 180, 10 + 360, 10 + 540, 10 + 720 };
            NumericUpDown[] nuds = { nudCal1, nudCal2, nudCal3, nudCal4, nudExamen };
            for (int i = 0; i < 5; i++)
            {
                pnl.Controls.Add(MkLbl(cals[i], nxArr[i], y2 + 3));
                pnl.Controls.Add(nuds[i]);
                nuds[i].ValueChanged += RecalcularPreview;
            }

            // tercera fila: muestra el calculo en vivo
            int y3 = 100;
            pnl.Controls.Add(MkLbl("Total (calc):", 10, y3));
            lblTotal = new Label { Location = new System.Drawing.Point(105, y3), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.FromArgb(30, 60, 120) };
            pnl.Controls.Add(lblTotal);

            pnl.Controls.Add(MkLbl("Clasificación:", 250, y3));
            lblClasificacion = new Label { Location = new System.Drawing.Point(355, y3), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) };
            pnl.Controls.Add(lblClasificacion);

            pnl.Controls.Add(MkLbl("Estado:", 470, y3));
            lblEstado = new Label { Location = new System.Drawing.Point(535, y3), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) };
            pnl.Controls.Add(lblEstado);

            var lblInfo = new Label
            {
                Text = "ℹ Total = 70% promedio(Cal1..Cal4) + 30% Examen   |   A≥90  B≥80  C≥70  F<70   |   Aprobado si Total ≥ 70",
                Location = new System.Drawing.Point(10, 140),
                AutoSize = true,
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font("Segoe UI", 8)
            };
            pnl.Controls.Add(lblInfo);

            // botones
            int bx = 20, by = 260, bw = 125, bh = 35, gap = 8;
            btnAgregar    = MkBtn("➕ Agregar",    bx,                    by, bw,  bh, System.Drawing.Color.FromArgb(34, 139, 34));
            btnActualizar = MkBtn("✏️ Actualizar", bx + (bw + gap),       by, bw,  bh, System.Drawing.Color.FromArgb(30, 100, 180));
            btnEliminar   = MkBtn("🗑️ Eliminar",   bx + (bw + gap) * 2,   by, bw,  bh, System.Drawing.Color.FromArgb(180, 40, 40));
            btnLimpiar    = MkBtn("🔄 Limpiar",    bx + (bw + gap) * 3,   by, bw,  bh, System.Drawing.Color.FromArgb(120, 120, 120));
            btnExportCSV  = MkBtn("📄 Export CSV", bx + (bw + gap) * 4 + 20,  by, 140, bh, System.Drawing.Color.FromArgb(30, 60, 120));
            btnExportPDF  = MkBtn("📕 Export PDF", bx + (bw + gap) * 4 + 170, by, 140, bh, System.Drawing.Color.FromArgb(180, 60, 0));

            btnAgregar.Click    += BtnAgregar_Click;
            btnActualizar.Click += BtnActualizar_Click;
            btnEliminar.Click   += BtnEliminar_Click;
            btnLimpiar.Click    += (s, e) => LimpiarCampos();
            btnExportCSV.Click  += (s, e) =>
            {
                try { using (var ctx = new AppDbContext()) ExportHelper.ExportarCalificacionesCSV(ctx.Calificaciones.Include(c => c.Estudiante).Include(c => c.Materia).ToList()); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };
            btnExportPDF.Click += (s, e) =>
            {
                try { using (var ctx = new AppDbContext()) ExportHelper.ExportarCalificacionesPDF(ctx.Calificaciones.Include(c => c.Estudiante).Include(c => c.Materia).ToList()); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            };

            // la tabla de datos
            dgv = new DataGridView
            {
                Location = new System.Drawing.Point(20, 310),
                Size = new System.Drawing.Size(1040, 310),
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = System.Drawing.Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.FromArgb(30, 60, 120),
                    ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
                }
            };
            dgv.CellClick += Dgv_CellClick;

            this.Controls.AddRange(new Control[] { lblTitle, pnl, btnAgregar, btnActualizar, btnEliminar, btnLimpiar, btnExportCSV, btnExportPDF, dgv });
            RecalcularPreview(null, null);
        }

        private void RecalcularPreview(object sender, EventArgs e)
        {
            decimal prom  = (nudCal1.Value + nudCal2.Value + nudCal3.Value + nudCal4.Value) / 4m;
            decimal total = (prom * 0.70m) + (nudExamen.Value * 0.30m);
            string clasif = total >= 90 ? "A" : total >= 80 ? "B" : total >= 70 ? "C" : "F";
            string estado = total >= 70 ? "Aprobado" : "Reprobado";

            lblTotal.Text = total.ToString("0.00");
            lblClasificacion.Text = clasif;
            lblClasificacion.ForeColor = clasif == "A" ? System.Drawing.Color.Green :
                                          clasif == "B" ? System.Drawing.Color.DarkBlue :
                                          clasif == "C" ? System.Drawing.Color.DarkOrange :
                                          System.Drawing.Color.Red;
            lblEstado.Text = estado;
            lblEstado.ForeColor = estado == "Aprobado" ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }

        private void CargarDatos()
        {
            try
            {
                using (var ctx = new AppDbContext())
                {
                    var lista = ctx.Calificaciones
                        .Include(c => c.Estudiante)
                        .Include(c => c.Materia)
                        .ToList()
                        .Select(c => new
                        {
                            c.CalificacionID,
                            Estudiante = c.Estudiante != null ? $"{c.Estudiante.Nombre} {c.Estudiante.Apellido}" : c.EstudianteID.ToString(),
                            Materia    = c.Materia != null ? c.Materia.Nombre : c.MateriaID.ToString(),
                            c.Calificacion1, c.Calificacion2, c.Calificacion3, c.Calificacion4,
                            c.Examen, c.TotalCalificacionDB, c.ClasificacionDB, c.EstadoDB
                        }).ToList();
                    dgv.DataSource = lista;
                }
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
                    int id    = int.Parse(txtID.Text.Trim());
                    int estId = int.Parse(txtEstudianteID.Text.Trim());
                    int matId = int.Parse(txtMateriaID.Text.Trim());

                    if (ctx.Calificaciones.Any(c => c.CalificacionID == id))
                    { MessageBox.Show("Ya existe una calificación con ese ID.", "ID Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (!ctx.Estudiantes.Any(x => x.EstudianteID == estId))
                    { MessageBox.Show("El EstudianteID no existe en la base de datos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (!ctx.Materias.Any(x => x.MateriaID == matId))
                    { MessageBox.Show("El MateriaID no existe en la base de datos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    var cal = new Calificacion
                    {
                        CalificacionID = id, EstudianteID = estId, MateriaID = matId,
                        Calificacion1 = nudCal1.Value, Calificacion2 = nudCal2.Value,
                        Calificacion3 = nudCal3.Value, Calificacion4 = nudCal4.Value,
                        Examen = nudExamen.Value
                    };
                    cal.ActualizarCalculados();
                    ctx.Calificaciones.Add(cal);
                    ctx.SaveChanges();
                }
                MessageBox.Show("Calificación agregada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    int id    = int.Parse(txtID.Text.Trim());
                    int estId = int.Parse(txtEstudianteID.Text.Trim());
                    int matId = int.Parse(txtMateriaID.Text.Trim());

                    var cal = ctx.Calificaciones.Find(id);
                    if (cal == null) { MessageBox.Show("No existe esa calificación.", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (!ctx.Estudiantes.Any(x => x.EstudianteID == estId)) { MessageBox.Show("El EstudianteID no existe.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (!ctx.Materias.Any(x => x.MateriaID == matId)) { MessageBox.Show("El MateriaID no existe.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                    cal.EstudianteID = estId; cal.MateriaID = matId;
                    cal.Calificacion1 = nudCal1.Value; cal.Calificacion2 = nudCal2.Value;
                    cal.Calificacion3 = nudCal3.Value; cal.Calificacion4 = nudCal4.Value;
                    cal.Examen = nudExamen.Value;
                    cal.ActualizarCalculados();
                    ctx.Entry(cal).State = EntityState.Modified;
                    ctx.SaveChanges();
                }
                MessageBox.Show("Calificación actualizada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos(); CargarDatos();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtID.Text) || !int.TryParse(txtID.Text, out int id))
            { MessageBox.Show("Ingrese un ID válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (MessageBox.Show("¿Eliminar esta calificación?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                using (var ctx = new AppDbContext())
                {
                    var cal = ctx.Calificaciones.Find(id);
                    if (cal == null) { MessageBox.Show("No existe esa calificación.", "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    ctx.Calificaciones.Remove(cal);
                    ctx.SaveChanges();
                }
                MessageBox.Show("Calificación eliminada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos(); CargarDatos();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            txtID.Text = row.Cells["CalificacionID"].Value?.ToString();

            // busco el registro completo para llenar los campos
            if (int.TryParse(txtID.Text, out int id))
            {
                try
                {
                    using (var ctx = new AppDbContext())
                    {
                        var cal = ctx.Calificaciones.Find(id);
                        if (cal != null)
                        {
                            txtEstudianteID.Text = cal.EstudianteID.ToString();
                            txtMateriaID.Text    = cal.MateriaID.ToString();
                            nudCal1.Value   = cal.Calificacion1;
                            nudCal2.Value   = cal.Calificacion2;
                            nudCal3.Value   = cal.Calificacion3;
                            nudCal4.Value   = cal.Calificacion4;
                            nudExamen.Value = cal.Examen;
                        }
                    }
                }
                catch { }
            }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtID.Text) || !int.TryParse(txtID.Text.Trim(), out int id) || id <= 0)
            { MessageBox.Show("El ID debe ser un número entero positivo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            if (string.IsNullOrWhiteSpace(txtEstudianteID.Text) || !int.TryParse(txtEstudianteID.Text.Trim(), out _))
            { MessageBox.Show("EstudianteID debe ser un número entero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            if (string.IsNullOrWhiteSpace(txtMateriaID.Text) || !int.TryParse(txtMateriaID.Text.Trim(), out _))
            { MessageBox.Show("MateriaID debe ser un número entero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            return true;
        }

        private void LimpiarCampos()
        {
            txtID.Text = txtEstudianteID.Text = txtMateriaID.Text = "";
            nudCal1.Value = nudCal2.Value = nudCal3.Value = nudCal4.Value = nudExamen.Value = 0;
        }

        // metodos para no repetir codigo al crear controles
        private Label MkLbl(string t, int x, int y) => new Label { Text = t, Location = new System.Drawing.Point(x, y + 3), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold) };
        private TextBox MkTxt(int x, int y, int w) => new TextBox { Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(w, 24) };
        private NumericUpDown MkNud(int x, int y)
        {
            var n = new NumericUpDown { Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(80, 24), Minimum = 0, Maximum = 100, DecimalPlaces = 2 };
            return n;
        }
        private Button MkBtn(string t, int x, int y, int w, int h, System.Drawing.Color c)
        {
            var b = new Button { Text = t, Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(w, h), BackColor = c, ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat, Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }
    }
}
