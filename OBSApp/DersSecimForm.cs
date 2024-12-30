using OBSApp.Models;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OBSApp
{
    public partial class DersSecimForm : Form
    {
        private Ogrenci _ogrenci;

        public DersSecimForm(Ogrenci ogrenci)
        {
            InitializeComponent();
            _ogrenci = ogrenci;
            LoadDersler();
            DisplayOgrenciBilgileri();
        }

        private void DisplayOgrenciBilgileri()
        {
            if (_ogrenci != null)
            {
                // Label üzerine öğrenci bilgilerini yaz
                lblOgrenciBilgileri.Text = $"Ad: {_ogrenci.Ad}, Soyad: {_ogrenci.Soyad}, " +
                                           $"Numara: {_ogrenci.Numara}, Sınıf: {_ogrenci.Sinif.SinifAd}";
            }
            else
            {
                MessageBox.Show("Öğrenci bilgisi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close(); // Öğrenci bulunamazsa formu kapat
            }
        }

        private void LoadDersler()
        {
            using (var ctx = new AppDbContext())
            {
                var dersler = ctx.Dersler
                    .Select(d => new
                    {
                        d.DersId,
                        d.DersKod,
                        d.DersAd
                    })
                    .ToList();

                dgvDersler.DataSource = dersler;

                // CheckBox ekliyoruz
                DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn
                {
                    HeaderText = "Seç",
                    Name = "DersSecim"
                };
                dgvDersler.Columns.Insert(0, checkColumn); // İlk sıraya ekliyoruz
            }

            dgvDersler.Columns["DersId"].Visible = false;
        }

        private void btnDersleriKaydet_Click(object sender, EventArgs e)
        {
           
                bool derslerEklendi = false; // Ders eklenip eklenmediğini kontrol etmek için bir bayrak

                try
                {
                    using (var ctx = new AppDbContext())
                    {
                        foreach (DataGridViewRow row in dgvDersler.Rows)
                        {
                            // Eğer checkbox işaretli ise
                            if (Convert.ToBoolean(row.Cells["DersSecim"].Value) == true)
                            {
                                int dersId = Convert.ToInt32(row.Cells["DersId"].Value);

                                // Aynı dersin daha önce seçilip seçilmediğini kontrol et
                                bool dersZatenSecilmis = ctx.OgrenciDersler.Any(od => od.OgrenciId == _ogrenci.OgrenciId && od.DersId == dersId);

                                if (dersZatenSecilmis)
                                {
                                    MessageBox.Show($"Bu ders zaten seçilmiş: {row.Cells["DersAd"].Value}", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    continue; // Aynı dersi tekrar eklememek için döngünün bu iterasyonunu atla
                                }

                                // Ders seçimi kaydet
                                var ogrenciDers = new OgrenciDers
                                {
                                    OgrenciId = _ogrenci.OgrenciId,
                                    DersId = dersId
                                };

                                ctx.OgrenciDersler.Add(ogrenciDers);
                                derslerEklendi = true; // Ders eklendiği için bayrağı true yap
                            }
                        }

                        // Eğer yeni dersler eklendiyse, değişiklikleri kaydet
                        if (derslerEklendi)
                        {
                            ctx.SaveChanges();
                            MessageBox.Show("Dersler başarıyla kaydedildi!");
                        }
                        else
                        {
                            MessageBox.Show("Seçilen derslerde değişiklik yok.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        

        }
    }
}
