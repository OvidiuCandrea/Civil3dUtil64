using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CivilDBS = Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Runtime;
using Autodesk.Civil.Settings;
using Ovidiu.StringUtil;

namespace Ovidiu.x64.Civil3dUtil
{
    public class Class1 : IExtensionApplication
    {
        #region IExtensionApplication Members
        public void Initialize()
        {
            //throw new NotImplementedException();
        }

        public void Terminate()
        {
            //throw new NotImplementedException();
        }
        #endregion

        [CommandMethod("test")]
        public void test()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("test succesful");
        }

        [CommandMethod("zs")] //Comanda care centreaza imaginea la km solicitat al unui ax
        public void ZoomStation()
        {
            CivilDocument doc = CivilApplication.ActiveDocument;
            ObjectIdCollection alignmentIds = doc.GetAlignmentIds(); //lista ID aliniamente
            //ObjectIdCollection sites = doc.GetSiteIds();

            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;

            ///Alegere ax principal///
            ///Citire fisier de optiuni
            //Cauta fisierul de configurare in calea implicita sau creaza unul
            if (alignmentIds.Count == 0)
            {
                ed.WriteMessage("\nNo alignments found! ");
                return;
            }
            string cale;
            cale = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Optiuni.cfg";
            cale = Path.GetFullPath(cale);
            if (File.Exists(cale) == false)
            {
                try
                {
                    using (StreamWriter scriitor = new StreamWriter(cale))
                    {
                        scriitor.Write("\r\n");
                    }
                }
                catch
                {
                    ed.WriteMessage("\nUnsuccessful selection of Configuration File");
                }
            }
            Ovidiu.x64.General.Configurator config = new x64.General.Configurator(cale);
            string[] optiuni = config.citireConf("@ZoomStation");


            //Cauta prezenta optiunii cu axul implicit si inregistrarea lui
            string axImplicit = "";
            foreach (string optiune in optiuni)
            {
                if (optiune.StartsWith("Ax Implicit")) axImplicit = optiune.Substring(optiune.IndexOf('=') + 1);
            }


            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                ///Selectarea axului
                ObjectId idAx = new ObjectId();
                double lungAx = 0;
                foreach (ObjectId alignmentId in alignmentIds)
                {
                    Alignment alin = (Alignment)trans.GetObject(alignmentId, OpenMode.ForRead);
                    if (alin.Name == axImplicit)
                    {
                        idAx = alignmentId;
                        break;
                    }
                    if (alin.Length > lungAx)
                    {
                        lungAx = alin.Length;
                        idAx = alignmentId;
                    }
                }
                Alignment Ax = (Alignment)trans.GetObject(idAx, OpenMode.ForRead);


                ///Solicitarea pozitiei kilometrice de-a lungul axului
                PromptKeywordOptions prOpt = new PromptKeywordOptions("\nInput Station on alignment " + Ax.Name + ", or \n");
                prOpt.Keywords.Add("List");
                prOpt.Keywords.Add("Select");
                prOpt.Keywords.Add("Name");
                prOpt.Keywords.Add("nUmber");
                prOpt.Keywords.Add("Height");
                prOpt.Keywords.Default = "Select";
                prOpt.AppendKeywordsToMessage = true;
                prOpt.AllowArbitraryInput = true;
                prOpt.AllowNone = true;
                PromptResult prRes = ed.GetKeywords(prOpt);

                if (prRes.Status == PromptStatus.OK)
                {
                    string strRes = "";

                    switch (prRes.StringResult)
                    {
                        case "List":
                            for (int i = 1; i <= alignmentIds.Count; i++)
                            {
                                Alignment alin = (Alignment)trans.GetObject(alignmentIds[i - 1], OpenMode.ForRead);
                                ed.WriteMessage("\n" + i + " - " + alin.Name);
                            }
                            return;

                        case "Select":
                        case "":
                            //optiunile si filtrele selectiei noului ax
                            PromptSelectionOptions prSelOpt = new PromptSelectionOptions();
                            prSelOpt.MessageForAdding = "\nSelect Alignment: ";
                            prSelOpt.SingleOnly = true;

                            TypedValue[] tvs = { new TypedValue((int)DxfCode.Start, "AECC_ALIGNMENT") };
                            SelectionFilter sf = new SelectionFilter(tvs);

                            //Realizarea selectiei
                            PromptSelectionResult prSelRes = ed.GetSelection(prSelOpt, sf);
                            if (prSelRes.Status == PromptStatus.OK && prSelRes.Value.Count == 1)
                            {
                                idAx = prSelRes.Value[0].ObjectId;
                                Ax = (Alignment)trans.GetObject(idAx, OpenMode.ForRead);
                                strRes = ed.GetString("\nSpecify station to zoom to: ").StringResult;
                            }
                            else
                            {
                                ed.WriteMessage("\nNo Alignment Selected");
                                return;
                            }
                            break;

                        case "Name":
                            PromptStringOptions prStrOpt = new PromptStringOptions("\nAlignment name: ");
                            prStrOpt.AllowSpaces = true;
                            string nume = ed.GetString(prStrOpt).StringResult;
                            bool gasit = false;
                            foreach (ObjectId alignmentId in alignmentIds)
                            {
                                Alignment alin = (Alignment)trans.GetObject(alignmentId, OpenMode.ForRead);
                                if (alin.Name == nume)
                                {
                                    idAx = alignmentId;
                                    Ax = (Alignment)trans.GetObject(idAx, OpenMode.ForRead);
                                    gasit = true;
                                }
                            }
                            if (gasit == false)
                            {
                                ed.WriteMessage("\nAlignment not found! ");
                                return;
                            }
                            strRes = ed.GetString("\nSpecify station to zoom to: ").StringResult;
                            break;

                        case "nUmber":
                            int numar = ed.GetInteger("\nEnter a number from 1 to " + alignmentIds.Count + ": ").Value;
                            if (numar > 0 && numar <= alignmentIds.Count)
                            {
                                idAx = alignmentIds[numar - 1];
                                Ax = (Alignment)trans.GetObject(idAx, OpenMode.ForRead);
                            }
                            else
                            {
                                ed.WriteMessage("\nInteger out of range! ");
                                return;
                            }
                            strRes = ed.GetString("\nSpecify station to zoom to: ").StringResult;
                            break;

                        case "Height":
                            string[] scaraNoua = new string[1];
                            scaraNoua[0] = "Scara=" + ed.GetString("\nSet the new zoom height as integer or use \"pan\" to use current! ").StringResult;
                            config.scriereConf("@ZoomStation", scaraNoua);
                            return;

                        default:
                            strRes = prRes.StringResult;
                            break;
                    }

                    try
                    {
                        //Citire pozitie kilometrica
                        double km;
                        if (strRes.Contains('+'))
                        {
                            foreach (string optiune in optiuni)
                            {
                                if (strRes.EndsWith("+") && optiune.StartsWith("Format") && optiune.Contains('+'))
                                {
                                    strRes = strRes + optiune.Substring(optiune.LastIndexOf('+') + 1);
                                }
                            }
                            km = double.Parse(strRes.Replace("+", ""));
                        }
                        else km = double.Parse(strRes);

                        //Mutarea imaginii pe noile coordonate
                        if (km >= Ax.StartingStation && km <= Ax.EndingStation)
                        {
                            //Citirea optiunii de scara
                            double scara = 500;
                            bool pan = false;
                            foreach (string optiune in optiuni)
                            {
                                if (optiune.StartsWith("Scara") && optiune.Substring(optiune.LastIndexOf("=") + 1) != "pan")
                                {
                                    try
                                    {
                                        scara = double.Parse(optiune.Substring(optiune.IndexOf('=') + 1));
                                        if (scara <= 0) throw new System.Exception();
                                        else ed.WriteMessage("\nZoom height set to " + scara.ToString());
                                    }
                                    catch { ed.WriteMessage("\nDefault zoom height of 500 is used! "); }
                                }
                                else if (optiune.StartsWith("Scara") && optiune.Substring(optiune.LastIndexOf("=") + 1) == "pan")
                                {
                                    pan = true;
                                }
                            }

                            //Calcularea destinatiei si executia
                            double est = 0;
                            double nord = 0;
                            Ax.PointLocation(km, 0, ref est, ref nord);

                            string cmd;
                            //Metoda Pan
                            if (pan)
                            {
                                ViewTableRecord view = ed.GetCurrentView();
                                ed.WriteMessage("Current zoom height of " + view.Height + " is used! ");
                                scara = view.Height;
                            }

                            cmd = "_.ZOOM _C " + est.ToString() + "," + nord.ToString() + " " + scara.ToString() + " ";
                            ed.Document.SendStringToExecute(cmd, true, false, false);
                        }
                        else
                        {
                            ed.WriteMessage("\nStation out of range! ");
                        }
                    }

                    catch
                    {
                        ed.WriteMessage("\nStation could not be read or incorrect station! ");
                    }


                }



            }
        }

        [CommandMethod("ep")] //Comanda care exporta punctele din desenul curent in format PSOENZD csv
        public void ExportPoints()
        {
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            if (civDoc.GetAllPointIds().Count == 0)
            {
                ed.WriteMessage("\nNo Cogo Points found!");
                return;
            }
            if (civDoc.GetAlignmentIds().Count == 0)
            {
                ed.WriteMessage("\nNo Alignments Points found!");
                return;
            }
            Ovidiu.x64.Civil3dUtil.ExportPoints Formular = new ExportPoints();
            Application.ShowModalDialog(Formular);
            ed.WriteMessage(Formular.cmbBoxAlignment.SelectedItem.ToString());
        }

        [CommandMethod("expsurfpts")] //Comanda pentru exportarea intr-un fisier PENZD.csv a punctelor unei suprafete
        public void ExpSurfPts()
        {
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            Document acadDoc = Application.DocumentManager.MdiActiveDocument;
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = acadDoc.Editor;
            ObjectIdCollection surfIds = civDoc.GetSurfaceIds();
            //Verificarea existentei a cel putin unei suprafete
            if (surfIds.Count == 0)
            {
                ed.WriteMessage("\nNo Surfaces found!");
                return;
            }

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //Selectarea suprafetei
                Autodesk.Civil.DatabaseServices.Surface suprafata;
                if (surfIds.Count == 1) suprafata = (Autodesk.Civil.DatabaseServices.Surface)trans.GetObject(surfIds[0], OpenMode.ForRead);
                else
                {
                    PromptEntityOptions PrEntOpt = new PromptEntityOptions("\nSelect Surface: ");
                    PrEntOpt.AllowNone = false;
                    PrEntOpt.SetRejectMessage("\nSelected object is not a Civil3D surface!");
                    PrEntOpt.AddAllowedClass(typeof(Autodesk.Civil.DatabaseServices.Surface), false);
                    PromptEntityResult PrEntRes = ed.GetEntity(PrEntOpt);
                    if (PrEntRes.Status != PromptStatus.OK)
                    {
                        ed.WriteMessage("\nAborting!");
                        return;
                    }
                    suprafata = (Autodesk.Civil.DatabaseServices.Surface)trans.GetObject(PrEntRes.ObjectId, OpenMode.ForRead);
                }

                //Obtinerea punctelor suprafetei
                List<string> puncte = new List<string>();
                int nr = 0;
                if (suprafata is Autodesk.Civil.DatabaseServices.TinSurface)
                {
                    Autodesk.Civil.DatabaseServices.TinSurface suprafTIN = (Autodesk.Civil.DatabaseServices.TinSurface)suprafata;
                    //Autodesk.Civil.DatabaseServices.TinSurfaceVertexCollection vertecsiTIN = suprafTIN.Vertices;
                    foreach(Autodesk.Civil.DatabaseServices.TinSurfaceVertex V in suprafTIN.Vertices)
                    {
                        nr = nr + 1;
                        puncte.Add(nr.ToString() + "," + V.Location.ToString().Replace("(", "").Replace(")", "") + "," + suprafata.Name);
                    }
                }
                else if (suprafata is Autodesk.Civil.DatabaseServices.GridSurface)
                {
                    Autodesk.Civil.DatabaseServices.GridSurface suprafGRID = (Autodesk.Civil.DatabaseServices.GridSurface)suprafata;
                    foreach(Autodesk.Civil.DatabaseServices.GridSurfaceVertex V in suprafGRID.Vertices)
                    {
                        nr = nr + 1;
                        puncte.Add(nr.ToString() + "," + V.Location.ToString().Replace("(", "").Replace(")", "") + "," + suprafata.Name);
                    }
                }
                else
                {
                    ed.WriteMessage("\nSurface type not supported! Aborting.");
                    return;
                }

                ////TEST: se listeaza punctele
                //foreach(string p in puncte) ed.WriteMessage(p);

                //Selectia fisierului .csv si scrierea punctelor
                PromptSaveFileOptions PrFileOpt = new PromptSaveFileOptions("\nSelect file for point export: ");
                PrFileOpt.Filter = ("CSV file (*.csv)|*.csv");
                string caleDocAcad = HostApplicationServices.Current.FindFile(acadDoc.Name, acadDoc.Database, FindFileHint.Default);
                //PrFileOpt.InitialDirectory = caleDocAcad.Remove(caleDocAcad.LastIndexOf("\\");
                PrFileOpt.InitialFileName = caleDocAcad.Replace(caleDocAcad.Substring(caleDocAcad.LastIndexOf('.')), ".csv");
                ed.WriteMessage(PrFileOpt.InitialDirectory);

                PromptFileNameResult PrFileRes = ed.GetFileNameForSave(PrFileOpt);
                if (PrFileRes.Status != PromptStatus.OK)
                {
                    ed.WriteMessage("\nPoint export unsuccessful!");
                    return;
                }
                StreamWriter scriitor = new StreamWriter(PrFileRes.StringResult);
                foreach (string p in puncte) scriitor.WriteLine(p);
                scriitor.Dispose();
            }

        }

        [CommandMethod("convacadpts")] //Comanda pentru convertirea punctelor autocad in puncte COGO de civil3d
        public void ConvAcadPts()
        {
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            //Selectia punctelor autocad
            PromptSelectionOptions PrSelOpt = new PromptSelectionOptions();
            PrSelOpt.MessageForAdding = "\nSelect points to add: ";
            PrSelOpt.MessageForRemoval = "\nSelect points to remove: ";
            
            TypedValue[] tvs = { new TypedValue((int)DxfCode.Start, "POINT") };
            SelectionFilter sf = new SelectionFilter(tvs);

            PromptSelectionResult PrSelRes = ed.GetSelection(PrSelOpt, sf);
            if (PrSelRes.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nInvalid Selection! Aborting.");
                return;
            }

            //Solicitarea descrierii punctelor COGO
            string descriere = ed.GetString("\nSpecify points description: ").StringResult;

            //Creearea puntelor COGO
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                CogoPointCollection cogoPoints = civDoc.CogoPoints;
                foreach (SelectedObject selObj in PrSelRes.Value)
                {
                    Autodesk.AutoCAD.DatabaseServices.DBPoint punctAcad = (Autodesk.AutoCAD.DatabaseServices.DBPoint)trans.GetObject(selObj.ObjectId, OpenMode.ForRead);
                    ObjectId pointId = cogoPoints.Add(punctAcad.Position);
                    CogoPoint cogo = pointId.GetObject(OpenMode.ForWrite) as CogoPoint;
                    cogo.RawDescription = descriere;
                }
                trans.Commit();
            }
        }

        [CommandMethod("gsi")] //Comanda revizuita care importa puncte din fisiere gsi
        public void ImportGSI()
        {
            //Selectia fisierelor gsi
            Document acadDoc = Application.DocumentManager.MdiActiveDocument;
            CivilDocument civilDoc = CivilApplication.ActiveDocument;
            Editor ed = acadDoc.Editor;
            Database db = HostApplicationServices.WorkingDatabase;

            PromptOpenFileOptions POFO = new PromptOpenFileOptions("Select gsi file: ");
            POFO.Filter = ".gsi";

            PromptFileNameResult FileRes = ed.GetFileNameForOpen(POFO);
            if (FileRes.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nFile selection failed! Aborting.");
                return;
            }
            string cale = FileRes.StringResult;

            PromptResult PSR = ed.GetString("\nSpecify points description");
            if (PSR.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nInvalid description! Aborting.");
                return;
            }
            string descriere = PSR.StringResult;

            String3D listaPuncte = new String3D();
            listaPuncte.ImportGSI(cale);

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                CogoPointCollection cogoPoints = civilDoc.CogoPoints;
                foreach (Punct3D punct in listaPuncte)
                {
                    ObjectId pointId = cogoPoints.Add(new Point3d(punct.X , punct.Y, punct.Z));
                    CogoPoint cogo = pointId.GetObject(OpenMode.ForWrite) as CogoPoint;
                    cogo.RawDescription = descriere;
                }
                trans.Commit();
            }
        }

        [CommandMethod("ES")] //Comanda care exporta un set de sectiuni in format KmOZD txt
        public void ExportSections()
        {
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            Document acadDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = HostApplicationServices.WorkingDatabase;
            string caleDocAcad = HostApplicationServices.Current.FindFile(acadDoc.Name, db, FindFileHint.Default);

            //Optiuni pentru selectia unei sectiuni
            PromptEntityOptions PrEntOpt = new PromptEntityOptions("\nSelect a section from the desired set to be exported: ");
            PrEntOpt.SetRejectMessage("\nThe selected object is not a Section object! ");
            PrEntOpt.AddAllowedClass(typeof(CivilDBS.Section), true);

            //Selectia unei sectiuni din setul ce trebuie exportat
            PromptEntityResult PrEntRes = ed.GetEntity(PrEntOpt);
            if (PrEntRes.Status == PromptStatus.OK)
            {
                ObjectId SSID = PrEntRes.ObjectId;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    //Obtinerea sectiunii din baza de date, aflarea sursei acesteia si a grupului de sectiuni din care face parte
                    CivilDBS.Section SS = (CivilDBS.Section)trans.GetObject(SSID, OpenMode.ForRead);
                    int SSstart = SS.Name.LastIndexOf("- ") + 2;
                    int SSend = SS.Name.LastIndexOf('(');
                    string SSsource = SS.Name.Substring(SSstart, SSend - SSstart);
                    ed.WriteMessage("\nSelected sections's source is: {0}", SSsource);

                    CivilDBS.SampleLine SL = (CivilDBS.SampleLine)trans.GetObject(SS.SampleLineId, OpenMode.ForRead);
                    CivilDBS.SampleLineGroup SLG = (CivilDBS.SampleLineGroup)trans.GetObject(SL.GroupId, OpenMode.ForRead);

                    //Aflarea aliniamentului de care apartine grupul de sectiuni
                    string numeAx = String.Empty;
                    ObjectIdCollection ALIDS = civDoc.GetAlignmentIds();
                    try
                    {
                        foreach (ObjectId ALID in ALIDS)
                        {
                            Alignment AL = (Alignment)trans.GetObject(ALID, OpenMode.ForRead);
                            if (AL.GetSampleLineGroupIds().Contains(SL.GroupId))
                            {
                                numeAx = AL.Name;
                            }
                        }
                    }
                    catch
                    {                        
                    }

                    string caleFisSect = caleDocAcad.Remove(caleDocAcad.LastIndexOf('.')) + " - " + numeAx + " - " + SSsource + ".txt";
                    StreamWriter scriitor = new StreamWriter(caleFisSect);

                    //Exportarea datelor
                    
                    ObjectIdCollection SLIDS = SLG.GetSampleLineIds();
                    foreach (ObjectId SLID in SLIDS)
                    {
                        CivilDBS.SampleLine currSL = (CivilDBS.SampleLine)trans.GetObject(SLID, OpenMode.ForRead);
                        ObjectIdCollection SIDS = currSL.GetSectionIds();
                        foreach (ObjectId SID in SIDS)
                        {
                            CivilDBS.Section S = (CivilDBS.Section)trans.GetObject(SID, OpenMode.ForRead);
                            int Sstart = S.Name.LastIndexOf("- ") + 2;
                            int Send = S.Name.LastIndexOf('(');
                            string Ssource = S.Name.Substring(Sstart, Send - Sstart);
                            if (SSsource == Ssource)
                            {
                                CivilDBS.SectionPointCollection Spoints = S.SectionPoints;
                                double LO = -999;
                                double RO = 999;
                                try
                                {
                                    LO = S.LeftOffset;
                                    RO = S.RightOffset;
                                }
                                catch
                                {
                                    ed.WriteMessage("\nLimits of section at chainage: {0} cound not be read! Section skipped.", S.Station);
                                    continue;
                                }
                                for (int i = 0; i < Spoints.Count; i++)
                                {
                                    //if (Spoints[i].Location.X >= S.LeftOffset && Spoints[i].Location.X <= S.RightOffset)
                                    if (Spoints[i].Location.X >= LO && Spoints[i].Location.X <= RO)
                                    {
                                        string sectiune = S.Station.ToString() + "," +
                                                                        Spoints[i].Location.X.ToString("F4") + "," +
                                                                        Spoints[i].Location.Y.ToString("F4") + "," + Ssource;
                                        scriitor.WriteLine(sectiune);
                                    }
                                }
                                //Polyline poly = (Polyline)S.BaseCurve;
                                //for (int i = 0; i < poly.NumberOfVertices; i++)
                                //{
                                //    Point3d p = poly.GetPoint3dAt(i);
                                //    string sectiune = S.Station.ToString() + "," +
                                //    p.X.ToString("F3") + "," +
                                //    p.Y.ToString("F3") + "," + Ssource;
                                //    scriitor.WriteLine(sectiune);
                                //}
                            }
                        }
                    }

                    scriitor.Dispose();
                }
            }
        }

        //Comanda pentru generarea fisierelor cu sisteme de coordonate ale sectiunilor (*.SCS)
        //din grupurile de sectiuni transversale Civil3D
        [CommandMethod("GENSCS")]
        public void genscs()
        {
            //Definitii generale
            Document acadDoc = Application.DocumentManager.MdiActiveDocument;
            CivilDocument civilDoc = CivilApplication.ActiveDocument;
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = acadDoc.Editor;

            //Verificarea existentei macar a unui singur grup de sectiuni transversale
            //SectionViewGroupCollection SVGColl = civilDoc.get
            //SelectionSet SS =  

            //Gasirea caii documentului curent
            string caleDocAcad = acadDoc.Database.Filename;
            //Daca fisierul este unul nou, ne salvat inca, calea apartine sablonului folosit pentru acesta.
            //Se verifica daca fisierul este unul sablon, se atentioneaza utilizatorul si se paraseste programul.
            if (caleDocAcad.EndsWith(".dwt") == true)
            {
                ed.WriteMessage("\nThe current drawing is a template file (*.dwt). Exiting program! ");
                return;
            }
            caleDocAcad = HostApplicationServices.Current.FindFile(acadDoc.Name, acadDoc.Database, FindFileHint.Default);


            //Selectarea grupului de sectiuni transversale
            ed.WriteMessage("\nCommand for generating section coordinate systems file (*.SCS) from section view groups");
            PromptEntityOptions PEO = new PromptEntityOptions("\nSelect a section view belonging to the desired section view group: ");
            PEO.SetRejectMessage("\nThe selected object is not a section view! Select object: ");
            PEO.AddAllowedClass(typeof(SectionView), true);

            PromptEntityResult PER = ed.GetEntity(PEO);
            if (PER.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nInvalid selection! Aborting.");
                return;
            }

            //Inceperea tranzactiei pentru interogarea sectiunilor transversale
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                SectionView SV = PER.ObjectId.GetObject(OpenMode.ForRead) as SectionView; //Sectiunea selectata
                SampleLine SL = SV.ParentEntityId.GetObject(OpenMode.ForRead) as SampleLine; //Pichetul de care apartine sectiunea

                //Gasirea grupului de sectiuni din care face parte sectiunea, folosindu-ne de numarul acesteia
                int nrSect = int.Parse(SV.Name.Substring(SV.Name.IndexOf('(') + 1).Replace(")", ""));
                ObjectIdCollection ids = SL.GetSectionViewIds();
                List<int> seturi = new List<int>();
                foreach (ObjectId id in ids)
                {
                    string SVname = ((SectionView)id.GetObject(OpenMode.ForRead)).Name;
                    seturi.Add(int.Parse(SVname.Substring(SVname.IndexOf('(') + 1).Replace(")", "")));
                }
                int set = seturi.IndexOf(nrSect);


                SampleLineGroup SLG = SL.GroupId.GetObject(OpenMode.ForRead) as SampleLineGroup;
                ObjectIdCollection OIDS = SLG.GetSampleLineIds();

                //Obtinerea sistemelor de coordonate si scrierea lor in fisierul SCS
                string caleSCS = caleDocAcad.Remove(caleDocAcad.LastIndexOf('.')) + ".SCS";
                StreamWriter scriitor = new StreamWriter(caleSCS, false);
                foreach (ObjectId OID in OIDS)
                {
                    SampleLine SLcurr = OID.GetObject(OpenMode.ForRead) as SampleLine;
                    SectionView SVcurr = SLcurr.GetSectionViewIds()[set].GetObject(OpenMode.ForRead) as SectionView;
                    double x = -999; double y = -999;
                    SVcurr.FindXYAtOffsetAndElevation(0, 0, ref x, ref y);
                    string km = SVcurr.Name.Remove(SVcurr.Name.IndexOf('(') - 1).Replace("+", "");
                    scriitor.WriteLine(string.Format("{0},{1},{2}", km, x, y));                    
                }
                scriitor.Dispose();
                ed.WriteMessage("\nCoordinate System File written succesfully.");
            }

        }
    }

    //public class SectUtil
    //{
    //    [CommandMethod("ES")] //Comanda care exporta un set de sectiuni in format KmOZD txt
    //    public void ExportSections()
    //    {
    //        CivilDocument civDoc = CivilApplication.ActiveDocument;
    //        Document acadDoc = Application.DocumentManager.MdiActiveDocument;
    //        Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
    //        Database db = HostApplicationServices.WorkingDatabase;
    //        string caleDocAcad = HostApplicationServices.Current.FindFile(acadDoc.Name, db, FindFileHint.Default);

    //        PromptEntityOptions PrEntOpt = new PromptEntityOptions("\nSelect a section from the desired set to be exported: ");
    //        PrEntOpt.SetRejectMessage("\nThe selected object is not a Section object! ");
    //        PrEntOpt.AddAllowedClass(typeof(CivilDBS.Section), true);

    //        PromptEntityResult PrEntRes = ed.GetEntity(PrEntOpt);
    //        if (PrEntRes.Status == PromptStatus.OK)
    //        {
    //            ObjectId SSID = PrEntRes.ObjectId;
    //            using (Transaction trans = db.TransactionManager.StartTransaction())
    //            {
    //                CivilDBS.Section SS = (CivilDBS.Section)trans.GetObject(SSID, OpenMode.ForRead);
    //                int SSstart = SS.Name.LastIndexOf("- ") + 2;
    //                int SSend = SS.Name.LastIndexOf('(');
    //                string SSsource = SS.Name.Substring(SSstart, SSend - SSstart);
    //                ed.WriteMessage("\nSelected sections's source is: {0}", SSsource);

    //                string caleFisSect = caleDocAcad.Remove(caleDocAcad.LastIndexOf('.')) + " - " + SSsource + ".txt";
    //                StreamWriter scriitor = new StreamWriter(caleFisSect);

    //                CivilDBS.SampleLine SL = (CivilDBS.SampleLine)trans.GetObject(SS.SampleLineId, OpenMode.ForRead);
    //                CivilDBS.SampleLineGroup SLG = (CivilDBS.SampleLineGroup)trans.GetObject(SL.GroupId, OpenMode.ForRead);
    //                ObjectIdCollection SLIDS = SLG.GetSampleLineIds();
    //                foreach (ObjectId SLID in SLIDS)
    //                {
    //                    CivilDBS.SampleLine currSL = (CivilDBS.SampleLine)trans.GetObject(SLID, OpenMode.ForRead);
    //                    ObjectIdCollection SIDS = currSL.GetSectionIds();
    //                    foreach (ObjectId SID in SIDS)
    //                    {
    //                        CivilDBS.Section S = (CivilDBS.Section)trans.GetObject(SID, OpenMode.ForRead);
    //                        int Sstart = S.Name.LastIndexOf("- ") + 2;
    //                        int Send = S.Name.LastIndexOf('(');
    //                        string Ssource = S.Name.Substring(Sstart, Send - Sstart);
    //                        if (SSsource == Ssource)
    //                        {
    //                            CivilDBS.SectionPointCollection Spoints = S.SectionPoints;
    //                            double LO = -999;
    //                            double RO = 999;
    //                            try
    //                            {
    //                                LO = S.LeftOffset;
    //                                RO = S.RightOffset;
    //                            }
    //                            catch
    //                            {
    //                                ed.WriteMessage("\nLimits of section at chainage: {0} cound not be read! Section skipped.", S.Station);
    //                                continue;
    //                            }
    //                            for (int i = 0; i < Spoints.Count; i++)
    //                            {
    //                                //if (Spoints[i].Location.X >= S.LeftOffset && Spoints[i].Location.X <= S.RightOffset)
    //                                if (Spoints[i].Location.X >= LO && Spoints[i].Location.X <= RO)
    //                                {
    //                                    string sectiune = S.Station.ToString() + "," +
    //                                                                    Spoints[i].Location.X.ToString("F3") + "," +
    //                                                                    Spoints[i].Location.Y.ToString("F3") + "," + Ssource;
    //                                    scriitor.WriteLine(sectiune);
    //                                }
    //                            }
    //                            //Polyline poly = (Polyline)S.BaseCurve;
    //                            //for (int i = 0; i < poly.NumberOfVertices; i++)
    //                            //{
    //                            //    Point3d p = poly.GetPoint3dAt(i);
    //                            //    string sectiune = S.Station.ToString() + "," +
    //                            //    p.X.ToString("F3") + "," +
    //                            //    p.Y.ToString("F3") + "," + Ssource;
    //                            //    scriitor.WriteLine(sectiune);
    //                            //}
    //                        }
    //                    }
    //                }

    //                scriitor.Dispose();
    //            }
    //        }
    //    }

    //    //Comanda pentru generarea fisierelor cu sisteme de coordonate ale sectiunilor (*.SCS)
    //    //din grupurile de sectiuni transversale Civil3D
    //    [CommandMethod("GENSCS")]
    //    public void genscs()
    //    {
    //        //Definitii generale
    //        Document acadDoc = Application.DocumentManager.MdiActiveDocument;
    //        CivilDocument civilDoc = CivilApplication.ActiveDocument;
    //        Database db = HostApplicationServices.WorkingDatabase;
    //        Editor ed = acadDoc.Editor;

    //        //Verificarea existentei macar a unui singur grup de sectiuni transversale
    //        //SectionViewGroupCollection SVGColl = civilDoc.get
    //        //SelectionSet SS =  

    //        //Gasirea caii documentului curent
    //        string caleDocAcad = acadDoc.Database.Filename;
    //        //Daca fisierul este unul nou, ne salvat inca, calea apartine sablonului folosit pentru acesta.
    //        //Se verifica daca fisierul este unul sablon, se atentioneaza utilizatorul si se paraseste programul.
    //        if (caleDocAcad.EndsWith(".dwt") == true)
    //        {
    //            ed.WriteMessage("\nThe current drawing is a template file (*.dwt). Exiting program! ");
    //            return;
    //        }
    //        caleDocAcad = HostApplicationServices.Current.FindFile(acadDoc.Name, acadDoc.Database, FindFileHint.Default);


    //        //Selectarea grupului de sectiuni transversale
    //        ed.WriteMessage("\nCommand for generating section coordinate systems file (*.SCS) from section view groups");
    //        PromptEntityOptions PEO = new PromptEntityOptions("\nSelect a section view belonging to the desired section view group: ");
    //        PEO.SetRejectMessage("\nThe selected object is not a section view! Select object: ");
    //        PEO.AddAllowedClass(typeof(SectionView), true);

    //        PromptEntityResult PER = ed.GetEntity(PEO);
    //        if (PER.Status != PromptStatus.OK)
    //        {
    //            ed.WriteMessage("\nInvalid selection! Aborting.");
    //            return;
    //        }

    //        using (Transaction trans = db.TransactionManager.StartTransaction())
    //        {
    //            SectionView SV = PER.ObjectId.GetObject(OpenMode.ForRead) as SectionView;
    //            SampleLine SL = SV.OwnerId.GetObject(OpenMode.ForRead) as SampleLine;
    //            if (SL == null) //Verificare temporara pt. testare
    //            {
    //                ed.WriteMessage("\nNu s-a obtinut pichetul dorit!");
    //                return;
    //            }
    //            SampleLineGroup SLG = SL.GroupId.GetObject(OpenMode.ForRead) as SampleLineGroup;
    //            ObjectIdCollection OIDS = SLG.GetSampleLineIds();
                
    //            //Obtinerea sistemelor de coordonate si scrierea lor in fisierul SCS
    //            foreach (ObjectId OID in OIDS)
    //            {
    //                SampleLine SLcurr = OID.GetObject(OpenMode.ForRead) as SampleLine;
    //                ObjectIdCollection SVIDS = SLcurr.GetSectionViewIds();
    //                ed.WriteMessage("\nSample line {0} has the folowing Section Views:", SLcurr.Name);
    //                foreach (ObjectId SVID in SVIDS)
    //                {
    //                    ed.WriteMessage("\n" + ((SectionView)SVID.GetObject(OpenMode.ForRead)).Name);
    //                }
    //            }


    //        }

    //    }
    //}
}
