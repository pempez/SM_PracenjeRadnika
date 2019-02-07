using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

using CrystalDecisions.CrystalReports.Engine;
using System.Threading;
using System.Globalization;

namespace SM_PracenjeRadnika
{
    public partial class Form1 : Form
    {
        private static double granicnoVreme = 120;//120
        ReportDocument ReportDoc;
        public Form1()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us", false);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us", false);

            InitializeComponent();

            popuniComboBoxProdOrderNo("3");

            popuniCBartikal();
            popuniCBradnik();
            popuniWC();
            popuniWGroup();
            cbRadnik.SelectedIndex = -1;
            cbRadniCentar.SelectedIndex = -1;
            cbProdOrderNo.SelectedIndex = -1;
            cbItemNo.SelectedIndex = -1;
            cbRadnaGrupa.SelectedIndex = -1;


            //cbPeriodOJD.Checked = true;
            //dtpOJDod.Value = new DateTime(2018, 7, 23);
            //dtpOJDdo.Value = new DateTime(2018, 7, 23);

        }

        private void popuniCBradnik()
        {
            DataTable dt = metode.DB.baza_upit("SELECT   TOP (100) PERCENT No_, Name " +
                                    " FROM            dbo.[Stirg Produkcija$Resource] WHERE        (Name <> N'') ORDER BY Name");
            cbRadnik.DataSource = dt;
            cbRadnik.DisplayMember = "Name";
            cbRadnik.ValueMember = "No_";

        }

        private void popuniWC()
        {
            cbRadniCentar.DataSource = metode.DB.baza_upit("SELECT        No_, Name " +
                        " FROM              dbo.[Stirg Produkcija$Work Center] " +
                        " ORDER BY  No_");
            cbRadniCentar.DisplayMember = "Name";
            cbRadniCentar.ValueMember = "No_";
        }


        private void popuniWGroup()
        {
            cbRadnaGrupa.DataSource = metode.DB.baza_upit("SELECT        Code, Name " +
                        " FROM            [Stirg Produkcija$Work Center Group] " +
                        " ORDER BY  Code");
            cbRadnaGrupa.DisplayMember = "Name";
            cbRadnaGrupa.ValueMember = "Code";
        }

        private void popuniComboBoxProdOrderNo()
        {
            cbProdOrderNo.DataSource = metode.DB.baza_upit("SELECT DISTINCT TOP (100) PERCENT No_ " +
          " FROM            dbo.[Stirg Produkcija$Production Order] " +
          "  ORDER BY No_");
            cbProdOrderNo.DisplayMember = "No_";
            cbProdOrderNo.ValueMember = "No_";
        }

        private void popuniComboBoxProdOrderNo(string status)
        {
            cbProdOrderNo.DataSource = metode.DB.baza_upit("SELECT DISTINCT TOP (100) PERCENT No_ " +
          " FROM            dbo.[Stirg Produkcija$Production Order] " +
          " where status = '" + status + "'" +
          "  ORDER BY No_");
            cbProdOrderNo.DisplayMember = "No_";
            cbProdOrderNo.ValueMember = "No_";
        }




        private void popuniCBartikal()
        {

            cbItemNo.DataSource = metode.DB.baza_upit("SELECT DISTINCT TOP (100) PERCENT [Item No_]" +
                         " FROM           dbo.[Stirg Produkcija$Prod_ Order Line]  " +
                         "  ORDER BY [Item No_]");
            cbItemNo.DisplayMember = "Item No_";
            cbItemNo.ValueMember = "Item No_";
        }



        private void btnPronadji_Click(object sender, EventArgs e)
        {
            try
            {
                dgvRadnici.Columns.Remove("cmb");

            }
            catch { }
            //string qSelect = "SELECT DISTINCT dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] AS [Šifra resursa], dbo.[Stirg Produkcija$Resource].Name AS [Ime resursa]" +
            //    " FROM dbo.[Stirg Produkcija$Output Journal Data] INNER JOIN  " +
            //    "  dbo.[Stirg Produkcija$Resource] ON dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ " +
            //    "WHERE(1 = 1)";

            string qSelect = "SELECT DISTINCT dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] AS [Šifra resursa], dbo.[Stirg Produkcija$Resource].Name AS [Ime resursa]" +
                " FROM   dbo.[Stirg Produkcija$Output Journal Data] INNER JOIN " +
                "  dbo.[Stirg Produkcija$Resource] ON dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ INNER JOIN " +
             "  dbo.[Stirg Produkcija$Prod_ Order Line] ON dbo.[Stirg Produkcija$Output Journal Data].[Item No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Item No_] AND " +
                "  dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] INNER JOIN  " +
           " dbo.[Stirg Produkcija$Prod_ Order Routing Line] ON dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Prod_ Order No_] AND " +
                "  dbo.[Stirg Produkcija$Prod_ Order Line].[Line No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Routing Reference No_] AND " +
                "  dbo.[Stirg Produkcija$Output Journal Data].[Operation No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Operation No_] " +
                "WHERE(1 = 1)  AND (DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) >= 0)" +
                " and dbo.[Stirg Produkcija$Resource].Name<>N'' ";

            qSelect = PrimeniFiltere(qSelect);

            DataTable dt = metode.DB.baza_upit(qSelect);

            if (dt.Rows.Count > 0)
            {
                lblRadnici.Text = dt.Rows.Count.ToString();
                dgvRadnici.DataSource = dt;
                //      dgvRadnici_Click(null, null);

                //dodajem checkBox
                DataGridViewCheckBoxColumn cmb = new DataGridViewCheckBoxColumn();
                cmb.HeaderText = "Izbaci";
                cmb.Name = "cmb";

                dgvRadnici.Columns.Add(cmb);
                foreach (DataGridViewRow r in dgvRadnici.Rows)
                    r.Cells[2].Value = false;
            }
            else
            {
                lblRadnici.Text = "";
                lblOJD.Text = "";
                lblPOL.Text = "";
                dgvRadnici.DataSource = null;
                dgvOutpuJournalData.DataSource = null;
                dgvProdOrderRoutingLine.DataSource = null;
                MessageBox.Show("Nema podataka za zadati kriterijum");
            }

        }

        private void UcitajOJD(string radnik)
        {

            //string qSelect = "SELECT        dbo.[Stirg Produkcija$Output Journal Data].[Item No_] AS [Broj artikla], CASE WHEN dbo.[Stirg Produkcija$Output Journal Data].Status = 0 THEN 'započeta' ELSE 'proknjižena' END AS status,  " +
            //           "  dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] AS[Broj naloga za proizvodnju], dbo.[Stirg Produkcija$Output Journal Data].[Operation No_] AS[Broj operacije],  " +
            //         "     dbo.[Stirg Produkcija$Output Journal Data].[Last Operation No_] AS[Broj poslednje operacije], dbo.[Stirg Produkcija$Output Journal Data].[Posting Date] AS[Datum knjiženja], CONVERT(char(5), " +
            //        "    dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], 108) AS[Vreme prvog registrovanog početka], CONVERT(char(5), dbo.[Stirg Produkcija$Output Journal Data].[Ending Time], 108) " +
            //          "    AS[Vreme poslednjeg registrovanog završetka], DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], " +
            //        "    dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) AS[stvarno Trajanje], dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity] AS[Izlazna količina], dbo.[Stirg Produkcija$Output Journal Data].[Scrap Quantity] AS[Količina škarta],  " +
            //          "    dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] AS[Šifra resursa], dbo.[Stirg Produkcija$Resource].Name AS[Ime resursa], dbo.[Stirg Produkcija$Output Journal Data].[Line No_] AS[Broj reda],  " +
            //          "    dbo.[Stirg Produkcija$Output Journal Data].[Controlled Operation No_] AS[Broj kontrolisane operacije], dbo.[Stirg Produkcija$Prod_ Order Line].[Line No_] " +
            //        " FROM dbo.[Stirg Produkcija$Output Journal Data]   INNER JOIN   dbo.[Stirg Produkcija$Resource] ON dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ INNER JOIN " +
            //        " dbo.[Stirg Produkcija$Prod_ Order Line] ON dbo.[Stirg Produkcija$Output Journal Data].[Item No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Item No_] AND " +
            //"  dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] " +
            //           " WHERE    (dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = N'" + radnik + "') AND (DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) >= 0) ";

            string qSelect = "SELECT DISTINCT TOP(100) PERCENT dbo.[Stirg Produkcija$Output Journal Data].[Item No_] AS[Broj artikla], CASE WHEN dbo.[Stirg Produkcija$Output Journal Data].Status = 0 THEN 'započeta' ELSE 'proknjižena' END AS status, " +
                         "  dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] AS[Broj naloga za proizvodnju], dbo.[Stirg Produkcija$Output Journal Data].[Operation No_] AS[Broj operacije],  " +
                         "  dbo.[Stirg Produkcija$Output Journal Data].[Last Operation No_] AS[Broj poslednje operacije], dbo.[Stirg Produkcija$Prod_ Order Line].Quantity AS [Naručena količina], " +
                         "  dbo.[Stirg Produkcija$Prod_ Order Line].[Finished Quantity] AS[Urađena količina], dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity] AS[Izlazna količina], dbo.[Stirg Produkcija$Output Journal Data].[Scrap Quantity] AS[Količina škarta], dbo.[Stirg Produkcija$Output Journal Data].[Posting Date] AS[Datum knjiženja], CONVERT(char(5),  " +
                         "  dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], 108) AS[Vreme prvog registrovanog početka], CONVERT(char(5), dbo.[Stirg Produkcija$Output Journal Data].[Ending Time], 108)  " +
                         "  AS[Vreme poslednjeg registrovanog završetka],    CASE WHEN DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) = 0 THEN 1 ELSE" +
                         " DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) END  AS[stvarno Trajanje],  " +
                         "   " +
                         "  dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] AS[Šifra resursa], dbo.[Stirg Produkcija$Resource].Name AS[Ime resursa], dbo.[Stirg Produkcija$Output Journal Data].[Line No_] AS[Broj reda],  " +
                         "  dbo.[Stirg Produkcija$Output Journal Data].[Controlled Operation No_] AS[Broj kontrolisane operacije], dbo.[Stirg Produkcija$Prod_ Order Line].[Line No_], dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Run Time] " +
        "  FROM dbo.[Stirg Produkcija$Output Journal Data] INNER JOIN " +

                        "  dbo.[Stirg Produkcija$Resource] ON dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ INNER JOIN " +

                       "   dbo.[Stirg Produkcija$Prod_ Order Line] ON dbo.[Stirg Produkcija$Output Journal Data].[Item No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Item No_] AND " +
                       "   dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] INNER JOIN " +

                       "   dbo.[Stirg Produkcija$Prod_ Order Routing Line] ON dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Prod_ Order No_] AND " +
                       "   dbo.[Stirg Produkcija$Prod_ Order Line].[Line No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Routing Reference No_]" +
                       "  AND      dbo.[Stirg Produkcija$Output Journal Data].[Operation No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Operation No_] " +
                    " WHERE    (dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = N'" + radnik + "')  AND (DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) >= 0) ";


            qSelect = PrimeniFiltere(qSelect);

            //if (cbRadnaGrupa.Text != "")
            //{
            //    qSelect += " and (dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Work Center Group Code] = N'" + cbRadnaGrupa.SelectedValue + "')";
            //}

            qSelect += " ORDER BY [Datum knjiženja], [Vreme prvog registrovanog početka]  ";

            DataTable dt = metode.DB.baza_upit(qSelect);

            if (dt.Rows.Count > 0)
            {
                lblOJD.Text = dt.Rows.Count.ToString();
                dgvOutpuJournalData.DataSource = dt;
                dgvOutpuJournalData.Columns["Urađena količina"].DefaultCellStyle.Format = "N2";
                dgvOutpuJournalData.Columns["Naručena količina"].DefaultCellStyle.Format = "N2";
                dgvOutpuJournalData.Columns["Izlazna količina"].DefaultCellStyle.Format = "N2";
                dgvOutpuJournalData.Columns["Količina škarta"].DefaultCellStyle.Format = "N2";
                dgvOutpuJournalData_Click(null, null);

            }
            else
            {
                lblOJD.Text = "";
                dgvOutpuJournalData.DataSource = null;
                dgvProdOrderRoutingLine.DataSource = null;
            }
        }

        private void ucitajProdOrderRoutingLine(string prodOrderNo, string referenceNo, string operationNo)
        {

            string qUpit = " select [Routing Reference No_] as [referentni broj proizvodnog postupka], [Operation No_] as [Broj operacije]," +
                " 'Radni centar' as Vrsta,[Work Center No_] as Broj, Description as Opis ,[Setup Time] as [vreme podešavanja], [Run Time] as [vreme izvođenja],[Setup Time Unit of Meas_ Code] as [Vremenska jedinica] ,[Send-Ahead Quantity] as [unapred poslata količina], [Concurrent Capacities] as  [uporedni kapaciteti], [Work Center Group Code] " +
                " FROM            [Stirg Produkcija$Prod_ Order Routing Line]" +
                 " WHERE        ([Prod_ Order No_] = N'" + prodOrderNo + "') AND ([Routing Reference No_] = '" + referenceNo + "') and [Operation No_]='" + operationNo + "'";
            //if (cbRadnaGrupa.Text != "")
            //{
            //    qUpit += " and [Work Center Group Code]=N'" + cbRadnaGrupa.SelectedValue + "' ";
            //}


            DataTable dt = metode.DB.baza_upit(qUpit);

            if (dt.Rows.Count > 0)
            {
                dgvProdOrderRoutingLine.DataSource = dt;
                dgvProdOrderRoutingLine.Columns["vreme podešavanja"].DefaultCellStyle.Format = "N2";
                dgvProdOrderRoutingLine.Columns["vreme izvođenja"].DefaultCellStyle.Format = "N2";
                dgvProdOrderRoutingLine.Columns["unapred poslata količina"].DefaultCellStyle.Format = "N2";
                dgvProdOrderRoutingLine.Columns["uporedni kapaciteti"].DefaultCellStyle.Format = "N2";
            }
            else
            {
                dgvProdOrderRoutingLine.DataSource = null;
            }

        }

        private string PrimeniFiltere(string qSelect)
        {
            DateTime datumOd = new DateTime(dtpOJDod.Value.Year, dtpOJDod.Value.Month, dtpOJDod.Value.Day);
            DateTime datumDo = new DateTime(dtpOJDdo.Value.Year, dtpOJDdo.Value.Month, dtpOJDdo.Value.Day);


            //zatim izbaciti sva čekiranja gde se koristi radni centar eksterna usluga ima 2 kom sa -M i - S
            qSelect += " AND (dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Work Center No_] <> N'RC5010') " +
                " and (dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Work Center No_] <> N'RC5020')";

            if (cbRadnik.Text != "")
            {
                qSelect += " AND (dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = N'" + cbRadnik.SelectedValue + "')";
            }

            if (cbItemNo.Text != "")
            {
                qSelect += " and  (dbo.[Stirg Produkcija$Output Journal Data].[Item No_] = N'" + cbItemNo.Text + "')";
            }

            if (cbProdOrderNo.Text != "")
            {
                qSelect += " AND (dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] = N'" + cbProdOrderNo.Text + "')";
            }

            if (cbSmena.Text != "")
            {
                int satiOd = 0;
                int satiDo = 0;
                if (cbSmena.Text == "I")
                {
                    satiOd = 7;
                    satiDo = 15;
                    qSelect += " AND (CAST(dbo.[Stirg Produkcija$Output Journal Data].[Starting Time] AS time)>= '" + satiOd + ":00:00')" +
                   " AND (CAST(dbo.[Stirg Produkcija$Output Journal Data].[Starting Time] AS time)<= '" + satiDo + ":00:00') ";
                }
                else if (cbSmena.Text == "III")
                {
                    satiOd = 23;
                    satiDo = 7;
                    qSelect += " AND ((CAST(dbo.[Stirg Produkcija$Output Journal Data].[Starting Time] AS time)>= '" + satiOd + ":00:00') and (CAST(dbo.[Stirg Produkcija$Output Journal Data].[Starting Time] AS time) <= '23:59:59')  " +
                   " or  (CAST(dbo.[Stirg Produkcija$Output Journal Data].[Starting Time] AS time) >= '00:00:00')  and (CAST(dbo.[Stirg Produkcija$Output Journal Data].[Starting Time] AS time)<= '" + satiDo + ":00:00')) ";
                }
                else if (cbSmena.Text == "II")
                {
                    satiOd = 15;
                    satiDo = 23;
                    qSelect += " AND (CAST(dbo.[Stirg Produkcija$Output Journal Data].[Starting Time] AS time)>= '" + satiOd + ":00:00')" +
                   " AND (CAST(dbo.[Stirg Produkcija$Output Journal Data].[Starting Time] AS time)<= '" + satiDo + ":00:00') ";
                }
            }

            if (cbPeriodOJD.Checked)
            {
                qSelect += " and (   dbo.[Stirg Produkcija$Output Journal Data].[Posting Date] >= CONVERT(DATETIME, '" + datumOd.Year + "-" + datumOd.Month + "-" + datumOd.Day + " 00:00:00', 102))" +
                     " and (   dbo.[Stirg Produkcija$Output Journal Data].[Posting Date] <= CONVERT(DATETIME, '" + datumDo.Year + "-" + datumDo.Month + "-" + datumDo.Day + " 23:59:59', 102)) ";
            }

            if (cbRadniCentar.Text != "")
            {
                qSelect += " AND (dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Work Center No_] = N'" + cbRadniCentar.SelectedValue + "')";
            }

            if (cbRadnaGrupa.Text != "")
            {
                qSelect += " and (dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Work Center Group Code] = N'" + cbRadnaGrupa.SelectedValue + "')";
            }

            return qSelect;
        }

        private void dgvRadnici_Click(object sender, EventArgs e)
        {
            if (dgvRadnici.Rows.Count > 0)
            {

                if (dgvRadnici.CurrentCell.OwningColumn.Name == "cmb")
                {
                    // dgvRadnici.CurrentRow.Cells["cmb"].Value = !Boolean.Parse(dgvRadnici.CurrentRow.Cells["cmb"].Value.ToString());
                }
                else
                {
                    UcitajOJD(dgvRadnici.CurrentRow.Cells["Šifra resursa"].Value.ToString());
                    // UcitajIskoriscenost(dgvRadnici.CurrentRow.Cells["Šifra resursa"].Value.ToString());

                    gpKontejner.Visible = false;

                    #region ucitaj predvidjeoNula ukupno
                    double ukupnoPredvidjeno = 0;
                    double ukupnoUtroseno = 0;
                    double ukupnoPredvidjenoNula = 0;
                    double ukupnoUtrosenoNula = 0;


                    ArrayList skup = new ArrayList();
                    string[] uslov = new string[3];
                    string[] uslov1 = new string[3];

                    //skup.Add(uslov1);

                    int i = 0;
                    foreach (DataGridViewRow r in dgvOutpuJournalData.Rows)
                    {
                        string prodOrderNo = r.Cells["Broj naloga za proizvodnju"].Value.ToString();
                        string lineNo = r.Cells["Line No_"].Value.ToString();
                        string operationNo = r.Cells["broj Operacije"].Value.ToString();
                        string itemNo = r.Cells["broj artikla"].Value.ToString();

                        //nadji ko je radio i vrati broj ukupnih cekiranja
                        double ukupnoCekirani = KoJeJosRadio(prodOrderNo, itemNo, operationNo);
                        ucitajProdOrderRoutingLine(prodOrderNo, lineNo, operationNo);

                        uslov[0] = prodOrderNo;
                        uslov[1] = itemNo;
                        uslov[2] = operationNo;
                        bool ima = false;
                        foreach (string[] s in skup)
                        {
                            if (s[0] == prodOrderNo && s[1] == itemNo && s[2] == operationNo)
                            {
                                ima = true;
                            }
                        }
                        if (!ima)
                        {
                            double[] rez = IzracunajVreme(ukupnoCekirani, r);
                            if (rez[2] == 1)
                            {
                                ukupnoPredvidjeno += rez[0];
                                ukupnoUtroseno += rez[1];
                            }
                            else
                            {
                                ukupnoPredvidjenoNula += rez[0];
                                ukupnoUtrosenoNula += rez[1];
                            }
                            skup.Insert(i, new string[3] { prodOrderNo, itemNo, operationNo });
                            i++;
                        }


                    }
                    skup.Clear();

                    tbUkupnoPredvidjenoVreme.Text = ukupnoPredvidjeno.ToString();
                    tbUkupnoUtrosenoVreme.Text = ukupnoUtroseno.ToString();

                    tbUkupnoPredvidjenoVremeNula.Text = ukupnoPredvidjenoNula.ToString();
                    tbUkupnoUtrosenoVremeNula.Text = ukupnoUtrosenoNula.ToString();
                    #endregion

                    #region odradjeno
                    //u min
                    double ostatakMinuti = 0;
                    double procenat = 0;
                    tbUkupnoPredvidjenoVreme.Text = ukupnoPredvidjeno.ToString("##.##") + "min";
                    tbUkupnoUtrosenoVreme.Text = ukupnoUtroseno.ToString("##.##") + "min";
                    procenat = ukupnoPredvidjeno * 100 / ukupnoUtroseno;
                    tbUkupnoProcenat.Text = procenat.ToString("##.##") + "%";

                    //u satima
                    ostatakMinuti = ukupnoUtroseno % 60;
                    double vremeUStaimaUtroseno = Math.Floor(ukupnoUtroseno / 60) + ostatakMinuti / 60;
                    tbUkupnoUtrosenoVremeSati.Text = vremeUStaimaUtroseno.ToString("##.##") + "h";

                    ostatakMinuti = ukupnoPredvidjeno % 60;
                    double vremeUStaimaPredvidjeno = Math.Floor(ukupnoPredvidjeno / 60) + ostatakMinuti / 60;
                    tbUkupnoPredvidjenoVremeSati.Text = vremeUStaimaPredvidjeno.ToString("##.##") + "h";
                    #endregion

                    #region u toku

                    //if (ukupnoPredvidjenoNula == 0)
                    //    ukupnoUtrosenoNula = 0;
                    tbUkupnoUtrosenoVremeNula.Text = ukupnoUtrosenoNula.ToString("##.##") + "min";
                    tbUkupnoPredvidjenoVremeNula.Text = ukupnoPredvidjenoNula.ToString("##.##") + "min";

                    //u satima
                    ostatakMinuti = ukupnoUtrosenoNula % 60;
                    double vremeUStaimaUtrosenoNula = Math.Floor(ukupnoUtrosenoNula / 60) + ostatakMinuti / 60;
                    tbUkupnoUtrosenoVremeSatiNula.Text = vremeUStaimaUtrosenoNula.ToString("##.##") + "h";

                    ostatakMinuti = ukupnoPredvidjenoNula % 60;
                    double vremeUStaimaPredvidjenoNula = Math.Floor(ukupnoPredvidjenoNula / 60) + ostatakMinuti / 60;
                    tbUkupnoPredvidjenoVremeSatiNula.Text = vremeUStaimaPredvidjenoNula.ToString("##.##") + "h";
                    #endregion

                    gpKontejner.Visible = true;
                    dgvOutpuJournalData_Click(null, null);
                    UcitajRadnikVreme();
                }
            }
        }

        private void dgvOutpuJournalData_Click(object sender, EventArgs e)
        {
            if (dgvOutpuJournalData.CurrentRow != null)
            {
                string prodOrderNo = dgvOutpuJournalData.CurrentRow.Cells["Broj naloga za proizvodnju"].Value.ToString();
                string lineNo = dgvOutpuJournalData.CurrentRow.Cells["Line No_"].Value.ToString();
                string operationNo = dgvOutpuJournalData.CurrentRow.Cells["broj Operacije"].Value.ToString();
                string itemNo = dgvOutpuJournalData.CurrentRow.Cells["broj artikla"].Value.ToString();

                //nadji ko je radio i vrati broj ukupnih cekiranja
                double ukupnoCekirani = KoJeJosRadio(prodOrderNo, itemNo, operationNo);
                ucitajProdOrderRoutingLine(prodOrderNo, lineNo, operationNo);

                double vreme = IzracunajVreme(ukupnoCekirani);

            }
        }

        private double KoJeJosRadio(string prodOrderNo, string itemNo, string operationNo)
        {
            DateTime datumOd = new DateTime(dtpOJDod.Value.Year, dtpOJDod.Value.Month, dtpOJDod.Value.Day);
            DateTime datumDo = new DateTime(dtpOJDdo.Value.Year, dtpOJDdo.Value.Month, dtpOJDdo.Value.Day);

            string qSelect = "SELECT distinct dbo.[Stirg Produkcija$Resource].No_, dbo.[Stirg Produkcija$Resource].Name, " +
                "sum(dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity]) as [Izlazna količina], dbo.[Stirg Produkcija$Output Journal Data].[Posting Date] " +
                "FROM            dbo.[Stirg Produkcija$Output Journal Data] INNER JOIN   dbo.[Stirg Produkcija$Resource] ON " +
                "dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ " +
                "WHERE(dbo.[Stirg Produkcija$Output Journal Data].[Item No_] = N'" + itemNo + "') AND(dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] = N'" + prodOrderNo + "') AND" +
                " (dbo.[Stirg Produkcija$Output Journal Data].[Operation No_] = N'" + operationNo + "') " +
                " AND(DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) >= 0)";


            if (cbPeriodOJD.Checked)
            {
                qSelect += " and (    dbo.[Stirg Produkcija$Output Journal Data].[Posting Date]>= CONVERT(DATETIME, '" + datumOd.Year + "-" + datumOd.Month + "-" + datumOd.Day + " 00:00:00', 102))" +
                     " and (   dbo.[Stirg Produkcija$Output Journal Data].[Posting Date] <= CONVERT(DATETIME, '" + datumDo.Year + "-" + datumDo.Month + "-" + datumDo.Day + " 23:59:59', 102)) ";
            }

            qSelect += " GROUP BY  dbo.[Stirg Produkcija$Resource].Name,dbo.[Stirg Produkcija$Resource].No_, dbo.[Stirg Produkcija$Output Journal Data].[Posting Date]" +
                " ORDER BY dbo.[Stirg Produkcija$Resource].No_";
            dgvRadiciDodatni.DataSource = metode.DB.baza_upit(qSelect);
            dgvRadiciDodatni.Columns["Izlazna količina"].DefaultCellStyle.Format = "N2";
            lblCekiraniRadnici.Text = dgvRadiciDodatni.Rows.Count.ToString();

            return dgvRadiciDodatni.Rows.Count;
        }

        //private double KoJeJosRadio(string prodOrderNo, string itemNo, string operationNo)
        //{
        //    DateTime datumOd = new DateTime(dtpOJDod.Value.Year, dtpOJDod.Value.Month, dtpOJDod.Value.Day);
        //    DateTime datumDo = new DateTime(dtpOJDdo.Value.Year, dtpOJDdo.Value.Month, dtpOJDdo.Value.Day);

        //    string qSelect = "SELECT        [Resource No_],dbo.[Stirg Produkcija$Resource].Name, [Prod_ Order No_], [Item No_], [Operation No_], [Posting Date], CONVERT(char(5), [Starting Time], 108) AS pocetak," +
        //        " CONVERT(char(5), [Ending Time], 108) AS kraj, [Output Quantity] as [Izlazna količina] " +
        //        "FROM            dbo.[Stirg Produkcija$Output Journal Data] INNER JOIN   dbo.[Stirg Produkcija$Resource] ON " +
        //        "dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ " +
        //        "WHERE(dbo.[Stirg Produkcija$Output Journal Data].[Item No_] = N'" + itemNo + "') AND(dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] = N'" + prodOrderNo + "') AND" +
        //        " (dbo.[Stirg Produkcija$Output Journal Data].[Operation No_] = N'" + operationNo + "') " +
        //        " AND(DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) >= 0)";


        //    if (cbPeriodOJD.Checked)
        //    {
        //        qSelect += " and (    dbo.[Stirg Produkcija$Output Journal Data].[Posting Date]>= CONVERT(DATETIME, '" + datumOd.Year + "-" + datumOd.Month + "-" + datumOd.Day + " 00:00:00', 102))" +
        //             " and (   dbo.[Stirg Produkcija$Output Journal Data].[Posting Date] <= CONVERT(DATETIME, '" + datumDo.Year + "-" + datumDo.Month + "-" + datumDo.Day + " 23:59:59', 102)) ";
        //    }

        //    qSelect += " ORDER BY dbo.[Stirg Produkcija$Resource].No_";
        //    dgvRadiciDodatni.DataSource = metode.DB.baza_upit(qSelect);
        //    dgvRadiciDodatni.Columns["Izlazna količina"].DefaultCellStyle.Format = "N2";
        //    lblCekiraniRadnici.Text = dgvRadiciDodatni.Rows.Count.ToString();

        //    return dgvRadiciDodatni.Rows.Count;
        //}

        private double IzracunajUkupnoVreme(string radnik, string nulaJedan)
        {
            //string qSelect = "SELECT DISTINCT    TOP(100) PERCENT dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity] AS[Izlazna količina], dbo.[Stirg Produkcija$Output Journal Data].[Scrap Quantity] AS[Količina škarta], "+
            //            " dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Setup Time], dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Run Time],  " +
            //              "  dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Setup Time] + (dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity] + dbo.[Stirg Produkcija$Output Journal Data].[Scrap Quantity])  " +
            //           "     * dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Run Time]  AS potrebnoVreme " +
            //      
            string qSelect = "";
            if (nulaJedan == ">") qSelect += "SELECT       isnull( SUM(dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Setup Time] + (dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity] + dbo.[Stirg Produkcija$Output Journal Data].[Scrap Quantity]) " +
                              "     * dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Run Time]),0) AS potrebnoVreme";

            else qSelect += "SELECT        ISNULL(SUM(dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Setup Time] + dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Run Time] *" +
                    " (dbo.[Stirg Produkcija$Prod_ Order Line].Quantity-  dbo.[Stirg Produkcija$Prod_ Order Line].[Finished Quantity] - dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity])), 0) AS potrebnoVreme";

            //"SELECT       isnull( SUM(dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Setup Time] +  dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Run Time]),0) AS potrebnoVreme";


            qSelect += " FROM dbo.[Stirg Produkcija$Output Journal Data] INNER JOIN " +
                          "  dbo.[Stirg Produkcija$Resource] ON dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ INNER JOIN " +
                      "      dbo.[Stirg Produkcija$Prod_ Order Line] ON dbo.[Stirg Produkcija$Output Journal Data].[Item No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Item No_] AND " +
                       "     dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] INNER JOIN " +
                       "     dbo.[Stirg Produkcija$Prod_ Order Routing Line] ON dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Prod_ Order No_] AND " +
                        "    dbo.[Stirg Produkcija$Prod_ Order Line].[Line No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Routing Reference No_] AND " +
                        "    dbo.[Stirg Produkcija$Output Journal Data].[Operation No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Operation No_] " +
            " WHERE    (dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = N'" + radnik + "') AND (DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) >= 0) " +
            "AND  (dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity] " + nulaJedan + " 0)";

            if (nulaJedan == "=")
                qSelect += " and dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Run Time]>=" + granicnoVreme + " ";// and dbo.[Stirg Produkcija$Prod_ Order Line].[Remaining Quantity]>0";//and   dbo.[Stirg Produkcija$Prod_ Order Line].[Finished Quantity]=0

            qSelect = PrimeniFiltere(qSelect);

            DataTable dt = metode.DB.baza_upit(qSelect);

            if (dt.Rows.Count > 0)
            {
                return double.Parse(dt.Rows[0]["potrebnoVreme"].ToString());

            }
            else
            {
                return 0;
            }
        }


        private double[] UtrosenoVremePoArtiklu(DataGridView dv, string nalog, string artikal, string operationNo)
        {
            double utrosenoVreme = 0;
            double izlaznaKolicina = 0;

            foreach (DataGridViewRow row in dv.Rows)
            {
                if (row.Cells["Broj artikla"].Value.ToString().Equals(artikal) && row.Cells["Broj naloga za proizvodnju"].Value.ToString().Equals(nalog)
                    && row.Cells["Broj operacije"].Value.ToString().Equals(operationNo))
                {

                    utrosenoVreme += double.Parse(row.Cells["stvarno trajanje"].Value.ToString());
                    izlaznaKolicina += double.Parse(row.Cells["izlazna količina"].Value.ToString());
                }
            }

            double[] izlaz = new double[2] { utrosenoVreme, izlaznaKolicina };
            return izlaz;
        }


        //ovde sam menjao dgvOutpuJournalData.CurrentRow+
        private double IzracunajVreme(double ukupnoCekirani)
        {
            granicnoVreme = 1;
            if (dgvProdOrderRoutingLine.CurrentRow != null)
            {
                double[] rez = UtrosenoVremePoArtiklu(dgvOutpuJournalData, dgvOutpuJournalData.CurrentRow.Cells["Broj naloga za proizvodnju"].Value.ToString(),
                    dgvOutpuJournalData.CurrentRow.Cells["Broj artikla"].Value.ToString(), dgvOutpuJournalData.CurrentRow.Cells["Broj operacije"].Value.ToString());


                double utrosenoVreme = rez[0];
                double uradjeno = 0;// rez[1]; 
                tbUtrosenoVreme.Text = utrosenoVreme.ToString();


                double predvidjenoVreme = 0;
                double procenat = 0;

                double skart = double.Parse(dgvOutpuJournalData.CurrentRow.Cells["količina škarta"].Value.ToString());
                double predvidjenoVremePoKomadu = double.Parse(dgvProdOrderRoutingLine.CurrentRow.Cells["vreme izvođenja"].Value.ToString());
                double vremePodesavanja = double.Parse(dgvProdOrderRoutingLine.CurrentRow.Cells["vreme podešavanja"].Value.ToString());
                double narucenaKolicina = double.Parse(dgvOutpuJournalData.CurrentRow.Cells["Naručena količina"].Value.ToString());
                double uradjenaKolicina = double.Parse(dgvOutpuJournalData.CurrentRow.Cells["urađena količina"].Value.ToString());
                double radnikUpunoCekirao = 0;

                //pronadji koilko puta je radio
                foreach (DataGridViewRow row in dgvRadiciDodatni.Rows)
                {
                    uradjeno += double.Parse(row.Cells["izlazna količina"].Value.ToString());
                    if (row.Cells[0].Value.ToString().Equals(dgvRadnici.CurrentRow.Cells["Šifra resursa"].Value.ToString()))
                    {
                        radnikUpunoCekirao++;
                    }
                }
                tbIzlaznaKolicina.Text = uradjeno.ToString();
                if (dgvProdOrderRoutingLine.CurrentRow.Cells["Vremenska jedinica"].Value.ToString() == "SAT")
                    predvidjenoVremePoKomadu = predvidjenoVremePoKomadu * 60;
                if (uradjeno > 0)
                {


                    tbSkart.Text = skart.ToString();

                    predvidjenoVreme = (vremePodesavanja + predvidjenoVremePoKomadu * (uradjeno + skart)) / ukupnoCekirani * radnikUpunoCekirao;
                    tbPredvidjenoVreme.Text = predvidjenoVreme.ToString("##.##");

                    procenat = predvidjenoVreme * 100 / utrosenoVreme;
                    tbProcenat.Text = procenat.ToString("##.##") + "%";
                    gbOdradjeno.Visible = true;
                    gbPredvidjenoVreme.Visible = false;
                    return predvidjenoVreme;
                }
                else
                {
                    //proveri da li ima odradjenih komada
                    double uradjenoOdOstalihRadnika = 0;
                    foreach (DataGridViewRow r in dgvRadiciDodatni.Rows)
                    {
                        uradjenoOdOstalihRadnika += double.Parse(r.Cells["izlazna količina"].Value.ToString());
                    }
                    if (predvidjenoVremePoKomadu > granicnoVreme)
                    {
                        tbUtrosenoVremeNula.Text = utrosenoVreme.ToString();
                        predvidjenoVreme = vremePodesavanja + predvidjenoVremePoKomadu * (narucenaKolicina - uradjenaKolicina - uradjeno);
                        predvidjenoVreme = predvidjenoVreme / ukupnoCekirani * radnikUpunoCekirao;
                        tbPredvidjenoVremePoKomadu.Text = predvidjenoVreme.ToString("##.##");
                        gbPredvidjenoVreme.Visible = true;
                        gbOdradjeno.Visible = false;
                        return predvidjenoVreme;
                    }
                    else
                    {
                        gbPredvidjenoVreme.Visible = false;
                        gbOdradjeno.Visible = false;
                        return 0;
                    }
                }
            }
            else return 0;
        }

        //private double IzracunajVreme(double ukupnoCekirani,DataGridViewRow r)
        //{
        //    if (dgvProdOrderRoutingLine.CurrentRow != null)
        //    {
        //        double utrosenoVreme = double.Parse(r.Cells["stvarno Trajanje"].Value.ToString());
        //        double predvidjenoVreme = 0;
        //        double procenat = 0;
        //        double uradjeno = double.Parse(r.Cells["izlazna količina"].Value.ToString());
        //        double skart = double.Parse(r.Cells["količina škarta"].Value.ToString());
        //        double predvidjenoVremePoKomadu = double.Parse(dgvProdOrderRoutingLine.CurrentRow.Cells["vreme izvođenja"].Value.ToString());
        //        double vremePodesavanja = double.Parse(dgvProdOrderRoutingLine.CurrentRow.Cells["vreme podešavanja"].Value.ToString());
        //        double narucenaKolicina = double.Parse(r.Cells["Naručena količina"].Value.ToString());
        //        double uradjenaKolicina = double.Parse(r.Cells["urađena količina"].Value.ToString());
        //        double radnikUpunoCekirao = 0;

        //        //pronadji koilko puta je radio
        //        foreach (DataGridViewRow row in dgvRadiciDodatni.Rows)
        //        {
        //            if (row.Cells[0].Value.ToString().Equals(dgvRadnici.CurrentRow.Cells["Šifra resursa"].Value.ToString()))
        //            {
        //                radnikUpunoCekirao++;
        //            }
        //        }
        //        if (dgvProdOrderRoutingLine.CurrentRow.Cells["Vremenska jedinica"].Value.ToString() == "SAT")
        //            predvidjenoVremePoKomadu = predvidjenoVremePoKomadu * 60;
        //        if (uradjeno > 0)
        //        {
        //            tbUtrosenoVreme.Text = utrosenoVreme.ToString();
        //            tbIzlaznaKolicina.Text = uradjeno.ToString();
        //            tbSkart.Text = skart.ToString();

        //            predvidjenoVreme = vremePodesavanja + predvidjenoVremePoKomadu * (uradjeno + skart);
        //            predvidjenoVreme = predvidjenoVreme / ukupnoCekirani * radnikUpunoCekirao;
        //            tbPredvidjenoVreme.Text = predvidjenoVreme.ToString();

        //            procenat = predvidjenoVreme * 100 / utrosenoVreme;
        //            tbProcenat.Text = procenat.ToString("##.##") + "%";
        //            gbOdradjeno.Visible = true;
        //            gbPredvidjenoVreme.Visible = false;
        //            return predvidjenoVreme;//0
        //        }
        //        else
        //        {
        //            if (predvidjenoVremePoKomadu > 119)
        //            {
        //                tbUtrosenoVremeNula.Text = utrosenoVreme.ToString();
        //                predvidjenoVreme = vremePodesavanja + predvidjenoVremePoKomadu * (narucenaKolicina - uradjenaKolicina - uradjeno);
        //                predvidjenoVreme = predvidjenoVreme / ukupnoCekirani * radnikUpunoCekirao;
        //                tbPredvidjenoVremePoKomadu.Text = predvidjenoVreme.ToString();
        //                gbPredvidjenoVreme.Visible = true;
        //                gbOdradjeno.Visible = false;
        //                return predvidjenoVreme;
        //            }
        //            else
        //            {
        //                gbPredvidjenoVreme.Visible = false;
        //                gbOdradjeno.Visible = false;
        //                return 0;
        //            }
        //        }
        //    }
        //    else return 0;
        //}


        private double[] IzracunajVreme(double ukupnoCekirani, DataGridViewRow r)
        {
            granicnoVreme = 1;
            if (dgvProdOrderRoutingLine.CurrentRow != null)
            {
                double[] rez = UtrosenoVremePoArtiklu(dgvOutpuJournalData, r.Cells["Broj naloga za proizvodnju"].Value.ToString(),
                            r.Cells["Broj artikla"].Value.ToString(), r.Cells["Broj operacije"].Value.ToString());


                double utrosenoVreme = rez[0];
                double uradjeno = 0;// rez[1];

                double predvidjenoVreme = 0;

                double skart = double.Parse(r.Cells["količina škarta"].Value.ToString());
                double predvidjenoVremePoKomadu = double.Parse(dgvProdOrderRoutingLine.CurrentRow.Cells["vreme izvođenja"].Value.ToString());
                double vremePodesavanja = double.Parse(dgvProdOrderRoutingLine.CurrentRow.Cells["vreme podešavanja"].Value.ToString());
                double narucenaKolicina = double.Parse(r.Cells["Naručena količina"].Value.ToString());
                double uradjenaKolicina = double.Parse(r.Cells["urađena količina"].Value.ToString());
                double radnikUpunoCekirao = 0;


                //pronadji koilko puta je radio
                foreach (DataGridViewRow row in dgvRadiciDodatni.Rows)
                {
                    uradjeno += double.Parse(row.Cells["izlazna količina"].Value.ToString());
                    if (row.Cells[0].Value.ToString() == r.Cells["Šifra resursa"].Value.ToString())
                    {
                        radnikUpunoCekirao++;
                    }
                }


                if (dgvProdOrderRoutingLine.CurrentRow.Cells["Vremenska jedinica"].Value.ToString() == "SAT")
                    predvidjenoVremePoKomadu = predvidjenoVremePoKomadu * 60;
                if (uradjeno > 0)
                {

                    predvidjenoVreme = vremePodesavanja + predvidjenoVremePoKomadu * (uradjeno + skart);
                    predvidjenoVreme = predvidjenoVreme / ukupnoCekirani * radnikUpunoCekirao;


                    double[] izlaz = new double[3] { predvidjenoVreme, utrosenoVreme, 1 };
                    return izlaz;

                }
                else
                {
                    if (predvidjenoVremePoKomadu >= granicnoVreme)
                    {

                        predvidjenoVreme = vremePodesavanja + predvidjenoVremePoKomadu * (narucenaKolicina - uradjenaKolicina - uradjeno);
                        predvidjenoVreme = predvidjenoVreme / ukupnoCekirani * radnikUpunoCekirao;
                        //      tbPredvidjenoVremePoKomadu.Text = predvidjenoVreme.ToString();


                        double[] izlaz = new double[3] { predvidjenoVreme, utrosenoVreme, 0 };
                        return izlaz;
                    }
                    else
                    {

                        double[] izlaz = new double[3] { 0, 0, 0 };
                        return izlaz;
                    }
                }
            }
            else
            {
                double[] izlaz = new double[3] { 0, 0, 0 };
                return izlaz;
            }
        }


        private void UcitajIskoriscenost(string IdRadnik)
        {
            double ukupnoUtroseno = 0;
            double ukupnoUtrosenoNula = 0;
            double ukupnoPredvidjeno = IzracunajUkupnoVreme(IdRadnik, ">");
            double ukupnoUtrosenoSat = 0;
            double ukupnoPredvidjenoNula = IzracunajUkupnoVreme(IdRadnik, "=");
            double ostatakMinuti = 0;
            double procenat = 0;

            if (dgvOutpuJournalData.Rows.Count > 0)
            {
                foreach (DataGridViewRow r in dgvOutpuJournalData.Rows)
                {

                    if ((double.Parse(r.Cells["Izlazna količina"].Value.ToString()) + double.Parse(r.Cells["Urađena količina"].Value.ToString())) > 0)
                    {
                        ukupnoUtroseno += double.Parse(r.Cells["stvarno trajanje"].Value.ToString());
                    }
                    else
                    {
                        if (double.Parse(r.Cells["run time"].Value.ToString()) >= granicnoVreme)
                            ukupnoUtrosenoNula += double.Parse(r.Cells["stvarno trajanje"].Value.ToString());
                    }
                }


                #region odradjeno
                //u min
                tbUkupnoPredvidjenoVreme.Text = ukupnoPredvidjeno.ToString("##.##") + "min";
                tbUkupnoUtrosenoVreme.Text = ukupnoUtroseno.ToString("##.##") + "min";
                procenat = ukupnoPredvidjeno * 100 / ukupnoUtroseno;
                tbUkupnoProcenat.Text = procenat.ToString("##.##") + "%";

                //u satima
                ostatakMinuti = ukupnoUtroseno % 60;
                double vremeUStaimaUtroseno = Math.Floor(ukupnoUtroseno / 60) + ostatakMinuti / 60;
                tbUkupnoUtrosenoVremeSati.Text = vremeUStaimaUtroseno.ToString("##.##") + "h";

                ostatakMinuti = ukupnoPredvidjeno % 60;
                double vremeUStaimaPredvidjeno = Math.Floor(ukupnoPredvidjeno / 60) + ostatakMinuti / 60;
                tbUkupnoPredvidjenoVremeSati.Text = vremeUStaimaPredvidjeno.ToString("##.##") + "h";
                #endregion

                #region u toku

                //if (ukupnoPredvidjenoNula == 0)
                //    ukupnoUtrosenoNula = 0;
                tbUkupnoUtrosenoVremeNula.Text = ukupnoUtrosenoNula.ToString("##.##") + "min";
                tbUkupnoPredvidjenoVremeNula.Text = ukupnoPredvidjenoNula.ToString("##.##") + "min";

                //u satima
                ostatakMinuti = ukupnoUtrosenoNula % 60;
                double vremeUStaimaUtrosenoNula = Math.Floor(ukupnoUtrosenoNula / 60) + ostatakMinuti / 60;
                tbUkupnoUtrosenoVremeSatiNula.Text = vremeUStaimaUtrosenoNula.ToString("##.##") + "h";

                ostatakMinuti = ukupnoPredvidjenoNula % 60;
                double vremeUStaimaPredvidjenoNula = Math.Floor(ukupnoPredvidjenoNula / 60) + ostatakMinuti / 60;
                tbUkupnoPredvidjenoVremeSatiNula.Text = vremeUStaimaPredvidjenoNula.ToString("##.##") + "h";
                #endregion
            }
            else
            {
                tbUkupnoUtrosenoVreme.Text = "";
                tbUkupnoProcenat.Text = "";
                tbUkupnoPredvidjenoVreme.Text = "";
                tbUkupnoUtrosenoVremeSati.Text = "";
                tbUkupnoPredvidjenoVremeSati.Text = "";
            }
        }


        private void dgvRadnici_SelectionChanged(object sender, EventArgs e)
        {

            dgvRadnici_Click(null, null);
        }

        private void dgvOutpuJournalData_SelectionChanged(object sender, EventArgs e)
        {
            dgvOutpuJournalData_Click(null, null);
        }

        private void dgvRadnici_MouseEnter(object sender, EventArgs e)
        {
            dgvRadnici.Focus();
        }

        private void dgvOutpuJournalData_MouseEnter(object sender, EventArgs e)
        {
            dgvOutpuJournalData.Focus();
        }

        private void dgvOutpuJournalData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            if (dgvOutpuJournalData.CurrentRow != null)
            {
                if (dgvOutpuJournalData.Columns[dgvOutpuJournalData.CurrentCell.ColumnIndex].Name == "Broj naloga za proizvodnju")
                    cbProdOrderNo.Text = dgvOutpuJournalData.CurrentCell.Value.ToString();

                if (dgvOutpuJournalData.Columns[dgvOutpuJournalData.CurrentCell.ColumnIndex].Name == "Broj artikla")
                    cbItemNo.Text = dgvOutpuJournalData.CurrentCell.Value.ToString();
            }
        }

        private void btnUnesiVreme_Click(object sender, EventArgs e)
        {
            //provere
            if (!cbPeriodOJD.Checked)
            {
                MessageBox.Show("Niste odabrali period", "Greška");

                return;
            }

            if (dgvRadnici.CurrentRow != null)
            {
                DateTime datumOd = new DateTime(dtpOJDod.Value.Year, dtpOJDod.Value.Month, dtpOJDod.Value.Day);
                DateTime datumDo = new DateTime(dtpOJDdo.Value.Year, dtpOJDdo.Value.Month, dtpOJDdo.Value.Day);


                metode.DB.pristup_bazi("INSERT  INTO  radnikVreme(resourceNo, datumOd, datumDo, vremeProvedeno) " +
                    "VALUES(N'" + dgvRadnici.CurrentRow.Cells["šifra resursa"].Value.ToString() + "',(CONVERT(date, '" + datumOd + "', 105)), (CONVERT(date, '" + datumDo + "', 105)) ," + tbVremeRadnik.Text + ")");
                UcitajRadnikVreme();

            }

        }

        private void UcitajRadnikVreme()
        {
            DataTable dt = metode.DB.baza_upit("SELECT  datumOd, datumDo, vremeProvedeno FROM   [stirg_local].dbo.radnikVreme " +
                " WHERE(resourceNo = N'" + dgvRadnici.CurrentRow.Cells["šifra resursa"].Value.ToString() + "') " +
                "order by datumod,datumdo");
            if (dt.Rows.Count > 0)
            {
                dgvRadnikVreme.DataSource = dt;
                dgvRadnikVreme.Columns["datumOd"].DefaultCellStyle.Format = "dd.MM.yyyy";
                dgvRadnikVreme.Columns["datumDo"].DefaultCellStyle.Format = "dd.MM.yyyy";
            }
            else dgvRadnikVreme.DataSource = null;
        }

        private void btnStampa_Click(object sender, EventArgs e)
        {

            //provere
            if (!cbPeriodOJD.Checked)
            {
                MessageBox.Show("Niste odabrali period", "Greška");

                return;
            }
            string datumOd = dtpOJDod.Value.Year + "-" + dtpOJDod.Value.Month + "-" + dtpOJDod.Value.Day;
            string datumDo = dtpOJDdo.Value.Year + "-" + dtpOJDdo.Value.Month + "-" + dtpOJDdo.Value.Day;
            double cekirano = 0;// double.Parse(tbUkupnoUtrosenoVremeSati.Text.Remove(tbUkupnoUtrosenoVremeSati.Text.Length - 1));
            double pred = 0;// double.Parse(tbUkupnoPredvidjenoVremeSati.Text.Remove(tbUkupnoPredvidjenoVremeSati.Text.Length - 1));

            double predNula = 0;
            //if (!tbUkupnoPredvidjenoVremeSatiNula.Text.Substring(0, 1).StartsWith("h"))
            //{
            //    predNula = double.Parse(tbUkupnoPredvidjenoVremeSatiNula.Text.Remove(tbUkupnoPredvidjenoVremeSatiNula.Text.Length - 1));
            //}

            makeReport("C:\\Program files\\SM\\PracenjeRadnika.rpt");
            SetParameters(dgvRadnici.CurrentRow.Cells["šifra resursa"].Value.ToString(), datumOd, datumDo, cekirano, pred, predNula);

            Report rep = new Report(ReportDoc);

            rep.ShowDialog();
        }

        private void SetParameters(string resourceNo, string datumOd, string datumDo, double vreme, double predvidjeno, double vremePredvidjenoNula)
        {

            ReportDoc.SetParameterValue("datumOd", datumOd);
            ReportDoc.SetParameterValue("datumDo", datumDo);
            // ReportDoc.SetParameterValue("vremeCekirano", vreme);
            ReportDoc.SetParameterValue("resourceNo", resourceNo);
            // ReportDoc.SetParameterValue("vremePredvidjeno", predvidjeno);
            // ReportDoc.SetParameterValue("vremePredvidjenoNula", vremePredvidjenoNula);
        }

        private void makeReport(string ReportFile)
        {
            ReportDoc = new ReportDocument();
            TextReader textReader = new StreamReader("c:\\Program files\\SM\\dblogon.txt");
            string uid = textReader.ReadLine();
            string pwd = textReader.ReadLine();
            string server = textReader.ReadLine();
            string db = textReader.ReadLine();
            textReader.Close();

            ReportDoc.Load(ReportFile);
            ReportDoc.SetDatabaseLogon(uid, pwd, server, db, true);
            ReportDoc.DataSourceConnections[0].IntegratedSecurity = false;
        }

        private void btnObrisi_Click(object sender, EventArgs e)
        {
            if (dgvRadnikVreme.CurrentRow != null)
            {
                DateTime dod = DateTime.Parse(dgvRadnikVreme.CurrentRow.Cells["datumOd"].Value.ToString());
                DateTime ddo = DateTime.Parse(dgvRadnikVreme.CurrentRow.Cells["datumDo"].Value.ToString());

                string datumOd = dod.Year + "-" + dod.Month + "-" + dod.Day;
                string datumDo = ddo.Year + "-" + ddo.Month + "-" + ddo.Day;

                metode.DB.pristup_bazi("DELETE FROM  radnikVreme " +
                    " WHERE (resourceNo = N'" + dgvRadnici.CurrentRow.Cells["šifra resursa"].Value.ToString() + "')" +
                    " AND(datumOd = CONVERT(DATETIME, '" + datumOd + "', 102))" +
                    " AND(datumDo = CONVERT(DATETIME, '" + datumDo + "', 102))");

                UcitajRadnikVreme();
            }
        }

        private void btnStampaSve_Click(object sender, EventArgs e)
        {
            //provere
            if (!cbPeriodOJD.Checked)
            {
                MessageBox.Show("Niste odabrali period", "Greška");

                return;
            }
            string izbaci = "''";
            //izbavi (stirg_local.dbo.radnikVreme.resourceNo NOT IN ('002-sak', '109-vuk', '111-gre'))
            foreach (DataGridViewRow row in dgvRadnici.Rows)
            {
                if (Boolean.Parse(row.Cells[2].Value.ToString()))
                {
                    izbaci += ",'"+row.Cells[0].Value.ToString()+"'";
                }
            }

            string uslov = " and (stirg_local.dbo.radnikVreme.resourceNo NOT IN ("+izbaci+"))";
            //report
            string datumOd = dtpOJDod.Value.Year + "-" + dtpOJDod.Value.Month + "-" + dtpOJDod.Value.Day;
            string datumDo = dtpOJDdo.Value.Year + "-" + dtpOJDdo.Value.Month + "-" + dtpOJDdo.Value.Day;
            makeReport("C:\\Program files\\SM\\PracenjeRadnikaSvi.rpt");
            ReportDoc.SetParameterValue("datumOd", datumOd);
            ReportDoc.SetParameterValue("datumDo", datumDo);
            ReportDoc.SetParameterValue("izbaci", uslov);

            Report rep = new Report(ReportDoc);

            rep.ShowDialog();

        }

        private void IzracunajVremeZaPeriod(Thread thread)
        {
            DateTime datumOd = new DateTime(dtpOJDod.Value.Year, dtpOJDod.Value.Month, dtpOJDod.Value.Day);
            DateTime datumDo = new DateTime(dtpOJDdo.Value.Year, dtpOJDdo.Value.Month, dtpOJDdo.Value.Day);
            string datumOdFormat = dtpOJDod.Value.Year.ToString() + "-" + dtpOJDod.Value.Day.ToString() + "-" + dtpOJDod.Value.Month.ToString();
            string datumDoFormat = dtpOJDdo.Value.Year + "-" + dtpOJDdo.Value.Day + "-" + dtpOJDdo.Value.Month;
            int iii = 0;
            thread.Start();
            foreach (DataGridViewRow row in dgvRadnici.Rows)
            {
                if (!Boolean.Parse(row.Cells[2].Value.ToString()))
                {
                    string aaa = row.Cells["Šifra resursa"].Value.ToString();
                    UcitajOJD(row.Cells["Šifra resursa"].Value.ToString());
                    //dgvRadnici.Rows[iii].Selected = true;
                    //dgvRadnici_Click(null, null);
                    iii++;
                    double ukupnoPredvidjeno = 0;
                    double ukupnoUtroseno = 0;
                    double ukupnoPredvidjenoNula = 0;
                    double ukupnoUtrosenoNula = 0;


                    ArrayList skup = new ArrayList();
                    string[] uslov = new string[3];
                    string[] uslov1 = new string[3];

                    //skup.Add(uslov1);

                    int i = 0;
                    foreach (DataGridViewRow r in dgvOutpuJournalData.Rows)
                    {



                        string prodOrderNo = r.Cells["Broj naloga za proizvodnju"].Value.ToString();
                        string lineNo = r.Cells["Line No_"].Value.ToString();
                        string operationNo = r.Cells["broj Operacije"].Value.ToString();
                        string itemNo = r.Cells["broj artikla"].Value.ToString();

                        //nadji ko je radio i vrati broj ukupnih cekiranja
                        double ukupnoCekirani = KoJeJosRadio(prodOrderNo, itemNo, operationNo);
                        ucitajProdOrderRoutingLine(prodOrderNo, lineNo, operationNo);

                        uslov[0] = prodOrderNo;
                        uslov[1] = itemNo;
                        uslov[2] = operationNo;
                        bool ima = false;
                        foreach (string[] s in skup)
                        {
                            if (s[0] == prodOrderNo && s[1] == itemNo && s[2] == operationNo)
                            {
                                ima = true;
                            }
                        }
                        if (!ima)
                        {
                            double[] rez = IzracunajVreme(ukupnoCekirani, r);
                            if (rez[2] == 1)
                            {
                                ukupnoPredvidjeno += rez[0];
                                ukupnoUtroseno += rez[1];
                            }
                            else
                            {
                                ukupnoPredvidjenoNula += rez[0];
                                ukupnoUtrosenoNula += rez[1];
                            }
                            skup.Insert(i, new string[3] { prodOrderNo, itemNo, operationNo });
                            i++;
                        }


                    }
                    skup.Clear();

                    #region odradjeno
                    //u min
                    double ostatakMinuti = 0;

                    //u satima
                    ostatakMinuti = ukupnoUtroseno % 60;
                    double vremeUStaimaUtroseno = Math.Round(Math.Floor(ukupnoUtroseno / 60) + ostatakMinuti / 60, 2);

                    ostatakMinuti = ukupnoPredvidjeno % 60;
                    double vremeUStaimaPredvidjeno = Math.Round(Math.Floor(ukupnoPredvidjeno / 60) + ostatakMinuti / 60, 2);
                    #endregion

                    #region u toku
                    //u satima
                    ostatakMinuti = ukupnoUtrosenoNula % 60;
                    double vremeUStaimaUtrosenoNula = Math.Floor(ukupnoUtrosenoNula / 60) + ostatakMinuti / 60;


                    ostatakMinuti = ukupnoPredvidjenoNula % 60;
                    double vremeUStaimaPredvidjenoNula = Math.Round(Math.Floor(ukupnoPredvidjenoNula / 60) + ostatakMinuti / 60, 2);
                    #endregion

                    //proverava da li je vec upisao
                    if (metode.DB.baza_upit("select resourceNo from stirg_local.dbo.radnikVreme where " +
                         "resourceNo ='" + row.Cells["Šifra resursa"].Value.ToString() + "' and datumOd=(CONVERT(datetime, '" + datumOdFormat + "', 105)) and datumDo = (CONVERT(datetime, '" + datumDoFormat + "', 105))  ").Rows.Count > 0)


                    {

                        //update vremePredvidjeno, vremePredvidjenoNula
                        metode.DB.pristup_bazi(" update radnikVreme set vremePredvidjeno=" + vremeUStaimaPredvidjeno + ", vremePredvidjenoNula= " + vremeUStaimaPredvidjenoNula + "" +
                            " where resourceNo ='" + row.Cells["Šifra resursa"].Value.ToString() + "' and datumOd=(CONVERT(datetime, '" + datumOdFormat + "', 105)) and datumDo = (CONVERT(datetime, '" + datumDoFormat + "', 105)) ");

                    }
                    else
                    {

                        //insert
                        //metode.DB.pristup_bazi("INSERT INTO radnikVreme(resourceNo, vremeProvedeno, vremePredvidjeno, vremePredvidjenoNula,datumOd, datumDo) " +
                        //    " VALUES('" + row.Cells["Šifra resursa"].Value.ToString() + "'," + vremeUStaimaUtroseno + "," + vremeUStaimaPredvidjeno + "," + vremeUStaimaPredvidjenoNula + ",(CONVERT(datetime, '" + datumOdFormat + "', 105)), (CONVERT(datetime, '" + datumDoFormat + "', 105)))");
                        metode.DB.pristup_bazi("INSERT INTO radnikVreme(resourceNo, vremeProvedeno, vremePredvidjeno, vremePredvidjenoNula,datumOd, datumDo) " +
                           " VALUES('" + row.Cells["Šifra resursa"].Value.ToString() + "',0," + vremeUStaimaPredvidjeno + "," + vremeUStaimaPredvidjenoNula + ",(CONVERT(datetime, '" + datumOdFormat + "', 105)), (CONVERT(datetime, '" + datumDoFormat + "', 105)))");


                    }
                    try
                    {
                        dgvRadnici.Rows[iii].Selected = false;

                    }
                    catch { }
                }
            }
            thread.Abort();
        }

        private void prikaziProgresBar()
        {
            FormProgressBar un = new FormProgressBar();
            un.ShowDialog();
        }

        private void btnIzracunaj_Click(object sender, EventArgs e)
        {
            //provere
            if (!cbPeriodOJD.Checked)
            {
                MessageBox.Show("Niste odabrali period", "Greška");

                return;
            }
            DateTime sad = DateTime.Now;
            Thread thread = new Thread(new ThreadStart(prikaziProgresBar));

            IzracunajVremeZaPeriod(thread);

            DateTime kraj = DateTime.Now;

            var seconds = System.Math.Abs((sad - kraj).TotalSeconds);
            MessageBox.Show("Uradjeno za " + seconds + " sekundi");

        }

        private void dgvRadnikVreme_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DateTime datumOd = new DateTime(dtpOJDod.Value.Year, dtpOJDod.Value.Month, dtpOJDod.Value.Day);
            DateTime datumDo = new DateTime(dtpOJDdo.Value.Year, dtpOJDdo.Value.Month, dtpOJDdo.Value.Day);

            if (dgvRadnikVreme.Columns[e.ColumnIndex].Name == "vremeProvedeno")
            {
                metode.DB.pristup_bazi(" UPDATE radnikVreme SET     vremeProvedeno = " + dgvRadnikVreme.CurrentRow.Cells["vremeProvedeno"].Value.ToString() + " " +
                    " where resourceNo= N'" + dgvRadnici.CurrentRow.Cells["šifra resursa"].Value.ToString() + "' and datumOd=(CONVERT(date, '" + datumOd + "', 105)) and" +
                    " datumdo= (CONVERT(date, '" + datumDo + "', 105)) ");
            }

        }

        private void dgvRadnici_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRadnici.CurrentCell.OwningColumn.Name == "cmb")
            {
                dgvRadnici.CurrentRow.Cells["cmb"].Value = !Boolean.Parse(dgvRadnici.CurrentRow.Cells["cmb"].Value.ToString());
            }
        }
    }
}
