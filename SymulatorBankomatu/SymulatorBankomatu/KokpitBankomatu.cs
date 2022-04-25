using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymulatorBankomatu
{
    public partial class KokpitBankomatu : Form
    {
        const ushort dlMaxLicznośćNominałów = 100;
        const int dlBanknotOnajniższejWartości = 10;
        struct Nominały
        {
            public ushort dlLiczność;
            public float dlWartość;
        }
        int[] dlWartościNominałów = { 200, 100, 50, 20, 10, 5, 2, 1};
        Nominały[] dlPojemnikNominałów;

        Label dllblEtykietaDolnejGranicyPrzedziału = new Label();
        TextBox dltxtEtykietaDolnejGranicyPrzedziału = new TextBox();
        Label dllblEtykietaGórnejGranicyPrzedziału = new Label();
        TextBox dltxtEtykietaGórnejGranicyPrzedziału = new TextBox();
        Button dlbtnPrzyciskAkceptacjiNominałów = new Button();

        bool dlKontrolkiDodaneDoFormularza = false;
        public KokpitBankomatu()
        {
            InitializeComponent();
            dlPojemnikNominałów = new Nominały[dlWartościNominałów.Length];
            dlcmbListaWalut.SelectedIndex = 0;
            dlbtnReset.Enabled = false;
            dlbtnExit.Enabled = false;
        }
        #region Akceptacja
        private void dlbtnAkceptacja_Click(object sender, EventArgs e)
        {
            dlpanelLeft.Height = dlbtnAcceptance.Height;
            dlpanelLeft.Top = dlbtnAcceptance.Top;
            dlErrorProvider.Dispose();
            float dlKwotaDoWypłaty;
            while (!float.TryParse(dltxtWysokośćKwoty.Text, out dlKwotaDoWypłaty))
            {
                dlErrorProvider.SetError(dltxtWysokośćKwoty, "Error: wystąpił niedozwolony znak w zapisie kwoty do wypłaty");
                return;
            }
            if (dlKwotaDoWypłaty <= 0.0F)
            {
                dlErrorProvider.SetError(dltxtWysokośćKwoty, "Error: wartość do wypłaty nie może być <= 0.0");
                return;
            }
            if (!CzyWypłataMożeByćZrealizowana (dlPojemnikNominałów, dlKwotaDoWypłaty))
            {
                dlErrorProvider.SetError(dlbtnAcceptance, "Przepraszam: ale nie mogę zrealizować tą wypłate");
                return;
            }
            if (dlcmbListaWalut.SelectedIndex < 0)
            {
                dlErrorProvider.SetError(dlcmbListaWalut, "Error: musisz wybrać walutę ");
                return;
            }
            else
            {
                float dlResztaDoWypłaty = dlKwotaDoWypłaty;
                ushort dlIndexPojemnikaNominałów = 0;
                ushort dlIndexDGV = 0;
                ushort dlLiczbaNominałów;
                dldgvWypłacaneNominały.Rows.Clear();
                dllblWypłacaneNominały.Text = "Wypłacane nominały";
                while ((dlResztaDoWypłaty > 0.0)&&(dlIndexPojemnikaNominałów < dlPojemnikNominałów.Length))
                {
                    dlLiczbaNominałów = (ushort)(dlResztaDoWypłaty / dlPojemnikNominałów[dlIndexPojemnikaNominałów].dlWartość);

                    if (dlLiczbaNominałów > dlPojemnikNominałów[dlIndexPojemnikaNominałów].dlLiczność)
                    {
                        dlLiczbaNominałów = dlPojemnikNominałów[dlIndexPojemnikaNominałów].dlLiczność;
                        dlPojemnikNominałów[dlIndexPojemnikaNominałów].dlLiczność = 0;
                    }
                    else
                    {
                        dlPojemnikNominałów[dlIndexPojemnikaNominałów].dlLiczność = (ushort)(dlPojemnikNominałów[dlIndexPojemnikaNominałów].dlLiczność - dlLiczbaNominałów);
                    }
                    if (dlLiczbaNominałów > 0)
                    {
                        dldgvWypłacaneNominały.Rows.Add();
                        dldgvWypłacaneNominały.Rows[dlIndexDGV].Cells[0].Value = dlLiczbaNominałów;
                        dldgvWypłacaneNominały.Rows[dlIndexDGV].Cells[1].Value = dlPojemnikNominałów[dlIndexPojemnikaNominałów].dlWartość;
                        if (dlPojemnikNominałów[dlIndexPojemnikaNominałów].dlWartość >= dlBanknotOnajniższejWartości)
                            dldgvWypłacaneNominały.Rows[dlIndexDGV].Cells[2].Value = "banknot";
                        else
                            dldgvWypłacaneNominały.Rows[dlIndexDGV].Cells[2].Value = "moneta";
                        dldgvWypłacaneNominały.Rows[dlIndexDGV].Cells[3].Value = dlcmbListaWalut.SelectedItem;

                        dldgvWypłacaneNominały.Rows[dlIndexDGV].Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dldgvWypłacaneNominały.Rows[dlIndexDGV].Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dldgvWypłacaneNominały.Rows[dlIndexDGV].Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dldgvWypłacaneNominały.Rows[dlIndexDGV].Cells[3].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dlIndexDGV++;
                    }
                    dlResztaDoWypłaty = dlResztaDoWypłaty - dlLiczbaNominałów * dlPojemnikNominałów[dlIndexPojemnikaNominałów].dlWartość;/////////
                    dlIndexPojemnikaNominałów++;
                }
                if (dlResztaDoWypłaty > 0)
                {
                    dlErrorProvider.SetError(dlbtnAcceptance, "Pszepraszam: ale w tej chwile nie mam wystarczającej liczby nominałów. Czy możesz podać inną kwotę do wypłaty?");
                    return;
                }
                else
                {
                    dllblWypłacanaKwota.Visible = true;
                    dltxtWypłacanaKwota.Visible = true;
                    dltxtWypłacanaKwota.Text = dltxtWysokośćKwoty.Text;
                }
                dltxtWysokośćKwoty.Enabled = false;
                dlcmbListaWalut.Enabled = false;
                dlbtnAcceptance.Enabled = false;
                dlbtnReset.Enabled = true;
                dlbtnExit.Enabled = true;
                dlbtnExit.Visible = true;
                dlbtnReset.Visible = true;
                dllblWypłacaneNominały.Visible = true;
                dldgvWypłacaneNominały.Visible = true;
                dlbtnAcceptance.Enabled = false;
                dltxtWypłacanaKwota.Enabled = false;
                dlrdbUstawianaLosowo.Enabled = false;
                dlrdbUstawieniaDomyślne.Enabled = false;
            }
        }
        #endregion;
        #region Reset
        private void dlbtnReset_Click(object sender, EventArgs e)
        {
            dlrdbUstawianaLosowo.Enabled = true;
            dlrdbUstawieniaDomyślne.Enabled = true;
            //////////////////////////////////////////
            dlpanelLeft.Height = dlbtnReset.Height;
            dlpanelLeft.Top = dlbtnReset.Top;
            dltxtWysokośćKwoty.Text = null;
            dltxtWysokośćKwoty.Enabled = true;
            dlbtnAcceptance.Enabled = true;
            dllblWypłacanaKwota.Visible = false;
            dltxtWypłacanaKwota.Visible = false;
            dlbtnReset.Enabled = false;
            dlbtnExit.Enabled = false;
            dldgvWypłacaneNominały.Rows.Clear();
            dldgvWypłacaneNominały.Visible = false;
            dllblWypłacaneNominały.Visible = false;
            dlcmbListaWalut.SelectedIndex = 0;
            dlcmbListaWalut.Enabled = true;
            dllblEtykietaDolnejGranicyPrzedziału.Visible = false;
            dltxtEtykietaDolnejGranicyPrzedziału.Text = null;
            dltxtEtykietaDolnejGranicyPrzedziału.Visible = false;
            dltxtEtykietaDolnejGranicyPrzedziału.Enabled = true;
            dllblEtykietaGórnejGranicyPrzedziału.Visible = false;
            dltxtEtykietaGórnejGranicyPrzedziału.Text = null;
            dltxtEtykietaGórnejGranicyPrzedziału.Visible = false;
            dltxtEtykietaGórnejGranicyPrzedziału.Enabled = true;
            dllblWysokośćKwoty.Enabled = true;
            dlbtnPrzyciskAkceptacjiNominałów.Enabled = true;
            dlbtnPrzyciskAkceptacjiNominałów.Visible = false;
        }
        #endregion
        #region Exit
        private void dlbtnExit_Click(object sender, EventArgs e)
        {
            dlpanelLeft.Height = dlbtnExit.Height;
            dlpanelLeft.Top = dlbtnExit.Top;
            Application.Exit();
        }
        #endregion
        #region Info
        private void dlbtnInfo_Click(object sender, EventArgs e)
        {
            if (dlpanelInfo.Visible == false)
            {
                dlpanelInfo.Visible = true;
            }
            else 
                if (dlpanelInfo.Visible == true)
                {
                    dlpanelInfo.Visible = false;
                }
        }
        #endregion
        #region Ustawienie domyślne
        private void dlrdbUstawieniaDomyślne_CheckedChanged(object sender, EventArgs e)
        {
            const ushort dlLicznośćNominałów = 50;
            for (ushort dli = 0; dli < dlPojemnikNominałów.Length; dli++)
            {
                dlPojemnikNominałów[dli].dlLiczność = dlLicznośćNominałów;
                dlPojemnikNominałów[dli].dlWartość = dlWartościNominałów[dli];
            }
            dlrdbUstawieniaDomyślne.Enabled = false;
            dlrdbUstawianaLosowo.Enabled = false;
            dlrdbUstawianaLosowo.Checked = false;
            dlrdbUstawieniaDomyślne.Checked = true;
            dllblWysokośćKwoty.Enabled = true;
            dltxtWysokośćKwoty.Enabled = true;
            dlbtnAcceptance.Enabled = true;
        }
        #endregion
        #region Czy wpłata może być realizowana
        static bool CzyWypłataMożeByćZrealizowana(Nominały[] dlPojemnikNominałów, float dlKwotaDoWypłaty)
        {
            float dlKapitałBankomatu = 0.0F;
            for (ushort i = 0; i < dlPojemnikNominałów.Length; i++)
                if (dlPojemnikNominałów[i].dlWartość > 0.0F)
                    dlKapitałBankomatu += dlPojemnikNominałów[i].dlLiczność * dlPojemnikNominałów[i].dlWartość;
            return dlKapitałBankomatu >= dlKwotaDoWypłaty;
        }
        #endregion
        #region Ustawiana losowo
        private void dlrdbUstawianaLosowo_CheckedChanged(object sender, EventArgs e)
        {
            if (dlKontrolkiDodaneDoFormularza)
            {
                dllblEtykietaDolnejGranicyPrzedziału.Visible = true;
                dltxtEtykietaDolnejGranicyPrzedziału.Visible = true;
                dltxtEtykietaDolnejGranicyPrzedziału.Enabled = true;
                dllblEtykietaGórnejGranicyPrzedziału.Visible = true;
                dltxtEtykietaGórnejGranicyPrzedziału.Enabled = true;
                dltxtEtykietaGórnejGranicyPrzedziału.Visible = true;
                dlbtnPrzyciskAkceptacjiNominałów.Visible = true;
                dlbtnPrzyciskAkceptacjiNominałów.Enabled = true;
            }
            else
            {
                dlKontrolkiDodaneDoFormularza = true;
                //Dolna granica przedziału
                dllblEtykietaDolnejGranicyPrzedziału.Text = "Dolna granica przedziału liczności nominałów";
                dllblEtykietaDolnejGranicyPrzedziału.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Italic);
                dllblEtykietaDolnejGranicyPrzedziału.TextAlign = ContentAlignment.MiddleCenter;
                dllblEtykietaDolnejGranicyPrzedziału.Location = new Point(250, 80);
                dllblEtykietaDolnejGranicyPrzedziału.Height = 60;
                dllblEtykietaDolnejGranicyPrzedziału.Width = 150;
                dllblEtykietaDolnejGranicyPrzedziału.BackColor = this.BackColor;
                dllblEtykietaDolnejGranicyPrzedziału.ForeColor = this.ForeColor;
                this.Controls.Add(dllblEtykietaDolnejGranicyPrzedziału);

                dltxtEtykietaDolnejGranicyPrzedziału.BackColor = dlbtnAcceptance.BackColor;
                dltxtEtykietaDolnejGranicyPrzedziału.ForeColor = dlbtnAcceptance.ForeColor;
                dltxtEtykietaDolnejGranicyPrzedziału.Text = "";
                dltxtEtykietaDolnejGranicyPrzedziału.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
                dltxtEtykietaDolnejGranicyPrzedziału.TextAlign = HorizontalAlignment.Center;
                dltxtEtykietaDolnejGranicyPrzedziału.Location = new Point(265, 150);
                dltxtEtykietaDolnejGranicyPrzedziału.Size = new Size(120, 20);
                this.Controls.Add(dltxtEtykietaDolnejGranicyPrzedziału);
                
                //Górna granica przedziału
                dllblEtykietaGórnejGranicyPrzedziału.Text = "Górna granica przedziału liczności nominałów";
                dllblEtykietaGórnejGranicyPrzedziału.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Italic);
                dllblEtykietaGórnejGranicyPrzedziału.TextAlign = ContentAlignment.MiddleCenter;
                dllblEtykietaGórnejGranicyPrzedziału.Location = new Point(450, 80);
                dllblEtykietaGórnejGranicyPrzedziału.Height = 60;
                dllblEtykietaGórnejGranicyPrzedziału.Width = 150;
                dllblEtykietaGórnejGranicyPrzedziału.BackColor = this.BackColor;
                dllblEtykietaGórnejGranicyPrzedziału.ForeColor = this.ForeColor;
                this.Controls.Add(dllblEtykietaGórnejGranicyPrzedziału);

                dltxtEtykietaGórnejGranicyPrzedziału.BackColor = dlbtnAcceptance.BackColor;
                dltxtEtykietaGórnejGranicyPrzedziału.ForeColor = dlbtnAcceptance.ForeColor;
                dltxtEtykietaGórnejGranicyPrzedziału.Text = "";
                dltxtEtykietaGórnejGranicyPrzedziału.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);
                dltxtEtykietaGórnejGranicyPrzedziału.TextAlign = HorizontalAlignment.Center;
                dltxtEtykietaGórnejGranicyPrzedziału.Location = new Point(465, 150);
                dltxtEtykietaGórnejGranicyPrzedziału.Size = new Size(120, 20);
                this.Controls.Add(dltxtEtykietaGórnejGranicyPrzedziału);
                //dlbtnPrzyciskAkceptacjiNominałów
                dlbtnPrzyciskAkceptacjiNominałów.Text = "Akceptacja nominałów";
                dlbtnPrzyciskAkceptacjiNominałów.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Italic);
                dlbtnPrzyciskAkceptacjiNominałów.TextAlign = ContentAlignment.MiddleCenter;
                dlbtnPrzyciskAkceptacjiNominałów.Location = new Point(365, 200);
                dlbtnPrzyciskAkceptacjiNominałów.Size = new Size(100, 50);
                dlbtnPrzyciskAkceptacjiNominałów.FlatAppearance.BorderSize = 0;
                dlbtnPrzyciskAkceptacjiNominałów.FlatStyle = this.dlbtnAcceptance.FlatStyle;
                this.Controls.Add(dlbtnPrzyciskAkceptacjiNominałów);

                dlbtnPrzyciskAkceptacjiNominałów.Click += new EventHandler(dlbtnPrzyciskAkceptacjiNominałów_Click);
                dllblWysokośćKwoty.Enabled = true;
                dltxtWysokośćKwoty.Enabled = true;
                dlbtnAcceptance.Enabled = true;
                dlrdbUstawieniaDomyślne.Enabled = false;
                dlrdbUstawianaLosowo.Enabled = false;
                dlrdbUstawianaLosowo.Checked = true;
                dlrdbUstawieniaDomyślne.Checked = false;
            }
        }
        #endregion
        #region Akceptacja nominałów
        void dlbtnPrzyciskAkceptacjiNominałów_Click(object sender, EventArgs e)
        {
            ushort dlDolnaGranica, dlGórnaGranica;
            dlErrorProvider.Dispose();
            while (!ushort.TryParse(dltxtEtykietaDolnejGranicyPrzedziału.Text, out dlDolnaGranica))
            {
                dlErrorProvider.SetError(dltxtEtykietaDolnejGranicyPrzedziału, "Error: wystąpił niedozwolony znak w zapisie dolnej granicy liczności nominałów");
                return;
            }
            while (!ushort.TryParse(dltxtEtykietaGórnejGranicyPrzedziału.Text, out dlGórnaGranica))
            {
                dlErrorProvider.SetError(dltxtEtykietaGórnejGranicyPrzedziału, "Error: wystąpił niedozwolony znak w zapisie górnej granicy liczności nominałów");
                return;
            }
            if (dlDolnaGranica >= dlGórnaGranica)
            {
                dlErrorProvider.SetError(dltxtEtykietaGórnejGranicyPrzedziału, "Error: górna granica przedziału liczności nominałów musi być większa od granicy dolnej");
                return;
            }
            Random dlRND = new Random();
            for (int dlk = 0; dlk < dlPojemnikNominałów.Length; dlk++)
            {
                dlPojemnikNominałów[dlk].dlLiczność = (ushort) dlRND.Next(dlDolnaGranica, dlGórnaGranica);
                dlPojemnikNominałów[dlk].dlWartość = dlWartościNominałów[dlk];
            }
            dldgvWypłacaneNominały.Visible = true;
            dllblWypłacaneNominały.Text = "Zawartość pojemnika nominałów";
            for (int dlk = 0; dlk < dlPojemnikNominałów.Length; dlk++)
            {
                dldgvWypłacaneNominały.Rows.Add();
                dldgvWypłacaneNominały.Rows[dlk].Cells[0].Value = dlPojemnikNominałów[dlk].dlLiczność;
                dldgvWypłacaneNominały.Rows[dlk].Cells[1].Value = dlPojemnikNominałów[dlk].dlWartość;
                if (dlPojemnikNominałów[dlk].dlWartość >= dlBanknotOnajniższejWartości)
                    dldgvWypłacaneNominały.Rows[dlk].Cells[2].Value = "banknot";
                else
                    dldgvWypłacaneNominały.Rows[dlk].Cells[2].Value = "moneta";
                dldgvWypłacaneNominały.Rows[dlk].Cells[3].Value = dlcmbListaWalut.SelectedItem;

                dldgvWypłacaneNominały.Rows[dlk].Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dldgvWypłacaneNominały.Rows[dlk].Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dldgvWypłacaneNominały.Rows[dlk].Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dldgvWypłacaneNominały.Rows[dlk].Cells[3].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dltxtEtykietaDolnejGranicyPrzedziału.Enabled = false;
            dltxtEtykietaGórnejGranicyPrzedziału.Enabled = false;
            dlcmbListaWalut.Enabled = false;
            dlbtnPrzyciskAkceptacjiNominałów.Enabled = false;
            dllblWypłacaneNominały.Visible = true;
            dldgvWypłacaneNominały.Visible = true;
            dllblWysokośćKwoty.Enabled = true;
            dltxtWysokośćKwoty.Enabled = true;
            dlbtnAcceptance.Enabled = true;
        }
        #endregion
    }
}
