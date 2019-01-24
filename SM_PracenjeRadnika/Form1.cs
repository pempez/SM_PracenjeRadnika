using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SM_PracenjeRadnika
{
    public partial class Form1 : Form
    {
        private static double granicnoVreme = 1;//120

        public Form1()
        {
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
                dgvRadnici_Click(null, null);


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
                UcitajOJD(dgvRadnici.CurrentRow.Cells["Šifra resursa"].Value.ToString());
                UcitajIskoriscenost(dgvRadnici.CurrentRow.Cells["Šifra resursa"].Value.ToString());

                gpKontejner.Visible = false;
                //ucitaj predvidjeoNula ukupno
                double ukupnoPredvidjeno = 0;
                foreach(DataGridViewRow r in dgvOutpuJournalData.Rows)
                {
                    string prodOrderNo = r.Cells["Broj naloga za proizvodnju"].Value.ToString();
                    string lineNo = r.Cells["Line No_"].Value.ToString();
                    string operationNo = r.Cells["broj Operacije"].Value.ToString();
                    string itemNo = r.Cells["broj artikla"].Value.ToString();

                    //nadji ko je radio i vrati broj ukupnih cekiranja
                    double ukupnoCekirani = KoJeJosRadio(prodOrderNo, itemNo, operationNo);
                    ucitajProdOrderRoutingLine(prodOrderNo, lineNo, operationNo);

                   double ukupno= IzracunajVreme(ukupnoCekirani,r);
                    ukupnoPredvidjeno += ukupno;
                }
                gpKontejner.Visible = true;
                tbUkupnoPredvidjenoVremeNula.Text = ukupnoPredvidjeno.ToString();
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

                IzracunajVreme(ukupnoCekirani);
               
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

        private double IzracunajUkupnoVreme(string radnik,string nulaJedan)
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
                qSelect +=  " and dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Run Time]>="+granicnoVreme+" ";

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

        private double IzracunajVreme(double ukupnoCekirani)
        {
            if (dgvProdOrderRoutingLine.CurrentRow != null)
            {
                double utrosenoVreme = double.Parse(dgvOutpuJournalData.CurrentRow.Cells["stvarno Trajanje"].Value.ToString());
                double predvidjenoVreme = 0;
                double procenat = 0;
                double uradjeno = double.Parse(dgvOutpuJournalData.CurrentRow.Cells["izlazna količina"].Value.ToString());
                double skart = double.Parse(dgvOutpuJournalData.CurrentRow.Cells["količina škarta"].Value.ToString());
                double predvidjenoVremePoKomadu = double.Parse(dgvProdOrderRoutingLine.CurrentRow.Cells["vreme izvođenja"].Value.ToString());
                double vremePodesavanja = double.Parse(dgvProdOrderRoutingLine.CurrentRow.Cells["vreme podešavanja"].Value.ToString());
                double narucenaKolicina = double.Parse(dgvOutpuJournalData.CurrentRow.Cells["Naručena količina"].Value.ToString());
                double uradjenaKolicina = double.Parse(dgvOutpuJournalData.CurrentRow.Cells["urađena količina"].Value.ToString());
                double radnikUpunoCekirao = 0;

                //pronadji koilko puta je radio
                foreach (DataGridViewRow row in dgvRadiciDodatni.Rows)
                {
                    if (row.Cells[0].Value.ToString().Equals(dgvRadnici.CurrentRow.Cells["Šifra resursa"].Value.ToString()))
                    {
                        radnikUpunoCekirao++;
                    }
                }
                if (dgvProdOrderRoutingLine.CurrentRow.Cells["Vremenska jedinica"].Value.ToString() == "SAT")
                    predvidjenoVremePoKomadu = predvidjenoVremePoKomadu * 60;
                if (uradjeno >= 0)
                {
                    tbUtrosenoVreme.Text = utrosenoVreme.ToString();
                    tbIzlaznaKolicina.Text = uradjeno.ToString();
                    tbSkart.Text = skart.ToString();

                    predvidjenoVreme = vremePodesavanja + predvidjenoVremePoKomadu * (uradjeno + skart);
                    tbPredvidjenoVreme.Text = predvidjenoVreme.ToString();

                    procenat = predvidjenoVreme * 100 / utrosenoVreme;
                    tbProcenat.Text = procenat.ToString("##.##") + "%";
                    gbOdradjeno.Visible = true;
                    gbPredvidjenoVreme.Visible = false;
                    return 0;
                }
                else
                {
                    //proveri da li ima odradjenih komada
                    double uradjenoOdOstalihRadnika = 0;
                    foreach(DataGridViewRow r in dgvRadiciDodatni.Rows)
                    {
                        uradjenoOdOstalihRadnika+= double.Parse(r.Cells["izlazna količina"].Value.ToString());
                    }
                    if (predvidjenoVremePoKomadu > 119)
                    {
                        tbUtrosenoVremeNula.Text = utrosenoVreme.ToString();
                        predvidjenoVreme = vremePodesavanja + predvidjenoVremePoKomadu * (narucenaKolicina - uradjenaKolicina - uradjeno);
                        predvidjenoVreme = predvidjenoVreme / ukupnoCekirani * radnikUpunoCekirao;
                        tbPredvidjenoVremePoKomadu.Text = predvidjenoVreme.ToString();
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

        private double IzracunajVreme(double ukupnoCekirani,DataGridViewRow r)
        {
            if (dgvProdOrderRoutingLine.CurrentRow != null)
            {
                double utrosenoVreme = double.Parse(r.Cells["stvarno Trajanje"].Value.ToString());
                double predvidjenoVreme = 0;
                double procenat = 0;
                double uradjeno = double.Parse(r.Cells["izlazna količina"].Value.ToString());
                double skart = double.Parse(r.Cells["količina škarta"].Value.ToString());
                double predvidjenoVremePoKomadu = double.Parse(dgvProdOrderRoutingLine.CurrentRow.Cells["vreme izvođenja"].Value.ToString());
                double vremePodesavanja = double.Parse(dgvProdOrderRoutingLine.CurrentRow.Cells["vreme podešavanja"].Value.ToString());
                double narucenaKolicina = double.Parse(r.Cells["Naručena količina"].Value.ToString());
                double uradjenaKolicina = double.Parse(r.Cells["urađena količina"].Value.ToString());
                double radnikUpunoCekirao = 0;

                //pronadji koilko puta je radio
                foreach (DataGridViewRow row in dgvRadiciDodatni.Rows)
                {
                    if (row.Cells[0].Value.ToString().Equals(dgvRadnici.CurrentRow.Cells["Šifra resursa"].Value.ToString()))
                    {
                        radnikUpunoCekirao++;
                    }
                }
                if (dgvProdOrderRoutingLine.CurrentRow.Cells["Vremenska jedinica"].Value.ToString() == "SAT")
                    predvidjenoVremePoKomadu = predvidjenoVremePoKomadu * 60;
                if (uradjeno > 0)
                {
                    tbUtrosenoVreme.Text = utrosenoVreme.ToString();
                    tbIzlaznaKolicina.Text = uradjeno.ToString();
                    tbSkart.Text = skart.ToString();

                    predvidjenoVreme = vremePodesavanja + predvidjenoVremePoKomadu * (uradjeno + skart);
                    tbPredvidjenoVreme.Text = predvidjenoVreme.ToString();

                    procenat = predvidjenoVreme * 100 / utrosenoVreme;
                    tbProcenat.Text = procenat.ToString("##.##") + "%";
                    gbOdradjeno.Visible = true;
                    gbPredvidjenoVreme.Visible = false;
                    return 0;
                }
                else
                {
                    if (predvidjenoVremePoKomadu > 119)
                    {
                        tbUtrosenoVremeNula.Text = utrosenoVreme.ToString();
                        predvidjenoVreme = vremePodesavanja + predvidjenoVremePoKomadu * (narucenaKolicina - uradjenaKolicina - uradjeno);
                        predvidjenoVreme = predvidjenoVreme / ukupnoCekirani * radnikUpunoCekirao;
                        tbPredvidjenoVremePoKomadu.Text = predvidjenoVreme.ToString();
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

        private void UcitajIskoriscenost(string IdRadnik)
        {
            double ukupnoUtroseno = 0;
            double ukupnoUtrosenoNula = 0;
            double ukupnoPredvidjeno = IzracunajUkupnoVreme(IdRadnik,">");
            double ukupnoUtrosenoSat = 0;
            double ukupnoPredvidjenoNula = IzracunajUkupnoVreme(IdRadnik,"=");
            double ostatakMinuti = 0;
            double procenat = 0;
            if (dgvOutpuJournalData.Rows.Count > 0)
            {
                foreach (DataGridViewRow r in dgvOutpuJournalData.Rows)
                {
                    if (double.Parse(r.Cells["Izlazna količina"].Value.ToString())>0)
                    { 
                        ukupnoUtroseno += double.Parse(r.Cells["stvarno trajanje"].Value.ToString());
                    }
                    else
                    {
                        if (double.Parse(r.Cells["run time"].Value.ToString())>=granicnoVreme)
                        ukupnoUtrosenoNula += double.Parse(r.Cells["stvarno trajanje"].Value.ToString());
                    }
                }
                #region odradjeno
                //u min
                tbUkupnoPredvidjenoVreme.Text = ukupnoPredvidjeno.ToString("##.##") + "min";
                tbUkupnoUtrosenoVreme.Text = ukupnoUtroseno.ToString("##.##") + "min";
                procenat = ukupnoPredvidjeno * 100 / ukupnoUtroseno;
                tbUkupnoProcenat.Text = procenat.ToString("##.##")+"%";

                //u satima
                ostatakMinuti= ukupnoUtroseno%60;
                double vremeUStaimaUtroseno= Math.Floor(ukupnoUtroseno/60)+ ostatakMinuti / 60;
                tbUkupnoUtrosenoVremeSati.Text = vremeUStaimaUtroseno.ToString("##.##") + "h";

                ostatakMinuti = ukupnoPredvidjeno % 60;
                double vremeUStaimaPredvidjeno= Math.Floor(ukupnoPredvidjeno / 60) + ostatakMinuti / 60;
                tbUkupnoPredvidjenoVremeSati.Text = vremeUStaimaPredvidjeno.ToString("##.##")+"h";
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
                if(dgvOutpuJournalData.Columns[dgvOutpuJournalData.CurrentCell.ColumnIndex].Name=="Broj naloga za proizvodnju")
                    cbProdOrderNo.Text = dgvOutpuJournalData.CurrentCell.Value.ToString();

                if (dgvOutpuJournalData.Columns[dgvOutpuJournalData.CurrentCell.ColumnIndex].Name == "Broj artikla")
                    cbItemNo.Text = dgvOutpuJournalData.CurrentCell.Value.ToString();
            }
        }
    }
}
