using Microsoft.EntityFrameworkCore;
using OBSApp.Models;

namespace OBSApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (var context = new AppDbContext())
            {
                // Sýnýflarý ComboBox'a yükleyelim
                var siniflar = context.Siniflar.ToList();

                if (siniflar.Count == 0)
                {
                    MessageBox.Show("Sýnýflar tablosunda veri bulunmamaktadýr.");
                    return;
                }

                cmbSiniflar.DisplayMember = "SinifAd";
                cmbSiniflar.ValueMember = "SinifId";
                cmbSiniflar.DataSource = siniflar;
            }
        }

        //KAYDETME ÝÞLEMÝ
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                // Kontrolleri yapalým (zorunlu alanlar)
                if (string.IsNullOrEmpty(txtAd.Text) || string.IsNullOrEmpty(txtSoyad.Text) || string.IsNullOrEmpty(txtNumara.Text) || cmbSiniflar.SelectedIndex == -1)
                {
                    MessageBox.Show("Tüm alanlarý doldurduðunuzdan emin olun.");
                    return;
                }

                // Sýnýf seçildi mi kontrolü
                int sinifId = Convert.ToInt32(cmbSiniflar.SelectedValue);

                // Seçilen sýnýfýn kontenjaný kontrol edilsin
                using (var context = new AppDbContext())
                {
                    var sinif = context.Siniflar.FirstOrDefault(s => s.SinifId == sinifId);
                    if (sinif != null && sinif.Kontenjan <= context.Ogrenciler.Count(o => o.SinifId == sinifId))
                    {
                        MessageBox.Show("Bu sýnýfýn kontenjaný dolmuþ.");
                        return;
                    }

                    // Ayný numara ile zaten bir öðrenci kaydedilip kaydedilmediðini kontrol et
                    var mevcutOgrenci = context.Ogrenciler.FirstOrDefault(o => o.Numara == txtNumara.Text);
                    if (mevcutOgrenci != null)
                    {
                        MessageBox.Show("Bu numara ile zaten bir öðrenci kaydý bulunmaktadýr. Lütfen farklý bir numara giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Öðrenciyi kaydet
                    var ogrenci = new Ogrenci
                    {
                        Ad = txtAd.Text,
                        Soyad = txtSoyad.Text,
                        Numara = txtNumara.Text,
                        SinifId = sinifId
                    };

                    context.Ogrenciler.Add(ogrenci);
                    context.SaveChanges();

                    MessageBox.Show("Öðrenci baþarýyla kaydedildi.");
                }

                // Formu temizle
                txtAd.Clear();
                txtSoyad.Clear();
                txtNumara.Clear();
                cmbSiniflar.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluþtu: {ex.Message}");
            }
        }

        //BUL ÝÞLEMÝ
        private Ogrenci FindOgrenciByNumber(string ogrenciNumara)
        {
            using (var context = new AppDbContext())
            {
                // Öðrenciyi numarasýna göre bul
                var ogrenci = context.Ogrenciler
                    .Include(o => o.Sinif) // Sýnýf bilgilerini de çekmek için
                    .FirstOrDefault(o => o.Numara == ogrenciNumara);

                return ogrenci;
            }
        }
        //BUL ÝÞLEMÝ
        private void btnBul_Click(object sender, EventArgs e)
        {

            string ogrenciNumara = txtNumara.Text; // Öðrenci numarasýný TextBox'tan al

            if (string.IsNullOrWhiteSpace(ogrenciNumara))
            {
                MessageBox.Show("Lütfen bir öðrenci numarasý girin.");
                return;
            }

            var ogrenci = FindOgrenciByNumber(txtNumara.Text);

            if (ogrenci == null)
            {
                MessageBox.Show("Öðrenci bulunamadý.");
            }
            else
            {
                // Bulunan öðrenciyi formdaki ilgili alanlara doldur
                txtAd.Text = ogrenci.Ad;
                txtSoyad.Text = ogrenci.Soyad;
                txtNumara.Text = ogrenci.Numara;
                txtNumara.Tag = ogrenci.OgrenciId;
                cmbSiniflar.SelectedValue = ogrenci.SinifId; // Sýnýf seçimini güncelle

                MessageBox.Show("Öðrenci bilgileri baþarýyla yüklendi.");
            }
        }
        //UPDATE ÝÞLEMÝ
        private void UpdateOgrenci(Ogrenci ogrenci)
        {
            using (var context = new AppDbContext())
            {
                // Öðrenciyi veritabanýnda bul
                var existingOgrenci = context.Ogrenciler.FirstOrDefault(o => o.OgrenciId == ogrenci.OgrenciId);

                if (existingOgrenci != null)
                {
                    // Öðrenci bilgilerini güncelle
                    existingOgrenci.Ad = ogrenci.Ad;
                    existingOgrenci.Soyad = ogrenci.Soyad;
                    existingOgrenci.Numara = ogrenci.Numara;
                    existingOgrenci.SinifId = ogrenci.SinifId;

                    context.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Güncellenecek öðrenci bulunamadý.");
                }
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {


            // Formdaki bilgileri kontrol et
            if (string.IsNullOrWhiteSpace(txtAd.Text) ||
                string.IsNullOrWhiteSpace(txtSoyad.Text) ||
                string.IsNullOrWhiteSpace(txtNumara.Text) ||
                cmbSiniflar.SelectedValue == null)
            {
                MessageBox.Show("Lütfen tüm alanlarý doldurun.");
                return;
            }

            // Öðrencinin mevcut bilgilerini doldur
            var ogrenci = new Ogrenci
            {
                OgrenciId = int.Parse(txtNumara.Tag.ToString()), // Numara tag'e atanmýþ olacak
                Ad = txtAd.Text,
                Soyad = txtSoyad.Text,
                Numara = txtNumara.Text,
                SinifId = (int)cmbSiniflar.SelectedValue
            };

            // Güncelleme iþlemini yap
            UpdateOgrenci(ogrenci);

            MessageBox.Show("Öðrenci bilgileri baþarýyla güncellendi.");


        }

        private void btnDers_Click(object sender, EventArgs e)
        {
               
                var ogrenciId = Convert.ToInt32(txtNumara.Tag);
                using (var ctx = new AppDbContext())
                {
                    var ogrenci = ctx.Ogrenciler
                        .Include(o => o.Sinif)
                        .FirstOrDefault(o => o.OgrenciId == ogrenciId);

                    if (ogrenci != null)
                    {
                        DersSecimForm dersSecimForm = new DersSecimForm(ogrenci);
                        dersSecimForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Öðrenci bulunamadý.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            
        }
    }
}