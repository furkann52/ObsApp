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
                // S�n�flar� ComboBox'a y�kleyelim
                var siniflar = context.Siniflar.ToList();

                if (siniflar.Count == 0)
                {
                    MessageBox.Show("S�n�flar tablosunda veri bulunmamaktad�r.");
                    return;
                }

                cmbSiniflar.DisplayMember = "SinifAd";
                cmbSiniflar.ValueMember = "SinifId";
                cmbSiniflar.DataSource = siniflar;
            }
        }

        //KAYDETME ��LEM�
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                // Kontrolleri yapal�m (zorunlu alanlar)
                if (string.IsNullOrEmpty(txtAd.Text) || string.IsNullOrEmpty(txtSoyad.Text) || string.IsNullOrEmpty(txtNumara.Text) || cmbSiniflar.SelectedIndex == -1)
                {
                    MessageBox.Show("T�m alanlar� doldurdu�unuzdan emin olun.");
                    return;
                }

                // S�n�f se�ildi mi kontrol�
                int sinifId = Convert.ToInt32(cmbSiniflar.SelectedValue);

                // Se�ilen s�n�f�n kontenjan� kontrol edilsin
                using (var context = new AppDbContext())
                {
                    var sinif = context.Siniflar.FirstOrDefault(s => s.SinifId == sinifId);
                    if (sinif != null && sinif.Kontenjan <= context.Ogrenciler.Count(o => o.SinifId == sinifId))
                    {
                        MessageBox.Show("Bu s�n�f�n kontenjan� dolmu�.");
                        return;
                    }

                    // Ayn� numara ile zaten bir ��renci kaydedilip kaydedilmedi�ini kontrol et
                    var mevcutOgrenci = context.Ogrenciler.FirstOrDefault(o => o.Numara == txtNumara.Text);
                    if (mevcutOgrenci != null)
                    {
                        MessageBox.Show("Bu numara ile zaten bir ��renci kayd� bulunmaktad�r. L�tfen farkl� bir numara giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // ��renciyi kaydet
                    var ogrenci = new Ogrenci
                    {
                        Ad = txtAd.Text,
                        Soyad = txtSoyad.Text,
                        Numara = txtNumara.Text,
                        SinifId = sinifId
                    };

                    context.Ogrenciler.Add(ogrenci);
                    context.SaveChanges();

                    MessageBox.Show("��renci ba�ar�yla kaydedildi.");
                }

                // Formu temizle
                txtAd.Clear();
                txtSoyad.Clear();
                txtNumara.Clear();
                cmbSiniflar.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata olu�tu: {ex.Message}");
            }
        }

        //BUL ��LEM�
        private Ogrenci FindOgrenciByNumber(string ogrenciNumara)
        {
            using (var context = new AppDbContext())
            {
                // ��renciyi numaras�na g�re bul
                var ogrenci = context.Ogrenciler
                    .Include(o => o.Sinif) // S�n�f bilgilerini de �ekmek i�in
                    .FirstOrDefault(o => o.Numara == ogrenciNumara);

                return ogrenci;
            }
        }
        //BUL ��LEM�
        private void btnBul_Click(object sender, EventArgs e)
        {

            string ogrenciNumara = txtNumara.Text; // ��renci numaras�n� TextBox'tan al

            if (string.IsNullOrWhiteSpace(ogrenciNumara))
            {
                MessageBox.Show("L�tfen bir ��renci numaras� girin.");
                return;
            }

            var ogrenci = FindOgrenciByNumber(txtNumara.Text);

            if (ogrenci == null)
            {
                MessageBox.Show("��renci bulunamad�.");
            }
            else
            {
                // Bulunan ��renciyi formdaki ilgili alanlara doldur
                txtAd.Text = ogrenci.Ad;
                txtSoyad.Text = ogrenci.Soyad;
                txtNumara.Text = ogrenci.Numara;
                txtNumara.Tag = ogrenci.OgrenciId;
                cmbSiniflar.SelectedValue = ogrenci.SinifId; // S�n�f se�imini g�ncelle

                MessageBox.Show("��renci bilgileri ba�ar�yla y�klendi.");
            }
        }
        //UPDATE ��LEM�
        private void UpdateOgrenci(Ogrenci ogrenci)
        {
            using (var context = new AppDbContext())
            {
                // ��renciyi veritaban�nda bul
                var existingOgrenci = context.Ogrenciler.FirstOrDefault(o => o.OgrenciId == ogrenci.OgrenciId);

                if (existingOgrenci != null)
                {
                    // ��renci bilgilerini g�ncelle
                    existingOgrenci.Ad = ogrenci.Ad;
                    existingOgrenci.Soyad = ogrenci.Soyad;
                    existingOgrenci.Numara = ogrenci.Numara;
                    existingOgrenci.SinifId = ogrenci.SinifId;

                    context.SaveChanges();
                }
                else
                {
                    MessageBox.Show("G�ncellenecek ��renci bulunamad�.");
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
                MessageBox.Show("L�tfen t�m alanlar� doldurun.");
                return;
            }

            // ��rencinin mevcut bilgilerini doldur
            var ogrenci = new Ogrenci
            {
                OgrenciId = int.Parse(txtNumara.Tag.ToString()), // Numara tag'e atanm�� olacak
                Ad = txtAd.Text,
                Soyad = txtSoyad.Text,
                Numara = txtNumara.Text,
                SinifId = (int)cmbSiniflar.SelectedValue
            };

            // G�ncelleme i�lemini yap
            UpdateOgrenci(ogrenci);

            MessageBox.Show("��renci bilgileri ba�ar�yla g�ncellendi.");


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
                        MessageBox.Show("��renci bulunamad�.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            
        }
    }
}