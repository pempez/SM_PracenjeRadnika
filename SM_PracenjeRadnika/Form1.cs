﻿using System;
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
                         "  dbo.[Stirg Produkcija$Output Journal Data].[Last Operation No_] AS[Broj poslednje operacije], dbo.[Stirg Produkcija$Output Journal Data].[Posting Date] AS[Datum knjiženja], CONVERT(char(5),  " +
                         "  dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], 108) AS[Vreme prvog registrovanog početka], CONVERT(char(5), dbo.[Stirg Produkcija$Output Journal Data].[Ending Time], 108)  " +
                         "  AS[Vreme poslednjeg registrovanog završetka],    CASE WHEN DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) = 0 THEN 1 ELSE" +
                         " DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) END  AS[stvarno Trajanje],  " +
                         "  dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity] AS[Izlazna količina], dbo.[Stirg Produkcija$Output Journal Data].[Scrap Quantity] AS[Količina škarta],  " +
                         "  dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] AS[Šifra resursa], dbo.[Stirg Produkcija$Resource].Name AS[Ime resursa], dbo.[Stirg Produkcija$Output Journal Data].[Line No_] AS[Broj reda],  " +
                         "  dbo.[Stirg Produkcija$Output Journal Data].[Controlled Operation No_] AS[Broj kontrolisane operacije], dbo.[Stirg Produkcija$Prod_ Order Line].[Line No_] " +
        "  FROM dbo.[Stirg Produkcija$Output Journal Data] INNER JOIN " +

                        "  dbo.[Stirg Produkcija$Resource] ON dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ INNER JOIN " +

                       "   dbo.[Stirg Produkcija$Prod_ Order Line] ON dbo.[Stirg Produkcija$Output Journal Data].[Item No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Item No_] AND " +
                       "   dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] INNER JOIN " +

                       "   dbo.[Stirg Produkcija$Prod_ Order Routing Line] ON dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Prod_ Order No_] AND " +
                       "   dbo.[Stirg Produkcija$Prod_ Order Line].[Line No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Routing Reference No_]" +
                       "  AND      dbo.[Stirg Produkcija$Output Journal Data].[Operation No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Operation No_] " +
                    " WHERE    (dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = N'" + radnik + "') AND (DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) >= 0) ";


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
            UcitajOJD(dgvRadnici.CurrentRow.Cells["Šifra resursa"].Value.ToString());

           
            UcitajIskoriscenost(dgvRadnici.CurrentRow.Cells["Šifra resursa"].Value.ToString());

        }

        private void dgvOutpuJournalData_Click(object sender, EventArgs e)
        {
            if (dgvOutpuJournalData.CurrentRow != null)
            {
                string prodOrderNo = dgvOutpuJournalData.CurrentRow.Cells["Broj naloga za proizvodnju"].Value.ToString();
                string lineNo = dgvOutpuJournalData.CurrentRow.Cells["Line No_"].Value.ToString();
                string operationNo = dgvOutpuJournalData.CurrentRow.Cells["broj Operacije"].Value.ToString();
                ucitajProdOrderRoutingLine(prodOrderNo, lineNo, operationNo);

                IzracunajVreme();
            }
        }

        private double IzracunajUkupnoVreme(string radnik)
        {
            //string qSelect = "SELECT DISTINCT    TOP(100) PERCENT dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity] AS[Izlazna količina], dbo.[Stirg Produkcija$Output Journal Data].[Scrap Quantity] AS[Količina škarta], "+
            //            " dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Setup Time], dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Run Time],  " +
            //              "  dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Setup Time] + (dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity] + dbo.[Stirg Produkcija$Output Journal Data].[Scrap Quantity])  " +
            //           "     * dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Run Time]  AS potrebnoVreme " +
            //             
            string qSelect = "SELECT       isnull( SUM(dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Setup Time] + (dbo.[Stirg Produkcija$Output Journal Data].[Output Quantity] + dbo.[Stirg Produkcija$Output Journal Data].[Scrap Quantity]) " +
                           "     * dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Run Time]),0) AS potrebnoVreme"+
                            " FROM dbo.[Stirg Produkcija$Output Journal Data] INNER JOIN " +
                          "  dbo.[Stirg Produkcija$Resource] ON dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = dbo.[Stirg Produkcija$Resource].No_ INNER JOIN " +
                      "      dbo.[Stirg Produkcija$Prod_ Order Line] ON dbo.[Stirg Produkcija$Output Journal Data].[Item No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Item No_] AND " +
                       "     dbo.[Stirg Produkcija$Output Journal Data].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] INNER JOIN " +
                       "     dbo.[Stirg Produkcija$Prod_ Order Routing Line] ON dbo.[Stirg Produkcija$Prod_ Order Line].[Prod_ Order No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Prod_ Order No_] AND " +
                        "    dbo.[Stirg Produkcija$Prod_ Order Line].[Line No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Routing Reference No_] AND " +
                        "    dbo.[Stirg Produkcija$Output Journal Data].[Operation No_] = dbo.[Stirg Produkcija$Prod_ Order Routing Line].[Operation No_] "+
            " WHERE    (dbo.[Stirg Produkcija$Output Journal Data].[Resource No_] = N'" + radnik + "') AND (DATEDIFF(minute, dbo.[Stirg Produkcija$Output Journal Data].[Starting Time], dbo.[Stirg Produkcija$Output Journal Data].[Ending Time]) >= 0) ";


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

        private void IzracunajVreme()
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

                if (dgvProdOrderRoutingLine.CurrentRow.Cells["Vremenska jedinica"].Value.ToString() == "SAT")
                    predvidjenoVremePoKomadu = predvidjenoVremePoKomadu * 60;

                tbUtrosenoVreme.Text = utrosenoVreme.ToString();
                tbIzlaznaKolicina.Text = uradjeno.ToString();
                tbSkart.Text = skart.ToString();

                predvidjenoVreme = vremePodesavanja + predvidjenoVremePoKomadu * (uradjeno + skart);
                tbPredvidjenoVreme.Text = predvidjenoVreme.ToString();

                procenat = predvidjenoVreme * 100 / utrosenoVreme;
                tbProcenat.Text = procenat.ToString("##.##") + "%";
            }
        }

        private void UcitajIskoriscenost(string IdRadnik)
        {
            double ukupnoUtroseno = 0;
            double ukupnoPredvidjeno = IzracunajUkupnoVreme(IdRadnik);
            double ukupnoUtrosenoSat = 0;
            double ukupnoPredvidjenoSat = IzracunajUkupnoVreme(IdRadnik);
            double ostatakMinuti = 0;
            double procenat = 0;
            if (dgvOutpuJournalData.Rows.Count > 0)
            {
                foreach (DataGridViewRow r in dgvOutpuJournalData.Rows)
                {
                    ukupnoUtroseno += double.Parse(r.Cells["stvarno trajanje"].Value.ToString());

                }
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
    }
}
