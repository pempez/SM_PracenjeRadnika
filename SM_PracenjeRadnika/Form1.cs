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


        }

        private void popuniCBradnik()
        {
            cbRadnik.DataSource = metode.DB.baza_upit("SELECT   TOP (100) PERCENT No_, Name " +
                                    " FROM            dbo.[Stirg Produkcija$Resource] WHERE        (Name <> N'') ORDER BY Name");
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

            string qSelect = "SELECT DISTINCT dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] AS [Šifra resursa], dbo.[Stirg Produkcija$Resource].Name AS [Ime resursa]" +
                " FROM dbo.[Stirg Produkcija$Output Journal Data] INNER JOIN  " +
                "  dbo.[Stirg Produkcija$Resource] ON dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ " +
                "WHERE(1 = 1)";

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
            }

        }

        private void UcitajOJD(string radnik)
        {
            //string qSelect = " SELECT        dbo.[Stirg Produkcija$Output Journal Data].[Item No_] as [Broj artikla] , CASE WHEN dbo.[Stirg Produkcija$Output Journal Data].Status = 0 THEN 'započeta' ELSE 'proknjižena' END AS status, dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] as [Broj naloga za proizvodnju], dbo.[Stirg Produkcija$Output Journal Data].[Operation No_] as [Broj operacije], " +
            //            "  dbo.[Stirg Produkcija$Output Journal Data].[Last Operation No_] as [Broj poslednje operacije] , dbo.[Stirg Produkcija$Output Journal Data].[Posting Date] as [Datum knjiženja],   " +
            //         "  CONVERT(char(5),  dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], 108) AS [Vreme prvog registrovanog početka] , " +
            //            "  CONVERT(char(5),  dbo.[Stirg Produkcija$Output Journal Data].[Ending Time], 108) AS [Vreme poslednjeg registrovanog završetka], " +
            //            "   dbo.[Stirg Produkcija$Output Journal Data].[output quantity] as [Izlazna količina], dbo.[Stirg Produkcija$Output Journal Data].[Scrap Quantity] as [Količina škarta],  " +
            //             "  dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] as [Šifra resursa], dbo.[Stirg Produkcija$Resource].Name as [Ime resursa], DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time],  " +
            //             "  dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) AS [stvarno Trajanje], dbo.[Stirg Produkcija$Output Journal Data].[Line No_] as [Broj reda] , dbo.[Stirg Produkcija$Output Journal Data].[Controlled Operation No_] as [Broj kontrolisane operacije] " +
            //             " FROM            dbo.[Stirg Produkcija$Output Journal Data] INNER JOIN " +
            //           "    dbo.[Stirg Produkcija$Resource] ON dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ " +
            //           " WHERE    (dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = N'" + radnik + "')   ";

            string qSelect = "SELECT        dbo.[Stirg Produkcija$Output Journal Data].[Item No_] AS [Broj artikla], CASE WHEN dbo.[Stirg Produkcija$Output Journal Data].Status = 0 THEN 'započeta' ELSE 'proknjižena' END AS status,  " +
                       "  dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] AS[Broj naloga za proizvodnju], dbo.[Stirg Produkcija$Output Journal Data].[Operation No_] AS[Broj operacije],  " +
                     "     dbo.[Stirg Produkcija$Output Journal Data].[Last Operation No_] AS[Broj poslednje operacije], dbo.[Stirg Produkcija$Output Journal Data].[Posting Date] AS[Datum knjiženja], CONVERT(char(5), " +
                    "    dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], 108) AS[Vreme prvog registrovanog početka], CONVERT(char(5), dbo.[Stirg Produkcija$Output Journal Data].[Ending Time], 108) " +
                      "    AS[Vreme poslednjeg registrovanog završetka], DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], " +
                    "    dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) AS[stvarno Trajanje], dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity] AS[Izlazna količina], dbo.[Stirg Produkcija$Output Journal Data].[Scrap Quantity] AS[Količina škarta],  " +
                      "    dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] AS[Šifra resursa], dbo.[Stirg Produkcija$Resource].Name AS[Ime resursa], dbo.[Stirg Produkcija$Output Journal Data].[Line No_] AS[Broj reda],  " +
                      "    dbo.[Stirg Produkcija$Output Journal Data].[Controlled Operation No_] AS[Broj kontrolisane operacije], dbo.[Stirg Produkcija$Prod_ Order Line].[Line No_] " +
                    " FROM dbo.[Stirg Produkcija$Output Journal Data]   INNER JOIN   dbo.[Stirg Produkcija$Resource] ON dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ INNER JOIN " +
                    " dbo.[Stirg Produkcija$Prod_ Order Line] ON dbo.[Stirg Produkcija$Output Journal Data].[Item No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Item No_] AND " +
            "  dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] " +
                       " WHERE    (dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = N'" + radnik + "')   ";


            qSelect = PrimeniFiltere(qSelect);

            DataTable dt = metode.DB.baza_upit(qSelect);

            if (dt.Rows.Count > 0)
            {
                lblOJD.Text = dt.Rows.Count.ToString();
                dgvOutpuJournalData.DataSource = dt;
                dgvOutpuJournalData.Columns["Izlazna količina"].DefaultCellStyle.Format = "N2";
                dgvOutpuJournalData.Columns["Količina škarta"].DefaultCellStyle.Format = "N2";
                dgvOutpuJournalData_Click(null, null);

            }
            else
            {
                lblOJD.Text = "";
                dgvOutpuJournalData.DataSource = null;
            }
        }

        private void ucitajProdOrderRoutingLine(string prodOrderNo, string referenceNo, string operationNo)
        {

            string qUpit = " select [Routing Reference No_] as [referentni broj proizvodnog postupka], [Operation No_] as [Broj operacije]," +
                " 'Radni centar' as Vrsta,[Work Center No_] as Broj, Description as Opis ,[Setup Time] as [vreme podešavanja], [Run Time] as [vreme izvođenja],[Setup Time Unit of Meas_ Code] as [Vremenska jedinica] ,[Send-Ahead Quantity] as [unapred poslata količina], [Concurrent Capacities] as  [uporedni kapaciteti] " +
                " FROM            [Stirg Produkcija$Prod_ Order Routing Line]" +
                 " WHERE        ([Prod_ Order No_] = N'" + prodOrderNo + "') AND ([Routing Reference No_] = '" + referenceNo + "') and [Operation No_]='" + operationNo + "'";
          if (cbRadnaGrupa.Text != "")
            {
                qUpit += " and [Work Center Group Code]=N'" + cbRadnaGrupa.SelectedValue + "' ";
            }


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
            return qSelect;
        }

        private void dgvRadnici_Click(object sender, EventArgs e)
        {
            UcitajOJD(dgvRadnici.CurrentRow.Cells["Šifra resursa"].Value.ToString());

        }

        private void dgvOutpuJournalData_Click(object sender, EventArgs e)
        {

            string prodOrderNo = dgvOutpuJournalData.CurrentRow.Cells["Broj naloga za proizvodnju"].Value.ToString();
            string lineNo = dgvOutpuJournalData.CurrentRow.Cells["Line No_"].Value.ToString();
            string operationNo = dgvOutpuJournalData.CurrentRow.Cells["broj Operacije"].Value.ToString();
            ucitajProdOrderRoutingLine(prodOrderNo, lineNo, operationNo);
        }

    }
}
