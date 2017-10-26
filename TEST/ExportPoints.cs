using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsSystem;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
//using Autodesk.Civil.Land.DatabaseServices;


namespace Ovidiu. x64.Civil3dUtil
{
    public partial class ExportPoints : Form
    {
        System.Collections.Hashtable alinIdTable = new System.Collections.Hashtable();
        Ovidiu.StringUtil.String3D listaPuncte = new StringUtil.String3D();
        Ovidiu.StringUtil.String3D listaScurta = new StringUtil.String3D();
        string[] Filtre;

        public ExportPoints()
        {
            InitializeComponent();
        }

        //Functie de populare a tabelului de date
        private void fillTable(Ovidiu.StringUtil.String3D lista)
        {
            //Sortare lista de puncte dupa numarul punctului
            lista.Sort((StringUtil.Punct3D p1, StringUtil.Punct3D p2) => p1.Nr.CompareTo(p2.Nr));
            
            System.Data.DataTable tabelDate = new System.Data.DataTable("tabelDate");
            tabelDate.Columns.Add("Include",typeof(bool));
            string[] numeColoane = new string[] {"Point Nr.", "Chainage", "Offset", "Easting", "Northing", "Elevation", "Description"};
            //tabelDate.Columns.Add("Point Nr.");
            //tabelDate.Columns.Add("Chainage");
            //tabelDate.Columns.Add("Offset");
            //tabelDate.Columns.Add("Easting");
            //tabelDate.Columns.Add("Northing");
            //tabelDate.Columns.Add("Elevation");
            //tabelDate.Columns.Add("Description");
            foreach (string numeColoana in numeColoane)
            {
                tabelDate.Columns.Add(numeColoana);
                tabelDate.Columns[numeColoana].ReadOnly = true;
            }

            //DataRow rand = tabelDate.NewRow();
            foreach (Ovidiu.StringUtil.Punct3D punct in lista)
            {
                DataRow rand = tabelDate.NewRow();
                rand["Include"] = true;
                rand["Point Nr."] = punct.Nr;
                rand["Chainage"] = punct.KM;
                rand["Offset"] = punct.Offset;
                rand["Easting"] = punct.X;
                rand["Northing"] = punct.Y;
                rand["Elevation"] = punct.Z;
                rand["Description"] = punct.D;

                tabelDate.Rows.Add(rand);
            }

            this.dataGridView1.DataSource = tabelDate;
            //dataGridView1.Update();
        }

        private void ExportPoints_Load(object sender, EventArgs e)
        {
            Autodesk.AutoCAD.ApplicationServices.Document AcadDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            CivilDocument CivilDoc = CivilApplication.ActiveDocument;
            Database db = AcadDoc.Database;
            Editor ed = AcadDoc.Editor;

            ObjectIdCollection alignmentIds = CivilDoc.GetAlignmentIds(); //lista ID aliniamente
            ObjectIdCollection pointIds = CivilDoc.GetAllPointIds(); //lista ID puncte
          
            using (Transaction trans = AcadDoc.TransactionManager.StartTransaction())
            {
                
                //Popularea listei de aliniamente
                double length = 0;
                string LongestAlignName = string.Empty;
                foreach (ObjectId alinId in alignmentIds)
                {
                    Alignment alin = (Alignment)trans.GetObject(alinId, OpenMode.ForRead);
                    this.cmbBoxAlignment.Items.Add(alin.Name);
                    alinIdTable.Add(alin.Name, alinId);
                    if (alin.Length > length)
                    {
                        LongestAlignName = alin.Name;
                        length = alin.Length;
                    }
                }
                this.cmbBoxAlignment.Text = LongestAlignName;

                //Obtinerea listei cu toate punctele din desen
                foreach (ObjectId pointId in pointIds)
                {
                    CogoPoint point = (CogoPoint)trans.GetObject(pointId, OpenMode.ForRead);
                    Ovidiu.StringUtil.Punct3D punct = new StringUtil.Punct3D();
                    Alignment alin = (Alignment)trans.GetObject((ObjectId)alinIdTable[cmbBoxAlignment.Text], OpenMode.ForRead);

                    punct.Nr = (int)point.PointNumber;
                    punct.X = point.Location.X;
                    punct.Y = point.Location.Y;
                    punct.Z = point.Location.Z;
                    punct.D = point.RawDescription;
                    double kmPunct = 0;
                    double offsetPunct = 0;
                    try
                    {
                        alin.StationOffset(punct.X, punct.Y, ref kmPunct, ref offsetPunct);
                    }
                    catch
                    {
                        kmPunct = -999;
                        offsetPunct = -999;
                    }
                    punct.KM = kmPunct;
                    punct.Offset = offsetPunct;

                    listaPuncte.Add(punct);
                }
                //listaScurta = listaPuncte;

                //Popularea tabelului de previzualizare
                fillTable(listaPuncte);

                ed.WriteMessage(listaPuncte.Count.ToString());
                trans.Commit();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog Browse = new OpenFileDialog();
            Browse.Filter = "csv files (*.csv)|**.csv";
            Browse.Multiselect = false;
            Browse.CheckFileExists = false;
            Browse.CheckPathExists = true;
            DialogResult BrowseRez = Browse.ShowDialog();
            if (BrowseRez == DialogResult.OK)
            {
                txtBoxFile.Text = Browse.FileName;
            }
        }

        private void txtBoxFilter_TextChanged(object sender, EventArgs e)
        {
            if (txtBoxFilter.Text == "") return;
            //Citire filtru descriere puncte
            Filtre = txtBoxFilter.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //Resetare lista scurta si completare cu punctele care indeplinesc criteriile
            listaScurta.Clear();
            for (int i = 0; i < listaPuncte.Count; i++)
            {
                if (Filtre.Contains(listaPuncte[i].D))
                {
                    listaScurta.Add(listaPuncte[i]);
                }
            }
            //Actualizare tabel de previzualizare
            fillTable(listaScurta);
        }

        private void txtBoxFilter_Validated(object sender, EventArgs e)
        {
            //Daca se sterge lista de filtre se recompleteaza tabelul de previzualizare cu lista completa
            if (txtBoxFilter.Text == "")
            {
                listaScurta = new StringUtil.String3D();
                fillTable(listaPuncte);
            }
        }

        private void btnSelAlign_Click(object sender, EventArgs e)
        {
            PromptSelectionOptions prSelOpt = new PromptSelectionOptions();
            prSelOpt.MessageForAdding = "\nSelect Alignment: ";
            prSelOpt.SingleOnly = true;

            SelectionFilter filtru = new SelectionFilter(new TypedValue[1] {new TypedValue((int)DxfCode.Start, "AECC_ALIGNMENT")});

            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult prSelRes = ed.GetSelection(prSelOpt, filtru);
            if (prSelRes.Status == PromptStatus.OK)
            {
                using (Transaction trans = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction())
                {
                    Alignment alin = (Alignment)trans.GetObject(prSelRes.Value[0].ObjectId, OpenMode.ForRead);
                    cmbBoxAlignment.Text = alin.Name;
                    trans.Commit();
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //Exportarea listei finale de puncte
            Ovidiu.StringUtil.String3D lista = new StringUtil.String3D();
            try
            {
                foreach (DataGridViewRow rand in dataGridView1.Rows)
                {
                    if ((bool)rand.Cells["Include"].Value)
                    {
                        Ovidiu.StringUtil.Punct3D punct = new StringUtil.Punct3D();
                        if (rand.Cells["Point Nr."].Value != null) punct.Nr = Convert.ToInt32(rand.Cells["Point Nr."].Value);
                        if (rand.Cells["Chainage"].Value != null) punct.KM = Math.Round(Convert.ToDouble(rand.Cells["Chainage"].Value), 3);
                        if (rand.Cells["Offset"].Value != null) punct.Offset = Convert.ToDouble(rand.Cells["Offset"].Value);
                        if (rand.Cells["Easting"].Value != null) punct.X = Convert.ToDouble(rand.Cells["Easting"].Value);
                        if (rand.Cells["Northing"].Value != null) punct.Y = Convert.ToDouble(rand.Cells["Northing"].Value);
                        if (rand.Cells["Elevation"].Value != null) punct.Z = Convert.ToDouble(rand.Cells["Elevation"].Value);
                        if (rand.Cells["Description"].Value != null) punct.D = rand.Cells["Description"].Value.ToString();
                        lista.Add(punct);
                    }
                }

                //Confirmare suplimentara de suprascriere a fisierului indicat (special pentru fisierul implicit)
                //if (System.IO.File.Exists(txtBoxFile.Text) &&
                //    System.IO.File.
                //    MessageBox.Show("File exists, overwrite?", "Overwrite confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                //    == System.Windows.Forms.DialogResult.No)
                //{
                //    return;
                //}
                if (lista.ExportPoints(txtBoxFile.Text, StringUtil.Punct3D.Format.NrKmOENZD, StringUtil.Punct3D.DelimitedBy.Comma, 3, false) == false)
                {
                    MessageBox.Show("File already open in another application! File export unsuccessful.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.Close();
            }
            catch(SystemException error)
            {
                MessageBox.Show("Unsuccessful point export!\n" + error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Actualizarea valorilor KM si Offset la schimbarea axului selectat
        private void cmbBoxAlignment_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (Transaction trans = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction())
            {
                Alignment alin = (Alignment)trans.GetObject((ObjectId)alinIdTable[cmbBoxAlignment.Text], OpenMode.ForRead);
                foreach (Ovidiu.StringUtil.Punct3D punct in listaPuncte)
                {
                    double kmPunct = 0;
                    double offsetPunct = 0;
                    try
                    {
                        alin.StationOffset(punct.X, punct.Y, ref kmPunct, ref offsetPunct);
                    }
                    catch
                    {
                        kmPunct = -999;
                        offsetPunct = -999;
                    }
                    punct.KM = kmPunct;
                    punct.Offset = offsetPunct;
                }
                foreach (Ovidiu.StringUtil.Punct3D punct in listaScurta)
                {
                    double kmPunct = 0;
                    double offsetPunct = 0;
                    try
                    {
                        alin.StationOffset(punct.X, punct.Y, ref kmPunct, ref offsetPunct);
                    }
                    catch
                    {
                        kmPunct = -999;
                        offsetPunct = -999;
                    }
                    punct.KM = kmPunct;
                    punct.Offset = offsetPunct;
                }
                if (listaScurta.Count > 0) fillTable(listaScurta);
                else fillTable(listaPuncte);

                trans.Commit();
            }
        }



        
    }
}
